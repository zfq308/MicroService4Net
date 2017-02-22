using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroService4Net.QA.Models
{
    public class Student
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }


    public class DiffXMLObj
    {
        public string SourceXMLContent { get; set; }
        public string TargetXMLContent { get; set; }
    }

    public class JsonValidateModel
    {
        /// <summary>
        /// Json Data content
        /// </summary>
        public string JsonDataContent { get; set; }
        /// <summary>
        /// Json schema content
        /// </summary>
        public string JsonSchemaContent { get; set; }
    }

}
