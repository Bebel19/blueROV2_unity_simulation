using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.Splines;


public class Spline_Follow : MonoBehaviour
{
    // スプライン
    [SerializeField] private SplineContainer _splineContainer;
    
    // スプラインに沿って移動させる対象
    [SerializeField] private Transform _followTarget;
    public string Participant_NAME;
    public Vector3 World_pos; 
    private StreamWriter csv;

    // 補間の割合
    [SerializeField, Range(0, 1)] private float _percentage;

    private void Start() {
        _percentage = 0.0f;
        // 新しくcsvファイルを作成して、{}の中の要素分csvに追記をする
        csv = new StreamWriter(@"SaveData.csv", false, Encoding.GetEncoding("Shift_JIS"));

        // CSV1行目のカラムで、StreamWriter オブジェクトへ書き込む
        string[] s1 = { "x", "y", "z" };

        /**
         * s1の文字列配列のすべての要素を「,」で連結する
         * @see https://docs.microsoft.com/ja-jp/dotnet/api/system.string.join?view=net-6.0#System_String_Join_System_String_System_String___
         */
        string s2 = string.Join(",", s1);

        /**
         * s2文字列をcsvファイルへ書き込む
         * @see https://docs.microsoft.com/ja-jp/dotnet/api/system.io.streamwriter.writeline?view=net-6.0#System_IO_StreamWriter_WriteLine_System_String_
         */
        csv.WriteLine(s2);
    }
    private void Update()
    {
        // 念のためNullチェック
        if (_splineContainer == null || _followTarget == null)
            return;


        // 計算した位置（ワールド座標）をターゲットに代入
        _followTarget.position = _splineContainer.EvaluatePosition(_percentage);
        World_pos = _followTarget.position;
        string[] s1 = { World_pos.x.ToString(), World_pos.y.ToString(), World_pos.z.ToString() };
        string s2 = string.Join(",", s1);
        csv.WriteLine(s2);
        if (_percentage >= 1.0f) {
            csv.Close();
            EndGame();
        }

        _percentage += 0.0001f;
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif 
    }
}
