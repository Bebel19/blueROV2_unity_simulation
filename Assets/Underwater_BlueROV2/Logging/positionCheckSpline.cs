using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class positionCheckSpline : MonoBehaviour
{
    [SerializeField] private Transform Splines;
    [SerializeField] private Transform Splines2;
    [SerializeField] private NearestPointFromRayExample NPRE;

    public string Participant_NAME;

    private StreamWriter SW_main;
    private float timeStamp = 0.0f;
    private float[] logData_main;
    private int CSV_flag = 0;

    void Start()
    {
        SW_main = Spline_CSV(SW_main, Participant_NAME);
    }

    void FixedUpdate()
    {
        if (CSV_flag == 0)
        {
            Vector3 Spline_pos = Splines.position;
            Vector3 Spline_Euler = Splines.eulerAngles;

            Vector3 Spline_pos2 = Splines2.position;
            Vector3 Spline_Euler2 = Splines2.eulerAngles;

            // Record current spline and reference spline data with errors
            logData_main = new float[]
            {
                timeStamp,
                Spline_pos.x, Spline_pos.y, Spline_pos.z,
                Spline_Euler.x, Spline_Euler.y, Spline_Euler.z,
                Spline_pos2.x, Spline_pos2.y, Spline_pos2.z,
                Spline_Euler2.x, Spline_Euler2.y, Spline_Euler2.z,
                NPRE.TANGENT,
                NPRE.error_y,
                Spline_pos.x - Spline_pos2.x,
                Spline_pos.y - Spline_pos2.y,
                Spline_pos.z - Spline_pos2.z,
                Spline_Euler.x - Spline_Euler2.x,
                Spline_Euler.y - Spline_Euler2.y,
                Spline_Euler.z - Spline_Euler2.z
            };

            timeStamp += Time.deltaTime;

            for (int i = 0; i < logData_main.Length; i++)
            {
                SW_main.Write(logData_main[i].ToString());
                SW_main.Write(",");
            }
            SW_main.Write("\n");

            // End current log session on RETURN
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SW_main.Flush();
                SW_main.Close();
                CSV_flag = 1;
            }
        }
        else if (CSV_flag == 1)
        {
            // Start new log session on RETURN
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

    private StreamWriter Spline_CSV(StreamWriter csvWriter, string participantName)
    {
        string[] header = new string[]
        {
            "UnityTime [s]", "X", "Y", "Z", "RX", "RY", "RZ",
            "X2", "Y2", "Z2", "RX2", "RY2", "RZ2",
            "Tangent", "Error_Y",
            "ΔX", "ΔY", "ΔZ", "ΔRX", "ΔRY", "ΔRZ"
        };

        string directoryPath = "C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/" + participantName;
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        int fileNum = Directory.GetFiles(directoryPath).Length + 1;
        string filename = fileNum.ToString("00") + "Spline";
        FileInfo file = new FileInfo(Path.Combine(directoryPath, filename + ".csv"));

        csvWriter = file.AppendText();

        for (int i = 0; i < header.Length; i++)
        {
            csvWriter.Write(header[i]);
            csvWriter.Write(",");
        }
        csvWriter.Write("\n");

        return csvWriter;
    }
}
