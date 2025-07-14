using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVReader : MonoBehaviour {

    TextAsset csvFile; // CSVファイル
    [SerializeField] private Transform ROV;
    public int p_num;
    public string textname;
    public string file_name; 

    List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;
    StringReader reader;
    Vector3 tmp;
    Vector3 tmpAngle;
    int Flag = 0;
    public int CSV_FLAG = 0;
    FileInfo file;

    int count = 0;
    StreamWriter SW_CSV;
    float[] logData_main;
    [SerializeField] private Transform spline_ball;
    [SerializeField] private CreateTexture CreT;

    float err_theta;

    Vector3 Errors;

    float Time_fixed = 0.0f;

    void Start()
    {
        // csvFile = Resources.Load(textname) as TextAsset; // Resouces下のCSV読み込み
        // reader = new StringReader(csvFile.text);
        CSV_FLAG = 1;
        count = 1;
        csvDatas = new List<string[]>();
        p_num += 1;
        textname = "previous";

        file_name = "psub7/psub7_" + textname + "CHECK";

        csvFile = Resources.Load(file_name) as TextAsset; // Resouces下のCSV読み込み
        reader = new StringReader(csvFile.text);

        string[] header = new string[] {"times", "error x", "error z","error angle", "error Py","error Pangle"};
        file = new FileInfo("C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/MainResult/" + file_name + "Errors.csv");

        SW_CSV = file.AppendText();
        for (int i = 0; i < header.Length; i++)             
        {                                                   
            SW_CSV.Write(header[i].ToString());                
            SW_CSV.Write(",");                                 
        }                                                   
        SW_CSV.Write("\n");
        // , で分割しつつ一行ずつ読み込み
        // リストに追加していく
        
        

        // csvDatas[行][列]を指定して値を自由に取り出せる
        // SW_CSV = Define_CSV(SW_CSV, Participant_NAME);


    }
    private void FixedUpdate() {
        if (CSV_FLAG == 0){
            count += 1;
            CSV_FLAG = 1;
            csvDatas = new List<string[]>();
            if (count % 2 == 0)
            {
                textname = "proposed";
            }
            else
            {
                p_num += 1;
                textname = "previous";

                if (p_num == 11) Application.Quit();
            }

            file_name = "psub" + p_num.ToString("0") + "/psub" + p_num.ToString("0") + "_" + textname + "CHECK";

            csvFile = Resources.Load(file_name) as TextAsset; // Resouces下のCSV読み込み
            reader = new StringReader(csvFile.text);

            string[] header = new string[] {"times", "error x", "error z","error angle", "error Py","error Pangle"};
            file = new FileInfo("C:/Users/hurol/Desktop/RA/RA-Unity/BlueROV simulator 2022/Unity_sim_CSV/MainResult/" + file_name + "Errors.csv");

            SW_CSV = file.AppendText();
            for (int i = 0; i < header.Length; i++)             
            {                                                   
                SW_CSV.Write(header[i].ToString());                
                SW_CSV.Write(",");                                 
            }                                                   
            SW_CSV.Write("\n");

        }else{
            if (reader.Peek() != -1) // reader.Peaekが-1になるまで
            {
                string line = reader.ReadLine(); // 一行ずつ読み込み
                if (Flag == 1){
                    tmp.x = float.Parse(line.Split(',')[1]);
                    tmp.y = float.Parse(line.Split(',')[2]);
                    tmp.z = float.Parse(line.Split(',')[3]);
                    tmpAngle.x = float.Parse(line.Split(',')[4]);
                    tmpAngle.y = float.Parse(line.Split(',')[5]);
                    tmpAngle.z = float.Parse(line.Split(',')[6]);
                    ROV.position = tmp;
                    ROV.eulerAngles = tmpAngle + new Vector3(90.0f, 0.0f, 0.0f);

                    Errors = ROV.position - spline_ball.position;
                    // float er_y = Mathf.Sqrt(Mathf.Pow(Errors.x, 2.0f) + Mathf.Pow(Errors.z, 2.0f));

                    err_theta = ROV.eulerAngles.y - spline_ball.eulerAngles.y;

                    logData_main = new float[] {Time_fixed, Errors.x, Errors.z, err_theta, CreT.errory_mat, CreT.errorag_mat};
                    for (int i = 0; i < logData_main.Length; i++)
                    {
                        SW_CSV.Write(logData_main[i].ToString());
                        SW_CSV.Write(",");
                    }

                    SW_CSV.Write("\n");
                    Time_fixed += 0.005f;


                }else{
                    Flag = 1;
                }
                // csvDatas.Add(line.Split(',')); // , 区切りでリストに追加
                
            }else{
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
