using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IOC_control : MonoBehaviour
{
    public CreateTexture CreTex;
    public Joystick_inputs JSI;

    [SerializeField] private ROV_dynamics RD;
    [SerializeField] private space SP;

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
    private float Dt;
    private float Cutoff_u = 0.5f;



    // private float before_

    // Start is called before the first frame update
    void Start()
    {
        float ErrY = CreTex.errory_mat - CreTex.errory_mat_before;
        float ErrT = CreTex.errorag_mat - CreTex.errorag_mat_before;

        error_y = (float)(CreTex.errory_mat) / 100.0f;
        error_angle = (float)CreTex.errorag_mat * Mathf.Deg2Rad;

        Vel = JSI.inputs.x;

        float result;
        if (JSI.confidence < 1.0f){
            beta = (float)Kill_switch * Mathf.Atan(error_y) * JSI.confidence;
        }else{
            beta = (float)Kill_switch * Mathf.Atan(error_y);
        }
        sys_conf = CreTex.confidence;
        float CosErr = Mathf.Cos(error_angle);
        if (CosErr != 0.0f){
            vr = (Vel * Mathf.Cos(beta) - error_y * RD.nu_now[5]) / Mathf.Cos(error_angle);
        }else{
            vr = 0.0f;
        }
        float f1 = vr * Mathf.Sin(error_angle) - Vel * Mathf.Sin(beta);
        a = Ka * error_y * f1 + Kb * 1;
        b = -Kb * error_angle;
        if (b == 0.0f){
            input_u = 0.0f;
        }else{
            input_u = -(float)Kill_switch * (a + Mathf.Sqrt(a * a + b * b * b * b)) / b;
        }

        if (Mathf.Abs(input_u) > 1.2f) input_u = 1.2f * Mathf.Sign(input_u);

        result = before_u + (input_u - before_u) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff_u;
        before_u = result;

        joy_send_angle = result;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        // if (Input.GetKeyDown(KeyCode.Space)){
        //     if (Kill_switch == 0) Kill_switch = 1;
        //     else Kill_switch = 0;
        // }

        // float IOC_start_time = Time.deltaTime;
        sw.Start();
        Kill_switch = SP.space_is;
        float ErrY = CreTex.errory_mat - CreTex.errory_mat_before;
        float ErrT = CreTex.errorag_mat - CreTex.errorag_mat_before;


        // if (Mathf.Abs(ErrY) > ThresHoldY){
        //     error_y = ((float)(CreTex.errory_mat_before) + Mathf.Sign(ErrY) * ThresHoldY) / 150.0f;
        // }else{
        //     error_y = (float)(CreTex.errory_mat) / 150.0f;
        // }

        // if (Mathf.Abs(ErrT) > ThresHoldT){
        //     error_angle = ((float)(CreTex.errorag_mat_before) + Mathf.Sign(ErrT) * ThresHoldT) * Mathf.Deg2Rad;
        // }else{
        //     error_angle = (float)CreTex.errorag_mat * Mathf.Deg2Rad;
        // }
        error_y = (float)(CreTex.errory_mat) / 100.0f;
        error_angle = (float)CreTex.errorag_mat * Mathf.Deg2Rad;

        // if(error_y > 1000.0f / 150.0f){
        //     error_y = 0.0f;
        // }
        // if(error_angle > 360.0f * Mathf.Deg2Rad){
        //     error_angle = 0.0f;
        // }
        Vel = JSI.inputs.x;

        float result;
        if (JSI.confidence < 1.0f){
            beta = (float)Kill_switch * Mathf.Atan(error_y) * JSI.confidence;
        }else{
            beta = (float)Kill_switch * Mathf.Atan(error_y);
        }
        sys_conf = CreTex.confidence;
        float CosErr = Mathf.Cos(error_angle);
        // beta = -Mathf.Atan(error_y);
        if (CosErr != 0.0f){
            vr = (Vel * Mathf.Cos(beta) - error_y * RD.nu_now[5]) / Mathf.Cos(error_angle);
        }else{
            vr = 0.0f;
        }
        float f1 = vr * Mathf.Sin(error_angle) - Vel * Mathf.Sin(beta);
        a = Ka * error_y * f1;
        b = -Kb * error_angle;
        if (b == 0.0f){
            input_u = 0.0f;
        }else{
            input_u = -(float)Kill_switch * (a + Mathf.Sqrt(a * a + b * b * b * b)) / b;
        }

        if (Mathf.Abs(input_u) > 1.2f) input_u = 1.2f * Mathf.Sign(input_u);

        result = before_u + (input_u - before_u) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff_u;
        before_u = result;



        // if (Mathf.Abs(result) > 1.2f) result = 1.2f * Mathf.Sign(result);
        joy_send_angle = result;
        // joy_send_angle = 1.2f * Mathf.Sin(2.0f * Mathf.PI * frequency * time_u);
        // float IOC_end_time = Time.deltaTime;
        
        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds);
    }
}
