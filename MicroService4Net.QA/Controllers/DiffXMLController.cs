using System;
using System.Linq;
using System.Collections.Generic;
using MicroService4Net.QA.Models;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace MicroService4Net.QA.Controllers
{

    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("DiffXML")]
    public class DiffXMLController : ApiController
    {
        /// <summary>
        /// 比较两个XML的内容，并输出差异报告
        /// </summary>
        /// <param name="diffXMLObj">Json对象字符串</param>
        /// <returns>差异报告，输出HTML格式的字符串</returns>
        /// <remarks>
        /// Post传递过来的对象是形同{"SourceXMLContent":"xml_content","TargetXMLContent":"xml_content"}的Json字串, 注意两边有大括号
        /// Post地址为http://localhost:8080/DiffXML/CompareXMLDiff
        /// Post命令执行不成功的话，请检查XML的内容是否都经过转义处理，双引号是否转成了单引号。
        /// </remarks>
        [Route("CompareXMLDiff")]
        [HttpPost]
        public HttpResponseMessage CompareXMLDiff([FromBody]DiffXMLObj diffXMLObj)
        {
            if (diffXMLObj == null)
            {
                throw new HttpRequestException();
            }
            string SourceXMLContent = diffXMLObj.SourceXMLContent;
            string TargetXMLContent = diffXMLObj.TargetXMLContent;

            bool isEqual = false;
            string result = XMLExtensionsHelper.CompareXML(SourceXMLContent, TargetXMLContent, out isEqual);
            return OutputHelper.ToHTML(result);
        }
    }

    [RoutePrefix("Json")]
    public class JsonSchemaController : ApiController
    {
        /// <summary>
        /// 校验Json Data 是否匹配Json Schema
        /// </summary>
        /// <param name="JsonModel">Json 提交对象字符串</param>
        /// <returns>校验成功字串或校验失败字串集合</returns>
        /// <remarks>
        /// Post传递过来的对象是形同{"JsonDataContent":"json_data_text","JsonSchemaContent":"json_schema_text"}的Json字串, 注意两边有大括号
        /// Post地址为http://localhost:8080/Json/ValidateJsondataWithJsonSchema
        /// Post命令执行不成功的话，请检查XML的内容是否都经过转义处理，双引号是否转成了单引号。
        /// </remarks>
        [Route("ValidateJsondataWithJsonSchema")]
        [HttpPost]
        public HttpResponseMessage ValidateJsondataWithJsonSchema([FromBody]JsonValidateModel JsonModel)
        {
            if (JsonModel == null)
            {
                throw new HttpRequestException();
            }
            string JsonDataContent = JsonModel.JsonDataContent;
            string JsonSchemaContent = JsonModel.JsonSchemaContent;
            var result = JsonHelper.ValidateJsonSchema(JsonSchemaContent, JsonDataContent);
            return OutputHelper.ToHTML(result);
        }
    }


}