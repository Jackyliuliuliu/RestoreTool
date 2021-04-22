using MysqlRestoreTool.Common;
using MysqlRestoreTool.Restore.FileMessageWorkflow;
using MysqlRestoreTool.Restore.GetBranchInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using UIH.Mcsf.Restore.Logger;

namespace MysqlRestoreTool.Restore.GetRestoreInfo
{
    public class GetOldSWVersionsDictionary : Subtask//返回dictionary
    {
        public override void Execute()
        {
            var message = Arguments[0];
            McsfRestoreLogger.WriteLog("GetOldSWVersionsDictionary message=" + message);
            Dictionary<string, string> versionDic = new Dictionary<string, string>();
            if (Require<IsMessageBranch69>(message))
            {
                versionDic = Require<Branch69GetMessageInfo>(message);
                if (versionDic.ContainsKey("SystemID"))
                {
                    versionDic.Remove("SystemID");
                }
                if (versionDic.ContainsKey("BkpVersion"))
                {
                    versionDic.Remove("BkpVersion");
                }
                if (versionDic.ContainsKey("SystemMacAddress"))
                {
                    versionDic.Remove("SystemMacAddress");
                }
            }
            else
            {
                XmlDocument xDoc_Bkp = Require<GetBkpXmlDocument>(message);
                string version = Require<GetBranch69FormerBkpVersionInfo>(message, xDoc_Bkp);
                versionDic.Add("MCSF", version);
            }
            Export = versionDic;
        }
    }

    public class Branch69GetMessageInfo : Subtask
    {
        public override void Execute()
        {
            var message = Arguments[0];
            Dictionary<string, string> messageDic = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(message);
            Export = messageDic;
        }
    }
}
