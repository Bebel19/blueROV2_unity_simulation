using UnityEngine;
using System.IO.Ports;

namespace Underwater_BlueROV2
{
    /// <summary>
    /// Manages which input device to use (joystick or gamepad) and delegates input access.
    /// Attach either GamepadInput or MozaJoystickInput to the same GameObject.
    /// TODO : Implemnent 1 and 2 joysticks
    /// TODO : Implement Haptic feedback methods
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        // Choose in the Inspector whether to use joystick or gamepad input
        [SerializeField] private bool useJoystick = false;

        // This will store the actual input handler (either a GamepadInput or MozaJoystickInput)
        private BaseInputHandler inputHandler;

        /// <summary>
        /// Called once when the script starts. It sets up the input handler.
        /// </summary>
        private void Awake()
        {
            // If the checkbox "useJoystick" is true, use Moza joystick input
            if (useJoystick)
            {
                inputHandler = GetComponent<MozaJoystickInput>();
                Debug.Log("Using MozaJoystickInput");

            }
            else // Otherwise, use standard gamepad input
            {
                inputHandler = GetComponent<GamepadInput>();
                Debug.Log("Using GamepadInput");
            }

            // Safety check: make sure we found the input handler
            if (inputHandler == null)
            {
                Debug.LogError("InputHandler not found. Did you attach the right input script to the GameObject?");
            }
        }

        /// <summary>
        /// Returns the full input vector (usually 6 values: X1, Y1, X2, Y2, G1, G2)
        /// </summary>
        public float[] GetInputs()
        {
            float[] inputs = inputHandler.GetFullInputVector();
            Debug.Log("[InputManager] J = [" + string.Join(", ", inputs) + "]");
            return inputs;
        }

        /// <summary>
        /// Returns the reference to the active input handler (can be used to access advanced features)
        /// </summary>
        public BaseInputHandler GetInputHandler()
        {
            return inputHandler;
        }
    }
}