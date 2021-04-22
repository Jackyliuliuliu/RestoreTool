using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using UIH.Mcsf.Restore.Logger;

namespace MysqlRestoreTool.Common
{
    public class DllRefection
    {

        public enum CommandType
        {
            BackupPreCommand = 0,
            BackupPostCommand = 1,
            BackupFailedCommand = 2,

            RestorePreCommand = 3,
            RestorePostCommand = 4,
            RestoreFailedCommand = 5  //if restore failed, execute this method
        }

        /// <summary>
        /// Execute specified function in specified class by assembly reflection
        /// </summary>
        /// <param name="dllName">the specified dll file name</param>
        /// <param name="nameSpace">the namespace name</param>
        /// <param name="className">the class name</param>
        /// <param name="functionName">the function name</param>
        /// <param name="args">the parameters of the specified function</param>
        /// <returns>the return value is the return value of the specified function
        /// </returns>
        public object DLLReflection(string dllName, string nameSpace, string className, string functionName, object[] args, out string response)
        {
            try
            {
                if (-1 == dllName.LastIndexOf(".dll"))
                {
                    dllName += ".dll";
                }
                var binPath = Path.GetFullPath(@"../bin/");

                string dllPath = binPath + dllName;

                if (!File.Exists(dllPath))
                {
                    response = "dll path is not exist";
                    return false;
                }

                Assembly assembly = Assembly.LoadFrom(dllPath);

                string loadType = nameSpace + "." + className;
                Type type = assembly.GetType(loadType);
                object instance = Activator.CreateInstance(type);

                int iArg = args.Length;
                MethodInfo method = null;
                if (iArg == 5)//mobilesite
                {
                    method = type.GetMethod(functionName, new Type[] { typeof(Dictionary<string, string>), typeof(string), typeof(List<string>), typeof(List<string>), typeof(List<string>) });
                }
                else if (iArg == 4)//restore
                {
                    method = type.GetMethod(functionName, new Type[] { typeof(Dictionary<string, string>), typeof(string), typeof(List<string>), typeof(List<string>) });
                }
                else//backup
                {
                    method = type.GetMethod(functionName);
                }

                //if not have this method, continue execute
                if (null == method)
                {
                    response = "method is null iArg is" + iArg;
                    return false;
                }

                object value = method.Invoke(instance, args);
                response = "";
                return value;
            }
            catch (Exception ex)
            {
                response = ex.Message + "\n";
                throw ex;
            }
        }


        public object DLLReflection2(string dllName, string nameSpace, string className, string functionName, object[] args, out string response)
        {
            try
            {
                if (-1 == dllName.LastIndexOf(".dll"))
                {
                    dllName += ".dll";
                }
                var binPath = Path.GetFullPath(@"../bin/");

                string dllPath = binPath + dllName;

                if (!File.Exists(dllPath))
                {
                    response = "dll path is not exist";
                    return false;
                }

                Assembly assembly = Assembly.LoadFrom(dllPath);

                string loadType = nameSpace + "." + className;
                Type type = assembly.GetType(loadType);
                object instance = Activator.CreateInstance(type);         
                int iArg = args.Length;
                response = "iArg is " + iArg;
                MethodInfo method = null;

                if (iArg == 1)
                {
                    method = type.GetMethod(functionName, new Type[] { typeof(Action<string>) });
                }
                else if (iArg == 3)
                {
                    method = type.GetMethod(functionName, new Type[] { typeof(int), typeof(int), typeof(Action<string>) });
                }
                else//backup
                {
                    method = type.GetMethod(functionName);
                }

                //if not have this method, continue execute
                if (null == method)
                {
                    response = "method is null iArg is" + iArg;
                    return false;
                }

                object value = method.Invoke(instance, args);
                response = "";
                return value;
            }
            catch (Exception ex)
            {
                response = ex.Message + "\n";
                throw ex;
            }
        }

