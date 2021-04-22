using McsfRestoreTool.ViewModel;
using MysqlRestoreTool.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UIH.Mcsf.Restore.Logger;

namespace MysqlRestoreTool.Restore.CommonTask
{
    public class GetRootFolderPath : Subtask
    {
        public override void Execute()
        {
            const string RestoreRootFolderPath = RestoreToolConstant.SYSTEM_Restore_FOLDER; //D:\MysqlResotre\Restore\
            Export = RestoreRootFolderPath;
        }
    }
    public class GetTempFolderPath : Subtask
    {
        public override void Execute()
        {
            string rootFolderPath = Require<GetRootFolderPath>();
            var tempFolderPath = string.Format(@"{0}restore{1}\", rootFolderPath, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            McsfRestoreLogger.WriteLog("[GetTempFolderPath]: tempFolderPath is " + tempFolderPath);
            Export = tempFolderPath;
        }
    }
    public class GetCompressedFilePathFromFolderPath : Subtask
    {
        public override void Execute()
        {
            string folderPath = Arguments[0];//D:\UIH\BackupRestore\Restore\时间戳

            var parentFolderPath = new DirectoryInfo(folderPath).Parent.FullName;
            var folderName = new DirectoryInfo(folderPath).Name;

            var compressedFilePath = string.Format(@"{0}\{1}Compressed.bkp", parentFolderPath, folderName);
            McsfRestoreLogger.WriteLog("[GetCompressedFilePathFromFolderPath]: compressedFilePath is " + compressedFilePath);
            Export = compressedFilePath;
        }
    }
    public class CreateFolder : Subtask
    {
        public override void Execute()
        {
            try
            {
                string folderPath = Arguments[0];
                new DirectoryInfo(folderPath).Create();
            }
            catch (Exception ex)
            {
                McsfRestoreLogger.WriteLog("[CreateFolder]:exception is " + ex.Message, LogMessageType.Error);
            }
        }
    }

    public class ExcuteBat : Subtask
    {
        public override void Execute()
        {
            var batPath = Arguments[0];
            McsfRestoreLogger.WriteLog("[ExcuteBat]:batPath is " + batPath);
            bool result = true;
            Process process = null;
            try
            {
                process = new Process();
                FileInfo fileInfo = new FileInfo(batPath);
                process.StartInfo.WorkingDirectory = fileInfo.Directory.FullName;
                process.StartInfo.FileName = batPath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                StreamReader standardOutput = process.StandardOutput;
                string str = standardOutput.ReadToEnd();
                standardOutput.Close();
                process.WaitForExit();
            }
            catch
            {
                result = false;
            }
            finally
            {
                process.Close();
            }
            Export = result;
        }
    }

    //public class Decompress : Subtask
    //{
    //    public override void Execute()
    //    {
    //        try
    //        {
    //            string compressedFilePath = Arguments[0];
    //            string targetFolderPath = Arguments[1];
    //            Action<RestoreInfo> restoreAciton = Arguments[2];
    //            if (!Directory.Exists(targetFolderPath))
    //            {
    //                Directory.CreateDirectory(targetFolderPath);
    //            }
    //            var serviceCompress = new Compress();

    //            string response;
    //            bool isOkToDecompress = serviceCompress.unzipFiles(compressedFilePath, targetFolderPath, restoreAciton, out response);
    //            Export = isOkToDecompress;
    //            McsfRestoreLogger.WriteLog(string.Format("[Decompress]: isOkToDecompress:" + isOkToDecompress + " response:" + response));
    //            if (!isOkToDecompress)
    //            {
    //                throw new Exception(string.Format("Failed to decompress content, error message: {0}", response));
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            McsfRestoreLogger.WriteLog("[Decompress]: exception is " + ex.Message);
    //        }

    //    }
    //}

    public class GetBranch69FormerFiles : Subtask//需要先解压到BR路径，在获取文件列表,需要传对新的BR路径
    {
        public override void Execute()
        {

            var message = Arguments[0];
            McsfRestoreLogger.WriteLog("[GetBranch69FormerFiles]: GetBranch69FormerFiles  Begin .");
            if (null == message)
            {
                Export = null;
                return;
            }

            try
            {
                List<string> FilesList = new List<string>();
                var xDoc_Restore = new XmlDocument();
                xDoc_Restore.LoadXml(message);
                XmlNodeList packageNodeList = xDoc_Restore.SelectNodes("Root/Packages/Package");
                foreach (XmlNode packageNode in packageNodeList)
                {
                    XmlNode groupsNode = packageNode.SelectSingleNode("Groups");
                    if (null != groupsNode)
                    {
                        XmlNodeList groupNodeList = groupsNode.SelectNodes("Group");
                        if (groupNodeList != null)
                        {
                            foreach (XmlNode groupNode in groupNodeList)
                            {
                                var fileCounter = 0;
                                XmlNodeList FilesNodeList = groupNode.SelectNodes("Files");
                                if (null != FilesNodeList)
                                {
                                    foreach (XmlNode filesNode in FilesNodeList)
                                    {
                                        XmlNodeList fileNodeList = filesNode.SelectNodes("File");
                                        if (null != fileNodeList)
                                        {
                                            foreach (XmlNode file in fileNodeList)
                                            {
                                                if (null != file && null != file.Attributes["Name"])
                                                {
                                                    string module = (null == file.Attributes["Module"]) ? string.Empty : file.Attributes["Module"].Value;
                                                    string filePath = file.Attributes["Name"].Value;
                                                    var fileFolderPath = string.Format(@"{0}\{1}\{2}\{2}{3}\", RestoreToolConstant.DECOMPRESS_FOLDER, packageNode.Attributes["Name"].Value, groupNode.Attributes["Name"].Value, fileCounter++);
                                                    var backupPath = IsFolder(filePath)
                                                           ? string.Format(@"{0}{1}\", fileFolderPath, new DirectoryInfo(filePath).Name)
                                                           : string.Format("{0}{1}", fileFolderPath, new FileInfo(filePath).Name);

                                                    if (module.Equals(string.Empty))
                                                    {
                                                        FilesList.Add(backupPath);
                                                    }
                                                    else
                                                    {
                                                        FilesList.Add(module + "|" + backupPath);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Export = FilesList;
            }
            catch (Exception ex)
            {
                McsfRestoreLogger.WriteLog("[GetBranch69FormerFiles]: ex is " + ex);
                Export = new List<string>();
            }
        }

        private static bool IsFolder(string path)
        {
            return Path.GetExtension(path) == "";
        }
    }

    public class GetUsers : Subtask
    {
        public override void Execute()
        {
            string usersFolderPath = @"D: \UIH\appdata\user_settings\users";
            var directoryInfo = new DirectoryInfo(usersFolderPath);
            var fileSystemInfos = directoryInfo.GetFileSystemInfos();
            List<string> userList = new List<string>();
            foreach (var fileSystemInfo in fileSystemInfos)
            {
                if (fileSystemInfo as DirectoryInfo == null)
                {
                    continue;
                }
                userList.Add(fileSystemInfo.Name);
            }
            Export = userList;
        }
    }


}
