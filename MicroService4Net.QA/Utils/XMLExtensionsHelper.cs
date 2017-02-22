using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using Microsoft.XmlDiffPatch;
using System.Xml.Schema;
using System.Collections;

namespace MicroService4Net.QA
{

    public static class XMLExtensionsHelper
    {
        #region Private Methods

        private static string GetQName(XElement xe)
        {
            string prefix = xe.GetPrefixOfNamespace(xe.Name.Namespace);
            if (xe.Name.Namespace == XNamespace.None || prefix == null)
                return xe.Name.LocalName.ToString();
            else
                return prefix + ":" + xe.Name.LocalName.ToString();
        }

        private static string GetQName(XAttribute xa)
        {
            string prefix =
                xa.Parent.GetPrefixOfNamespace(xa.Name.Namespace);
            if (xa.Name.Namespace == XNamespace.None || prefix == null)
                return xa.Name.ToString();
            else
                return prefix + ":" + xa.Name.LocalName;
        }

        private static string NameWithPredicate(XElement el)
        {
            if (el.Parent != null && el.Parent.Elements(el.Name).Count() != 1)
                return GetQName(el) + "[" +
                    (el.ElementsBeforeSelf(el.Name).Count() + 1) + "]";
            else
                return GetQName(el);
        }

        private static string StrCat<T>(this IEnumerable<T> source, string separator)
        {
            return source.Aggregate(new StringBuilder(),
                       (sb, i) => sb
                           .Append(i.ToString())
                           .Append(separator),
                       s => s.ToString());
        }

        #endregion

        public static string GetXPath(this XObject xobj)
        {
            if (xobj.Parent == null)
            {
                XDocument doc = xobj as XDocument;
                if (doc != null)
                    return ".";
                XElement el = xobj as XElement;
                if (el != null)
                    return "/" + NameWithPredicate(el);
                // the XPath data model does not include white space text nodes  
                // that are children of a document, so this method returns null.  
                XText xt = xobj as XText;
                if (xt != null)
                    return null;
                XComment com = xobj as XComment;
                if (com != null)
                    return
                        "/" +
                        (
                            com
                            .Document
                            .Nodes()
                            .OfType<XComment>()
                            .Count() != 1 ?
                            "comment()[" +
                            (com
                            .NodesBeforeSelf()
                            .OfType<XComment>()
                            .Count() + 1) +
                            "]" :
                            "comment()"
                        );
                XProcessingInstruction pi = xobj as XProcessingInstruction;
                if (pi != null)
                    return
                        "/" +
                        (
                            pi.Document.Nodes()
                            .OfType<XProcessingInstruction>()
                            .Count() != 1 ?
                            "processing-instruction()[" +
                            (pi
                            .NodesBeforeSelf()
                            .OfType<XProcessingInstruction>()
                            .Count() + 1) +
                            "]" :
                            "processing-instruction()"
                        );
                return null;
            }
            else
            {
                XElement el = xobj as XElement;
                if (el != null)
                {
                    return
                        "/" +
                        el
                        .Ancestors()
                        .InDocumentOrder()
                        .Select(e => NameWithPredicate(e))
                        .StrCat("/") +
                        NameWithPredicate(el);
                }
                XAttribute at = xobj as XAttribute;
                if (at != null)
                    return
                        "/" +
                        at
                        .Parent
                        .AncestorsAndSelf()
                        .InDocumentOrder()
                        .Select(e => NameWithPredicate(e))
                        .StrCat("/") +
                        "@" + GetQName(at);
                XComment com = xobj as XComment;
                if (com != null)
                    return
                        "/" +
                        com
                        .Parent
                        .AncestorsAndSelf()
                        .InDocumentOrder()
                        .Select(e => NameWithPredicate(e))
                        .StrCat("/") +
                        (
                            com
                            .Parent
                            .Nodes()
                            .OfType<XComment>()
                            .Count() != 1 ?
                            "comment()[" +
                            (com
                            .NodesBeforeSelf()
                            .OfType<XComment>()
                            .Count() + 1) + "]" :
                            "comment()"
                        );
                XCData cd = xobj as XCData;
                if (cd != null)
                    return
                        "/" +
                        cd
                        .Parent
                        .AncestorsAndSelf()
                        .InDocumentOrder()
                        .Select(e => NameWithPredicate(e))
                        .StrCat("/") +
                        (
                            cd
                            .Parent
                            .Nodes()
                            .OfType<XText>()
                            .Count() != 1 ?
                            "text()[" +
                            (cd
                            .NodesBeforeSelf()
                            .OfType<XText>()
                            .Count() + 1) + "]" :
                            "text()"
                        );
                XText tx = xobj as XText;
                if (tx != null)
                    return
                        "/" +
                        tx
                        .Parent
                        .AncestorsAndSelf()
                        .InDocumentOrder()
                        .Select(e => NameWithPredicate(e))
                        .StrCat("/") +
                        (
                            tx
                            .Parent
                            .Nodes()
                            .OfType<XText>()
                            .Count() != 1 ?
                            "text()[" +
                            (tx
                            .NodesBeforeSelf()
                            .OfType<XText>()
                            .Count() + 1) + "]" :
                            "text()"
                        );
                XProcessingInstruction pi = xobj as XProcessingInstruction;
                if (pi != null)
                    return
                        "/" +
                        pi
                        .Parent
                        .AncestorsAndSelf()
                        .InDocumentOrder()
                        .Select(e => NameWithPredicate(e))
                        .StrCat("/") +
                        (
                            pi
                            .Parent
                            .Nodes()
                            .OfType<XProcessingInstruction>()
                            .Count() != 1 ?
                            "processing-instruction()[" +
                            (pi
                            .NodesBeforeSelf()
                            .OfType<XProcessingInstruction>()
                            .Count() + 1) + "]" :
                            "processing-instruction()"
                        );
                return null;
            }
        }

