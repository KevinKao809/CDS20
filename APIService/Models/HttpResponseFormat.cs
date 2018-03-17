using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfAPIService.Models
{
    public class HttpResponseFormat
    {
        class Detail
        {
            public int ErrorCode;
            public string ErrorMessage;

            public Detail(int id, string message)
            {
                this.ErrorCode = id;
                this.ErrorMessage = message;
            }
        }

        public static Object Success()
        {
            return new { };
        }
        public static Object Success(int id)
        {
            return new { Id = id };
        }
        public static Object Success(string imageUrl)
        {
            return new { ImageUrl = imageUrl };
        }
        public static Object UnsupportedMediaType()
        {
            return new { };
        }
        public static Object Error(string errorMsg)
        {
            Detail errorMessage = new Detail(0, errorMsg);
            return errorMessage;
        }
        public static Object InvaildData()
        {
            Detail errorMessage = new Detail(0, "Invaild data");
            return errorMessage;
        }
    }
}
