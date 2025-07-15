using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Computes the signed angle between the ROV's forward direction and a reference direction.
/// Used to determine angular misalignment in the horizontal plane.
/// </summary>
public class Inverse : MonoBehaviour
{
    // Dependency providing nearest point (not used here, possibly legacy)
    NearestPointFromRayExample npe;

    [SerializeField] private Transform _inputPoint;   // The ROV or UUV's current direction
    [SerializeField] private Transform _outputPoint;  // Target or spline point direction

    Vector3 UUV_direction;
    Vector3 point_direction;

    public float angle;  // Signed horizontal angle (degrees)

    void Update()
    {
        // Get forward direction of the ROV
        UUV_direction = _inputPoint.forward;

        // Get forward direction of the target and project onto XZ plane
        point_direction = _outputPoint.forward;
        point_direction.y = 0.0f;

        // Compute cosine of angle between ROV and target direction
        float cosTheta = Vector3.Dot(UUV_direction, point_direction);

        // Compute cross product to determine rotation direction (CW or CCW)
        Vector3 cross = Vector3.Cross(UUV_direction, point_direction - UUV_direction);
        bool isClockwise = Vector3.Dot(cross, Vector3.up) > 0f;

        // Compute signed angle in degrees
        angle = isClockwise ?
            Mathf.Acos(cosTheta) * Mathf.Rad2Deg :
            -Mathf.Acos(cosTheta) * Mathf.Rad2Deg;

        // Alternate method (unsigned angle): Vector3.Angle(UUV_direction, point_direction);
    }
}