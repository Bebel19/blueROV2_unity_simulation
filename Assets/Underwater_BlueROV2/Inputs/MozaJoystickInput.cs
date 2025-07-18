using UnityEngine;
using UnityEngine.InputSystem;

namespace Underwater_BlueROV2
{
    /// <summary>
    /// Handles input from the Moza Racing joystick.
    /// For now: assumes only one joystick is connected and duplicates its values.
    /// G1 and G2 are set to zero (reserved for Arduino input). 
    /// TODO: Implement G1 and G2 with serial communication
    /// </summary>
    public class MozaJoystickInput : BaseInputHandler
    {
        public override float[] GetFullInputVector()
        {
            var joystick = Joystick.current;
            if (joystick == null)
            {
                Debug.LogWarning("No joystick detected.");
                return new float[6] { 0f, 0f, 0f, 0f, 0f, 0f };
            }

            // Read axes from the joystick (assumes Moza uses standard X/Y)
            float x = joystick.stick.x.ReadValue(); // X1
            float y = joystick.stick.y.ReadValue(); // Y1

            return new float[6]
            {
                x,  // X1
                x,  // X2 (duplicated)
                y,  // Y1
                y,  // Y2 (duplicated)
                0f, // G1 placeholder (Arduino)
                0f  // G2 placeholder (Arduino)
            };
        }
    }
}