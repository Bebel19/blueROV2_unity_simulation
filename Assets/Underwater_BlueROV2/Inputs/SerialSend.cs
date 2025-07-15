using UnityEngine;

/// <summary>
/// SerialSend formats and sends control signals to an external device via SerialHandler.
/// Data includes forward/backward command, lateral (yaw) command, and vision-based confidence.
/// </summary>
public class SerialSend : MonoBehaviour
{
    public SerialHandler serialHandler;

    // Source for visual confidence (e.g., from image processing)
    public CreateTexture createTexture;

    // IOC controller providing joystick-related inputs
    public IOC_control iocController;

    [Range(0, 1)]
    public int flag = 0;

    public int joy_move_lateral;

    private int sendFrameCounter = 0;

    private void FixedUpdate()
    {
        // Read confidence from vision module
        float confidence = createTexture.confidence;
        confidence = Mathf.Min(confidence, 15.0f);  // Clamp to maximum 15

        // Override confidence based on flag and kill switch
        if (flag != 0)
        {
            confidence = (iocController.Kill_switch == 1) ? 35.0f : 60.0f;
        }

        // Map joy_send_angle (yaw control) to lateral servo signal
        joy_move_lateral = (int)(iocController.joy_send_angle * 408.0f / 1.2f + 482.0f);
        string sendJoyLateral = joy_move_lateral.ToString();

        // Map Kill_switch (forward control) to forward servo signal
        int joy_move_forward = -(int)(iocController.Kill_switch * 390.0f - 505.0f);
        string sendJoyForward = joy_move_forward.ToString();

        // Convert confidence to string
        string sendConfidence = confidence.ToString();

        // Send data every 3 FixedUpdate() calls
        sendFrameCounter++;
        if (sendFrameCounter > 2)
        {
            string message = $"{sendJoyForward},{sendJoyLateral},{sendConfidence}\n";
            serialHandler.Write(message);
            sendFrameCounter = 0;
        }
    }
}