using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class NearestPointFromRayExample : MonoBehaviour
{
    public float error_y;
    [SerializeField] private SplineContainer _spline;
    public float TANGENT;
    // スプライン
    [Serializable]
    private struct PathInfo
    {
        // SplineContainerのどのスプラインを使うかをインデックスで指定
        public int splineIndex;
        
        // 上記インデックスのスプラインにおける範囲情報
        public SplineRange range;
    }

    

    // 入力Rayのゲームオブジェクト
    [SerializeField] private Transform _inputRay;

    // 出力位置（直近位置）を反映するゲームオブジェクト
    [SerializeField] private Transform _outputPoint;

    [SerializeField] public Vector3 _tangent;
    [SerializeField] private PathInfo[] _path;
    // 解像度
    // 内部的にPickResolutionMin～PickResolutionMaxの範囲に丸められる
    [SerializeField] 
    [Range(SplineUtility.PickResolutionMin, SplineUtility.PickResolutionMax)]
    private int _resolution = 4;
    private SplinePath _splinePath;
    // 計算回数
    // 内部的に10回以下に丸められる
    [SerializeField]
    [Range(1, 100)]
    private int _iterations = 2;


    private void Start()
    {
        OnCreateSplinePath();
        // Nullチェック
        if (_spline == null || _inputRay == null || _outputPoint == null || _splinePath == null){
            Debug.Log("NULL");
            return;
        }
        // ワールド空間におけるスプラインを取得
        // スプラインはローカル空間なので、ローカル→ワールド変換行列を掛ける
        // Updateを抜けるタイミングでDisposeされる
        using var spline = new NativeSpline(_spline.Spline, _spline.transform.localToWorldMatrix);
         // ワールド空間のスプラインとして扱うため、変換行列を指定する

        
        // スプラインにおける直近位置を求める
        var distance = SplineUtility.GetNearestPoint(
            spline,
            new Ray(_inputRay.position, _inputRay.right),
            out var nearest,
            out var t,
            _resolution,
            _iterations
        );
        // Debug.Log(t);
        // if (!_splinePath.Evaluate(t, out var position, out var tangent, out var upVector)){
        //     // Debug.Log(_splinePath.Evaluate(t, out var position, out var tangent, out var upVector));
        //     return;
        // }
        // _tangent = tangent;
        _tangent = SplineUtility.EvaluateTangent(spline, t);
        TANGENT = _tangent.y;
        var upVector = SplineUtility.EvaluateUpVector(spline, t);

        // 結果を反映
        error_y = distance * Mathf.Sqrt(Mathf.Pow(_tangent.x, 2.0f) + Mathf.Pow(_tangent.z, 2.0f));
        _outputPoint.position = nearest;
        _outputPoint.rotation = Quaternion.LookRotation(_tangent, upVector);
        // _outputPoint.SetPositionAndRotation(
        //     nearest,
        //     Quaternion.LookRotation(tangent, upVector)
        // );
    }
    private void FixedUpdate()
    {
        // Nullチェック
        if (_spline == null || _inputRay == null || _outputPoint == null || _splinePath == null){
            Debug.Log("NULL");
            return;
        }
        // ワールド空間におけるスプラインを取得
        // スプラインはローカル空間なので、ローカル→ワールド変換行列を掛ける
        // Updateを抜けるタイミングでDisposeされる
        using var spline = new NativeSpline(_spline.Spline, _spline.transform.localToWorldMatrix);
         // ワールド空間のスプラインとして扱うため、変換行列を指定する

        
        // スプラインにおける直近位置を求める
        var distance = SplineUtility.GetNearestPoint(
            spline,
            new Ray(_inputRay.position, _inputRay.right),
            out var nearest,
            out var t,
            _resolution,
            _iterations
        );
        // Debug.Log(t);
        // if (!_splinePath.Evaluate(t, out var position, out var tangent, out var upVector)){
        //     // Debug.Log(_splinePath.Evaluate(t, out var position, out var tangent, out var upVector));
        //     return;
        // }
        // _tangent = tangent;
        _tangent = SplineUtility.EvaluateTangent(spline, t);
        TANGENT = _tangent.y;
        var upVector = SplineUtility.EvaluateUpVector(spline, t);

        // 結果を反映
        error_y = distance;
        _outputPoint.position = nearest;
        _outputPoint.rotation = Quaternion.LookRotation(_tangent, upVector);
        // _outputPoint.SetPositionAndRotation(
        //     nearest,
        //     Quaternion.LookRotation(tangent, upVector)
        // );

    }

    private void OnCreateSplinePath()
    {
        if (_spline == null){
            Debug.Log("Spline is Null");
            return;
        }

        // ワールド空間のスプラインとして扱うため、変換行列を指定する
        float4x4 matrix = _spline.transform.localToWorldMatrix;

        // 経路の作成情報からSplinePathインスタンスを作成
        _splinePath = new SplinePath(
            // PathInfoからSplineSlice型のコレクションに変換
            _path.Select(x => new SplineSlice<Spline>(
                    _spline[x.splineIndex],
                    x.range,
                    matrix
                )
            )
        );
    }
}