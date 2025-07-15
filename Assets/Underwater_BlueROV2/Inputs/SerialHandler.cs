using UnityEngine;
using System.IO.Ports;
using System.Threading;

/// <summary>
/// Manages asynchronous serial communication via a background thread.
/// Triggers OnDataReceived event when new data is available.
/// </summary>
public class SerialHandler : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    [Tooltip("Serial port name. Example: COM5 (Windows), /dev/ttyUSB0 (Linux)")]
    public string portName = "COM5";

    [Tooltip("Baud rate for the serial connection")]
    public int baudRate = 115200;

    private SerialPort serialPort_;
    private Thread thread_;
    private bool isRunning_ = false;

    private string message_;
    private string messageBuffer_;
    private bool isNewMessageReceived_ = false;

    private void Awake()
    {
        Open();
    }

    private void Update()
    {
        // If a new message has been received, invoke the callback event
        if (isNewMessageReceived_)
        {
            OnDataReceived?.Invoke(message_);
        }
        isNewMessageReceived_ = false;
    }

    private void OnDestroy()
    {
        Close();
    }

    /// <summary>
    /// Opens the serial port and starts the read thread.
    /// </summary>
    private void Open()
    {
        serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        serialPort_.Open();

        isRunning_ = true;

        thread_ = new Thread(Read);
        thread_.Start();
    }

    /// <summary>
    /// Closes the serial port and terminates the read thread safely.
    /// </summary>
    private void Close()
    {
        isNewMessageReceived_ = false;
        isRunning_ = false;

        if (thread_ != null && thread_.IsAlive)
        {
            thread_.Join();
        }

        if (serialPort_ != null && serialPort_.IsOpen)
        {
            serialPort_.Close();
            serialPort_.Dispose();
        }
    }

    /// <summary>
    /// Continuously reads incoming serial data in a background thread.
    /// </summary>
    private void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen)
        {
            try
            {
                messageBuffer_ = serialPort_.ReadExisting();
                message_ = messageBuffer_ + serialPort_.ReadLine();

                isNewMessageReceived_ = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Serial read error: {e.Message}");
            }
        }
    }

    /// <summary>
    /// Sends a message string to the serial device.
    /// </summary>
    public void Write(string message)
    {
        try
        {
            serialPort_.Write(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Serial write error: {e.Message}");
        }
    }
}