        /// <summary>
        /// Execute BackupRestore command
        /// </summary>
        /// <param name="dllName">dll file name</param>
        /// <param name="type">command type</param>
        /// <returns></returns>
        public bool ExecuteBackupRestoreCommand(string dllName, CommandType type, object[] args, out string response)
        {
            try
            {
                McsfRestoreLogger.WriteLog("[ExcuteBackupRestoreCommand]: dll name:" + dllName + " type:" + type.ToString());
                object result;

                if (null == args)
                {
                    result = DLLReflection(dllName, "UIH.Service.BackupRestore", "BackupRestoreCommand", type.ToString(), args, out response);
                }
                else
                {
                    result = DLLReflection(dllName, "UIH.Service.BackupRestore", "BackupRestoreCommand", type.ToString(), new object[] { args[0] }, out response);
                }

                if (result is bool)
                {
                    if (true == (bool)result)
                    {
                        response = "Run dll " + dllName + " " + type.ToString() + " successful!\n";
                        return true;
                    }
                    else
                    {
                        response = "Run dll " + dllName + " " + type.ToString() + " failed!\n";

                        McsfRestoreLogger.WriteLog("[ExcuteBackupRestoreCommand]: Dll reflection end,dll name:" + dllName + " failed!");
                        return false;
                    }
                }
                else if (result != null && result is string)
                {
                    if (result.ToString().Length > 0 && result.ToString().isSuccess())
                    {
                        //response = "Run dll " + dllName + " " + type.ToString() + " successful!\n";
                        response = result.ToString().ParseBizXml();
                        if (string.IsNullOrEmpty(response))
                        {
                            response = "Run dll " + dllName + " " + type.ToString() + " successful!\n";
                        }
                        return true;
                    }
                    else
                    {
                        //response = "Run dll " + dllName + " " + type.ToString() + " failed!\n";
                        response = result.ToString().ParseBizXml();
                        McsfRestoreLogger.WriteLog("[ExcuteBackupRestoreCommand]: Dll reflection for sw end,dll name:" + dllName + " response:failed," + response);
                        return false;
                    }
                }
                else
                {
                    response = "Run dll " + dllName + " " + type.ToString() + " failed, response is not right!\n";
                    McsfRestoreLogger.WriteLog("[ExcuteBackupRestoreCommand]: Dll reflection get wrong response,dll name:" + dllName);
                    return false;
                }
            }
            catch (Exception ex)
            {
                response = "Run dll " + dllName + " " + type.ToString() + " failed!\n" + ex.Message;
                McsfRestoreLogger.WriteLog("[ExecuteBackupRestoreCommand]: exception is " + ex.ToString());
                return false;
            }
        }

    }

    public static class XmlHelper
    {
        public static string ParseBizXml(this string value)
        {
            if (value == null || value.Length == 0) return "";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(value);
            // Add a price element.
            XmlNodeList nodeMouduleList = doc.SelectNodes("BizInfo/BizItems/Item");
            // Add a price element.
            XmlNode nodeMessage = doc.SelectSingleNode("BizInfo/RetMessage");
            if (null != nodeMouduleList)
            {
                string moduleInfo = "";
                for (int i = 0; i < nodeMouduleList.Count; i++)
                {
                    moduleInfo += nodeMouduleList[i].SelectSingleNode("SuccessCode").InnerText + "|";
                    moduleInfo += (nodeMouduleList[i].SelectSingleNode("Module").InnerText + "|");
                    moduleInfo += nodeMouduleList[i].SelectSingleNode("Msg").InnerText;
                    if (i != nodeMouduleList.Count - 1)
                    {
                        moduleInfo += ";";
                    }
                }
                return moduleInfo;

            }
            return "";
        }

        public static string toMessage(this string value)
        {
            if (value == null || value.Length == 0) return "";

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(value);
            // Add a price element.
            XmlNode node = doc.SelectSingleNode("BizInfo/RetMessage");
            if (null != node)
            {
                return node.InnerText;
            }
            return "";
        }

        public static bool isSuccess(this string value)
        {
            if (value == null || value.Length == 0) return false;

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(value);
            // Add a price element.
            XmlNode node = doc.SelectSingleNode("BizInfo/IsSuccess");
            if (null != node && node.InnerText == "True")
            {
                return true;
            }
            return false;
        }
    }
}
