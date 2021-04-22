using McsfRestoreTool.ViewModel;
using MysqlRestoreTool.Common;
using MysqlRestoreTool.Restore.CommonTask;
using MysqlRestoreTool.Restore.FileMessageWorkflow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UIH.Mcsf.Restore.Logger;

namespace MysqlRestoreTool.Restore
{
    public class RestoreMainWorkflow : Subtask
    {
        Action<RestoreInfo> restoreAciton;
        public override void Execute()
        {
            McsfRestoreLogger.WriteLog("[RestoreMainWorkflow]: restore begin.");      
            string bkpFilePath = Arguments[0];
            List<string> packageList = Arguments[1];
            restoreAciton = Arguments[2];
        
            SendMessage("Restore start");
            var tempFolderPath = Require<GetTempFolderPath>();//D:\MysqlResotre\Restore\时间戳
            McsfRestoreLogger.WriteLog("[RestoreMainWorkflow]: tempFolderPath is "+ tempFolderPath);
            var compressedFilePath = Require<GetCompressedFilePathFromFolderPath>(tempFolderPath);
            McsfRestoreLogger.WriteLog("[RestoreMainWorkflow]: compressedFilePath is " + compressedFilePath);
            var rootFolderPath = Require<GetRootFolderPath>();
            Require<CreateFolder>(rootFolderPath);
            var message = Require<GetRestoreInfoMainWorkflow>(Require<GetMessageFromFile>(bkpFilePath));
            McsfRestoreLogger.WriteLog("[RestoreMainWorkflow]: message is " + message);
            Require<GenerateCompressedFile>(bkpFilePath, compressedFilePath);//去除bkp头部字符串
            var serviceCompress = new Compress();

            //计算bkp包大小
            SendMessage("Calculate bkp size start");
            SendMessage("Calculate bkp size....");
            var totalsize = serviceCompress.getTotalSize(compressedFilePath);
            McsfRestoreLogger.WriteLog("[RestoreMainWorkflow]: totalsize is " + totalsize);

            //解压压缩包
            SendMessage("Decompress start", true);
            long addSize = (long)(totalsize * 0.1);
            bool isOkToDecompress = serviceCompress.unzipFiles(compressedFilePath, rootFolderPath + @"Backup", restoreAciton, totalsize + addSize, addSize);
            if (!isOkToDecompress)
            {
                Export = ResultType.Fail;
                SendMessage("Decompress failed.");
            }
            var depressRult = new RestoreInfo
            {
                TotalSize = totalsize + addSize,
                Size = totalsize + addSize,
                Message = "Decompress complete."
            };
            SendMessage(depressRult);
            McsfRestoreLogger.WriteLog(string.Format("[RestoreMainWorkflow]: decompress target is {0} and unzipFiles result is {1}", rootFolderPath + @"Backup", isOkToDecompress));
            SendMessage("Decompress success", true);

            //restore 文件
            SendMessage("Restore files start.", false, true);
            var restoreResult = Require<RestoreFiles>(packageList, message, restoreAciton);
            McsfRestoreLogger.WriteLog("[RestoreMainWorkflow]: restoreFiles result is " + restoreResult);
            if (!restoreResult)
            {
                Export = ResultType.Fail;
                SendMessage("Restore files failed", false, true);
            }
            else
            {
                SendMessage("Restore files end.", false, true);
                SendMessage("Restore success.", false, true);
            }

            SendMessage("Exprot start...", false, true);
            var batPath = Path.GetFullPath(@"../ExportTool/StartExport.bat");
            var isBatSuccess = Require<ExcuteBat>(batPath);
            if (!isBatSuccess)
            {
                Export = ResultType.Fail;
                SendMessage("Exprot failed", false, true);
            }
            else
            {
                SendMessage("Exprot end", false, true);
                SendMessage("Exprot success", false, true);
            }
            Export = ResultType.Successful;
        }

        public void SendMessage(string message, bool IsDecompress = false,bool isRestoreFile = false)
        {
            var currentStep = new RestoreInfo();
            currentStep.Message = message;
            currentStep.IsDecompress = IsDecompress;
            currentStep.IsRestoreFile = isRestoreFile;
            this.restoreAciton(currentStep);
        }

        public void SendMessage(RestoreInfo restoreInfo)
        {
            this.restoreAciton(restoreInfo);
        }


    }
}
