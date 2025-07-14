using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Joystick_inputs : MonoBehaviour{

	public SerialHandler serialHandler;
	public Vector3 joy_pos;
    public Vector3 inputs;
    public float angle;
    public float confidence;

    public float pwmA;
    public float pwmB;

    private Vector3 joy_buf;
	void Start(){
		serialHandler.OnDataReceived += OnDataReceived;
	}

	void Update(){
        joy_buf.x = -2.5f * (joy_pos.x - 505.0f) / 390.0f;
        inputs.y = 0.0f;
        inputs.z = 0.0f;
        joy_buf.y = 1.2f * (joy_pos.z - 482.0f) / 408.0f;
        if (Mathf.Abs(joy_buf.x) < 0.06f){
            inputs.x = 0.0f;
        }else{
            inputs.x = joy_buf.x;
        }
        if (Mathf.Abs(joy_buf.y) < 0.2f){
            angle = 0.0f;
        }else{
            angle = joy_buf.y;
        }
        // inputs.x = 0.0f;
        // inputs.y = 0.0f;
        // inputs.z = 0.0f;
        // angle = 0.0f;
        // inputs.x = joy_buf.x;
        // angle = joy_buf.y;
	}

	void OnDataReceived(string message){
		var data = message.Split(
                new string[] { "\n" }, System.StringSplitOptions.None);
        try
        {
            // Debug.Log(data[0]);//Unityのコンソールに受信データを表示
            joy_pos.x = float.Parse(data[0].Split(',')[0]);
            joy_pos.z = float.Parse(data[0].Split(',')[1]);
            confidence = float.Parse(data[0].Split(',')[2]);
            pwmA = float.Parse(data[0].Split(',')[3]);
            pwmB = float.Parse(data[0].Split(',')[4]);

            // Debug.Log(confidence);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);//エラーを表示
        }
	}
}