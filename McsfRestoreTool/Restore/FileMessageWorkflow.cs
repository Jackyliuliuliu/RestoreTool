using MysqlRestoreTool.Common;
using MysqlRestoreTool.Restore.GetBranchInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using UIH.Mcsf.Restore.Logger;

namespace MysqlRestoreTool.Restore.FileMessageWorkflow
{
    public class GetMessageWorkflow : Subtask
    {
        public override void Execute()
        {
            string filePath = Arguments[0];

            try
            {
                Require<InitialReadStream>(filePath);
                string message = Require<GetMessageInFileHead>();

                Export = message;
            }
            catch (Exception ex)
            {
                McsfRestoreLogger.WriteLog(string.Format("[GetMessageWorkflow]: exception is {0},{1}", ex.Message, ex.ToString()));
                throw ex;
            }
            finally
            {
                Require<CloseReadStream>();
            }
        }
    }

    public class InitialReadStream : Subtask
    {
        public override void Execute()
        {
            string filePath = Arguments[0];
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var binaryReader = new BinaryReader(fileStream);
            Export = new dynamic[] { binaryReader, fileStream };
        }
    }

    public class GetMessageInFileHead : Subtask
    {
        public override void Execute()
        {
            var streamSet = Require<InitialReadStream>();
            BinaryReader binaryReader = streamSet[0];
            FileStream fileStream = streamSet[1];

            if (fileStream.Length < sizeof(Int32))
            {
                McsfRestoreLogger.WriteLog(string.Format("[GetMessageInFileHead]: file stream length is less ", sizeof(Int32)));
                return;
            }

            var messageLength = binaryReader.ReadInt32();
            if (messageLength > fileStream.Length - sizeof(int))
            {
                McsfRestoreLogger.WriteLog(string.Format("[GetMessageInFileHead]: message length is less ", fileStream.Length - sizeof(int)));
                return;
            }

            var messageBytes = binaryReader.ReadBytes(messageLength);
            var message = Encoding.UTF8.GetString(messageBytes);

            Export = message;
        }
    }

    public class CloseReadStream : Subtask
    {
        public override void Execute()
        {
            var streamSet = Require<InitialReadStream>();
            new List<dynamic>(streamSet).ForEach(item =>
            {
                if (item == null)
                {
                    McsfRestoreLogger.WriteLog("[CloseReadStream]: item is null.");
                    return;
                }
                item.Close();
            });
        }
    }

    public class GetMessageFromFile : Subtask
    {
        public override void Execute()
        {
            string filePath = Arguments[0];
            try
            {
                string message = Require<GetMessageWorkflow>(filePath);
                Export = message;
            }
            catch (Exception ex)
            {
                McsfRestoreLogger.WriteLog("[GetMessageFromFile]: ex is " + ex.StackTrace);
                return;
            }
        }
    }

    public class GetRestoreInfoMainWorkflow : Subtask
    {
        public override void Execute()
        {
            string message = Arguments[0];
            if (Require<IsMessageBranch69>(message))//69是json字符串
            {
                Export = message;
            }
            else                                //69之前是xml字符串
            {
                XmlDocument xDoc_Bkp = Require<GetBkpXmlDocument>(message);
                string version = Require<GetBranch69FormerBkpVersionInfo>(message, xDoc_Bkp);

                ((XmlElement)xDoc_Bkp.SelectSingleNode("Root")).SetAttribute("Version", version);
                Export = xDoc_Bkp.InnerXml;
            }
        }
    }

    public class DeleteMessageWorkflow : Subtask
    {
        public override void Execute()
        {
            string sourceFilePath = Arguments[0];
            string targetFilePath = Arguments[1];

            try
            {
                Require<CreateEmptyFile>(targetFilePath);
                Require<InitialStream>(sourceFilePath, targetFilePath);
                Require<ReadFileHeadByte>();
                Require<CopyContentBlockByte>();
                Require<CopyContentRemainderByte>();
            }
            catch (Exception ex)
            {
                McsfRestoreLogger.WriteLog(string.Format("[DeleteMessageWorkflow]: throw exception:{0},{1}", ex.Message, ex.ToString()));
                throw ex;
            }
            finally
            {
                Require<CloseStream>();
            }
        }
    }

    public class CloseStream : Subtask
    {
        public override void Execute()
        {
            var streamSet = Require<InitialStream>();

            new List<dynamic>(streamSet).ForEach(item =>
            {
                if (item == null)
                {
                    return;
                }
                item.Close();
            });
        }
    }

    public class CreateEmptyFile : Subtask
    {
        public override void Execute()
        {
            string filePath = Arguments[0];

            if (File.Exists(filePath))
            {
                return;
            }
            File.Create(filePath).Close();
        }
    }

    public class InitialStream : Subtask
    {
        public override void Execute()
        {
            string sourceFilePath = Arguments[0];
            string targetFilePath = Arguments[1];

            var readFileStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read);

            var binaryReader = new BinaryReader(readFileStream);
            var writeFileStream = new FileStream(targetFilePath, FileMode.Truncate, FileAccess.Write);
            var binaryWriter = new BinaryWriter(writeFileStream);
            Export = new dynamic[] { binaryReader, binaryWriter, readFileStream, writeFileStream };
        }
    }

    public class ReadFileHeadByte : Subtask
    {
        public override void Execute()
        {
            var streamSet = Require<InitialStream>();
            BinaryReader binaryReader = streamSet[0];

            var messageLength = binaryReader.ReadInt32();
            binaryReader.ReadBytes(messageLength);
        }
    }

    public class CopyContentBlockByte : Subtask
    {
        public override void Execute()
        {
            int bufferSize = Require<GetBufferSize>();
            var streamSet = Require<InitialStream>();
            BinaryReader binaryReader = streamSet[0];
            BinaryWriter binaryWriter = streamSet[1];
            FileStream readFileStream = streamSet[2];

            var readFileStreamLength = readFileStream.Length;
            var bufferCount = readFileStreamLength / bufferSize;

            for (var i = 0; i < bufferCount; i++)
            {
                var byteBuffer = new byte[bufferSize];
                binaryReader.Read(byteBuffer, 0, bufferSize);
                binaryWriter.Write(byteBuffer);

                Require<TriggerProgressRecord>(i, bufferCount);
            }
        }
    }

    public class CopyContentRemainderByte : Subtask
    {
        public override void Execute()
        {
            int bufferSize = Require<GetBufferSize>();
            var streamSet = Require<InitialStream>();
            BinaryReader binaryReader = streamSet[0];
            BinaryWriter binaryWriter = streamSet[1];
            FileStream readFileStream = streamSet[2];

            var readFileStreamLength = readFileStream.Length;

            var lastBufferSize = readFileStreamLength % bufferSize;
            var lastBuffer = new byte[lastBufferSize];
            binaryReader.Read(lastBuffer, 0, (int)lastBufferSize);
            binaryWriter.Write(lastBuffer);
        }
    }

    public class GetBufferSize : Subtask
    {
        public override void Execute()
        {
            Export = 1024;
        }
    }

    public class TriggerProgressRecord : Subtask
    {
        public override void Execute()
        {
            long index = Arguments[0];
            long width = Arguments[1];

            var lastPercent = 100 * index / width;
            var currentPercent = 100 * (index + 1) / width;

            if (lastPercent == currentPercent)
            {
                return;
            }
        }
    }
}
