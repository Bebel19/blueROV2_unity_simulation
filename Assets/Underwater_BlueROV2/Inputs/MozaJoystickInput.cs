using UnityEngine;
using UnityEngine.InputSystem;

namespace Underwater_BlueROV2
{
    /// <summary>
    /// Handles input from the Moza Racing joysticks.
    /// Two joysticks are connected to control the ROV with two hands.
    /// G1 and G2 are set to zero (reserved for Arduino input). 
    /// TODO: Implement G1 and G2 with serial communication
    /// </summary>
    public class MozaJoystickInput : BaseInputHandler
    {
        public override float[] GetFullInputVector()
        {
            var joystickRight = Joystick.all[0]; // Right Joystick
            var joystickLeft = Joystick.all[2];  //Left Joystick

            if (joystickRight == null  || joystickLeft == null)
            {
                Debug.LogWarning("At least one joystick is not detected.");
                return new float[6] { 0f, 0f, 0f, 0f, 0f, 0f };
            }

            // Read axes from the joystick (assumes Moza uses standard X/Y)
            float x1 = joystickRight.stick.x.ReadValue(); // X1
            float y1 = joystickRight.stick.y.ReadValue(); // Y1
            float x2 = joystickLeft.stick.x.ReadValue(); // X2
            float y2 = joystickLeft.stick.y.ReadValue(); // Y2

            return new float[6]
            {
                x1,  // X1
                x2,  // X2 
                y1,  // Y1
                y2,  // Y2
                0f, // G1 placeholder (Arduino)
                0f  // G2 placeholder (Arduino)
            };
        }
    }
}