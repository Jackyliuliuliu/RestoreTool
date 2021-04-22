//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UIH.Mcsf.Core;

//namespace MysqlRestoreTool.Common
//{
//    public class ReadWriteXml
//    {
//        /// <summary>
//        /// 读xml全部内容
//        /// </summary>
//        /// <param name="xmlPath"></param>
//        /// <returns></returns>
//        public static string GetXmlString(string xmlPath)
//        {
//            try
//            {
//                bool paserResult;
//                IFileParserCSharp parser = ConfigParserFactory.Instance().CreateCSharpFileParser();
//                parser.Initialize();

//                if (xmlPath.IndexOf(":") != -1)//判断是否是绝对路径.
//                {
//                    paserResult = parser.ParseByURI(xmlPath);
//                }
//                else
//                {
//                    paserResult = parser.OpenFromUserSettingsDir(xmlPath);
//                }

//                Logger.WriteLog("[GetXmlString]: ParseByURI: " + paserResult + ",xmlPath:" + xmlPath);
//                string xmlString = parser.WriteToString();
//                parser.Terminate();
//                return xmlString;
//            }
//            catch (Exception ex)
//            {
//                Logger.WriteLog("[GetXmlString]: method throw exception: " + ex.Message);
//                return null;
//            }
//        }

//        /// <summary>
//        ///  读指定路径的xml的节点的值
//        /// </summary>
//        /// <param name="xmlPath">XML文件的路径</param>
//        /// <param name="nodeUrl">节点的路径</param>
//        /// <returns>节点的值</returns>
//        public static string GetXmlNodeValue(string xmlPath, string nodeUrl)
//        {
//            try
//            {
//                IFileParserCSharp parser = ConfigParserFactory.Instance().CreateCSharpFileParser();
//                parser.Initialize();
//                bool paserResult = parser.ParseByURI(xmlPath);
//                Logger.WriteLog("[GetXmlNodeValue]: ParseByURI: " + paserResult + ",xmlPath:" + xmlPath);
//                string nodeValue = parser.GetStringValueByPath(nodeUrl);
//                parser.Terminate();
//                return nodeValue;
//            }
//            catch (Exception ex)
//            {
//                Logger.WriteLog("[GetXmlNodeValue]:  method throw exception: " + ex.Message);
//                return null;
//            }
//        }
//    }
//}