        public static string CompareXML(string SourceXMLContent, string TargetXMLContent, out bool isEqual)
        {
            var diffdir = AppDomain.CurrentDomain.BaseDirectory;

            Random r = new Random();
            var difffile = Path.Combine(diffdir, r.Next() + ".txt");

            XmlDiffView dv = new XmlDiffView();

            StringReader sourceRdr = new StringReader(SourceXMLContent);
            XmlReader sourcexmlreader = XmlReader.Create(sourceRdr);

            StringReader targetRdr = new StringReader(TargetXMLContent);
            XmlReader targetxmlreader = XmlReader.Create(targetRdr);

            try
            {
                using (XmlTextWriter tw = new XmlTextWriter(new StreamWriter(difffile)) { Formatting = Formatting.Indented })
                {
                    XmlDiff diffnew = new XmlDiff() { Options = new XmlDiffOptions(), Algorithm = XmlDiffAlgorithm.Auto };
                    isEqual = diffnew.Compare(sourcexmlreader, targetxmlreader, tw);
                }

                using (StringReader sourceRdr1 = new StringReader(SourceXMLContent))
                {
                    using (XmlTextReader orig = new XmlTextReader(sourceRdr1))
                    {
                        using (XmlTextReader diffGram = new XmlTextReader(difffile))
                        {
                            dv.Load(orig, diffGram);
                        }
                        File.Delete(difffile);
                    }
                }
            }
            catch (Exception ex)
            {
                isEqual = false;
                string msg = ex.Message;

            }
            finally
            {
                sourcexmlreader.Close();
                targetxmlreader.Close();
                sourceRdr.Close();
                targetRdr.Close();
            }


            StringBuilder sb = new StringBuilder("");
            sb.Append("<html><body><table width='100%'>");
            //Write Legend.
            sb.Append("<tr><td colspan='2' align='center'><b>Legend:</b>");
            sb.Append("<font style='background-color: yellow' color='black'>added</font>&nbsp;&nbsp;");
            sb.Append("<font style='background-color: red' color='black'>removed</font>&nbsp;&nbsp;");
            sb.Append("<font style='background-color: lightgreen' color='black'>changed</font>&nbsp;&nbsp;");
            sb.Append("<font style='background-color: red' color='blue'>moved from</font>&nbsp;&nbsp;");
            sb.Append("<font style='background-color: yellow' color='blue'>moved to</font>&nbsp;&nbsp;");
            sb.Append("<font style='background-color: white' color='#AAAAAA'>ignored</font></td></tr>");
            sb.Append("<tr><td><b> Source XML Content : ");
            sb.Append("</b></td><td><b> Target XML Content : ");
            sb.Append("</b></td></tr>");

            string tempFile = Path.Combine(diffdir, r.Next() + ".htm");
            using (StreamWriter sw1 = new StreamWriter(tempFile))
            {
                sw1.Write(sb.ToString());
                dv.GetHtml(sw1);
                sw1.Write("</table></body></html>");
                sw1.Close();
            }

            string result = "";
            using (var sr = new System.IO.StreamReader(tempFile))
            {
                result = sr.ReadToEnd();
            }
            File.Delete(tempFile);
            return result;

        }

        /// <summary>
        /// Parse element from XSD file.
        /// </summary>
        /// <param name="XSDFilePath"></param>
        public static void ParseXSD(string XSDFilePath)
        {
            List<string> returnList = new List<string>();
            if (File.Exists(XSDFilePath))
            {

                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.ValidationEventHandler += new ValidationEventHandler(ShowCompileError);
                schemaSet.Add("http://equityapi.morningstar.com/", XSDFilePath);
                schemaSet.Compile();

                XmlSchema customerSchema = null;
                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    customerSchema = schema;
                }

                foreach (XmlSchemaElement element in customerSchema.Elements.Values)
                {
                    ParseSchemaElement(element);
                }
            }
            else
            {
                Console.WriteLine("文件不存在");
            }

        }

        private static void ParseSchemaElement(XmlSchemaElement element)
        {
            XmlSchemaComplexType complexType = element.ElementSchemaType as XmlSchemaComplexType;
            if (complexType != null)
            {
                if (complexType.AttributeUses.Count > 0)
                {
                    IDictionaryEnumerator enumerator = complexType.AttributeUses.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        XmlSchemaAttribute attribute = (XmlSchemaAttribute)enumerator.Value;
                        Console.WriteLine("Attribute: {0}", attribute.Name);
                    }
                }
                XmlSchemaSequence sequence = complexType.ContentTypeParticle as XmlSchemaSequence;
                foreach (XmlSchemaElement childElement in sequence.Items)
                {
                    Console.WriteLine("Element: {0}", childElement.Name);
                    //Console.WriteLine("DefaultValue:" + childElement.DefaultValue);
                    Console.WriteLine("ElementSchemaType:" + childElement.ElementSchemaType.ToString());
                    //Console.WriteLine("MaxOccursString:" + childElement.MaxOccursString);
                    //Console.WriteLine("MinOccursString:" + childElement.MinOccursString);
                    //Console.WriteLine("Parent:" + childElement.Parent.ToString());

                    ParseSchemaElement(childElement);
                }
            }

        }

        static void ShowCompileError(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Console.Write("WARNING: ");
                Console.WriteLine(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                Console.Write("ERROR: ");
                Console.WriteLine(e.Message);
            }
        }

    }

}