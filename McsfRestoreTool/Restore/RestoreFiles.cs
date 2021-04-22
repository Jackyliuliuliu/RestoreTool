using MysqlRestoreTool.Common;
using MysqlRestoreTool.Restore.CommonTask;
using MysqlRestoreTool.Restore.GetBranchInfo;
using MysqlRestoreTool.Restore.GetRestoreInfo;
using System;
using System.Collections.Generic;
using UIH.Mcsf.Restore.Logger;
using System.IO;
using McsfRestoreTool.Restore;
using McsfRestoreTool.ViewModel;

namespace MysqlRestoreTool.Restore
{
    public class RestoreFiles : Subtask
    {
        Action<RestoreInfo> restoreAciton;
        public override void Execute()
        {
            McsfRestoreLogger.WriteLog("[RestoreFiles]: RestoreFiles begin.");
            List<string> packageList = Arguments[0];
            string message = Arguments[1];
            restoreAciton = Arguments[2];
            //List<string> Users = Require<GetUsers>();
            List<string> Users = null;
            Dictionary<string, string> oldSwVersions = Require<GetOldSWVersionsDictionary>(message);
            bool isBkpBranch69 = Require<IsMessageBranch69>(message) ? true : false;
            DllRefection reflection = new DllRefection();
            List<string> Files = new List<string>();
            object result;
            string response = string.Empty;
            //get gourp info
            List<Dictionary<string, object>> groupListResult = Require<GetGroupInfo>();

            McsfRestoreLogger.WriteLog("[RestoreFiles]: groupListResult count is " + groupListResult.Count);
            try
            {

                for (var i = 0; i < groupListResult.Count; i++)
                {
                    var group = groupListResult[i];
                    string packageName = group.ContainsKey("packageName") ? (string)group["packageName"] : string.Empty;

                    if (string.IsNullOrWhiteSpace(packageName) || !packageList.Contains(packageName))
                    {
                        McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: packageName is {0}, packageName is null or packageList do not contains.", packageName));
                        continue;
                    }
                    //车载配置合并逻辑和rollback功能省略                  
                    string moduleName = (group.ContainsKey("moduleName")) ? (string)group["moduleName"] : string.Empty;
                    McsfRestoreLogger.WriteLog("[RestoreFiles]: Begin restore :" + moduleName);
                    if (isBkpBranch69)//如果bkp是69及之后的，Files从当前环境的servicerestore.xml中读取
                    {
                        Files = (!group.ContainsKey("Files")) ? new List<string>() : (List<string>)group["Files"];
                    }

                    //反射依次调用PreRestoreHandler
                    if (group.ContainsKey("PreRestoreHandler"))
                    {
                        var PreRestoreHandler = (string)group["PreRestoreHandler"];
                        SendMessage(string.Format("{0} preRestoreHandler begin", PreRestoreHandler));
                        McsfRestoreLogger.WriteLog("[RestoreFiles]: Begin PreRestoreHandler: " + PreRestoreHandler);
                        List<string> PreRestoreHandlerFiles = new List<string>();
                        if (!isBkpBranch69) //如果bkp是69之前的，Files从bkp的头读取.
                        {
                            ///固定路径
                            PreRestoreHandlerFiles = Require<GetBranch69FormerFiles>(message);
                        }
                        else
                        {
                            PreRestoreHandlerFiles = (!group.ContainsKey("Files")) ? new List<string>() : (List<string>)group["Files"];
                        }
                        try
                        {
                            SendMessage(string.Format("{0} preRestoreHandler ...", PreRestoreHandler));
                            result = reflection.DLLReflection(PreRestoreHandler, "UIH.Mcsf.Restore", "McsfRestore", "PreRestoreHandler", new object[] { oldSwVersions, RestoreToolConstant.SYSTEM_Restore_FOLDER, Users, PreRestoreHandlerFiles }, out response);
                            SendMessage(string.Format("{0} preRestoreHandler complete", PreRestoreHandler));

                        }
                        catch (Exception ex)
                        {
                            McsfRestoreLogger.WriteLog("[RestoreFiles]:exception is " + ex);
                            result = 1;
                        }
                        if (((result is int) && 0 != (int)result && 2 != (int)result) || ((result is bool) && !(bool)result))
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]:PreRestoreHandler {0},{1} failed, response is {2}:", moduleName, PreRestoreHandler, response));
                            //省去回滚操作
                            Export = false;
                            SendMessage(string.Format("{0} preRestoreHandler failed", PreRestoreHandler));
                        }
                        else if ((result is int) && 2 == (int)result)
                        {
                            //displayWarningInfoFlag = false;
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: PreRestoreHandler {0},{1} successfully, Negligible error occurred.", moduleName, PreRestoreHandler));
                            SendMessage(string.Format("{0} preRestoreHandler success", PreRestoreHandler));
                        }
                        else if ((result is int) && 0 == (int)result)
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: PreRestoreHandler {0},{1} successfully.", moduleName, PreRestoreHandler));
                            SendMessage(string.Format("{0} preRestoreHandler success", PreRestoreHandler));
                        }
                        else
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: PreRestoreHandler {0} {1} , result is {2} and response is {3}", moduleName, PreRestoreHandler, result, response));
                            Export = false;
                            SendMessage(string.Format("{0} preRestoreHandler failed", PreRestoreHandler));
                        }
                        
                    }


                    List<string> handlerFiles = (!group.ContainsKey("Files")) ? new List<string>() : (List<string>)group["Files"];

