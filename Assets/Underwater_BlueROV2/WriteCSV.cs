using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Logs experimental data to CSV files during the Unity simulation.
/// Tracks UUV and reference object position, orientation, errors, control signals, etc.
/// </summary>
public class WriteCSV : MonoBehaviour
{
    public Small_terrain_height height2terrain;
    public Joystick_inputs JoyInput;
    public Controller Conl;
    public CreateTexture CT;
    public NearestPointFromRayExample real_error;
    public ROV_dynamics ROV_D;
    public IOC_control IOC_C;

    [SerializeField] private Transform UUV;
    [SerializeField] private Transform spline_ball;
    [SerializeField] private SerialSend SSjoy;

    public string Participant_NAME;

    private StreamWriter SW_main;
    private StreamWriter SW_check;

    private string Method;
    private float timeStamp = 0.0f;
    private int CSV_flag = 0;
    private int start_pos_flag = 0;

    private Vector3 posUUV_ini;
    private Vector3 rotUUV_ini;
    private Vector3 posBall_ini;
    private Vector3 rotBall_ini;

    private Vector3 posUUV_F;
    private Vector3 rotUUV_F;
    private Vector3 posBall_F;
    private Vector3 rotBall_F;

    private float error_y;
    private float error_z;
    private float error_angle;

    private float[] logData_main;
    private float[] logData_check;

    void Start()
    {
        SW_main = Define_CSV(Participant_NAME);
        SW_check = Check_CSV(Participant_NAME);

        posUUV_ini = UUV.position;
        rotUUV_ini = UUV.eulerAngles - new Vector3(180f, 180f, 180f);
        posBall_ini = spline_ball.position;
        rotBall_ini = spline_ball.eulerAngles - new Vector3(180f, 180f, 180f);

        Method = SSjoy.flag == 0 ? "proposed" : "previous";
    }

