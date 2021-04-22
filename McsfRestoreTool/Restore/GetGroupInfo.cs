using MysqlRestoreTool.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UIH.Mcsf.Restore.Logger;

namespace MysqlRestoreTool.Restore
{
    public class GetGroupInfo : Subtask
    {
        public override void Execute()
        {
            #region
            //get gourp info
            List<Dictionary<string, object>> groupListResult = new List<Dictionary<string, object>>();
            var path = Path.GetFullPath(@"../config/");
            if (string.IsNullOrWhiteSpace(path))
            {
                McsfRestoreLogger.WriteLog("[GetGroupInfo]: path is null.");
                return;
            }
            if (!Directory.Exists(path))
            {
                McsfRestoreLogger.WriteLog(string.Format("[GetGroupInfo]: path {0} is not exist.", path));
                return;
            }
            var configPath = path + "ServiceRestore.xml";
            XmlDocument xDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(configPath, settings);
            xDoc.Load(reader);
            XmlElement root = (XmlElement)xDoc.SelectSingleNode("Root");
            var groupList = root["Groups"];

            foreach (XmlElement groupItem in groupList)
            {
                Dictionary<string, object> dictionaryTemp = new Dictionary<string, object>();
                string packageName = groupItem.GetAttribute("Package");
                dictionaryTemp.Add("packageName", packageName);

                string moduleName = groupItem.GetAttribute("Module");
                dictionaryTemp.Add("moduleName", moduleName);

                string flag = groupItem.GetAttribute("MobileSitesFlag");
                dictionaryTemp.Add("MobileSitesFlag", flag);

                List<string> fileList = new List<string>();
                if (groupItem["Files"] != null)
                {
                    foreach (XmlElement fileItem in groupItem["Files"])
                    {
                        fileList.Add(fileItem.InnerText);
                    }
                    dictionaryTemp.Add("Files", fileList);
                }
                else
                {
                    McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: {0}  package files  is null.", packageName));
                }

                if (groupItem["PreRestoreHandler"] != null)
                {
                    string preRestoreHandler = groupItem["PreRestoreHandler"].InnerText;
                    dictionaryTemp.Add("PreRestoreHandler", preRestoreHandler);
                }
                if (groupItem["RestoreHandler"] != null)
                {
                    string restoreHandler = groupItem["RestoreHandler"].InnerText;
                    dictionaryTemp.Add("RestoreHandler", restoreHandler);
                }
                if (groupItem["PostRestoreHandler"] != null)
                {
                    string postRestoreHandler = groupItem["PostRestoreHandler"].InnerText;
                    dictionaryTemp.Add("PostRestoreHandler", postRestoreHandler);
                }
                //if (groupItem["RollBackHandler"] != null)
                //{
                //    string rollBackHandler = groupItem["RollBackHandler"].InnerText;
                //    dictionaryTemp.Add("RollBackHandler", rollBackHandler);
                //}
                groupListResult.Add(dictionaryTemp);
            }
            Export = groupListResult;
            #endregion
        }
    }
}
