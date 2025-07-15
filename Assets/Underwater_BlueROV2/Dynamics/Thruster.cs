using System.Collections;
using UnityEngine;

/// <summary>
/// Converts the desired 6 DOF control wrench into 8 individual thruster forces,
/// applies transfer function filtering, and reconstructs the net force.
/// </summary>
public class Thruster : MonoBehaviour
{
    public Controller CO;
    public float Limit = 35.0f;

    // Mapping matrices
    private float[,] T = new float[6, 8];
    private float[,] T_inv = new float[8, 6];

    // Input/Output buffers
    private float[] u_input = new float[6];
    public float[] sub_list;                      // clamped input (raw)
    private float[] sub_list_bef = new float[8];  // backup of previous
    public float[] sub_list2;                     // filtered output
    public float[] tau_output;                    // final control force

    private float[] TransferFnc_C = new float[3];
    private float[] TransferFnc_A = new float[3];
    private float[,] X_TransferFnc_CSTATE = new float[8, 3];
    private float[,] XDot_TransferFnc_CSTATE = new float[8, 3];

    private float sum_index = 0.0f;
    private int Flag_in_first = 0;
    public float dt;

    void Start()
    {
        sub_list = new float[8];
        
        sub_list2 = new float[8];
        tau_output = new float[6];
        
        
        // Transfer function coefficients (third order)
        TransferFnc_A[2] = 89.0f;
        TransferFnc_A[1] = 9258.0f;
        TransferFnc_A[0] = 108700.0f;

        TransferFnc_C[2] = 0.0f;
        TransferFnc_C[1] = 6136.0f;
        TransferFnc_C[0] = 108700.0f;

        // Initialize inverse mapping matrix (8x6)
        InitTinv();

        // Initialize direct mapping matrix (6x8)
        InitT();

        // Zero all buffers
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                X_TransferFnc_CSTATE[i, j] = 0.0f;
                XDot_TransferFnc_CSTATE[i, j] = 0.0f;
            }

            sub_list[i] = 0.0f;
            sub_list_bef[i] = 0.0f;
        }
    }

    void FixedUpdate()
    {
        if (sub_list.Length != 8)
        {
            Debug.LogError("Thruster: sub_list has incorrect length = " + sub_list.Length);
            return;
        }

        dt = Time.deltaTime;

        // === Forward transform: tau → thrusts ===
        for (int i = 0; i < 8; i++)
        {
            sum_index = 0.0f;
            for (int j = 0; j < 6; j++)
                sum_index += CO.desired_tau[j] * T_inv[i, j];

            // Saturation
            sub_list[i] = Mathf.Abs(sum_index) > Limit ? Mathf.Sign(sum_index) * Limit : sum_index;

            // === Transfer function dynamics ===
            XDot_TransferFnc_CSTATE[i, 0] = X_TransferFnc_CSTATE[i, 1];
            XDot_TransferFnc_CSTATE[i, 1] = X_TransferFnc_CSTATE[i, 2];
            XDot_TransferFnc_CSTATE[i, 2] =
                -TransferFnc_A[0] * X_TransferFnc_CSTATE[i, 0]
                - TransferFnc_A[1] * X_TransferFnc_CSTATE[i, 1]
                - TransferFnc_A[2] * X_TransferFnc_CSTATE[i, 2]
                + sub_list[i];

            // Output = C*x
            sub_list2[i] =
                TransferFnc_C[0] * X_TransferFnc_CSTATE[i, 0]
                + TransferFnc_C[1] * X_TransferFnc_CSTATE[i, 1]
                + TransferFnc_C[2] * X_TransferFnc_CSTATE[i, 2];

            // Integrate state
            for (int j = 0; j < 3; j++)
                X_TransferFnc_CSTATE[i, j] += XDot_TransferFnc_CSTATE[i, j] * dt;
        }

        Flag_in_first = 1;

        // === Backward transform: thrusts → tau_output ===
        for (int i = 0; i < 6; i++)
        {
            sum_index = 0.0f;
            for (int j = 0; j < 8; j++)
                sum_index += sub_list2[j] * T[i, j];

            tau_output[i] = sum_index;
        }
    }

    private void InitTinv()
    {
        T_inv = new float[8, 6] {
            { 0.35355678f, -0.35355678f,  0f, 0f, 0f, -1.32415254f },
            { 0.35355678f,  0.35355678f,  0f, 0f, 0f,  1.32415254f },
            {-0.35355678f, -0.35355678f,  0f, 0f, 0f,  1.32415254f },
            {-0.35355678f,  0.35355678f,  0f, 0f, 0f, -1.32415254f },
            { 0f, 0f, -0.25f,  1.146789f,  2.083333f, 0f },
            { 0f, 0f,  0.25f,  1.146789f, -2.083333f, 0f },
            { 0f, 0f,  0.25f, -1.146789f,  2.083333f, 0f },
            { 0f, 0f, -0.25f, -1.146789f, -2.083333f, 0f }
        };
    }

    private void InitT()
    {
        T = new float[6, 8] {
            {  0.7071f,  0.7071f, -0.7071f, -0.7071f, 0f, 0f, 0f, 0f },
            { -0.7071f,  0.7071f, -0.7071f,  0.7071f, 0f, 0f, 0f, 0f },
            {  0f, 0f, 0f, 0f, -1f, 1f, 1f, -1f },
            {  0f, 0f, 0f, 0f,  0.218f,  0.218f, -0.218f, -0.218f },
            {  0f, 0f, 0f, 0f,  0.12f, -0.12f,  0.12f, -0.12f },
            { -0.1888f, 0.1888f, 0.1888f, -0.1888f, 0f, 0f, 0f, 0f }
        };
    }
}
