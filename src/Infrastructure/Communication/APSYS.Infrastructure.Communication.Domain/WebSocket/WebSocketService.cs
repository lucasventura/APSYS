namespace APSYS.Infrastructure.Communication.Domain.WebSocket
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Net.WebSockets;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using ServiceStack;

	/// <summary>
	/// Serviço que trata do Websocket
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Reviewed.")]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1642:ConstructorSummaryDocumentationMustBeginWithStandardText", Justification = "Reviewed.")]
	public class WebSocketService
	{
		private const int ReceiveBufferSize = 1024;
		private readonly string _url;
		private ClientWebSocket _cs;
		private CancellationTokenSource _cts;

		/// <summary>
		/// COnstrutor Padrão
		/// </summary>
		/// <param name="url">url do WebSocket</param>
		public WebSocketService(string url)
		{
			_url = url;
		}

		public delegate void ReceiveHandler(string message);

		public event ReceiveHandler OnReceive;

		/// <summary>
		/// Gets se o WebSocket está disponível
		/// </summary>
		public bool IsAvaliable
		{
			get { return _cs.State == WebSocketState.Open; }
		}

		/// <summary>
		/// Gets Estado do WebSocket
		/// </summary>
		public WebSocketState WebSocketState
		{
			get { return _cs.State; }
		}

		/// <summary>
		/// Envia uma string qualquer 
		/// </summary>
		/// <param name="dados">string que será enviada</param>
		/// <returns>Task de envio</returns>
		public async Task Send(string dados)
		{
			var outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(dados));
			await _cs.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
		}

		/// <summary>
		/// Envia uma mensagem padrão do websocket
		/// </summary>
		/// <typeparam name="T">parametro generico</typeparam>
		/// <param name="mensagem">mensagem tipada de envio</param>
		/// <returns>Task de envio</returns>
		public async Task Send<T>(MensagemWebSocketDTO<T> mensagem)
		{
			var dados = mensagem.ToJson();
			var outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(dados));
			await _cs.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
		}

		/// <summary>
		/// Conecta com o WebSocket
		/// </summary>
		/// <returns>Return a task</returns>
		public async Task Conectar()
		{
			_cs = new ClientWebSocket();
			_cts = new CancellationTokenSource();

			try
			{
				await _cs.ConnectAsync(new Uri(_url), _cts.Token);
			}
			catch (Exception ex)
			{
				if (OnReceive != null)
				{
					OnReceive("Failed to connect WebSocket (Uri: " + _url + "), Details: " + ex);
				}
			}

			while (_cs.State == WebSocketState.Open)
			{
				await Receive(_cs);
			}
		}

		public async void Close()
		{
			await Send("exit"); // await _cs.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, _cts.Token);
		}

		public void Cancelar()
		{
			_cts.Cancel();
		}

		private async Task Receive(ClientWebSocket cs)
		{
			var buffer = new byte[ReceiveBufferSize];

			try
			{
				WebSocketReceiveResult result = await cs.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

				string msg = (new UTF8Encoding()).GetString(buffer);
				if (!string.IsNullOrEmpty(msg))
				{
					if (OnReceive != null)
					{
						OnReceive(msg);
					}
				}

				if (result.MessageType == WebSocketMessageType.Close)
				{
					await cs.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception = {0}", ex.Message);
			}
		}
	}
}