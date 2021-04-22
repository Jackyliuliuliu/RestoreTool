using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MysqlRestoreTool.Common
{
    public class RestoreToolConstant
    {
        public const string Restore_FOLDER = @"D:\MysqlResotre\";
        public const string SYSTEM_Restore_FOLDER = @"D:\MysqlResotre\Restore\";
        public const string DECOMPRESS_FOLDER = @"D:\MysqlResotre\Restore\Backup";
    }
    public enum EnumMySQLStatus
    {
        Nothing = -1,
        Accessed = 0,
        UnAccessed = 1,
        NotExisted = 2,
        Running = 3,
        Stopped = 4
    }

    public enum ResultType
    {
        Fail = 0,
        Successful = 1
    }
}
