namespace APSYS.Infrastructure.Communication.Domain.Serial
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Ports;
    using NLog;

    /// <summary>
    /// Serial Port Service and Configure
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "This is OK here.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1642:ConstructorSummaryDocumentationMustBeginWithStandardText", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Reviewed.")]
    public class SerialPortService : ICommunicationOBC
    {
        private SerialPort _serialPort;

        /// <summary>
        /// Default Constructor with a portName and BaudRate 9600 
        /// </summary>
        /// <param name="portName">Serial Port Name</param>
        public SerialPortService(string portName)
        {
            InitializeSerialPort(portName, 9600);
        }

        /// <summary>
        /// Constructor based on a port Name and a baud Rate
        /// </summary>
        /// <param name="portName">Serial Port Name</param>
        /// <param name="baudRate">Serial Baud Rate</param>
        public SerialPortService(string portName, int baudRate)
        {
            InitializeSerialPort(portName, baudRate);
        }

        public SerialPortService()
        {
        }

        /// <summary>
        /// Serial port state enum
        /// </summary>
        public enum SerialPortStatus
        {
            /// <summary>
            /// The port is Open
            /// </summary>
            Open,

            /// <summary>
            /// The port is Close
            /// </summary>
            Close
        }

        public bool IsCalibration { get; set; }

        /// <summary>
        /// Verify that the Communication Channel is Connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _serialPort.IsOpen;
            }
        }

        /// <summary>
        /// Gets Verify that the Communication Channel is Open
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if (_serialPort == null)
                {
                    return false;
                }

                return _serialPort.IsOpen;
            }
        }

        /// <summary>
        /// Gets the Serial Port Status
        /// </summary>
        public SerialPortStatus Status
        {
            get
            {
                return IsOpen ? SerialPortStatus.Open : SerialPortStatus.Close;
            }
        }

        /// <summary>
        /// Receive Data Enqueue
        /// </summary>
        public ConcurrentQueue<string> DataEnqueue { get; set; }

        /// <summary>
        /// Receive Data Enqueue
        /// </summary>
        public ConcurrentQueue<string> CalibrationDataEnqueue { get; set; }

        /// <summary>
        /// Connect to the Communication Channel
        /// </summary>
        public void Connect()
        {
            try
            {
                _serialPort.Open();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Write data
        /// </summary>
        /// <param name="data">string to Write in the Communication Channel</param>
        public void Write(string data)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(data);
                return;
            }

            throw new Exception("The serial port must be open to write data.");
        }

        /// <summary>
        /// Write data
        /// </summary>
        /// <param name="data">string to Write in the Communication Channel with Line Ending</param>
        public void WriteLine(string data)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.WriteLine(data);
                return;
            }

            throw new Exception("The serial port must be open to write data.");
        }

        /// <summary>
        /// Receive Data Event
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="eventArgs">Event Args</param>
        public void ReceivedData(object sender, EventArgs eventArgs)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string newData = sp.ReadExisting();

                if (IsCalibration == true)
                {
                    CalibrationDataEnqueue.Enqueue(newData);
                }

                DataEnqueue.Enqueue(newData);
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Error(exception.Message);
            }
        }

        public void Close()
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public void InitializeSerialPort(string port, int baudRate)
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = port;
            _serialPort.BaudRate = baudRate;
            _serialPort.DataReceived += ReceivedData;
            DataEnqueue = new ConcurrentQueue<string>();
            CalibrationDataEnqueue = new ConcurrentQueue<string>();
            Connect();
        }
    }
}
