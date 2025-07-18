using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Underwater_BlueROV2;

/// <summary>
/// Main 6-DOF PID controller for the BlueROV2.
/// Computes desired forces and torques (desired_tau) from joystick inputs and vehicle state feedback.
/// </summary>
public class Controller : MonoBehaviour
{
    [SerializeField] private InputManager inputManager; // New: for unified input access // Provides the active input (Gamepad or Joystick)
    [SerializeField] private MappingMatrix mapper; // Used to map the inputs J (Joystick or controller values) to a command vector U (target velocities)
    public Small_terrain_height Terra_H; 
    public IOC_control IOOC;

    public float Tall = 2.0f;

    float[] M_RB;
    float[] M_A;
    float[] D_O;
    float[] D_N;
    float[] G;

    public float[,] Jacv_1;
    float[,] Jacv_2;

    float[] M;
    float[] C_nu;
    float[] D;
    public float[] nu_now;
    float[] nu_now_dot;
    float[] nu_now_ref;
    float[] nu_now_ref_dot;
    public float[] eta_now;
    public float[] eta_ref;
    float[] eta_now_dot;

    float[] nu_bef;
    float[] nu_bef_ref;
    float[] eta_bef;
    float[] eta_bef_dot;
    public float[] PID_nu;
    float[] PID_Integral;
    public float Kp = 0.2f;
    public float Ki = 0.0f;
    public float Kd = 0.17f;

    public float[] desired_tau;
    public float dt;

    float mass = 13.5f;
    float[] I_c;
    float gravity = 9.82f;
    float Volume = 0.0135f;
    float rho = 1000.0f;
    float z_b = -0.01f;

    public float W;
    public float B;

    int i;

    public float[] eta_error;
    public float[] robot_error;
    float[] robot_error_bef;

    Vector3 pos_buf;
    Vector3 angle_buf;

