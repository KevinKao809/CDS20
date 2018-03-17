using Newtonsoft.Json.Linq;
using System;
using System.Text;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using CDSShareLib.Helper;

namespace EmailApp.Model
{
    public class SendGridThread
    {        
        int _TaskId;
        string _ApiKey;
        EmailModel emailCtx = new EmailModel();

        public SendGridThread(string apiKey, JObject jsonMessage, int taskId)
        {
            _ApiKey = apiKey;
            _TaskId = taskId;
            emailCtx.senderName = jsonMessage["Content"]["senderName"].ToString();
            emailCtx.senderEmail = jsonMessage["Content"]["senderEmail"].ToString();
            emailCtx.receverName = jsonMessage["Content"]["receverName"].ToString();
            emailCtx.receverEmail = jsonMessage["Content"]["receverEmail"].ToString();
            emailCtx.subject = jsonMessage["Content"]["subject"].ToString();
            emailCtx.plainTextContent = jsonMessage["Content"]["plainTextContent"].ToString();
            emailCtx.htmlContent = jsonMessage["Content"]["htmlContent"].ToString();
        }

        public async void ThreadProc()
        {
            AzureSQLHelper.OperationTaskModel operationTask = new AzureSQLHelper.OperationTaskModel();
            try
            {
                var client = new SendGridClient(_ApiKey);
                var from = new EmailAddress(emailCtx.senderEmail, emailCtx.senderName);
                var to = new EmailAddress(emailCtx.receverEmail, emailCtx.receverName);
                var emailContext = MailHelper.CreateSingleEmail(from, to, emailCtx.subject, emailCtx.plainTextContent, emailCtx.htmlContent);
                await client.SendEmailAsync(emailContext);
                EmailApp._appLogger.Info("[SendGrid Thread] send email success. Receive Email: " + emailCtx.receverEmail);
                operationTask.UpdateTaskBySuccess(_TaskId);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("[Send Grid Thread] Failed. Receiver" + emailCtx.receverEmail);
                logMessage.AppendLine("\tMessage:" + JsonConvert.SerializeObject(this));
                logMessage.AppendLine("\tException:" + ex.Message);
                EmailApp._appLogger.Error(logMessage);
                operationTask.UpdateTaskByFail(_TaskId, ex.Message);
            }
        }
    }
}
