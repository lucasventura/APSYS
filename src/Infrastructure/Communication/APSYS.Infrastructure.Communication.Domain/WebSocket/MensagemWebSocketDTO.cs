namespace APSYS.Infrastructure.Communication.Domain.WebSocket
{
	/// <summary>
	/// Classe do protocolo de comunicação definido para o WebSocket
	/// </summary>
	/// <typeparam name="T">Tipo dos dados da mensagem</typeparam>
	public class MensagemWebSocketDTO<T>
	{
		/// <summary>
		/// Tipo da Mensagem
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		///  Dados da mensagem
		/// </summary>
		public T Data { get; set; }
	}
}