    void Start()
    {
        // Initialize arrays
        M_RB = new float[6];
        M_A = new float[6];
        M = new float[6];
        C_nu = new float[6];
        D_O = new float[6];
        D_N = new float[6];
        D = new float[6];
        G = new float[6];

        Jacv_1 = new float[3, 3];
        Jacv_2 = new float[3, 3];

        nu_now = new float[6];
        nu_now_dot = new float[6];
        eta_now = new float[6];
        eta_ref = new float[6];
        eta_now_dot = new float[6];
        nu_bef = new float[6];
        eta_bef = new float[6];
        eta_bef_dot = new float[6];

        eta_error = new float[6];
        robot_error = new float[6];
        robot_error_bef = new float[6];
        PID_nu = new float[3];
        PID_Integral = new float[3];

        I_c = new float[3] { 0.26f, 0.23f, 0.37f };

        desired_tau = new float[6];

        // Rigid-body mass and inertia
        M_RB[0] = mass;
        M_RB[1] = mass;
        M_RB[2] = mass;
        M_RB[3] = I_c[0];
        M_RB[4] = I_c[1];
        M_RB[5] = I_c[2];

        // Added mass coefficients
        M_A[0] = 6.356673886738176f;
        M_A[1] = 7.120600295756984f;
        M_A[2] = 18.686326861534997f;
        M_A[3] = 0.185765630747592f;
        M_A[4] = 0.134823349429660f;
        M_A[5] = 0.221510466644690f;

        // Gravity and buoyancy forces
        W = mass * gravity;
        B = -rho * gravity * Volume;

        // Initialize position and orientation (eta_now)
        eta_now[0] = transform.position.z;
        eta_now[1] = transform.position.x;
        eta_now[2] = 200.0f - transform.position.y;
        eta_now[3] = -Mathf.Deg2Rad * transform.eulerAngles.z;
        eta_now[4] = -Mathf.Deg2Rad * transform.eulerAngles.x;
        eta_now[5] = Mathf.Deg2Rad * transform.eulerAngles.y;

        // Set reference: keep x, y, yaw; set z from terrain height
        eta_ref[0] = eta_now[0];
        eta_ref[1] = eta_now[1];
        eta_ref[2] = 200.0f - Terra_H.Terrain_height - Tall;
        eta_ref[3] = eta_now[3];
        eta_ref[4] = eta_now[4];
        eta_ref[5] = eta_now[5];

        // Compute world-frame error
        for (i = 0; i < 6; i++)
            eta_error[i] = eta_ref[i] - eta_now[i];

        // Compute Jacobian from Euler angles
        Jacv_1[0, 0] = Mathf.Cos(eta_now[5]) * Mathf.Cos(eta_now[4]);
        Jacv_1[0, 1] = Mathf.Cos(eta_now[4]) * Mathf.Sin(eta_now[5]);
        Jacv_1[0, 2] = -Mathf.Sin(eta_now[4]);
        Jacv_1[1, 0] = Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[4]) - Mathf.Cos(eta_now[3]) * Mathf.Sin(eta_now[5]);
        Jacv_1[1, 1] = Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[5]) + Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[5]) * Mathf.Sin(eta_now[4]);
        Jacv_1[1, 2] = Mathf.Cos(eta_now[4]) * Mathf.Sin(eta_now[5]);
        Jacv_1[2, 0] = Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[5]) + Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[4]);
        Jacv_1[2, 1] = Mathf.Cos(eta_now[3]) * Mathf.Sin(eta_now[5]) * Mathf.Sin(eta_now[4]) - Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[3]);
        Jacv_1[2, 2] = Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[4]);

        // Convert world error to body-frame error and reset integrators
        for (int n = 0; n < 3; n++)
        {
            float buf_err = 0.0f;
            for (int m = 0; m < 3; m++)
            {
                buf_err += Jacv_1[n, m] * eta_error[m];
            }
            robot_error[n] = buf_err;
            robot_error_bef[n] = robot_error[n];
            PID_Integral[n] = 0.0f;
            PID_nu[n] = 0.0f;
        }

        // Compute gravity/buoyancy compensation
        G[0] = Jacv_1[0, 2] * (W + B);
        G[1] = Jacv_1[1, 2] * (W + B);
        G[2] = Jacv_1[2, 2] * (W + B);
        G[3] = z_b * Jacv_1[1, 2] * B;
        G[4] = -z_b * Jacv_1[0, 2] * B;
        G[5] = 0.0f;

        // Initial velocity estimate
        float[] U = mapper.GetMappedCommand(inputManager.GetInputs()); // Map the controller input to target speeds command vector U
        
        nu_now[0] = U[0] * Mathf.Cos(IOOC.beta);
        nu_now[1] = U[0] * Mathf.Sin(IOOC.beta);
        nu_now[2] = U[2];
        nu_now[3] = 0.0f;
        nu_now[4] = 0.0f;
        nu_now[5] = U[5];

        // Store initial values for derivative
        for (i = 0; i < 6; i++)
        {
            nu_bef[i] = nu_now[i];
            nu_now_dot[i] = 0.0f;
        }

        // Compute Coriolis matrix
        C_nu[0] = -(M_A[2] + mass) * nu_now[5] * nu_now[1] + (M_A[1] + mass) * nu_now[4] * nu_now[2];
        C_nu[1] = (M_A[2] + mass) * nu_now[5] * nu_now[0] - (M_A[0] + mass) * nu_now[3] * nu_now[2];
        C_nu[2] = -(M_A[1] + mass) * nu_now[4] * nu_now[0] + (M_A[0] + mass) * nu_now[3] * nu_now[1];
        C_nu[3] = -(M_A[5] + I_c[2] - M_A[4] - I_c[1]) * nu_now[4] * nu_now[5];
        C_nu[4] = (M_A[5] + I_c[2] - M_A[3] - I_c[0]) * nu_now[3] * nu_now[5];
        C_nu[5] = -(M_A[4] + I_c[1] - M_A[3] - I_c[0]) * nu_now[3] * nu_now[4];

        // Damping (linear + nonlinear)
        D_O[0] = 141.0f;
        D_O[1] = 217.0f;
        D_O[2] = 190.0f;
        D_O[3] = 1.192f;
        D_O[4] = 0.47f;
        D_O[5] = 1.5f;

        D_N[0] = 13.7f * nu_now[0];
        D_N[1] = 0.0f;
        D_N[2] = 33.0f * nu_now[2];
        D_N[3] = 0.0f;
        D_N[4] = 0.8f * nu_now[4];
        D_N[5] = 0.0f;

        // Final damping and total mass
        for (i = 0; i < 6; i++)
        {
            M[i] = M_RB[i] + M_A[i];
            D[i] = D_O[i] + D_N[i];
        }
    }

    void Update()
    {
        dt = Time.deltaTime;

        // Update current state from transform
        eta_now[0] = transform.position.z;
        eta_now[1] = transform.position.x;
        eta_now[2] = 200.0f - transform.position.y;
        eta_now[3] = -Mathf.Deg2Rad * transform.eulerAngles.z;
        eta_now[4] = -Mathf.Deg2Rad * transform.eulerAngles.x;
        eta_now[5] = Mathf.Deg2Rad * transform.eulerAngles.y;

        // Update reference and error
        eta_ref[0] = eta_now[0];
        eta_ref[1] = eta_now[1];
        eta_ref[2] = 200.0f - Terra_H.Terrain_height - Tall;
        eta_ref[3] = eta_now[3];
        eta_ref[4] = eta_now[4];
        eta_ref[5] = eta_now[5];

        for (i = 0; i < 6; i++)
            eta_error[i] = eta_ref[i] - eta_now[i];

        // Recompute Jacobian and body-frame error
        Jacv_1[0, 0] = Mathf.Cos(eta_now[5]) * Mathf.Cos(eta_now[4]);
        Jacv_1[0, 1] = Mathf.Cos(eta_now[4]) * Mathf.Sin(eta_now[5]);
        Jacv_1[0, 2] = -Mathf.Sin(eta_now[4]);
        Jacv_1[1, 0] = Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[4]) - Mathf.Cos(eta_now[3]) * Mathf.Sin(eta_now[5]);
        Jacv_1[1, 1] = Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[5]) + Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[5]) * Mathf.Sin(eta_now[4]);
        Jacv_1[1, 2] = Mathf.Cos(eta_now[4]) * Mathf.Sin(eta_now[3]);
        Jacv_1[2, 0] = Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[5]) + Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[4]);
        Jacv_1[2, 1] = Mathf.Cos(eta_now[3]) * Mathf.Sin(eta_now[5]) * Mathf.Sin(eta_now[4]) - Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[3]);
        Jacv_1[2, 2] = Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[4]);

        for (int n = 0; n < 3; n++)
        {
            float buf_err = 0.0f;
            for (int m = 0; m < 3; m++)
                buf_err += Jacv_1[n, m] * eta_error[m];

            robot_error[n] = buf_err;

            if (dt != 0.0f)
                PID_nu[n] = Kp * robot_error[n] + Ki * PID_Integral[n] + Kd * (robot_error[n] - robot_error_bef[n]) / dt;
            else
                PID_nu[n] = 0.0f;

            PID_Integral[n] += robot_error[n] * dt;
        }

        // Recompute gravity compensation
        G[0] = Jacv_1[0, 2] * (W + B);
        G[1] = Jacv_1[1, 2] * (W + B);
        G[2] = Jacv_1[2, 2] * (W + B);
        G[3] = z_b * Jacv_1[1, 2] * B;
        G[4] = -z_b * Jacv_1[0, 2] * B;
        G[5] = 0.0f;

        // Add PID correction to joystick commands
        float[] U = mapper.GetMappedCommand(inputManager.GetInputs()); // Map the controller input to target speeds command vector U


        nu_now[0] = U[0] * Mathf.Cos(IOOC.beta) + PID_nu[0];
        nu_now[1] = U[0] * Mathf.Sin(IOOC.beta) + PID_nu[1];
        nu_now[2] = U[2] + PID_nu[2];
        nu_now[3] = 0.0f;
        nu_now[4] = 0.0f;
        nu_now[5] = U[5];
        
        
        // Compute Coriolis
        C_nu[0] = -(M_A[2] + mass) * nu_now[5] * nu_now[1] + (M_A[1] + mass) * nu_now[4] * nu_now[2];
        C_nu[1] = (M_A[2] + mass) * nu_now[5] * nu_now[0] - (M_A[0] + mass) * nu_now[3] * nu_now[2];
        C_nu[2] = -(M_A[1] + mass) * nu_now[4] * nu_now[0] + (M_A[0] + mass) * nu_now[3] * nu_now[1];
        C_nu[3] = -(M_A[5] + I_c[2] - M_A[4] - I_c[1]) * nu_now[4] * nu_now[5];
        C_nu[4] = (M_A[5] + I_c[2] - M_A[3] - I_c[0]) * nu_now[3] * nu_now[5];
        C_nu[5] = -(M_A[4] + I_c[1] - M_A[3] - I_c[0]) * nu_now[3] * nu_now[4];

        // Nonlinear damping
        D_N[0] = 13.7f * nu_now[0];
        D_N[1] = 0.0f;
        D_N[2] = 33.0f * nu_now[2];
        D_N[3] = 0.0f;
        D_N[4] = 0.8f * nu_now[4];
        D_N[5] = 0.0f;

        for (i = 0; i < 6; i++)
        {
            D[i] = D_O[i] + D_N[i];
            if (dt != 0.0f)
                desired_tau[i] = M[i] * (nu_now[i] - nu_bef[i]) / dt + (C_nu[i] + D[i]) * nu_now[i] + G[i];
            else
                desired_tau[i] = (C_nu[i] + D[i]) * nu_now[i] + G[i];

            nu_bef[i] = nu_now[i];
            if (i < 3)
                robot_error_bef[i] = robot_error[i];
        }
    }
}
