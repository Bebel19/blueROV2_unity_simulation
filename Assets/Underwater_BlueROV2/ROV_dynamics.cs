using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROV_dynamics : MonoBehaviour
{
    public Thruster Thruster_tau;
    float[] M_RB;
    float[] M_A;
    // float[,] C_RB;
    // float[,] C_A;
    float[] D_O;
    float[] D_N;
    public float[] G;

    float[,] Jacv_1;
    float[,] Jacv_2;

    public float[] M;
    public float[] C_nu;
    public float[] D;
    public float[] nu_now;
    public float[] nu_now_dot;
    float[] nu_now_ref;
    float[] nu_now_ref_dot;
    public float[] eta_now;
    float[] eta_now_dot;
    float[] nu_bef;
    float[] nu_bef_ref;
    float[] eta_bef;
    float[] eta_bef_dot;

    public float[] tau;
    public float dt;

    float mass = 13.5f;
    float[] I_c;
    float gravity = 9.82f;
    float Volume = 0.0135f;
    float rho = 1000.0f;
    float z_b = -0.01f;

    public float X_dist_vel;
    public float Y_dist_vel;
    public float[] dist_vel;
    float[] World_dist;

    float W;
    float B;

    int i;

    Vector3 pos_buf;
    Vector3 angle_buf;
    // Start is called before the first frame update
    void Start()
    {
        M_RB = new float[6];
        M_A = new float[6];
        M = new float[6];
        // C_RB = new float[6,6];
        // C_A = new float[6,6];
        C_nu = new float[6];
        D_O = new float[6];
        D_N = new float[6];
        D = new float[6];
        G = new float[6];

        Jacv_1 = new float[3,3];
        Jacv_2 = new float[3,3];

        nu_now = new float[6];
        nu_now_dot = new float[6];
        nu_now_ref = new float[6];
        nu_now_ref_dot = new float[6];
        eta_now = new float[6];
        eta_now_dot = new float[6];
        nu_bef = new float[6];
        nu_bef_ref = new float[6];
        eta_bef = new float[6];
        eta_bef_dot = new float[6];

        dist_vel = new float[6];
        World_dist = new float[6];

        I_c = new float[3];
        I_c[0] = 0.26f;
        I_c[1] = 0.23f;
        I_c[2] = 0.37f;

        tau = new float[6];

        M_RB[0] = mass;
        M_RB[1] = mass;
        M_RB[2] = mass;
        M_RB[3] = I_c[0];
        M_RB[4] = I_c[1];
        M_RB[5] = I_c[2];

        M_A[0] = 6.356673886738176f;
        M_A[1] = 7.120600295756984f;
        M_A[2] = 18.686326861534997f;
        M_A[3] = 0.185765630747592f;
        M_A[4] = 0.134823349429660f;
        M_A[5] = 0.221510466644690f;


        // C_RB[0,0] = 0.0f;
        // C_RB[0,1] = -mass * nu_now[5];
        // C_RB[0,2] = mass * nu_now[4];
        // C_RB[0,3] = 0.0f;
        // C_RB[0,4] = 0.0f;
        // C_RB[0,5] = 0.0f;
        // C_RB[1,0] = mass * nu_now[5];
        // C_RB[1,1] = 0.0f;
        // C_RB[1,2] = -mass * nu_now[3];
        // C_RB[1,3] = 0.0f;
        // C_RB[1,4] = 0.0f;
        // C_RB[1,5] = 0.0f;
        // C_RB[2,0] = -mass * nu_now[4];
        // C_RB[2,1] = mass * nu_now[3];
        // C_RB[2,2] = 0.0f;
        // C_RB[2,3] = 0.0f;
        // C_RB[2,4] = 0.0f;
        // C_RB[2,5] = 0.0f;
        // C_RB[3,0] = 0.0f;
        // C_RB[3,1] = 0.0f;
        // C_RB[3,2] = 0.0f;
        // C_RB[3,3] = 0.0f;
        // C_RB[3,4] = I_c[2] * nu_now[5];
        // C_RB[3,5] = -I_c[1] * nu_now[4];
        // C_RB[4,0] = 0.0f;
        // C_RB[4,1] = 0.0f;
        // C_RB[4,2] = 0.0f;
        // C_RB[4,3] = -I_c[2] * nu_now[5];
        // C_RB[4,4] = 0.0f;
        // C_RB[4,5] = I_c[0] * nu_now[3];
        // C_RB[5,0] = 0.0f;
        // C_RB[5,1] = 0.0f;
        // C_RB[5,2] = 0.0f;
        // C_RB[5,3] = I_c[1] * nu_now[4];
        // C_RB[5,4] = -I_c[0] * nu_now[3];
        // C_RB[5,5] = 0.0f;

        // C_A[0,0] = 0.0f;
        // C_A[0,1] = -M_A[2] * nu_now[5];
        // C_A[0,2] = M_A[1] * nu_now[4];
        // C_A[0,3] = 0.0f;
        // C_A[0,4] = 0.0f;
        // C_A[0,5] = 0.0f;
        // C_A[1,0] = M_A[2] * nu_now[5];
        // C_A[1,1] = 0.0f;
        // C_A[1,2] = -M_A[0] * nu_now[3];
        // C_A[1,3] = 0.0f;
        // C_A[1,4] = 0.0f;
        // C_A[1,5] = 0.0f;
        // C_A[2,0] = -M_A[1] * nu_now[4];
        // C_A[2,1] = M_A[0] * nu_now[3];
        // C_A[2,2] = 0.0f;
        // C_A[2,3] = 0.0f;
        // C_A[2,4] = 0.0f;
        // C_A[2,5] = 0.0f;
        // C_A[3,0] = 0.0f;
        // C_A[3,1] = 0.0f;
        // C_A[3,2] = 0.0f;
        // C_A[3,3] = 0.0f;
        // C_A[3,4] = M_A[5] * nu_now[5];
        // C_A[3,5] = -M_A[4] * nu_now[4];
        // C_A[4,0] = 0.0f;
        // C_A[4,1] = 0.0f;
        // C_A[4,2] = 0.0f;
        // C_A[4,3] = -M_A[5] * nu_now[5];
        // C_A[4,4] = 0.0f;
        // C_A[4,5] = M_A[3] * nu_now[3];
        // C_A[5,0] = 0.0f;
        // C_A[5,1] = 0.0f;
        // C_A[5,2] = 0.0f;
        // C_A[5,3] = M_A[4] * nu_now[4];
        // C_A[5,4] = -M_A[3] * nu_now[3];
        // C_A[5,5] = 0.0f;

        for (i = 0; i < 6; i++) {
            nu_now[i] = 0.0f;
            nu_now_dot[i] = 0.0f;
        }
        C_nu[0] = -(M_A[2] + mass) * nu_now[5] * nu_now[1] + (M_A[1] + mass) * nu_now[4] * nu_now[2];
        C_nu[1] = (M_A[2] + mass) * nu_now[5] * nu_now[0] - (M_A[0] + mass) * nu_now[3] * nu_now[2];
        C_nu[2] = -(M_A[1] + mass) * nu_now[4] * nu_now[0] + (M_A[0] + mass) * nu_now[3] * nu_now[1];
        C_nu[3] = -(M_A[5] + I_c[2] - M_A[4] - I_c[1]) * nu_now[4] * nu_now[5];
        C_nu[4] = (M_A[5] + I_c[2] - M_A[3] - I_c[0]) * nu_now[3] * nu_now[5];
        C_nu[5] = -(M_A[4] + I_c[1] - M_A[3] - I_c[0]) * nu_now[3] * nu_now[4];

        D_O[0] = 141.0f;
        D_O[1] = 217.0f;
        D_O[2] = 190.0f;
        D_O[3] = 1.192f;
        D_O[4] = 0.47f;
        D_O[5] = 1.5f;

        D_N[0] = 13.7f * nu_now[0];
        D_N[1] = 0.0f; //0.0f * nu_now[1];
        D_N[2] = 33.0f * nu_now[2];
        D_N[3] = 0.0f; //0.0f * nu_now[3];
        D_N[4] = 0.8f * nu_now[4];
        D_N[5] = 0.0f; //0.0f * nu_now[5];

        eta_now[0] = this.transform.position.z;
        eta_now[1] = this.transform.position.x;
        eta_now[2] = 200.0f - this.transform.position.y;
        eta_now[3] = -Mathf.Deg2Rad * transform.eulerAngles.z;
        eta_now[4] = -Mathf.Deg2Rad * transform.eulerAngles.x;
        eta_now[5] = Mathf.Deg2Rad * this.transform.eulerAngles.y;

        // // Jacv_1[0,0] = Mathf.Cos(psi) * Mathf.Cos(theta);
        // // Jacv_1[0,1] = Mathf.Cos(theta) * Mathf.Sin(psi);
        // Jacv_1[0,2] = -Mathf.Sin(eta_now[4]);
        // // Jacv_1[1,0] = Mathf.Cos(psi) * Mathf.Sin(phi) * Mathf.Sin(theta) - Mathf.Cos(phi) * Mathf.Sin(psi);
        // // Jacv_1[1,1] = Mathf.Cos(phi) * Mathf.Cos(psi) + Mathf.Sin(phi) * Mathf.Sin(psi) * Mathf.Sin(theta);
        // Jacv_1[1,2] = Mathf.Cos(eta_now[4]) * Mathf.Sin(eta_now[5]);
        // // Jacv_1[2,0] = Mathf.Sin(phi) * Mathf.Sin(psi) + Mathf.Cos(phi) * Mathf.Cos(psi) * Mathf.Sin(theta); 
        // // Jacv_1[2,1] = Mathf.Cos(phi) * Mathf.Sin(psi) * Mathf.Sin(theta) - Mathf.Cos(psi) * Mathf.Sin(phi);
        // Jacv_1[2,2] = Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[4]);

        Jacv_1[0,0] = Mathf.Cos(eta_now[5]) * Mathf.Cos(eta_now[4]);
        Jacv_1[0,1] = Mathf.Cos(eta_now[4]) * Mathf.Sin(eta_now[5]);
        Jacv_1[0,2] = -Mathf.Sin(eta_now[4]);
        Jacv_1[1,0] = Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[4]) - Mathf.Cos(eta_now[3]) * Mathf.Sin(eta_now[5]);
        Jacv_1[1,1] = Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[5]) + Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[5]) * Mathf.Sin(eta_now[4]);
        Jacv_1[1,2] = Mathf.Cos(eta_now[4]) * Mathf.Sin(eta_now[3]);
        Jacv_1[2,0] = Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[5]) + Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[4]); 
        Jacv_1[2,1] = Mathf.Cos(eta_now[3]) * Mathf.Sin(eta_now[5]) * Mathf.Sin(eta_now[4]) - Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[3]);
        Jacv_1[2,2] = Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[4]);

        W = mass * gravity;
        B = -rho * gravity * Volume;

        G[0] = Jacv_1[0,2] * (W + B);
        G[1] = Jacv_1[1,2] * (W + B);
        G[2] = Jacv_1[2,2] * (W + B);
        G[3] = z_b * Jacv_1[1,2] * B;
        G[4] = -z_b * Jacv_1[0,2] * B;
        G[5] = 0.0f;




        for (i = 0; i < 6; i++){
            M[i] = M_RB[i] + M_A[i];
            D[i] = D_O[i] + D_N[i];
            tau[i] = 0.0f;

            nu_now_dot[i] = (tau[i] - (C_nu[i] + D[i] * nu_now[i] + G[i])) / M[i];
            nu_now[i] += nu_now_dot[i] * 0.005f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        dt = Time.deltaTime;             
                
        World_dist[0] = X_dist_vel;
        World_dist[1] = Y_dist_vel;
        World_dist[2] = 0.0f;
        World_dist[3] = 0.0f;
        World_dist[4] = 0.0f;
        World_dist[5] = 0.0f;

        



        // eta_now[0] = this.transform.position.z;
        // eta_now[1] = this.transform.position.x;
        // eta_now[2] = 200.0f - this.transform.position.y;
        eta_now[3] = -Mathf.Deg2Rad * this.transform.eulerAngles.z;
        eta_now[4] = -Mathf.Deg2Rad * this.transform.eulerAngles.x;
        eta_now[5] = Mathf.Deg2Rad * this.transform.eulerAngles.y;

        

        // Jacv_1[0,2] = -Mathf.Sin(eta_now[4]);
        // Jacv_1[1,2] = Mathf.Cos(eta_now[4]) * Mathf.Sin(eta_now[3]);
        // Jacv_1[2,2] = Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[4]);

        Jacv_1[0,0] = Mathf.Cos(eta_now[5]) * Mathf.Cos(eta_now[4]);
        Jacv_1[0,1] = Mathf.Cos(eta_now[4]) * Mathf.Sin(eta_now[5]);
        Jacv_1[0,2] = -Mathf.Sin(eta_now[4]);
        Jacv_1[1,0] = Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[4]) - Mathf.Cos(eta_now[3]) * Mathf.Sin(eta_now[5]);
        Jacv_1[1,1] = Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[5]) + Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[5]) * Mathf.Sin(eta_now[4]);
        Jacv_1[1,2] = Mathf.Cos(eta_now[4]) * Mathf.Sin(eta_now[3]);
        Jacv_1[2,0] = Mathf.Sin(eta_now[3]) * Mathf.Sin(eta_now[5]) + Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[4]); 
        Jacv_1[2,1] = Mathf.Cos(eta_now[3]) * Mathf.Sin(eta_now[5]) * Mathf.Sin(eta_now[4]) - Mathf.Cos(eta_now[5]) * Mathf.Sin(eta_now[3]);
        Jacv_1[2,2] = Mathf.Cos(eta_now[3]) * Mathf.Cos(eta_now[4]);

        for (int i = 0; i < 6; i++){
            float dist_buf = 0.0f;
            if (i < 3){
                for (int m = 0; m < 3; m++){
                    dist_buf += Jacv_1[i, m] * World_dist[m];
                }
                dist_vel[i] = dist_buf;
            }else{
                dist_vel[i] = 0.0f;
            }
            nu_now[i] += dist_vel[i];
        }

        C_nu[0] = -(M_A[2] + mass) * nu_now[5] * nu_now[1] + (M_A[1] + mass) * nu_now[4] * nu_now[2];
        C_nu[1] = (M_A[2] + mass) * nu_now[5] * nu_now[0] - (M_A[0] + mass) * nu_now[3] * nu_now[2];
        C_nu[2] = -(M_A[1] + mass) * nu_now[4] * nu_now[0] + (M_A[0] + mass) * nu_now[3] * nu_now[1];
        C_nu[3] = -(M_A[5] + I_c[2] - M_A[4] - I_c[1]) * nu_now[4] * nu_now[5];
        C_nu[4] = (M_A[5] + I_c[2] - M_A[3] - I_c[0]) * nu_now[3] * nu_now[5];
        C_nu[5] = -(M_A[4] + I_c[1] - M_A[3] - I_c[0]) * nu_now[3] * nu_now[4];

        D_N[0] = 13.7f * Mathf.Abs(nu_now[0]);
        D_N[2] = 33.0f * Mathf.Abs(nu_now[2]);
        D_N[4] = 0.8f * Mathf.Abs(nu_now[4]);

        G[0] = Jacv_1[0,2] * (W + B);
        G[1] = Jacv_1[1,2] * (W + B);
        G[2] = Jacv_1[2,2] * (W + B);
        G[3] = z_b * Jacv_1[1,2] * B;
        G[4] = -z_b * Jacv_1[0,2] * B;
        G[5] = 0.0f;


        
        for (i = 0; i < 6; i++){
            
            // if (i < 3){
            //     for (int m = 0; m < 3; m++){
            //         dist_buf += Jacv_1[i, m] * World_dist[m];
            //     }
            //     dist_vel[i] = dist_buf;
            // }else{
            //     dist_vel[i] = 0.0f;
            // }
            tau[i] = Thruster_tau.tau_output[i];
            D[i] = D_O[i] + D_N[i];
            nu_now_dot[i] = (tau[i] - (-C_nu[i] + D[i] * nu_now[i] + G[i])) / M[i];
            

            if (i == 0) pos_buf.z = nu_now[i] * dt;
            else if (i == 1) pos_buf.x = nu_now[i] * dt;
            else if (i == 2) pos_buf.y = -nu_now[i] * dt;
            else if (i == 3) angle_buf.z = -Mathf.Rad2Deg * nu_now[i] * dt;
            else if (i == 4) angle_buf.x = -Mathf.Rad2Deg * nu_now[i] * dt;
            else if (i == 5) angle_buf.y = Mathf.Rad2Deg * nu_now[i] * dt;
            nu_now[i] += nu_now_dot[i] * dt + dist_vel[i];
        }
        this.transform.position += transform.rotation * pos_buf;
        this.transform.eulerAngles += angle_buf;


    }
}
