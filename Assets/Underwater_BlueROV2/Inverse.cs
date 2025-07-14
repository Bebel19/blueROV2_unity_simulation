using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverse : MonoBehaviour
{
    NearestPointFromRayExample npe;

    [SerializeField] private Transform _inputPoint;
    [SerializeField] private Transform _outputPoint;

    Vector3 UUV_direction;
    Vector3 point_direction;

    public float angle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UUV_direction = _inputPoint.forward;
        point_direction = _outputPoint.forward;
        point_direction.y = 0.0f;
        var cosTheta = Vector3.Dot(UUV_direction, point_direction);
        var cross = Vector3.Cross(UUV_direction, point_direction - UUV_direction);
        var isClockwise = Vector3.Dot(cross, new Vector3(0,1,0)) > 0f;
        angle = isClockwise ? 
        Mathf.Acos(cosTheta) * Mathf.Rad2Deg :
        - Mathf.Acos(cosTheta) * Mathf.Rad2Deg;
        // angle = Vector3.Angle(UUV_direction, point_direction);

        



    }
}
