namespace Twilio
{
	public class Sender
	{
		private string AccountSid;
		private string AuthToken;

		public Sender(string accountSid, string authToken)
		{
			AccountSid = accountSid;
			AuthToken  = authToken;
		}

		public string Send(string fromNumber, string toNumber, string messageBody)
		{
			var twilio = new TwilioRestClient(AccountSid, AuthToken);

			return twilio.SendMessage(fromNumber, toNumber, messageBody).Sid;
		}
	}
}