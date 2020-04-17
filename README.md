# Purpose
This tool is used to monitor builds running in Visual Studio Team Services.

The tool will send text messages through Twilio when a build is completed.

# Prerequisites
## VSTS
You need builds configured in VSTS. You must know the following and add to `appSettings` in App.config:

 * **VSTSURI** The VSTS URI (the same URI used to access the service in your browser).
 * **VSTSCollection** The Collection your Project lives in (by default it is "DefaultCollection").
 * **VSTSProject** The Project name.
 * **VSTSBranchName** (optional) The Branch name to monitor builds for.
 * **VSTSBuildRequestedByUserName** (optional) The user name to monitor builds for.

## Twilio
You must have a Twilio account and add the following to `appSettings` in App.config:

 * **TwilioAccountSid** Your Twillio account SID.
 * **TwilioAuthToken** Your Twilio auth token.
 * **TwilioFromNumber** Your Twilio phone number to send text messages from.
 * **TwilioToNumber** A phone number to send text messages to.

# How to use
After configuring App.config, simply run the `Notifier` project.
