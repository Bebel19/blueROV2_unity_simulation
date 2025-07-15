using System;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Moves a Transform along a Spline path at a fixed rate (based on time).
/// Useful for simulating reference trajectories.
/// </summary>
public class RootSpline : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;

    // Target object to move along the spline
    [SerializeField] private Transform _followTarget;

    // Travel distance from the start of the spline
    [SerializeField] private float _distanceFromStart = 0.0f;

    private float timeStamp = 0.0f;

    // Simulation rate: 60 updates per second
    private float freq = 60.0f;

    private void Start()
    {
        if (_splineContainer == null || _followTarget == null)
            return;

        _distanceFromStart = timeStamp / freq;

        Vector3 position = _splineContainer.EvaluatePosition(_distanceFromStart);      
        Vector3 tangent = _splineContainer.EvaluateTangent(_distanceFromStart);
        Vector3 upVector = _splineContainer.EvaluateUpVector(_distanceFromStart);

        _followTarget.rotation = Quaternion.LookRotation(tangent, upVector);
        _followTarget.position = position;
    }

    private void FixedUpdate()
    {
        if (_splineContainer == null || _followTarget == null)
            return;

        _distanceFromStart = timeStamp / freq;

        Vector3 position = _splineContainer.EvaluatePosition(_distanceFromStart);      
        Vector3 tangent = _splineContainer.EvaluateTangent(_distanceFromStart);
        Vector3 upVector = _splineContainer.EvaluateUpVector(_distanceFromStart);

        _followTarget.rotation = Quaternion.LookRotation(tangent, upVector);
        _followTarget.position = position;

        timeStamp += Time.deltaTime;
    }
}