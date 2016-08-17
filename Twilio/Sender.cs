namespace Twilio
{
	public class Sender
	{
		/// <summary>
		/// Your Twilio Account SID.
		/// </summary>
		private string AccountSid;

		/// <summary>
		/// Your Twilio Auth Token.
		/// </summary>
		private string AuthToken;

		/// <summary>
		/// Configure the Twilio client.
		/// </summary>
		/// <param name="accountSid"></param>
		/// <param name="authToken"></param>
		public Sender(string accountSid, string authToken)
		{
			AccountSid = accountSid;
			AuthToken  = authToken;
		}

		/// <summary>
		/// Send a text message.
		/// </summary>
		/// <param name="fromNumber"></param>
		/// <param name="toNumber"></param>
		/// <param name="messageBody"></param>
		/// <returns></returns>
		public string Send(string fromNumber, string toNumber, string messageBody)
		{
			var twilio = new TwilioRestClient(AccountSid, AuthToken);

			return twilio.SendMessage(fromNumber, toNumber, messageBody).Sid;
		}
	}
}