using UnityEngine;
using UnityEngine.Splines;

public class NearestPointExample : MonoBehaviour
{
    // Reference to the spline container
    [SerializeField] private SplineContainer _spline;

    // Input point in world space (e.g., current robot position)
    [SerializeField] private Transform _inputPoint;

    // Output transform that will be moved to the nearest spline point
    [SerializeField] public Transform _outputPoint;

    // Search resolution (clamped internally between PickResolutionMin and Max)
    [SerializeField] 
    [Range(SplineUtility.PickResolutionMin, SplineUtility.PickResolutionMax)]
    private int _resolution = 4;

    // Number of refinement iterations (clamped internally to max 10)
    [SerializeField]
    [Range(1, 10)]
    private int _iterations = 2;

    private void Update()
    {
        // Null check for safety
        if (_spline == null || _inputPoint == null || _outputPoint == null)
            return;

        // Convert spline to world space using its local-to-world matrix
        using var spline = new NativeSpline(_spline.Spline, _spline.transform.localToWorldMatrix);

        // Compute the nearest point on the spline to the input position
        var distance = SplineUtility.GetNearestPoint(
            spline,
            _inputPoint.position,
            out var nearest,
            out var t,
            _resolution,
            _iterations
        );

        // Set the output transform to the nearest point
        _outputPoint.position = nearest;
    }
}