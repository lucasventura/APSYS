namespace APSYS.Infrastructure.Communication.Utils
{
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;

    /// <summary>
	/// Utils for Serial Port
	/// </summary>
	public class SerialPortUtil
	{
		/// <summary>
		/// Get a list of avaliables ports
		/// </summary>
		/// <returns>Avaliable Ports</returns>
		public static List<string> AvaliablesPorts()
		{
			return SerialPort.GetPortNames().ToList();
		}
	}
}
