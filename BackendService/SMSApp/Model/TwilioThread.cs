using CDSShareLib.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SMSApp.Model
{
    public class TwilioThread
    {
        int _TaskId;
        string _twilioAccountId, _twilioToken;
        SMSModel smsCtx = new SMSModel();

        public TwilioThread(string twilioAccountId, string twilioToken, string twilioPhoneNumber, JObject jsonMessage, int taskId)
        {
            _twilioAccountId = twilioAccountId;
            _twilioToken = twilioToken;
            _TaskId = taskId;
            smsCtx.senderPhoneNumber = twilioPhoneNumber;
            smsCtx.receiverPhoneNumber = jsonMessage["Content"]["receiverPhoneNumber"].ToString(); 
            smsCtx.smsContent = jsonMessage["Content"]["smsContent"].ToString();
        }

        public async void ThreadProc()
        {
            AzureSQLHelper.OperationTaskModel operationTask = new AzureSQLHelper.OperationTaskModel();
            try
            {
                if (!smsCtx.receiverPhoneNumber.StartsWith("+"))
                    smsCtx.receiverPhoneNumber = "+" + smsCtx.receiverPhoneNumber;

                // Initialize the Twilio client
                TwilioClient.Init(_twilioAccountId, _twilioToken);
                MessageResource.Create(
                        from: new PhoneNumber(smsCtx.senderPhoneNumber),
                        to: new PhoneNumber(smsCtx.receiverPhoneNumber),                                                    // Message content
                        body: smsCtx.smsContent);

                SMSApp._appLogger.Info("[Twilio Thread] send SMS success. Receiver PhoneNumber: " + smsCtx.receiverPhoneNumber);
                operationTask.UpdateTaskBySuccess(_TaskId);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("[Twilio Thread] Failed. Receiver" + smsCtx.receiverPhoneNumber);
                logMessage.AppendLine("\tMessage:" + JsonConvert.SerializeObject(this));
                logMessage.AppendLine("\tException:" + ex.Message);
                SMSApp._appLogger.Error(logMessage);
                operationTask.UpdateTaskByFail(_TaskId, ex.Message);
            }
        }
    }
}
