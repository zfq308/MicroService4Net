using System;
using System.Linq;
using System.Collections.Generic;
using MicroService4Net.QA.Models;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MicroService4Net.QA
{

    public class OutputHelper
    {
        public static HttpResponseMessage ToJson(object obj)
        {
            string str;
            if (obj is string || obj is char)
            {
                str = obj.ToString();
            }
            else
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                str = serializer.Serialize(obj);
            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        public static HttpResponseMessage ToXML(object obj)
        {
            string str;
            if (obj is string || obj is char)
            {
                str = obj.ToString();
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                MemoryStream mem = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(mem, Encoding.UTF8);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                serializer.Serialize(writer, obj, ns);
                writer.Close();
                str = Encoding.UTF8.GetString(mem.ToArray());
            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/xml") };
            return result;
        }

        public static HttpResponseMessage ToString(object obj)
        {
            string str;
            if (obj is string || obj is char)
            {
                str = obj.ToString();
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/text") };
                return result;
            }
            else
            {
                throw new HttpRequestException("ToString() in class OutputHelper need string object as input parameter.");
            }
        }


        public static HttpResponseMessage ToHTML(object obj)
        {
            string str;
            if (obj is string || obj is char)
            {
                str = obj.ToString();
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "text/HTML") };
                return result;
            }
            else
            {
                throw new HttpRequestException("ToString() in class OutputHelper need string object as input parameter.");
            }
        }
    }

}