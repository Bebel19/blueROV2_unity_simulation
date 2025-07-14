using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SerialSend : MonoBehaviour
{
    //SerialHandler.cのクラス
    public SerialHandler serialHandler;
    public CreateTexture CreTex2;
    public IOC_control IOCC;
    int i = 0;
    [Range(0, 1)]
    public int flag = 0;
    public int joy_move_lateral;

    void FixedUpdate() //ここは0.001秒ごとに実行される
    {
        float conf = CreTex2.confidence;
        if (conf > 15.0f) conf = 15.0f;
        if (flag != 0){
            if (IOCC.Kill_switch == 1){
                conf = 35.0f;
            }else{
                conf = 60.0f;
            }
        }
        
        string send_conf = conf.ToString();
        joy_move_lateral = (int)((IOCC.joy_send_angle) * 408.0f / 1.2f + 482.0f); // signal = 1.2f * (joy_pos.z - 482.0f) / 408.0f;
        string send_joy_move_lateral = joy_move_lateral.ToString();   //iを加算していって1秒ごとに"1"のシリアル送信を実行
        int joy_move_for = -(int)((float)IOCC.Kill_switch * 390.0f - 505.0f); //-1.0f * (joy_pos.x - 505.0f) / 390.0f;
        string send_joy_move_for = joy_move_for.ToString();   //iを加算していって1秒ごとに"1"のシリアル送信を実行
        i++;
        if (i > 2) //
        {
            string sending = send_joy_move_for + "," + send_joy_move_lateral + "," + send_conf + "\n";
            serialHandler.Write(sending);
            i = 0;
        }
    }
}
