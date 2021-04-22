using MysqlRestoreTool.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UIH.Mcsf.Restore.Logger;

namespace McsfRestoreTool.Restore
{
    public class DeleteFolderAndSubFiles : Subtask
    {
        public override void Execute()
        {
            McsfRestoreLogger.WriteLog("[DeleteFolderAndSubFiles]:start ");
            string deletefolder = Arguments[0];
            DeleteFolder(deletefolder);
        }
        public static void DeleteFolder(string deletefolder)
        {
            McsfRestoreLogger.WriteLog("[DeleteFolderAndSubFiles]:deletefolder is  " + deletefolder);
            if (Directory.Exists(deletefolder))
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(deletefolder);
                    foreach (FileSystemInfo i in dir.GetFileSystemInfos())
                    {
                        if (i is FileInfo)
                        {
                            if (i.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            {
                                i.Attributes = FileAttributes.Normal;//去掉文件属性
                            }
                            File.Delete(i.FullName);//直接删除其中的文件
                        }
                        else
                        {
                            DeleteFolder(i.FullName);
                        }
                    }

                    DirectoryInfo DirInfo = new DirectoryInfo(deletefolder);
                    DirInfo.Attributes = FileAttributes.Normal;//去掉文件夹属性  
                    DirInfo.Delete(true);//删除文件夹
                }
                catch (Exception ex)
                {
                    McsfRestoreLogger.WriteLog("DeleteFolderAndSubFiles ex" + ex.Message);
                }
            }
        }
    }
}
