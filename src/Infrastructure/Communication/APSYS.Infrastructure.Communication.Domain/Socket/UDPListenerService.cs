namespace APSYS.Infrastructure.Communication.Domain.Socket
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class UdpListenerService : ICommunicationOBC
    {
        private UdpClient _listener;
        private IPEndPoint _groupEp;

        public event EventHandler DataReceived;

        public string Address { get; set; }

        public int ListenPort { get; set; }

        public bool IsOpen
        {
            get
            {
                return IsConnected;
            }
        }

        public bool IsConnected { get; set; }

        public ConcurrentQueue<string> DataEnqueue { get; set; }

        public bool IsDataEnqueueEnable { get; set; }

        public void InitializeSocket(string address, int port)
        {
            ListenPort = port;
            Address = address;
            Connect();
            Task task = new Task(ReadSocketTask);
            task.Start();
        }

        public void Connect()
        {
            IsConnected = true;
            _listener = new UdpClient(ListenPort);

            IPAddress address;
            var parseIPSucessful = IPAddress.TryParse("192.168.1.104", out address);

            if (string.IsNullOrEmpty(Address) || !parseIPSucessful)
            {
                _groupEp = new IPEndPoint(IPAddress.Any, ListenPort);
            }
            else
            {
                _groupEp = new IPEndPoint(address, ListenPort);
            }
        }

        public void Close()
        {
            IsConnected = false;
            _listener.Close();
        }

        public void Write(string data)
        {
            byte[] dgram = Encoding.ASCII.GetBytes(data);
            int bytes = dgram.Length;
            _listener.Send(dgram, bytes, _groupEp);
        }

        public void WriteLine(string data)
        {
            data += "\r\n";
            byte[] dgram = Encoding.ASCII.GetBytes(data);
            int bytes = dgram.Length;
            _listener.Send(dgram, bytes, _groupEp);
        }

        public void ReceivedData(object sender, EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        private void ReadSocketTask()
        {
            try
            {
                while (true)
                {
                    byte[] receiveByteArray = _listener.Receive(ref _groupEp);
                    string receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);
                    DataReceived.Invoke(receivedData, new EventArgs());
                }
            }
            catch (Exception e)
            {
                Close();
                //// throw new Exception(e.Message);
            }
        }
    }
}
