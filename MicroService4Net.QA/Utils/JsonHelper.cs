using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace MicroService4Net.QA
{
    public class JsonHelper
    {
        /// <summary>
        /// Use Json data string to validate the json schema content.
        /// </summary>
        /// <param name="schematext"></param>
        /// <param name="jsondatatext"></param>
        /// <returns></returns>
        public static string ValidateJsonSchema(string schematext, string jsondatatext)
        {
            var schema = JsonSchema.Parse(schematext);
            JObject obj = JObject.Parse(jsondatatext);
            IList<string> ErrorMsgList = new List<string>();
            bool valid = obj.IsValid(schema, out ErrorMsgList);
            if (valid)
                return "Valid Success.";
            else
            {
                StringBuilder sb = new StringBuilder("");
                foreach (string err in ErrorMsgList)
                {
                    sb.AppendLine(err);
                }
                return sb.ToString();
            }
        }
    }
}
