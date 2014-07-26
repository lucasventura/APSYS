namespace APSYS.Infrastructure.Communication.Domain
{
	using System;
	using System.Collections.Concurrent;

	public interface ICommunicationOBC
	{
		/// <summary>
		/// Verify that the Communication Channel is Open
		/// </summary>
		bool IsOpen { get; }

		/// <summary>
		/// Verify that the Communication Channel is Connected
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// Receive Data Enqueue
		/// </summary>
		ConcurrentQueue<string> DataEnqueue { get; set; }

		/// <summary>
		/// Connect to the Communication Channel
		/// </summary>
		void Connect();

		/// <summary>
		/// Close the Communication Channel
		/// </summary>
		void Close();

		/// <summary>
		/// Write data
		/// </summary>
		/// <param name="data">string to Write in the Communication Channel</param>
		void Write(string data);

		/// <summary>
		/// Write data
		/// </summary>
		/// <param name="data">string to Write in the Communication Channel with Line Ending</param>
		void WriteLine(string data);

		/// <summary>
		/// Receive Data Event
		/// </summary>
		/// <param name="sender">Event Sender</param>
		/// <param name="eventArgs">Event Args</param>
		void ReceivedData(object sender, EventArgs eventArgs);
	}
}