using CDSShareLib;
using CDSShareLib.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace EventActionApp.Model
{
    public class EventThread
    {
        int _CompanyId;
        string _IoTDeviceId;
        int _MessageCatalogId;
        int _EventRuleCatalogId;
        string _EventRuleCatalogName;
        dynamic _Message;
        dynamic _FullAlarmMessage;
        
        public EventThread(JObject jsonMessage)
        {
            _CompanyId = int.Parse(jsonMessage["companyId"].ToString());
            _IoTDeviceId = jsonMessage["iotDeviceId"].ToString();
            _MessageCatalogId = int.Parse(jsonMessage["messageCatalogId"].ToString());
            _EventRuleCatalogId = int.Parse(jsonMessage["eventRuleCatalogId"].ToString());
            _EventRuleCatalogName = jsonMessage["eventRuleCatalogName"].ToString();
            _Message = JObject.Parse(jsonMessage["messageContent"].ToString());
            _FullAlarmMessage = JObject.Parse(jsonMessage.ToString());
        }

        public async void ThreadProc()
        {
            AzureSQLHelper.EventRuleCatalogModel eventRuleModel = new AzureSQLHelper.EventRuleCatalogModel();
            List<Application> appList = eventRuleModel.GetActionApplicationById(_MessageCatalogId);
            try
            {
                foreach (var app in appList)
                {
                    string applicationTargetType = app.TargetType.ToLower();
                    try
                    {
                        WebHelper webHelper = new WebHelper();
                        JObject outputTemplate = new JObject();
                        switch (applicationTargetType)
                        {
                            case "webapi":                                
                                outputTemplate = ParsingOutputTemplate(app.MessageTemplate);
                                switch (app.Method.ToLower())
                                {
                                    case "post-x-www":
                                        string postData = ConvertJObjectToQueryString(outputTemplate);
                                        switch (app.AuthType.ToLower())
                                        {
                                            case "none":

                                                webHelper.PostContent(app.ServiceURL, postData);
                                                break;
                                            case "basic auth":
                                                webHelper.PostContent(app.ServiceURL, postData, app.AuthID, app.AuthPW);
                                                break;
                                        }
                                        break;
                                    case "post-multi":
                                        NameValueCollection formData = new NameValueCollection();
                                        foreach (var elem in outputTemplate)
                                        {
                                            formData.Add(elem.Key, outputTemplate[elem.Key].ToString());
                                        }
                                        switch (app.AuthType.ToLower())
                                        {
                                            case "none":
                                                webHelper.PostMultipartContent(app.ServiceURL, formData);
                                                break;
                                            case "basic auth":
                                                break;
                                        }
                                        break;
                                    case "post-json":
                                        switch (app.AuthType.ToLower())
                                        {
                                            case "none":
                                                webHelper.PostJsonContent(app.ServiceURL, JsonConvert.SerializeObject(outputTemplate));
                                                break;
                                            case "basic auth":
                                                webHelper.PostJsonContent(app.ServiceURL, JsonConvert.SerializeObject(outputTemplate), app.AuthID, app.AuthPW);
                                                break;
                                        }
                                        break;
                                }
                                break;
                            case "dashboard":
                                try
                                {
                                    webHelper.PostContent(EventActionApp._rtEventFeedInURL, JsonConvert.SerializeObject(_FullAlarmMessage));
                                }
                                catch (Exception ex)
                                {
                                    EventActionApp._appLogger.Error("RTEventFeedInURL Exception: " + ex.Message);
                                }                                
                                break;
                            case "iot device":
                                int deviceId = int.Parse(app.ServiceURL);
                                AzureSQLHelper.IoTDeviceModel iotDeviceModel = new AzureSQLHelper.IoTDeviceModel();
                                IoTDevice iotDevice = iotDeviceModel.GetById(deviceId);
                                try
                                {
                                    IoTHubHelper iotHubHelper = new IoTHubHelper(iotDevice.IoTHub.IoTHubConnectionString);
                                    outputTemplate = ParsingOutputTemplate(app.MessageTemplate);
                                    iotHubHelper.SendC2DMessage(iotDevice.IoTHubDeviceID, outputTemplate);
                                }
                                catch (Exception ex)
                                {
                                    EventActionApp._appLogger.Error("C2D Message Error, DeviceID:" + iotDevice.Id + "; Exception:" + ex.Message);
                                }
                                break;
                            case "email":
                                break;
                            case "sms":
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        EventActionApp._appLogger.Error("EventAction Exception: " + ex.Message + ";" + app.Name + "; (id:" + app.Id + ") failed: "); 
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("EventAction Task Exception: " + ex.Message);
                logMessage.AppendLine("EventAction Task Message: " + Convert.ToString(_FullAlarmMessage));
                EventActionApp._appLogger.Error(logMessage);
            }
        }

        private JObject ParsingOutputTemplate(string messageTemplate)
        {
            JObject outputTemplate = JObject.Parse(messageTemplate);

            foreach (var elem in outputTemplate)
            {
                string valueStr = elem.Value.ToString();
                List<int> allAtIndexOfString = AllIndexesOf(valueStr, "@");

                if (allAtIndexOfString.Count > 0 && (allAtIndexOfString.Count % 2 == 0))
                {
                    Dictionary<string, string> strMappintReplacement = new Dictionary<string, string>();
                    for (int index = 0; index < allAtIndexOfString.Count; index += 2)
                    {
                        int length = allAtIndexOfString[index + 1] - allAtIndexOfString[index] + 1;
                        string waitReplaceStr = valueStr.Substring(allAtIndexOfString[index], length);
                        string messageKey = waitReplaceStr.Replace("@", "");

                        string replaceStr = _Message[messageKey];
                        if (!strMappintReplacement.ContainsKey(waitReplaceStr))
                            strMappintReplacement.Add(waitReplaceStr, replaceStr);
                    }
                    foreach (var key in strMappintReplacement.Keys)
                    {
                        string replaceStr = strMappintReplacement[key];
                        valueStr = valueStr.Replace(key, replaceStr);
                    }
                    outputTemplate[elem.Key] = valueStr;
                }
            }
            return outputTemplate;
        }
        private List<int> AllIndexesOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        private string ConvertJObjectToQueryString(JObject jObj)
        {
            string queryString = "";
            foreach (var obj in jObj)
            {
                queryString += obj.Key + "=" + obj.Value + "&";
            }

            if (!string.IsNullOrEmpty(queryString))
                queryString = queryString.Substring(0, queryString.Length - 1);

            return queryString;
        }
    }
}