    void FixedUpdate()
    {
        if (CSV_flag == 0)
        {
            Vector3 posUUV = UUV.position - posUUV_ini;
            Vector3 rotUUV = UUV.eulerAngles - new Vector3(180f, 180f, 180f) - rotUUV_ini;
            Vector3 posBall = spline_ball.position - posBall_ini;
            Vector3 rotBall = spline_ball.eulerAngles - new Vector3(180f, 180f, 180f) - rotBall_ini;

            Vector3 vec2ball = posBall - posUUV;
            Vector3 UUV_forward = UUV.forward;
            Vector3 Ball_forward = spline_ball.forward;

            // Zero vertical components for planar angle calculation
            UUV_forward.y = 0;
            vec2ball.y = 0;
            Ball_forward.y = 0;

            float Angle_of_direction = Vector3.SignedAngle(UUV_forward, vec2ball, Vector3.up);
            float Angle_of_Error = Vector3.SignedAngle(UUV_forward, Ball_forward, Vector3.up);

            if (start_pos_flag == 0)
            {
                posUUV_F = posUUV;
                rotUUV_F = rotUUV;
                posBall_F = posBall;
                rotBall_F = rotBall;
                start_pos_flag = 1;
            }

            // Compute errors in world frame
            float World_diff_x = -(posUUV.z - posBall.z);
            float World_diff_y = -(posUUV.x - posBall.x);
            float World_diff_z = posUUV.y - posBall.y;

            float World_diff_rz = rotUUV.y - rotBall.y;

            float WdX2 = Mathf.Pow(World_diff_x, 2f);
            float WdY2 = Mathf.Pow(World_diff_y, 2f);

            error_y = Mathf.Abs(Angle_of_direction) < 85f * Mathf.Deg2Rad
                ? Mathf.Sqrt(WdX2 + WdY2) * Mathf.Sign(Angle_of_direction)
                : 0f;
            error_z = World_diff_z;
            error_angle = -World_diff_rz;

            logData_main = new float[]
            {
                timeStamp,
                error_y,
                error_z,
                Angle_of_direction,
                Angle_of_Error,
                JoyInput.inputs.x,
                JoyInput.angle,
                JoyInput.confidence,
                CT.confidence,
                IOC_C.joy_send_angle,
                IOC_C.error_y,
                IOC_C.error_angle,
                CT.errory_mat,
                CT.errorag_mat,
                IOC_C.a,
                IOC_C.b,
                JoyInput.pwmA,
                JoyInput.pwmB,
                SSjoy.joy_move_lateral,
                ROV_D.dist_vel[0],
                ROV_D.dist_vel[1],
                ROV_D.dist_vel[2]
            };

            logData_check = new float[]
            {
                timeStamp,
                posUUV.x, posUUV.y, posUUV.z,
                rotUUV.x, rotUUV.y, rotUUV.z,
                posBall.x, posBall.y, posBall.z,
                rotBall.x, rotBall.y, rotBall.z,
                ROV_D.nu_now[0], ROV_D.nu_now[1], ROV_D.nu_now[2],
                ROV_D.nu_now[3], ROV_D.nu_now[4], ROV_D.nu_now[5],
                ROV_D.nu_now_dot[0], ROV_D.nu_now_dot[1], ROV_D.nu_now_dot[2],
                ROV_D.nu_now_dot[3], ROV_D.nu_now_dot[4], ROV_D.nu_now_dot[5],
                CT.y1LeftX, CT.y1RightX,
                CT.y2LeftX, CT.y2RightX,
                CT.y3LeftX, CT.y3RightX
            };

            timeStamp += Time.deltaTime;

            foreach (var val in logData_main)
            {
                SW_main.Write(val.ToString());
                SW_main.Write(",");
            }
            SW_main.WriteLine();

            foreach (var val in logData_check)
            {
                SW_check.Write(val.ToString());
                SW_check.Write(",");
            }
            SW_check.WriteLine();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                SW_main.Flush(); SW_main.Close();
                SW_check.Flush(); SW_check.Close();
                CSV_flag = 1;
            }
        }
        else if (CSV_flag == 1 && Input.GetKeyDown(KeyCode.Return))
        {
            SW_main = Define_CSV(Participant_NAME);
            SW_check = Check_CSV(Participant_NAME);
            CSV_flag = 0;
        }
    }

    private void OnApplicationQuit()
    {
        SW_main?.Flush(); SW_main?.Close();
        SW_check?.Flush(); SW_check?.Close();
    }

    private StreamWriter Define_CSV(string parname)
    {
        string[] header = {
            "UnityTime [s]", "error y", "error z", "forward angle", "error angle", "Joy angle x", "Joy angle y",
            "Mechanism Angle", "System conf", "IOCresult", "Calc error y", "Calc error angle",
            "Err Y(image)", "Err Angle(image)", "a(IOC)", "b(IOC)", "pwmA", "pwmB", "desiredB",
            "dist vx", "dist vy", "dist vz"
        };

        string path = $"C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/{parname}";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        string filename = $"{parname}_{Method}MAIN";
        var file = new FileInfo($"{path}/{filename}.csv");

        var writer = file.AppendText();
        foreach (string col in header) writer.Write($"{col},");
        writer.WriteLine();

        return writer;
    }

    private StreamWriter Check_CSV(string parname)
    {
        string[] header = {
            "UnityTime [s]",
            "UUV_World_eta X", "UUV_World_eta Y", "UUV_World_eta Z",
            "UUV_World_eta RX", "UUV_World_eta RY", "UUV_World_eta RZ",
            "LIN_World_eta X", "LIN_World_eta Y", "LIN_World_eta Z",
            "LIN_World_eta RX", "LIN_World_eta RY", "LIN_World_eta RZ",
            "ROV vx", "ROV vy", "ROV vz", "ROV wx", "ROV wy", "ROV wz",
            "ROV ax", "ROV ay", "ROV az", "ROV w_dot_x", "ROV w_dot_y", "ROV w_dot_z",
            "y1LeftX", "y1RightX", "y2LeftX", "y2RightX", "y3LeftX", "y3RightX"
        };

        string path = $"C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/{parname}";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        string filename = $"{parname}_{Method}CHECK";
        var file = new FileInfo($"{path}/{filename}.csv");

        var writer = file.AppendText();
        foreach (string col in header) writer.Write($"{col},");
        writer.WriteLine();

        return writer;
    }
}
