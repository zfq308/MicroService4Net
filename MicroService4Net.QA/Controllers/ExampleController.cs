using MicroService4Net.QA.Models;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace MicroService4Net.QA.Controllers
{


  

    //[RoutePrefix("api")]
    public class ExampleController : ApiController
    {
        #region Demo


        [Route("Example")]
        public string GetExample()
        {
            return "Example";
        }

        [Route("Example2.aspx", Order = 1)]
        public HttpResponseMessage GetExample2()
        {
            string userName = "Benjamin Zhou";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str = serializer.Serialize(userName);

            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        [Route("Example3.aspx")]
        public HttpResponseMessage GetExample3()
        {
            string userName = "<root>Benjamin Zhou</root>";
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(userName, Encoding.GetEncoding("UTF-8"), "application/xml") };
            return result;
        }

        [HttpGet]
        [Route("api/apiname/{name}/apipassword/{password}")]
        public string GetExample3(string name, string password)
        {
            return name + password;
        }

        [HttpGet]
        [Route("api2/name/{name}/score/{score:int}")]
        public string GetExample4(string name, int score)
        {
            return string.Format("Name:{0}, Score:{1}.", name, score);
        }

        /// <summary>
        /// 调用时， 将直接匹配http://localhost:8080/search.aspx?Id=1&Name=Benjamin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("search.aspx/{Id=id}/{Name=name}")]
        public string GetExample5(string id, string name)
        {
            return string.Format("Name:{0}, Id:{1}.", name, id);

        }






        //{x:alpha}     约束大小写英文字母
        //{x:bool}
        //{x:datetime}
        //{x:decimal}
        //{x:double}
        //{x:float}
        //{x:guid}
        //{x:int}
        //{x:length(6)}
        //{x:length(1,20)} 约束长度范围
        //{x:long}
        //{x:maxlength(10)}
        //{x:min(10)}
        //{x:range(10,50)}
        //{x:regex(正则表达式)}

        #endregion

        [Route("api/poststudent")]
        public HttpResponseMessage PostStudent([FromBody]Student student)
        {

            if (student == null)
            {
                throw new HttpRequestException();
            }
            student.Score += 100;
            return OutputHelper.ToJson(student);
        }


    }


}