                    //反射依次调用RestoreHandler
                    if (group.ContainsKey("RestoreHandler"))
                    {
                        var restoreHandler = (string)group["RestoreHandler"];
                        McsfRestoreLogger.WriteLog("[RestoreFiles]: Begin RestoreHandler: " + restoreHandler);
                        SendMessage(string.Format("{0} restoreHandler begin", restoreHandler));
                        try
                        {
                            SendMessage(string.Format("{0} restoreHandler ...", restoreHandler));
                            result = reflection.DLLReflection(restoreHandler, "UIH.Mcsf.Restore", "McsfRestore", "RestoreHandler", new object[] { oldSwVersions, RestoreToolConstant.SYSTEM_Restore_FOLDER, Users, handlerFiles }, out response);
                            SendMessage(string.Format("{0} restoreHandler complete", restoreHandler));
                        }
                        catch (Exception ex)
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: RestoreHandler {0} failed. exception is {1}", restoreHandler, ex));
                            result = 1;
                        }

                        if (((result is int) && 0 != (int)result && 2 != (int)result) || ((result is bool) && !(bool)result))
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: RestoreHandler {0},{1} failed, response is {2}:", moduleName, restoreHandler, response));
                            Export = false;
                            SendMessage(string.Format("{0} restoreHandler failed", restoreHandler));
                        }
                        else if ((result is int) && 2 == (int)result)
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: RestoreHandler {0},{1} successfully,Negligible error occurred.", moduleName, restoreHandler));
                            SendMessage(string.Format("{0} restoreHandler failed", restoreHandler));
                        }
                        else if ((result is int) && 0 == (int)result)
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: RestoreHandler {0} successfully.", moduleName));
                            SendMessage(string.Format("{0} restoreHandler success", restoreHandler));
                        }
                        else
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: PreRestoreHandler {0} {1} , result is {2} and response is {3}", moduleName, restoreHandler, result, response));
                            Export = false;
                            SendMessage(string.Format("{0} restoreHandler failed", restoreHandler));
                        }
                    }


                    //反射依次调用PostRestoreHandler
                    if (group.ContainsKey("PostRestoreHandler"))
                    {
                        var postRestoreHandler = (string)group["PostRestoreHandler"];
                        McsfRestoreLogger.WriteLog("[RestoreFiles]: Begin PostRestoreHandler: " + postRestoreHandler);
                        SendMessage(string.Format("{0} postRestoreHandler begin", postRestoreHandler));
                        try
                        {
                            SendMessage(string.Format("{0} postRestoreHandler ...", postRestoreHandler));
                            result = reflection.DLLReflection(postRestoreHandler, "UIH.Mcsf.Restore", "McsfRestore", "PostRestoreHandler", new object[] { oldSwVersions, RestoreToolConstant.SYSTEM_Restore_FOLDER, Users, handlerFiles }, out response);
                            SendMessage(string.Format("{0} postRestoreHandler complete", postRestoreHandler));
                        }
                        catch (Exception ex)
                        {
                            McsfRestoreLogger.WriteLog("[RestoreFiles]: postRestoreHandler exception is " + ex, LogMessageType.Error);
                            result = 1;
                        }

                        if (((result is int) && 0 != (int)result && 2 != (int)result) || ((result is bool) && !(bool)result))
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: PostRestoreHandler {0},{1} failed, response is {2}:", moduleName, postRestoreHandler, response));
                            Export = false;
                            SendMessage(string.Format("{0} postRestoreHandler failed", postRestoreHandler));
                        }
                        else if ((result is int) && 2 == (int)result)
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: PostRestoreHandler {0},{1} successfully, Negligible error occurred.", moduleName, postRestoreHandler));
                            SendMessage(string.Format("{0} postRestoreHandler success", postRestoreHandler));
                        }
                        else if ((result is int) && 0 == (int)result)
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: PostRestoreHandler {0},{1} successfully.", moduleName, postRestoreHandler));
                            SendMessage(string.Format("{0} postRestoreHandler success", postRestoreHandler));
                        }
                        else
                        {
                            McsfRestoreLogger.WriteLog(string.Format("[RestoreFiles]: PostRestoreHandler {0} {1} , result is {2} and response is {3}", moduleName, postRestoreHandler, result, response));
                            Export = false;
                            SendMessage(string.Format("{0} postRestoreHandler failed", postRestoreHandler));
                        }
                    }


                }
                if (Directory.Exists(RestoreToolConstant.SYSTEM_Restore_FOLDER))
                {
                    SendMessage("Delete files begin");
                    Require<DeleteFolderAndSubFiles>(RestoreToolConstant.Restore_FOLDER);
                    McsfRestoreLogger.WriteLog("[RestoreFiles]: DeleteFolderAndSubFiles end.");
                    SendMessage("Delete files end");
                }
                McsfRestoreLogger.WriteLog("[RestoreFiles]: RestoreFiles end.");
            }
            catch (Exception ex)
            {
                McsfRestoreLogger.WriteLog("[RestoreFiles]:ex is " + ex.Message);
            }

            Export = true;

        }

        public void SendMessage(string message, bool isRestoreFile = true)
        {
            var currentStep = new RestoreInfo();
            currentStep.Message = message;
            currentStep.IsRestoreFile = isRestoreFile;
            this.restoreAciton(currentStep);
        }

        public void SendMessage(RestoreInfo restoreInfo)
        {
            this.restoreAciton(restoreInfo);
        }
    }


}
