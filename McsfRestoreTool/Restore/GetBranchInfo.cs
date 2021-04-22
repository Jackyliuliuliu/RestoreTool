using MysqlRestoreTool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using UIH.Mcsf.Restore.Logger;

namespace MysqlRestoreTool.Restore.GetBranchInfo
{
    public class IsMessageBranch69 : Subtask
    {
        public override void Execute()
        {
            var message = Arguments[0];

            try
            {
                Dictionary<string, string> messageDic = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(message);
                Export = messageDic.ContainsKey("SystemID");
                McsfRestoreLogger.WriteLog("[IsMessageBranch69]:" + messageDic.ContainsKey("SystemID"));
            }
            catch (Exception ex)
            {
                McsfRestoreLogger.WriteLog("[IsMessageBranch69]: ex is " + ex.Message);
                if (ex.Message.Contains("Invalid JSON primitive") || ex.Message.Contains("无效的 JSON 基元"))
                {
                    McsfRestoreLogger.WriteLog("[IsMessageBranch69]: message is not branch 69.");
                    Export = false;
                    return;
                }
            }
        }
    }

    public class GetBkpXmlDocument : Subtask
    {
        public override void Execute()
        {
            var message = Arguments[0];
            XmlDocument xDoc_Bkp = Require<Branch69FormerGetBackupXmlDocument>(message);
            Export = xDoc_Bkp;
        }
    }

    public class Branch69FormerGetBackupXmlDocument : Subtask
    {
        public override void Execute()
        {
            var message = Arguments[0];
            var xDoc_Bkp = new XmlDocument();
            xDoc_Bkp.LoadXml(message);

            Export = xDoc_Bkp;
        }
    }

    public class GetBranch69FormerBkpVersionInfo : Subtask
    {
        public override void Execute()
        {
            string message = Arguments[0];
            XmlDocument xDoc_Bkp = Arguments[1];

            var received = false;
            string version = Require<IsBranch59Later, Branch59LaterGetVersionInfo>(ref received, message, xDoc_Bkp);
            version = Require<IsBranch59Former, Branch59FormerGetVersionInfo>(ref received, message, xDoc_Bkp) ?? version;
            Export = version;
        }
    }

    public class IsBranch59Later : Subtask
    {
        public override void Execute()
        {
            string message = Arguments[0];
            XmlDocument xDoc_Bkp = Arguments[1];

            var Is59Branch = Require<IsBranch59>(message);
            if (!Is59Branch)
            {
                Export = false;
                return;
            }

            var rootHasVersion = ((XmlElement)xDoc_Bkp.SelectSingleNode("Root")).HasAttribute("Version");
            if (!rootHasVersion)
            {
                Export = false;
                return;
            }

            Export = true;
        }
    }

    public class Branch59LaterGetVersionInfo : Subtask
    {
        public override void Execute()
        {
            string message = Arguments[0];
            XmlDocument xDoc_Bkp = Arguments[1];

            var version = xDoc_Bkp.SelectSingleNode("Root").Attributes["Version"].Value;
            Export = version;
        }
    }

    public class IsBranch59Former : Subtask
    {
        public override void Execute()
        {
            Export = Require<IsBranch59>();
        }
    }

    public class IsBranch59 : Subtask
    {
        public override void Execute()
        {
            string message = Arguments[0];
            if (null == message)
            {
                Export = false;
                return;
            }
            try
            {
                var xDoc_Restore = new XmlDocument();
                xDoc_Restore.LoadXml(message);
                var rootElement = (XmlElement)xDoc_Restore.SelectSingleNode("Root");
                Export = rootElement.HasAttribute("SN") || rootElement.HasAttribute("SystemID");
            }
            catch (Exception ex)
            {
                Export = false;
                return;
            }
        }
    }

    public class Branch59FormerGetVersionInfo : Subtask
    {
        public override void Execute()
        {
            Export = "ZHENGHE_59_186383.1.0.0.59.X.XXXXXXXX.XXXXXX";
        }
    }
}
