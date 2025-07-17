using UnityEngine;
using System.IO.Ports;

namespace Underwater_BlueROV2
{
    /// <summary>
    /// Manages the active input provider, switching between serial joystick (COM5) and gamepad.
    /// Attempts to open COM5 at startup; if successful, uses joystick, otherwise uses gamepad.
    /// </summary>
    public class InputManager : MonoBehaviour, IInputProvider
    {
        [Tooltip("Reference to the serial joystick input handler")]
        public Joystick_inputs joystick;

        [Tooltip("Reference to the gamepad input handler")]
        public GamepadInput gamepad;

        public bool useJoystick;

        void Start()
        {
            // Try opening COM5 to decide which input source to use
            try
            {
                using (SerialPort sp = new SerialPort("COM5", 9600))
                {
                    sp.Open();
                    sp.Close();
                    useJoystick = true;
                    Debug.Log("Using Joystick (COM5)");
                }
            }
            catch
            {
                useJoystick = false;
                Debug.Log("Using Gamepad");
            }
        }

        void Update()
        {
            // Optional: check COM5 at runtime if needed
        }

        /// <summary>Gets the current inputs from the active provider.</summary>
        public Vector3 GetInputs()
        {
            return useJoystick ? joystick.inputs : gamepad.GetInputs();
        }

        /// <summary>Gets the current angle from the active provider.</summary>
        public float GetAngle()
        {
            return useJoystick ? joystick.angle : gamepad.GetAngle();
        }
    }
}