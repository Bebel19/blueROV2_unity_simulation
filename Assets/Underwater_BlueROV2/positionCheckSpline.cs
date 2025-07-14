using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class positionCheckSpline : MonoBehaviour
{
    [SerializeField] private Transform Robot;
    [SerializeField] private Transform Splines;
    [SerializeField] private Transform Splines2;
    [SerializeField] private NearestPointFromRayExample NPRE;

    private float timeStamp = 0.0f;  //経過時間

    public string Participant_NAME;

    float[] logData_main;

    private StreamWriter SW_main;
    private int CSV_flag = 0;

    private Vector3 Spline_pos_0;
    private Quaternion Spline_quat_0;

    private Vector3 POS;
    private Quaternion QUAT;

    private int Start_check = 0;

    void Start()
    {
        SW_main = Spline_CSV(SW_main, Participant_NAME);
    }

    void FixedUpdate()
    {
        if (CSV_flag == 0){  
            Vector3 Spline_pos = Splines.position;
            Quaternion Spline_quat = Splines.rotation;
            Vector3 Spline_Euler = Splines.eulerAngles;

            Vector3 Spline_pos2 = Splines2.position;
            Quaternion Spline_quat2 = Splines2.rotation;
            Vector3 Spline_Euler2 = Splines2.eulerAngles;

            // if (Start_check == 0){
            //     Spline_pos_0 = Spline_pos;
            //     Spline_quat_0 = Spline_quat;
            //     Start_check = 100;
            // }

            // POS = Spline_pos - Spline_pos_0;
            // QUAT.w = Spline_quat_0.w * Spline_quat.w + Spline_quat_0.x * Spline_quat.x + Spline_quat_0.y * Spline_quat.y + Spline_quat_0.z * Spline_quat.z;
            // QUAT.x = Spline_quat_0.w * Spline_quat.x - Spline_quat.w * Spline_quat_0.x + Spline_quat.y * Spline_quat_0.z - Spline_quat.z * Spline_quat_0.y;
            // QUAT.y = Spline_quat_0.w * Spline_quat.y - Spline_quat.w * Spline_quat_0.y + Spline_quat.z * Spline_quat_0.x - Spline_quat.x * Spline_quat_0.z;
            // QUAT.z = Spline_quat_0.w * Spline_quat.z - Spline_quat.w * Spline_quat_0.z + Spline_quat.x * Spline_quat_0.y - Spline_quat.y * Spline_quat_0.x;

            // logData_main = new float[] {timeStamp, POS.x, POS.y, POS.z, QUAT.w, QUAT.x, QUAT.y, QUAT.z};
            logData_main = new float[] {timeStamp, Spline_pos.x, Spline_pos.y, Spline_pos.z, //Spline_quat.w, Spline_quat.x, Spline_quat.y, Spline_quat.z,
                                        Spline_Euler.x, Spline_Euler.y, Spline_Euler.z, 
                                        Spline_pos2.x, Spline_pos2.y, Spline_pos2.z, 
                                        Spline_Euler2.x, Spline_Euler2.y, Spline_Euler2.z, NPRE.TANGENT, NPRE.error_y, 
                                        Spline_pos.x - Spline_pos2.x, Spline_pos.y - Spline_pos2.y, Spline_pos.z - Spline_pos2.z, 
                                        Spline_Euler.x - Spline_Euler2.x, Spline_Euler.y - Spline_Euler2.y, Spline_Euler.z - Spline_Euler2.z, 
                                        };

            timeStamp += Time.deltaTime;

            for (int i = 0; i < logData_main.Length; i++)
            {
                SW_main.Write(logData_main[i].ToString());
                SW_main.Write(",");
            }
            SW_main.Write("\n");
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SW_main.Flush();
                SW_main.Close();
                CSV_flag = 1;
            }
        }else if (CSV_flag == 1){
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SW_main = Spline_CSV(SW_main, Participant_NAME);
                CSV_flag = 0;
            }
        }
    }

    private void OnApplicationQuit()
    {
        SW_main.Flush();
        SW_main.Close();
        CSV_flag = 1;
    }
    private StreamWriter Spline_CSV(StreamWriter CSV_wrokspace, string parname){
        string[] header = new string[] {"UnityTime [s]", "X", "Y", "Z", "RX", "RY", "RZ",//"Quat0", "Quat1", "Quat2", "Quat3", "RX", "RY", "RZ"};
                                        "X2", "Y2", "Z2", "RX2", "RY2", "RZ2","tangent", "ERROR y", 
                                        "X - X2", "Y - Y2", "Z - Z2", "RX - RX2", "RY - RY2", "RZ - RZ2"};

        // string filename = "MAIN" + System.String.Format("{0:MM_dd HH_mm_ss_ff}", System.DateTime.Now);
        string DirectoryPath = "C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/" + parname;
        if (System.IO.Directory.Exists(DirectoryPath) == false){
            System.IO.Directory.CreateDirectory(DirectoryPath);
        }
        int fileNum = System.IO.Directory.GetFiles(DirectoryPath).Length + 1;
        string filename = fileNum.ToString("00") + "Spline";
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
}
