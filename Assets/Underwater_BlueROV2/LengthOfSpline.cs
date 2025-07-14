using System;
using UnityEngine;
using UnityEngine.Splines;

public class LengthOfSpline : MonoBehaviour
{
    // スプライン
    [SerializeField] private SplineContainer _splineContainer;

    // スプラインに沿って移動させる対象
    // [SerializeField] private Transform _followTarget;

    public float lengthOFspline;
    // 始点から進む距離
    // [SerializeField] private float _distanceFromStart;

    private void Update()
    {
        // 念のためNullチェック
        if (_splineContainer == null)// || _followTarget == null)
            return;

        // 全体の道のり計算
        var length = _splineContainer.CalculateLength();
        lengthOFspline = length;

        // 進む距離と道のりから割合を算出
        // var percentage = _distanceFromStart / length;

        // 位置反映
        // _followTarget.position = _splineContainer.EvaluatePosition(percentage);
    }
}
