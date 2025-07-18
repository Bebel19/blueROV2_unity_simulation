using UnityEngine;
using UnityEngine.InputSystem; // Don't forget to install via package manager on Unity (from Unity Registry)

namespace Underwater_BlueROV2
{
    /// <summary>
    /// Handles input from a standard gamepad using the new Input System.
    /// Assumes the use of left and right sticks for translation,
    /// and triggers (L2, R2) for additional controls (G1, G2).
    /// </summary>


    public class GamepadInput : BaseInputHandler
    {
        public override float[] GetFullInputVector()
        {
            var gamepad = Gamepad.current;
            if (gamepad == null)
                return new float[6] { 0f, 0f, 0f, 0f, 0f, 0f };

            Vector2 leftStick = gamepad.leftStick.ReadValue();
            Vector2 rightStick = gamepad.rightStick.ReadValue();
            float leftTrigger = gamepad.leftTrigger.ReadValue();
            float rightTrigger = gamepad.rightTrigger.ReadValue();

            return new float[6]
            {
                leftStick.x, // X1
                rightStick.x, // X2
                leftStick.y, // Y1
                rightStick.y, // Y2
                leftTrigger, // G1
                rightTrigger // G2
            };
        }
        

    }

}