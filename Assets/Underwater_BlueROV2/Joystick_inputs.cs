using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serial joystick interface for BlueROV2 simulation.
/// Converts raw joystick serial input into normalized velocity and yaw commands,
/// along with system confidence and PWM feedback.
/// </summary>
public class Joystick_inputs : MonoBehaviour
{
    public SerialHandler serialHandler;

    public Vector3 joy_pos;      // Raw joystick position [x, z]
    public Vector3 inputs;       // Output velocity vector (only x used)
    public float angle;          // Output yaw command
    public float confidence;     // Confidence value (0.0 to 1.0)
    public float pwmA;           // Raw PWM signal A
    public float pwmB;           // Raw PWM signal B

    private Vector3 joy_buf;     // Internal buffer for scaled joystick inputs

    void Start()
    {
        // Subscribe to serial input event
        serialHandler.OnDataReceived += OnDataReceived;
    }

    void Update()
    {
        // Normalize joystick lateral motion
        joy_buf.x = -2.5f * (joy_pos.x - 505.0f) / 390.0f;

        // Normalize yaw input
        joy_buf.y = 1.2f * (joy_pos.z - 482.0f) / 408.0f;

        // Only lateral motion (x) is used; y and z are unused
        inputs.y = 0.0f;
        inputs.z = 0.0f;

        // Apply deadzone for forward/backward motion
        if (Mathf.Abs(joy_buf.x) < 0.06f)
            inputs.x = 0.0f;
        else
            inputs.x = joy_buf.x;

        // Apply deadzone for yaw
        if (Mathf.Abs(joy_buf.y) < 0.2f)
            angle = 0.0f;
        else
            angle = joy_buf.y;
    }

    /// <summary>
    /// Called when a new serial message is received.
    /// Expected format: "x,z,confidence,pwmA,pwmB\n"
    /// </summary>
    /// <param name="message">Raw serial string</param>
    void OnDataReceived(string message)
    {
        var data = message.Split(new string[] { "\n" }, System.StringSplitOptions.None);

        try
        {
            var values = data[0].Split(',');

            joy_pos.x = float.Parse(values[0]);
            joy_pos.z = float.Parse(values[1]);
            confidence = float.Parse(values[2]);
            pwmA = float.Parse(values[3]);
            pwmB = float.Parse(values[4]);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message); // Display parsing error
        }
    }
}
