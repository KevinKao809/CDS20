using System;
using System.IO;
using System.Net;
using System.Text;

namespace IoTHubReceiver.Utilities
{
    //class RealTimeMessageSender
    //{
    //    private static string rtMessageFeedInURL;

    //    public RealTimeMessageSender(string url)
    //    {
    //        rtMessageFeedInURL = url;
    //    }

    //    public string PostRealTimeMessage(string postData)
    //    {
    //        try
    //        {
    //            var data = Encoding.UTF8.GetBytes(postData);

    //            var request = (HttpWebRequest)WebRequest.Create(rtMessageFeedInURL);
    //            request.Method = "POST";
    //            request.ContentType = "application/x-www-form-urlencoded";
    //            request.ContentLength = data.Length;

    //            using (var stream = request.GetRequestStream())
    //            {
    //                stream.Write(data, 0, data.Length);
    //            }

    //            var response = (HttpWebResponse)request.GetResponse();
    //            return new StreamReader(response.GetResponseStream()).ReadToEnd();
    //        }
    //        catch (Exception ex)
    //        {
    //            return ex.Message;
    //        }
    //    }
    //}
}
