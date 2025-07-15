using System;
using UnityEngine;
using UnityEngine.Splines;

public class LengthOfSpline : MonoBehaviour
{
    // Reference to the spline container
    [SerializeField] private SplineContainer _splineContainer;

    // Output: total spline length
    public float lengthOFspline;

    private void Update()
    {
        // Null check for safety
        if (_splineContainer == null)
            return;

        // Calculate total arc length of the spline
        var length = _splineContainer.CalculateLength();
        lengthOFspline = length;
    }
}