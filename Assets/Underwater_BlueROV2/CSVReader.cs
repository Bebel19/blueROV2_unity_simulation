using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Replays recorded trajectories from CSV and logs positional/angular error compared to the spline reference.
/// Outputs results into new CSV files for performance analysis.
/// </summary>
public class CSVReader : MonoBehaviour
{
    [SerializeField] private Transform ROV;
    [SerializeField] private Transform spline_ball;
    [SerializeField] private CreateTexture CreT;

    public int p_num;
    public string textname;
    public string file_name;

    TextAsset csvFile;
    StringReader reader;
    FileInfo file;
    StreamWriter SW_CSV;

    List<string[]> csvDatas = new List<string[]>(); // Unused
    Vector3 tmp;
    Vector3 tmpAngle;
    Vector3 Errors;

    float Time_fixed = 0.0f;
    float err_theta;

    public int CSV_FLAG = 0;
    int Flag = 0;
    int count = 0;
    float[] logData_main;

    void Start()
    {
        CSV_FLAG = 1;
        count = 1;
        csvDatas = new List<string[]>();
        p_num += 1;
        textname = "previous";
        file_name = "psub7/psub7_" + textname + "CHECK";

        // Load initial trajectory CSV from Resources
        csvFile = Resources.Load(file_name) as TextAsset;
        reader = new StringReader(csvFile.text);

        // Create results file to store errors
        string[] header = { "times", "error x", "error z", "error angle", "error Py", "error Pangle" };
        file = new FileInfo("C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/MainResult/" + file_name + "Errors.csv");

        SW_CSV = file.AppendText();
        foreach (var h in header)
        {
            SW_CSV.Write(h);
            SW_CSV.Write(",");
        }
        SW_CSV.Write("\n");
    }

    private void FixedUpdate()
    {
        if (CSV_FLAG == 0)
        {
            count++;
            CSV_FLAG = 1;
            csvDatas = new List<string[]>();

            if (count % 2 == 0)
                textname = "proposed";
            else
            {
                p_num++;
                textname = "previous";
                if (p_num == 11) Application.Quit();
            }

            file_name = "psub" + p_num.ToString("0") + "/psub" + p_num.ToString("0") + "_" + textname + "CHECK";

            csvFile = Resources.Load(file_name) as TextAsset;
            reader = new StringReader(csvFile.text);

            string[] header = { "times", "error x", "error z", "error angle", "error Py", "error Pangle" };
            file = new FileInfo("C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/MainResult/" + file_name + "Errors.csv");

            SW_CSV = file.AppendText();
            foreach (var h in header)
            {
                SW_CSV.Write(h);
                SW_CSV.Write(",");
            }
            SW_CSV.Write("\n");
        }
        else
        {
            if (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                if (Flag == 1)
                {
                    string[] values = line.Split(',');

                    tmp.x = float.Parse(values[1]);
                    tmp.y = float.Parse(values[2]);
                    tmp.z = float.Parse(values[3]);
                    tmpAngle.x = float.Parse(values[4]);
                    tmpAngle.y = float.Parse(values[5]);
                    tmpAngle.z = float.Parse(values[6]);

                    ROV.position = tmp;
                    ROV.eulerAngles = tmpAngle + new Vector3(90.0f, 0.0f, 0.0f); // Rotate to match Unity coords

                    Errors = ROV.position - spline_ball.position;
                    err_theta = ROV.eulerAngles.y - spline_ball.eulerAngles.y;

                    logData_main = new float[] {
                        Time_fixed,
                        Errors.x,
                        Errors.z,
                        err_theta,
                        CreT.errory_mat,
                        CreT.errorag_mat
                    };

                    foreach (var value in logData_main)
                    {
                        SW_CSV.Write(value.ToString());
                        SW_CSV.Write(",");
                    }
                    SW_CSV.Write("\n");
                    Time_fixed += 0.005f;
                }
                else
                {
                    Flag = 1; // Skip first line (likely headers)
                }
            }
            else
            {
                CSV_FLAG = 0;
                SW_CSV.Flush();
                SW_CSV.Close();
                Time_fixed = 0.0f;
            }
        }
    }

    private void OnApplicationQuit()
    {
        SW_CSV.Flush();
        SW_CSV.Close();
    }
}
