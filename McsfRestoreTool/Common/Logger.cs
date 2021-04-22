//using log4net;
//using log4net.Appender;
//using log4net.Config;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;

//namespace MysqlRestoreTool.Common
//{
//    public class Logger
//    {

//        private static ILog logger;
//        static Logger()
//        {
//            string currentTimeHour = DateTime.Now.Hour.ToString();
//            if (1 == currentTimeHour.Length)
//            {
//                currentTimeHour = "0" + currentTimeHour;
//            }

//            string currentTimeMinute = DateTime.Now.Minute.ToString();
//            if (1 == currentTimeMinute.Length)
//            {
//                currentTimeMinute = "0" + currentTimeMinute;
//            }

//            string currentTimeSecond = DateTime.Now.Second.ToString();
//            if (1 == currentTimeSecond.Length)
//            {
//                currentTimeSecond = "0" + currentTimeSecond;
//            }

//            string currentTime = currentTimeHour + currentTimeMinute + currentTimeSecond;

           
//            var path = Path.GetFullPath(@"../../../log");
            
 
//            path = Path.Combine(path, "log_");
//            if (File.Exists(path))
//            {
//                log4net.Config.XmlConfigurator.Configure(new FileInfo(path));
//            }
//            else
//            {
//                //string logPath = Utility.LogPath;
//                //var tmplogPath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"];
//                //if (!string.IsNullOrWhiteSpace(tmplogPath))
//                //{
//                //    logPath = tmplogPath;
//                //}
//                RollingFileAppender appender = new RollingFileAppender();
//                appender.Name = "RollingLogFileAppender_DateFormat";
//                appender.File = path;
//                appender.AppendToFile = true;
//                appender.RollingStyle = RollingFileAppender.RollingMode.Composite;
//                appender.DatePattern = "yyyyMMdd_" + currentTime + "'.txt'";
//                appender.PreserveLogFileNameExtension = true;
//                appender.MaximumFileSize = "100MB";
//                appender.MaxSizeRollBackups = -1;
//                appender.LockingModel = new FileAppender.MinimalLock();
//                appender.Encoding = Encoding.UTF8;
//                appender.StaticLogFileName = false;
//                log4net.Layout.PatternLayout patternLayout = new log4net.Layout.PatternLayout("%newline[%d{yyyy-MM-dd HH:mm:ss.fff}][%level][%logger]%message");
//                patternLayout.ActivateOptions();
//                appender.Layout = patternLayout;
//                appender.ActivateOptions();
//                BasicConfigurator.Configure(appender);
//            }
//        }
//        public static void WriteLog(string content, LogMessageType messageType = LogMessageType.Info)
//        {
//            string className = string.Empty;
//            string methodName = string.Empty;

//            StackTrace trace = new StackTrace();
//            StackFrame stackFrame = trace.GetFrame(1);
//            MethodBase method = stackFrame.GetMethod();
//            if (method != null)
//            {
//                if (method.ReflectedType != null) className = method.ReflectedType.FullName;
//                methodName = method.Name;
//            }

//            string msgToLog = string.Format("{0}.{1}", className, methodName);
//            doLog(content, messageType, null, msgToLog);
//        }

//        //public static void RecordUserInfo(string content)
//        //{
//        //    //需要单独用文件记录历史信息

//        //    try
//        //    {
//        //        if (!Directory.Exists(Utility.RecordDir))
//        //        {
//        //            Directory.CreateDirectory(Utility.RecordDir);
//        //        }
//        //        string path = Path.Combine(Utility.RecordDir, "audit.txt");
//        //        FileStream fss = new FileStream(path, FileMode.Append);
//        //        StreamWriter sw = new StreamWriter(fss, Encoding.Default);
//        //        sw.WriteLine(content + "----" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
//        //        sw.WriteLine("\n");
//        //        sw.Close();
//        //        fss.Close();
//        //    }
//        //    catch
//        //    {
//        //    }
//        //}

//        private static void doLog(string message, LogMessageType messageType, Exception ex, string loggerName)
//        {
//            logger = LogManager.GetLogger(loggerName);
//            switch (messageType)
//            {
//                case LogMessageType.Info:
//                    if (logger.IsInfoEnabled)
//                        logger.Info(message, ex);
//                    break;
//                case LogMessageType.Warn:
//                    if (logger.IsWarnEnabled)
//                        logger.Warn(message, ex);
//                    break;
//                case LogMessageType.Error:
//                    if (logger.IsErrorEnabled)
//                        logger.Error(message, ex);
//                    break;
//            }
//        }


//    }

//    public enum LogMessageType
//    {
//        /// <summary>  
//        /// 信息  
//        /// </summary>  
//        Info,
//        /// <summary>  
//        /// 警告  
//        /// </summary>  
//        Warn,
//        /// <summary>  
//        /// 错误  
//        /// </summary>  
//        Error
//    }
//}
