using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    public Controller CO;
    public float Limit = 35.0f;
    float[,] T;
    float[,] T_inv;

    float[] u_input;
    public float[] sub_list;
    float[] sub_list_bef;
    public float[] sub_list2;
    float sum_index = 0.0f;
    public float[] tau_output;

    float[] TransferFnc_C;
    float[] TransferFnc_A;
    float[,] X_TransferFnc_CSTATE;
    float[,] XDot_TransferFnc_CSTATE;

    int Flag_in_first = 0;

    public float dt;

    // Start is called before the first frame update
    void Start()
    {
        T = new float[6,8];
        T_inv = new float[8,6];
        u_input = new float[6];
        sub_list = new float[8];
        sub_list_bef = new float[8];
        sub_list2 = new float[8];
        tau_output = new float[6];
        TransferFnc_C = new float[3];
        TransferFnc_A = new float[3];
        X_TransferFnc_CSTATE = new float[8,3];
        XDot_TransferFnc_CSTATE = new float[8,3];


        TransferFnc_A[2] = 89.0f;     // -c
        TransferFnc_A[1] = 9258.0f;   // -d
        TransferFnc_A[0] = 108700.0f; // -b

        TransferFnc_C[2] = 0.0f;       // 
        TransferFnc_C[1] = 6136.0f;    // a
        TransferFnc_C[0] = 108700.0f;  // b

        




        T_inv[0,0] = 0.35355678121906381f;
        T_inv[0,1] = -0.353556781219064f;
        T_inv[0,2] = -1.387778780781446e-17f;
        T_inv[0,3] = 0.0f;
        T_inv[0,4] = 0.0f;
        T_inv[0,5] = -1.3241525423728813f;
        T_inv[1,0] = 0.35355678121906381f;
        T_inv[1,1] = 0.35355678121906386f;
        T_inv[1,2] = -1.387778780781446e-17f;
        T_inv[1,3] = 0.0f;
        T_inv[1,4] = 0.0f;
        T_inv[1,5] = 1.3241525423728813f;
        T_inv[2,0] = -0.35355678121906381f;
        T_inv[2,1] = -0.35355678121906386f;
        T_inv[2,2] = -1.387778780781446e-17f;
        T_inv[2,3] = 0.0f;
        T_inv[2,4] = 0.0f;
        T_inv[2,5] = 1.3241525423728815f;
        T_inv[3,0] = -0.35355678121906381f;
        T_inv[3,1] = 0.35355678121906375f;
        T_inv[3,2] = -1.3877787807814463e-17f;
        T_inv[3,3] = 0.0f;
        T_inv[3,4] = 0.0f;
        T_inv[3,5] = -1.3241525423728817f;
        T_inv[4,0] = 0.0f;
        T_inv[4,1] = 0.0f;
        T_inv[4,2] = -0.25f;
        T_inv[4,3] = 1.1467889908256881f;
        T_inv[4,4] = 2.0833333333333335f;
        T_inv[4,5] = 0.0f;
        T_inv[5,0] = 0.0f;
        T_inv[5,1] = 0.0f;
        T_inv[5,2] = 0.25f;
        T_inv[5,3] = 1.1467889908256881f;
        T_inv[5,4] = -2.0833333333333335f;
        T_inv[5,5] = 0.0f;
        T_inv[6,0] = 0.0f;
        T_inv[6,1] = 0.0f;
        T_inv[6,2] = 0.25f;
        T_inv[6,3] = -1.1467889908256881f;
        T_inv[6,4] = 2.0833333333333335f;
        T_inv[6,5] = 0.0f;
        T_inv[7,0] = 0.0f;
        T_inv[7,1] = 0.0f;
        T_inv[7,2] = -0.25f;
        T_inv[7,3] = -1.1467889908256881f;
        T_inv[7,4] = -2.0833333333333335f;
        T_inv[7,5] = 0.0f;
        
        T[0,0] = 0.707106781186548f;
        T[0,1] = 0.707106781186548f;
        T[0,2] = -0.707106781186548f;
        T[0,3] = -0.707106781186548f;
        T[0,4] = 0.0f;
        T[0,5] = 0.0f;
        T[0,6] = 0.0f;
        T[0,7] = 0.0f;
        T[1,0] = -0.707106781186548f;
        T[1,1] = 0.707106781186548f;
        T[1,2] = -0.707106781186548f;
        T[1,3] = 0.707106781186548f;
        T[1,4] = 0.0f;
        T[1,5] = 0.0f;
        T[1,6] = 0.0f;
        T[1,7] = 0.0f;
        T[2,0] = 0.0f;
        T[2,1] = 0.0f;
        T[2,2] = 0.0f;
        T[2,3] = 0.0f;
        T[2,4] = -1.0f;
        T[2,5] = 1.0f;
        T[2,6] = 1.0f;
        T[2,7] = -1.0f;
        T[3,0] = 0.0f;
        T[3,1] = 0.0f;
        T[3,2] = 0.0f;
        T[3,3] = 0.0f;
        T[3,4] = 0.218f;
        T[3,5] = 0.218f;
        T[3,6] = -0.218f;
        T[3,7] = -0.218f;
        T[4,0] = 0.0f;
        T[4,1] = 0.0f;
        T[4,2] = 0.0f;
        T[4,3] = 0.0f;
        T[4,4] = 0.12f;
        T[4,5] = -0.12f;
        T[4,6] = 0.12f;
        T[4,7] = -0.12f;
        T[5,0] = -0.188797510576808f;
        T[5,1] = 0.188797510576808f;
        T[5,2] = 0.188797510576808f;
        T[5,3] = -0.188797510576808f;
        T[5,4] = 0.0f;
        T[5,5] = 0.0f;
        T[5,6] = 0.0f;
        T[5,7] = 0.0f;

        for (int i = 0; i < 8; i++){
            X_TransferFnc_CSTATE[i,0] = 0.0f;
            X_TransferFnc_CSTATE[i,1] = 0.0f;
            X_TransferFnc_CSTATE[i,2] = 0.0f;
            
            XDot_TransferFnc_CSTATE[i,0] = 0.0f;
            XDot_TransferFnc_CSTATE[i,1] = 0.0f;
            XDot_TransferFnc_CSTATE[i,2] = 0.0f;

            sub_list[i] = 0.0f;
            sub_list_bef[i] = sub_list[i];

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        dt = Time.deltaTime;
        for (int i = 0; i < 8; i++){
            sum_index = 0.0f;
            for (int j = 0; j < 6; j++){
                sum_index += CO.desired_tau[j] * T_inv[i, j];
            }
            if (Mathf.Abs(sum_index) > Limit){
                sub_list[i] = Mathf.Sign(sum_index) * Limit;
            }else sub_list[i] = sum_index;

            XDot_TransferFnc_CSTATE[i,0] = X_TransferFnc_CSTATE[i,1];
            XDot_TransferFnc_CSTATE[i,1] = X_TransferFnc_CSTATE[i,2];
            XDot_TransferFnc_CSTATE[i,2] = -TransferFnc_A[0] * X_TransferFnc_CSTATE[i,0]
                                         -  TransferFnc_A[1] * X_TransferFnc_CSTATE[i,1]
                                         -  TransferFnc_A[2] * X_TransferFnc_CSTATE[i,2]
                                         + sub_list[i];
            
            
            
            
            sub_list2[i] = TransferFnc_C[0] * X_TransferFnc_CSTATE[i,0]
                         + TransferFnc_C[1] * X_TransferFnc_CSTATE[i,1]
                         + TransferFnc_C[2] * X_TransferFnc_CSTATE[i,2];

            X_TransferFnc_CSTATE[i,0] += XDot_TransferFnc_CSTATE[i,0] * dt;
            X_TransferFnc_CSTATE[i,1] += XDot_TransferFnc_CSTATE[i,1] * dt;
            X_TransferFnc_CSTATE[i,2] += XDot_TransferFnc_CSTATE[i,2] * dt;
        }
        Flag_in_first = 1;
        for (int i = 0; i < 6; i++){
            sum_index = 0.0f;
            for (int j = 0; j < 8; j++){
                sum_index += sub_list2[j] * T[i, j];
            }
            tau_output[i] = sum_index;
        }

    }
    // void private void FixedUpdate() {
        
    // }
}
