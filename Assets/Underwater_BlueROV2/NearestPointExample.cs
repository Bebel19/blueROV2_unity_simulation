using UnityEngine;
using UnityEngine.Splines;

public class NearestPointExample : MonoBehaviour
{
    // スプライン
    [SerializeField] private SplineContainer _spline;

    // 入力位置のゲームオブジェクト
    [SerializeField] private Transform _inputPoint;

    // 出力位置（直近位置）を反映するゲームオブジェクト
    [SerializeField] public Transform _outputPoint;

    // 解像度
    // 内部的にPickResolutionMin～PickResolutionMaxの範囲に丸められる
    [SerializeField] 
    [Range(SplineUtility.PickResolutionMin, SplineUtility.PickResolutionMax)]
    private int _resolution = 4;

    // 計算回数
    // 内部的に10回以下に丸められる
    [SerializeField]
    [Range(1, 10)]
    private int _iterations = 2;

    private void Update()
    {
        // Nullチェック
        if (_spline == null || _inputPoint == null || _outputPoint == null)
            return;

        // ワールド空間におけるスプラインを取得
        // スプラインはローカル空間なので、ローカル→ワールド変換行列を掛ける
        // Updateを抜けるタイミングでDisposeされる
        using var spline = new NativeSpline(_spline.Spline, _spline.transform.localToWorldMatrix);

        // スプラインにおける直近位置を求める
        var distance = SplineUtility.GetNearestPoint(
            spline,
            _inputPoint.position,
            out var nearest,
            out var t,
            _resolution,
            _iterations
        );

        // 結果を反映
        _outputPoint.position = nearest;
        var length = _spline.CalculateLength();
    }
}
