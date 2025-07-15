using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class NearestPointFromRayExample : MonoBehaviour
{
    [SerializeField] private SplineContainer _spline;

    // Input ray origin and direction (usually robot transform)
    [SerializeField] private Transform _inputRay;

    // Output point updated with the nearest spline projection
    [SerializeField] private Transform _outputPoint;

    // Output tangent vector at nearest point
    [SerializeField] public Vector3 _tangent;

    // Public debug access to tangent Y component
    public float TANGENT;

    // Lateral error (projected distance from ray to spline)
    public float error_y;

    [SerializeField] private PathInfo[] _path;

    // Spline search resolution (internally clamped by Unity)
    [SerializeField]
    [Range(SplineUtility.PickResolutionMin, SplineUtility.PickResolutionMax)]
    private int _resolution = 4;

    // Number of refinement iterations (max 100)
    [SerializeField]
    [Range(1, 100)]
    private int _iterations = 2;

    private SplinePath _splinePath;

    [Serializable]
    private struct PathInfo
    {
        public int splineIndex;     // Index of spline in the container
        public SplineRange range;   // Range to extract from this spline
    }

    private void Start()
    {
        OnCreateSplinePath();
    }

    private void FixedUpdate()
    {
        if (_spline == null || _inputRay == null || _outputPoint == null || _splinePath == null)
        {
            Debug.Log("NULL");
            return;
        }

        // Convert spline to world space using its transform
        using var spline = new NativeSpline(_spline.Spline, _spline.transform.localToWorldMatrix);

        // Find the nearest point on the spline from a world-space ray
        var distance = SplineUtility.GetNearestPoint(
            spline,
            new Ray(_inputRay.position, _inputRay.right),
            out var nearest,
            out var t,
            _resolution,
            _iterations
        );

        // Evaluate tangent and up vector at the spline parameter t
        _tangent = SplineUtility.EvaluateTangent(spline, t);
        TANGENT = _tangent.y;
        var upVector = SplineUtility.EvaluateUpVector(spline, t);

        // Compute lateral error (horizontal projection)
        error_y = distance;

        // Apply position and orientation to output transform
        _outputPoint.position = nearest;
        _outputPoint.rotation = Quaternion.LookRotation(_tangent, upVector);
    }

    private void OnCreateSplinePath()
    {
        if (_spline == null)
        {
            Debug.Log("Spline is Null");
            return;
        }

        float4x4 matrix = _spline.transform.localToWorldMatrix;

        // Create a multi-segment spline path based on the specified ranges
        _splinePath = new SplinePath(
            _path.Select(x => new SplineSlice<Spline>(
                _spline[x.splineIndex],
                x.range,
                matrix
            ))
        );
    }
}
