using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.IO.Ports;


namespace Underwater_BlueROV2
{
    /// <summary>
    /// Handles input from the Moza Racing joysticks.
    /// Two joysticks are connected to control the ROV with two hands.
    /// G1 and G2 are set to zero (reserved for Arduino input).
    /// </summary>
    public class MozaJoystickInput : BaseInputHandler
    {   
        //Serial communication variables for grip 
        [Header("Serial Config")]
        public string portName = "COM5";
        public int baudRate = 9600;

        [Header("Control Inputs [0–1]")]
        [Range(0f, 1f)] public float normTorque = 1f;//placeholder for desired grip torque
        [Range(0f, 1f)] public float normTargetAngle = 1f; //Placeholder for desired grip angle

        [Header("Grip Feedback (Read-only)")]
        public float gripAngleReadback = 1f;
        
        private SerialPort serial;
        

        void Start()
        {
            InitializeSerialConnection();

            // Direct input:
            Debug.Log("🔍 Starting DirectInput device scan...");

            
        }
        
        void Update()
        {
            if (!IsSerialReady()) return;

            try
            {
                SendControlCommands(normTorque, normTargetAngle);
                ReadGripAngle();
            }
            catch (TimeoutException)
            {
                Debug.LogWarning("[MozaJoystickInput] Timeout during serial read/write.");
            }
            catch (Exception e)
            {
                HandleSerialException(e);
            }
        }
        
        
        //MOZA joystick functions
        public override float[] GetFullInputVector()
        {
            // If one of the joysticks is not detected retruns a null vector
            if (Joystick.all.Count < 3 || Joystick.all[0] == null || Joystick.all[2] == null)
            {
                Debug.LogWarning("[MozaJoystickInput] At least one joystick is not detected.");
                return new float[6] { 0f, 0f, 0f, 0f, 0f, 0f };
            }
            
            var joystickRight = Joystick.all[0]; // Right Joystick
            var joystickLeft = Joystick.all[2];  // Left Joystick

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
                1-((gripAngleReadback)/40), // Value from serial com //TODO use profile to convert
                1f  // G2 placeholder (Arduino)
            };
        }
        
            // ========== SERIAL COMMUNICATION METHODS ==========

    void InitializeSerialConnection()
    {
        try
        {
            Debug.Log($"[MozaJoystickInput] Trying to connect to serial port on {portName}");

            serial = new SerialPort(portName, baudRate)
            {
                ReadTimeout = 100,
                WriteTimeout = 100
            };
            serial.Open();
            Debug.Log($"[MozaJoystickInput] Serial port opened on {portName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[MozaJoystickInput] Failed to open serial port: {e.Message}");
        }
    }

    void SendControlCommands(float normTorque, float normTargetAngle)
    {
        byte[] torqueBytes = BitConverter.GetBytes(normTorque);
        byte[] angleBytes = BitConverter.GetBytes(normTargetAngle);

        serial.Write(torqueBytes, 0, 4);
        serial.Write(angleBytes, 0, 4);

        LogSentValues();
    }

    float ReadGripAngle()
    {
        if (serial.BytesToRead < 6)
        {
            Debug.Log("[MozaJoystickInput] Waiting for grip feedback...");
            return gripAngleReadback;
        }

        int marker = serial.ReadByte();
        if (marker != 'A')
        {
            Debug.LogWarning($"[MozaJoystickInput] Unexpected marker: 0x{marker:X2}");
            FlushInput();
            return gripAngleReadback;
        }

        byte[] buffer = new byte[4];
        serial.Read(buffer, 0, 4);
        gripAngleReadback = BitConverter.ToSingle(buffer, 0);
        LogReceivedAngle(gripAngleReadback);

        // Optionnel : jeter '\n' final
        if (serial.BytesToRead > 0)
            serial.ReadByte();
        
        return gripAngleReadback;
    }

    void CloseSerialConnection()
    {
        if (serial != null && serial.IsOpen)
        {
            serial.Close();
            Debug.Log("[MozaJoystickInput] Serial port closed.");
        }
    }

    void FlushInput()
    {
        try
        {
            while (serial.BytesToRead > 0)
                serial.ReadByte();
        }
        catch { }
    }

    void HandleSerialException(Exception e)
    {
        Debug.LogError($"[MozaJoystickInput] Serial error: {e.GetType().Name} → {e.Message}");
    }

    bool IsSerialReady()
    {
        return serial != null && serial.IsOpen;
    }

    void LogSentValues()
    {
        Debug.Log($"[MozaJoystickInput] Sent → Torque={normTorque:F2} | Angle={normTargetAngle:F2}");
    }

    void LogReceivedAngle(float angle)
    {
        Debug.Log($"[MozaJoystickInput] Received grip angle: {angle:F2}°");
    }
    
    }
}