using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WriteCSV : MonoBehaviour
{
    public Small_terrain_height height2terrain;
    public Joystick_inputs JoyInput;
    public Controller Conl;
    public CreateTexture CT;
    public NearestPointFromRayExample real_error;

    public ROV_dynamics ROV_D;

    [SerializeField] private Transform UUV;
    [SerializeField] private Transform spline_ball;
    [SerializeField] private SerialSend SSjoy;


    public IOC_control IOC_C;
    private float timeStamp = 0.0f;  //経過時間

    public string Participant_NAME;

    float[] logData_main;
    float[] logData_check;

    private StreamWriter SW_main;
    private StreamWriter SW_check;
    // private FileInfo file;
    // private string filename; 
    // private string[] header;
    private string expname;

    private string Method;
    int start_pos_flag = 0;

    Vector3 posUUV_F;
    Vector3 rotUUV_F;
    Vector3 posBall_F;
    Vector3 rotBall_F;
    Vector3 posUUV_ini;
    Vector3 rotUUV_ini;
    Vector3 posBall_ini;
    Vector3 rotBall_ini;

    private int CSV_flag = 0;

    private float error_y;
    private float error_z;
    private float error_angle;

    void Start()
    {
        SW_main = Define_CSV(SW_main, Participant_NAME);
        SW_check = Check_CSV(SW_check, Participant_NAME);

        Vector3 posUUV_ini = UUV.position;
        Vector3 rotUUV_ini = UUV.eulerAngles - new Vector3(180.0f, 180f, 180.0f);
        Vector3 posBall_ini = spline_ball.position;
        Vector3 rotBall_ini = spline_ball.eulerAngles - new Vector3(180.0f, 180f, 180.0f);

        if (SSjoy.flag == 0){
            Method = "proposed";
        }else{
            Method = "previous";
        }
    }

    void FixedUpdate()
    {
        if (CSV_flag == 0){
            Vector3 posUUV = UUV.position - posUUV_ini;
            Vector3 rotUUV = UUV.eulerAngles - new Vector3(180.0f, 180f, 180.0f) - rotUUV_ini;
            Vector3 posBall = spline_ball.position - posBall_ini;
            Vector3 rotBall = spline_ball.eulerAngles - new Vector3(180.0f, 180f, 180.0f) - rotBall_ini;

            Vector3 vec2ball = posBall - posUUV;
            Vector3 UUV_forward = UUV.forward;
            Vector3 Ball_forward = spline_ball.forward;

            UUV_forward.y = 0.0f;
            vec2ball.y = 0.0f;
            Ball_forward.y = 0.0f;

            var Upper_vec = new Vector3(0, 1, 0);

            float Angle_of_direction = Vector3.SignedAngle(UUV_forward, vec2ball, Upper_vec);
            float Angle_of_Error = Vector3.SignedAngle(UUV_forward, Ball_forward, Upper_vec);

        
            if (start_pos_flag == 0){
                posUUV_F = posUUV;
                rotUUV_F = rotUUV;
                posBall_F = posBall;
                rotBall_F = rotBall;
                start_pos_flag = 1;
            }
            float World_diff_x = -(posUUV.z - posBall.z);
            float World_diff_y = -(posUUV.x - posBall.x);
            float World_diff_z = posUUV.y - posBall.y;

            float World_diff_rx = rotUUV.z - rotBall.z;
            float World_diff_ry = rotUUV.x - rotBall.x;
            float World_diff_rz = rotUUV.y - rotBall.y;

            float WdX2 = Mathf.Pow(World_diff_x, 2.0f);
            float WdY2 = Mathf.Pow(World_diff_y, 2.0f);
            float WdZ2 = Mathf.Pow(World_diff_z, 2.0f);
            if (Mathf.Abs(Angle_of_direction) < 85.0f * Mathf.Deg2Rad) error_y = Mathf.Sqrt(WdX2 + WdY2) * Mathf.Sign(Angle_of_direction);
            else error_y = 0.0f;
            error_z = World_diff_z;
            error_angle = -World_diff_rz;


            logData_main = new float[] {timeStamp, error_y, error_z, Angle_of_direction, Angle_of_Error, JoyInput.inputs.x, JoyInput.angle, JoyInput.confidence, CT.confidence, IOC_C.joy_send_angle,
                                   IOC_C.error_y, IOC_C.error_angle, CT.errory_mat, CT.errorag_mat, IOC_C.a, IOC_C.b, JoyInput.pwmA, JoyInput.pwmB, SSjoy.joy_move_lateral, 
                                   ROV_D.dist_vel[0], ROV_D.dist_vel[1], ROV_D.dist_vel[2]
                                  };
            logData_check = new float[] {timeStamp, 
                                   posUUV.x, posUUV.y, posUUV.z, rotUUV.x, rotUUV.y, rotUUV.z,
                                   posBall.x, posBall.y, posBall.z, rotBall.x, rotBall.y, rotBall.z
                                   , ROV_D.nu_now[0], ROV_D.nu_now[1], ROV_D.nu_now[2], ROV_D.nu_now[3], ROV_D.nu_now[4], ROV_D.nu_now[5]
                                   , ROV_D.nu_now_dot[0], ROV_D.nu_now_dot[1], ROV_D.nu_now_dot[2], ROV_D.nu_now_dot[3], ROV_D.nu_now_dot[4], ROV_D.nu_now_dot[5],
                                   CT.y1LeftX, CT.y1RightX, CT.y2LeftX, CT.y2RightX, CT.y3LeftX, CT.y3RightX
                                  };    
            // logData_check = new float[] {timeStamp, 
            //                              IOC_C.error_y, IOC_C.error_angle, IOC_C.Vel, ROV_D.nu_now[5], IOC_C.vr, 0, IOC_C.beta, 15.6f, 14.0f, 0.014f, 
            //                              CT.y1LeftX, CT.y1RightX, CT.y2LeftX, CT.y2RightX, CT.y3LeftX, CT.y3RightX
            //                             };                  

            timeStamp += Time.deltaTime;

            for (int i = 0; i < logData_main.Length; i++)
            {
                SW_main.Write(logData_main[i].ToString());
                SW_main.Write(",");
            }
            for (int i = 0; i < logData_check.Length; i++)
            {
                SW_check.Write(logData_check[i].ToString());
                SW_check.Write(",");
            }
            SW_main.Write("\n");
            SW_check.Write("\n");
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SW_main.Flush();
                SW_main.Close();
                SW_check.Flush();
                SW_check.Close();
                CSV_flag = 1;
            }
        }else if (CSV_flag == 1){
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SW_main = Define_CSV(SW_main, Participant_NAME);
                SW_check = Check_CSV(SW_check, Participant_NAME);
                CSV_flag = 0;
            }
        }
    }

    private void OnApplicationQuit()
    {
        SW_main.Flush();
        SW_main.Close();
        SW_check.Flush();
        SW_check.Close();
        CSV_flag = 1;
    }
    private StreamWriter Define_CSV(StreamWriter CSV_wrokspace, string parname){
        string[] header = new string[] {"UnityTime [s]", "error y", "error z", "forward angle", "error angle", "Joy angle x", "Joy angle y", "Mechanism Angle", "System conf","IOCresult",
                                         "Calc error y", "Calc error angle", "Err Y(image)", "Err Angle(image)", "a(IOC)", "b(IOC)", "pwmA", "pwmB", "desiredB", 
                                         "dist vx", "dist vy", "dist vz"
                                        };
        // string filename = "MAIN" + System.String.Format("{0:MM_dd HH_mm_ss_ff}", System.DateTime.Now);

        if (SSjoy.flag == 0){
            Method = "proposed";
        }else{
            Method = "previous";
        }

        string DirectoryPath = "C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/" + parname;
        if (System.IO.Directory.Exists(DirectoryPath) == false){
            System.IO.Directory.CreateDirectory(DirectoryPath);
        }
        int fileNum = System.IO.Directory.GetFiles(DirectoryPath).Length / 2 + 1;
        string filename = parname + "_" + Method + "MAIN";//fileNum.ToString("00") + "MAIN";
        FileInfo file = new FileInfo(DirectoryPath + "/" + filename + ".csv");

        CSV_wrokspace = file.AppendText();
        for (int i = 0; i < header.Length; i++)             
        {                                                   
            CSV_wrokspace.Write(header[i].ToString());                
            CSV_wrokspace.Write(",");                                 
        }                                                   
        CSV_wrokspace.Write("\n");
        return CSV_wrokspace;
    }

    private StreamWriter Check_CSV(StreamWriter CSV_wrokspace, string parname){
        string[] header = new string[] {"UnityTime [s]", 
                                         "UUV_World_eta X", "UUV_World_eta Y", "UUV_World_eta Z", "UUV_World_eta RX", "UUV_World_eta RY", "UUV_World_eta RZ",
                                         "LIN_World_eta X", "LIN_World_eta Y", "LIN_World_eta Z", "LIN_World_eta RX", "LIN_World_eta RY", "LIN_World_eta RZ"
                                         , "ROV vx", "ROV vy", "ROV vz", "ROV wx", "ROV wy", "ROV wz"
                                         , "ROV ax", "ROV ay", "ROV az", "ROV w_dot_x", "ROV w_dot_y", "ROV w_dot_z"
                                         , "y1LeftX", "y1RightX", "y2LeftX", "y2RightX", "y3LeftX", "y3RightX"
                                        };
        // string[] header = new string[] {"UnityTime [s]", 
        //                                 "Y err input", "Yaw err inoput", "V", "Vr", "omg", "omg_r", "beta", "Ky", "Kyaw", "Kbeta", "y1L", "y1R", "y2L", "y2R", "y3L", "y3R"
        //                                 };



        // string filename = "CHECK" + System.String.Format("{0:MM_dd HH_mm_ss_ff}", System.DateTime.Now);
        string DirectoryPath = "C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/" + parname;
        if (System.IO.Directory.Exists(DirectoryPath) == false){
            System.IO.Directory.CreateDirectory(DirectoryPath);
        }

        if (SSjoy.flag == 0){
            Method = "proposed";
        }else{
            Method = "previous";
        }

        int fileNum = System.IO.Directory.GetFiles(DirectoryPath).Length / 2 + 1;
        string filename = parname + "_" + Method + "CHECK";//fileNum.ToString("00") + "CHECK";
        FileInfo file = new FileInfo(DirectoryPath + "/" + filename + ".csv");

        CSV_wrokspace = file.AppendText();
        for (int i = 0; i < header.Length; i++)             
        {                                                   
            CSV_wrokspace.Write(header[i].ToString());                
            CSV_wrokspace.Write(",");                                 
        }                                                   
        CSV_wrokspace.Write("\n");
        return CSV_wrokspace;
    }

    private StreamWriter Postures_CSV(StreamWriter CSV_wrokspace, string parname){
        string[] header = new string[] {"UnityTime [s]", 
                                        "x UUVpos", "y UUVpos", "z UUVpos", "quat0 UUV", "quat1 UUV", "quat2 UUV", "quat3 UUV"
                                        };
        // string filename = "CHECK" + System.String.Format("{0:MM_dd HH_mm_ss_ff}", System.DateTime.Now);
        string DirectoryPath = "C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/" + parname;
        if (System.IO.Directory.Exists(DirectoryPath) == false){
            System.IO.Directory.CreateDirectory(DirectoryPath);
        }
        int fileNum = System.IO.Directory.GetFiles(DirectoryPath).Length / 2 + 1;
        string filename = fileNum.ToString("00") + "CHECK";
        FileInfo file = new FileInfo(DirectoryPath + "/" + filename + ".csv");

        CSV_wrokspace = file.AppendText();
        for (int i = 0; i < header.Length; i++)             
        {                                                   
            CSV_wrokspace.Write(header[i].ToString());                
            CSV_wrokspace.Write(",");                                 
        }                                                   
        CSV_wrokspace.Write("\n");
        return CSV_wrokspace;
    }



    // void StreamWriter Define_CSV()
    // {
    //     StreamWriter _sw;
    //     expname = "Simulation";                                                                                      //ファイル関数
    //     header = new string[] { "TimeStampUnity[ms]", "real y", "real angle", "error_y", "error_z", "error_angle", "Joy angle", "sys conf", "resultIOC","Vel x", "Vel y", "Vel z", "Vel omega", "Joy long", "Joy lateral", 
    //                             "robot_eta[0]", "robot_eta[1]", "robot_eta[2]", "robot_eta[3]", "robot_eta[4]", "robot_eta[5]",
    //                             "posUUV.x", "posUUV.y", "posUUV.z", "rotUUV.x", "rotUUV.y", "rotUUV.z", 
    //                             "posBall.x", "posBall.y", "posBall.z", "rotBall.x", "rotBall.y", "rotBall.z"};                                         //
    //     filename = expname + "_" + System.String.Format("{0:MM月dd日_HH時mm分ss秒fff}", System.DateTime.Now); //CHANGE POINT;                //
    //     file = new FileInfo("C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/" + filename + ".csv");                                                                //
    //                                                                                                                                          //
    //     _sw = file.AppendText();                                                                                                              //
    //     for (int i = 0; i < header.Length; i++)                                                                                              //
    //     {                                                                                                                                    //
    //         _sw.Write(header[i].ToString());                                                                                                  //
    //         _sw.Write(",");                                                                                                                   //
    //     }                                                                                                                                    //
    //     _sw.Write("\n");

    //     return _sw;
    // }
    // // Start is called before the first frame update
    // void Start()
    // {
    //     // expname = "Simulation";                                                                                      //ファイル関数
    //     // header = new string[] { "TimeStampUnity[ms]", "real y", "real angle", "error_y", "error_z", "error_angle", "Joy angle", "sys conf", "resultIOC","Vel x", "Vel y", "Vel z", "Vel omega", "Joy long", "Joy lateral", 
    //     //                         "robot_eta[0]", "robot_eta[1]", "robot_eta[2]", "robot_eta[3]", "robot_eta[4]", "robot_eta[5]",
    //     //                         "posUUV.x", "posUUV.y", "posUUV.z", "rotUUV.x", "rotUUV.y", "rotUUV.z", 
    //     //                         "posBall.x", "posBall.y", "posBall.z", "rotBall.x", "rotBall.y", "rotBall.z"};                                         //
    //     // filename = expname + "_" + System.String.Format("{0:MM月dd日_HH時mm分ss秒fff}", System.DateTime.Now); //CHANGE POINT;                //
    //     // file = new FileInfo("C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/" + filename + ".csv");                                                                //
    //     //                                                                                                                                      //
    //     // sw = file.AppendText();                                                                                                              //
    //     // for (int i = 0; i < header.Length; i++)                                                                                              //
    //     // {                                                                                                                                    //
    //     //     sw.Write(header[i].ToString());                                                                                                  //
    //     //     sw.Write(",");                                                                                                                   //
    //     // }                                                                                                                                    //
    //     // sw.Write("\n");

    //     sw = Define_CSV();
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (CSV_flag == 0){
    //         Vector3 posUUV = UUV.position;
    //         Vector3 rotUUV = UUV.eulerAngles - new Vector3(180.0f, 180f, 180.0f);
    //         Vector3 posBall = spline_ball.position;
    //         Vector3 rotBall = spline_ball.eulerAngles - new Vector3(180.0f, 180f, 180.0f);

            
    //         timeStamp += Time.deltaTime;
    //         if (start_pos_flag == 0){
    //             posUUV_F = posUUV;
    //             rotUUV_F = rotUUV;
    //             posBall_F = posBall;
    //             rotBall_F = rotBall;
    //             start_pos_flag = 1;
    //         }
    //         logData = new float[] { timeStamp, real_error.error_y, real_error._tangent[2], IOC_C.error_y, Conl.robot_error[2], IOC_C.error_angle, JoyInput.confidence, IOC_C.sys_conf, IOC_C.joy_send_angle,
    //                                 Conl.nu_now[0], Conl.nu_now[1], Conl.nu_now[2], Conl.nu_now[5], JoyInput.inputs.x, JoyInput.angle, 
    //                                 Conl.eta_now[0], Conl.eta_now[1], Conl.eta_now[2], Conl.eta_now[3], Conl.eta_now[4], Conl.eta_now[5],
    //                                 posUUV_F.x - posUUV.x, posUUV_F.y - posUUV.y, posUUV_F.z - posUUV.z, rotUUV_F.x - rotUUV.x, rotUUV_F.y - rotUUV.y, rotUUV_F.z - rotUUV.z, 
    //                                 posBall_F.x - posBall.x, posBall_F.y - posBall.y, posBall_F.z - posBall.z, rotBall_F.x - rotBall.x, rotBall_F.y - rotBall.y, rotBall_F.z - rotBall.z, 
    //                                 (float)CT.tmp
    //                             };//ファイル書き込み
    //         for (int i = 0; i < logData.Length; i++)
    //         {
    //             sw.Write(logData[i].ToString());
    //             sw.Write(",");
    //         }
    //         sw.Write("\n");

    //         if (Input.GetKeyDown(KeyCode.Return))
    //         {
    //             sw.Flush();
    //             sw.Close();
    //             CSV_flag = 1;
    //         }
    //     }else if (CSV_flag == 1){
    //         // header = new string[] { "TimeStampUnity[ms]", "real y", "real angle", "error_y", "error_z", "error_angle", "Joy angle", "sys conf", "resultIOC","Vel x", "Vel y", "Vel z", "Vel omega", "Joy long", "Joy lateral", 
    //         //                     "robot_eta[0]", "robot_eta[1]", "robot_eta[2]", "robot_eta[3]", "robot_eta[4]", "robot_eta[5]",
    //         //                     "posUUV.x", "posUUV.y", "posUUV.z", "rotUUV.x", "rotUUV.y", "rotUUV.z", 
    //         //                     "posBall.x", "posBall.y", "posBall.z", "rotBall.x", "rotBall.y", "rotBall.z"};                                         //
    //         // filename = expname + "_" + System.String.Format("{0:MM月dd日_HH時mm分ss秒fff}", System.DateTime.Now); //CHANGE POINT;                //
    //         // file = new FileInfo("C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/" + filename + ".csv");                                                                //
    //         //                                                                                                                                     //
    //         // sw = file.AppendText();                                                                                                              //
    //         // for (int i = 0; i < header.Length; i++)                                                                                              //
    //         // {                                                                                                                                    //
    //         //     sw.Write(header[i].ToString());                                                                                                  //
    //         //     sw.Write(",");                                                                                                                   //
    //         // }                                                                                                                                    //
    //         // sw.Write("\n");
    //         // }
    //         sw = Define_CSV();
    //         CSV_flag = 0;
    //     }
    // }
}
