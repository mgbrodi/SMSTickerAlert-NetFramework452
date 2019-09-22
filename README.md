# SMSTickerAlert

SMS Ticker Alert is a .NET Framework4.5.2 legacy application that contains three projects:
- ModelsCommons -> a dll providing Utilities and Models shared between projects. The DB approach is Code First and in the file named Models/TickerDatabaseInit.cs the3re is a preconfigured list of tickers, change it to add or remove tickers
- SMSTickerAlert -> a WebForm application that allows to list all the tickers in the system and to configure alerts. An alerts is defined based on a selected ticker, high and a low threshold and a mobile number. Ony one alert is allowed per mobile number. Any subsequent add with the same number will result in an update of teh record already present.
- TickerUpdater -> a Console Application (acting as a windows service) that on predefined intervals downolads the latest value for the tickers defined in the DB form http://marketwatch.com/. After the update is complete it checks if there are text messages to be sent based on thresholds defined in each alert. Once an alert has been processed (failure or success) it is removed from the DB.

In order to run the applicationyou will need to:

Set up a SQLDB and collect credentials.

Set up an account from https://www.twilio.com/. Once connected to twilio create a project and go to the Dashboard, follow the Settings link to verify aone or more mobile numbers to send Text messages to. While on teh page to verify the mobile number click on the Manage Numbers link on the left side to collect the number that is used to configure the SMSFrom property. Go back to teh Dashboard and collect Account and Token information.

To configure the system:
- in web.config fill in the proper values for the connection string for "TickerSQLConnection"
- in app.config fill in the proper values for the connection string for "TickerSQLConnection", fill in the SMSKeyToken, SMSAccount and SMSFrom from your Twilio account from https://www.twilio.com/

To run in CloudFoundry:
To configure the system for CloudFoundry: Connect to CloudFoundry and select the org and space. Build the SMSTickerAlert solution.

Go to the TickerUpdater Directory.
- cf push -c C:\Users\vcap\app\TickerUpdater.exe

Publish the SMSTickerAlert Project. Go to the SMSTickerAlert directory.

- cf push 

Always start the Console application first to populate/update the DB with the latest values for the tickers, leave it running and start the Webform.
