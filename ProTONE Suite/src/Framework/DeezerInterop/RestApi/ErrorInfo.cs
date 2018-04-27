using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPMedia.DeezerInterop.RestApi
{
    [JsonObject("error")]
    public class ErrorInfo
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }

        public bool IsEmpty
        {
            get
            {
                bool ret = true;

                ret &= string.IsNullOrEmpty(Type);
                ret &= string.IsNullOrEmpty(Message);
                ret &= string.IsNullOrEmpty(Code);

                return ret;
            }
        }

        public override string ToString()
        {
            return $"{Type} Message={Message} Code={Code}";
        }

        public static bool IsNullOrEmpty(ErrorInfo err)
        {
            return (err == null || err.IsEmpty);
        }
    }
}
