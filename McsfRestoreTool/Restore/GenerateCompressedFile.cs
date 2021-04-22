using MysqlRestoreTool.Common;
using MysqlRestoreTool.Restore.FileMessageWorkflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MysqlRestoreTool.Restore
{
    public class GenerateCompressedFile : Subtask
    {
        public override void Execute()
        {
            string bkpFilePath = Arguments[0];
            string compressedFilePath = Arguments[1];
            var syncTask = new SyncTaskUtility();

            syncTask.Execute<DeleteMessageWorkflow>(bkpFilePath, compressedFilePath);
        }
    }
}
