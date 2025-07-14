using System;
using UnityEngine;
using UnityEngine.Splines;

public class RootSpline : MonoBehaviour
{
    // スプライン
    [SerializeField] private SplineContainer _splineContainer;
    

    // スプラインに沿って移動させる対象
    [SerializeField] private Transform _followTarget;

    // 始点から進む距離
    [SerializeField] private float _distanceFromStart = 0.0f;
    private float timeStamp = 0.0f;

    private Vector3 POS_TMP_Before;
    private float freq = 60.0f;

    private void Start()
    {
        // 念のためNullチェック
        if (_splineContainer == null || _followTarget == null)
            return;

        // 全体の道のり計算
        // var length = _splineContainer.CalculateLength();

        _distanceFromStart = timeStamp / freq;

        // 進む距離と道のりから割合を算出
        // var percentage = _distanceFromStart / length;

        // 位置反映
        Vector3 POS_TMP = _splineContainer.EvaluatePosition(_distanceFromStart);      
        var angles = _splineContainer.EvaluateTangent(_distanceFromStart);
        var UpVect = _splineContainer.EvaluateUpVector(_distanceFromStart);

        // POS_TMP -= 10.0f * _followTarget.right;

        _followTarget.rotation = Quaternion.LookRotation(angles, UpVect);
        _followTarget.position = POS_TMP;
    }

    private void FixedUpdate()
    {
        // 念のためNullチェック
        if (_splineContainer == null || _followTarget == null)
            return;

        // 全体の道のり計算
        // var length = _splineContainer.CalculateLength();

        _distanceFromStart = timeStamp / freq;

        // 進む距離と道のりから割合を算出
        // var percentage = _distanceFromStart / length;

        // 位置反映
        Vector3 POS_TMP = _splineContainer.EvaluatePosition(_distanceFromStart);      
        var angles = _splineContainer.EvaluateTangent(_distanceFromStart);
        var UpVect = _splineContainer.EvaluateUpVector(_distanceFromStart);

        // POS_TMP -= 10.0f * _followTarget.right;

        _followTarget.rotation = Quaternion.LookRotation(angles, UpVect);
        _followTarget.position = POS_TMP;

        timeStamp += Time.deltaTime;
    }
}
