using CDSShareLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfShareLib
{/*
    public class CDSException : System.Exception
    {
        public class Detail
        {
            public int ErrorCode;
            public string ErrorMessage;

            public Detail(int id, string message)
            {
                this.ErrorCode = id;
                this.ErrorMessage = message;
            }
        }

        private static Dictionary<int, string> cdsErrorMessage = new Dictionary<int, string>();
        private static void loadErrorMessage()
        {
            if (cdsErrorMessage.Count == 0)
            {
                AzureSQLHelper.ErrorMessageModel dbhelp = new AzureSQLHelper.ErrorMessageModel();
                var errorMessageList = dbhelp.GetAllErrorMessage();

                if (errorMessageList != null)
                    foreach (var c in errorMessageList)
                        cdsErrorMessage.Add(c.Id, c.Message);
            }
        }

        public static Detail GetCDSErrorMessageByCode(int id)
        {
            if (cdsErrorMessage.ContainsKey(id) == true)
            {
                return new Detail(id, cdsErrorMessage[id]);
            }
            else
            {
                loadErrorMessage();
                if (cdsErrorMessage.ContainsKey(id) == true)
                {
                    return new Detail(id, cdsErrorMessage[id]);
                }
                else
                {
                    return new Detail(id, "Error Message Not Found.");
                }
            }
        }


        public int ErrorId = 0;
        public CDSException() : base()
        {
        }
        public CDSException(int Id) : base()
        {
            this.ErrorId = Id;
        }
        public CDSException(string message, int Id) : base(message)
        {
            this.ErrorId = Id;
        }
        public CDSException(string message, Exception inner, int Id) : base(message, inner)
        {
            this.ErrorId = Id;
        }
    }
    */
}
