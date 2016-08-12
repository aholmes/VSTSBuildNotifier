// Download the twilio-csharp library from twilio.com/docs/csharp/install
using System;
using Twilio;

namespace Twilio
{
	public class Sender
	{
		private string AccountSid;
		private string AuthToken;

		public Sender(string accountSid, string authToken)
		{
			AccountSid = accountSid;
			AuthToken = authToken;
		}

		public string Send(string fromNumber, string toNumber, string messageBody)
		{
			// Find your Account Sid and Auth Token at twilio.com/user/account

			var twilio = new TwilioRestClient(AccountSid, AuthToken);

			return twilio.SendMessage(fromNumber, toNumber, messageBody).Sid;
		}
	}
}