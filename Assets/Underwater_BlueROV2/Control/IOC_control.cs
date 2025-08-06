using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Underwater_BlueROV2;

/// <summary>
/// Implements inverse optimal control (IOC) for shared angular command correction.
/// Combines user velocity, vision-based errors, and system confidence.
/// </summary>
public class IOC_control : MonoBehaviour
{
    public CreateTexture CreTex;           // Provides visual errors and confidence
    [SerializeField] private InputManager inputManager; // New: for unified input access // Provides the active input (Gamepad or Joystick)
    [SerializeField] private MappingMatrix mapper; // Used to map the inputs J (Joystick or controller values) to a command vector U (target velocities)
    [SerializeField] private ROV_dynamics RD; // Access to current angular velocity
    [SerializeField] private space SP;     // Provides kill switch toggle (0 or 1)

    public float error_y;
    public float error_angle;

    public float vel_angle;
    public float joy_send_y;
    public float joy_send_angle = 0.0f;

    public float before_send_y;
    public float before_send_angle = 0.0f;

    public float beta = 0.0f;
    public float sys_conf = 0.0f;
    public float Vel = 0.0f;

    public float ThresHoldY = 2.0f;
    public float ThresHoldT = 0.1f;
    public int Kill_switch = 1;

    public float a;
    public float b;
    public float Ka = 4.1f;
    public float Kb = 1.05f;
    public float vr;

    private float before_u = 0.0f;
    private float input_u = 0.0f;
    private float Cutoff_u = 0.5f;

    void Start()
    {
        // Initial error estimation
        error_y = CreTex.errory_mat / 100.0f;
        error_angle = CreTex.errorag_mat * Mathf.Deg2Rad;
        Vel = mapper.GetMappedCommand(inputManager.GetInputs())[0]; // Map the controller input to target speeds and i=O is the x translational speed
        float result;

        // Shared control weight based on confidence
        beta = (float)Kill_switch * Mathf.Atan(error_y);

        sys_conf = CreTex.confidence;

        float CosErr = Mathf.Cos(error_angle);
        if (CosErr != 0.0f)
            vr = (Vel * Mathf.Cos(beta) - error_y * RD.nu_now[5]) / CosErr;
        else
            vr = 0.0f;

        float f1 = vr * Mathf.Sin(error_angle) - Vel * Mathf.Sin(beta);
        a = Ka * error_y * f1 + Kb;
        b = -Kb * error_angle;

        if (b == 0.0f)
            input_u = 0.0f;
        else
            input_u = -Kill_switch * (a + Mathf.Sqrt(a * a + Mathf.Pow(b, 4))) / b;

        // Clamp output
        if (Mathf.Abs(input_u) > 1.2f)
            input_u = 1.2f * Mathf.Sign(input_u);

        // Apply low-pass filter
        result = before_u + (input_u - before_u) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff_u;
        before_u = result;

        joy_send_angle = result;
    }

    void FixedUpdate()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        // Update kill switch (1 = on, 0 = off)
        Kill_switch = SP.space_is;

        // Error update
        error_y = CreTex.errory_mat / 100.0f;
        error_angle = CreTex.errorag_mat * Mathf.Deg2Rad;
        Vel = mapper.GetMappedCommand(inputManager.GetInputs())[0]; // Map the controller input to target speeds and i=O is the x translational speed

        float result;

        // Shared control weight (beta) modulated by confidence
        beta = (float)Kill_switch * Mathf.Atan(error_y);
        sys_conf = CreTex.confidence;

        float CosErr = Mathf.Cos(error_angle);
        if (CosErr != 0.0f)
            vr = (Vel * Mathf.Cos(beta) - error_y * RD.nu_now[5]) / CosErr;
        else
            vr = 0.0f;

        float f1 = vr * Mathf.Sin(error_angle) - Vel * Mathf.Sin(beta);
        a = Ka * error_y * f1;
        b = -Kb * error_angle;

        if (b == 0.0f)
            input_u = 0.0f;
        else
            input_u = -Kill_switch * (a + Mathf.Sqrt(a * a + Mathf.Pow(b, 4))) / b;

        if (Mathf.Abs(input_u) > 1.2f)
            input_u = 1.2f * Mathf.Sign(input_u);

        result = before_u + (input_u - before_u) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff_u;
        before_u = result;

        joy_send_angle = result;

        sw.Stop();
        Debug.Log($"[IOC_control] Elapsed {sw.ElapsedMilliseconds} ms");
    }
}
