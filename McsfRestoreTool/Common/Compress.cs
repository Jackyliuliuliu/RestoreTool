using ICSharpCode.SharpZipLib.Zip;
using McsfRestoreTool.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UIH.Mcsf.Restore.Logger;

namespace MysqlRestoreTool.Common
{
    public class Compress
    {


        #region Member variables
        private const int bufferSize = 4096;
        private long alreadyCompressFileSize = 0;
        private long beforeCompressFileSize = 0;
        private long beforeDecompressFileSize = 0;
        private long alreadyDecompressFileSize = 0;

        private static string tempFileForFolderTime = "hsw_sf_1!2@34$5%6^7&.a1b2c3_tempDireTime.txt";
        private static string tempFileForFileTime = "hsw_sf_1!2@34$5%6^7&.a1b2c3_tempfileTime.txt";
        private static string tempFileFlag = "!!@%&_1!2@34$5%6^7&.a1b2c3";

        private static string flag_China = "C"; //can not fix the flag_China's length,20140731 binbin.xiang
        private static string flag_America = "A";//can not fix the flag_America's length
        private static string flag_Europe = "E";//can not fix the flag_Europe's length
        #endregion

        #region Properties
        /// <summary>
        /// Already compress total file size this time
        /// </summary>
        public long AlreadyCompressFileSize
        {
            get { return alreadyCompressFileSize; }
            set { alreadyCompressFileSize = value; }
        }

        /// <summary>
        /// Already compress total file size last time
        /// </summary>
        public long BeforeCompressFileSize
        {
            get { return beforeCompressFileSize; }
            set { beforeCompressFileSize = value; }
        }

        /// <summary>
        /// Already decompress total file size this time
        /// </summary>
        public long AlreadyDecompressFileSize
        {
            get { return alreadyDecompressFileSize; }
            set { alreadyDecompressFileSize = value; }
        }

        /// <summary>
        /// Already decompress total file size last time
        /// </summary>
        public long BeforeDecompressFileSize
        {
            get { return beforeDecompressFileSize; }
            set { beforeDecompressFileSize = value; }
        }
        #endregion

        /// <summary>
        /// Compress file.
        /// </summary>
        /// <param name="FileToZip"></param>
        /// <param name="ZipedFile"></param>
        public bool ZipFile(string FileToZip, string ZipedFile)
        {
            //If file is not exist.
            if (!System.IO.File.Exists(FileToZip))
            {
                McsfRestoreLogger.WriteLog("[ZipFile]: file is not exist: " + FileToZip);
                return false;
            }

            try
            {

                System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);
                ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
                ZipEntry ZipEntry = new ZipEntry(Path.GetFileName(FileToZip));
                ZipEntry.IsUnicodeText = true;
                ZipStream.PutNextEntry(ZipEntry);
                ZipStream.SetLevel(6);
                byte[] buffer = new byte[bufferSize];
                System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);

                ZipStream.Write(buffer, 0, size);
                while (size < StreamToZip.Length)
                {
                    int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                    ZipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }

                ZipStream.Finish();
                ZipStream.Close();
                StreamToZip.Close();
                StreamToZip.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                McsfRestoreLogger.WriteLog("[ZipFile]: exception: " + ex.Message);
                return false;
            }
        }



        /// <summary>
        /// Decompress zip file.
        /// </summary>   
        /// <param name="strZipFile">Zip file path</param>   
        /// <param name="strDir">The path of the directory after decompress</param>   
        /// <returns>true:Decompress successfully.
        ///          failed:Decompress failed. 
        public bool unzipFiles(string strZipFile, string strDir, Action<RestoreInfo> restoreAciton,long totalSize,long addSize)
        {
            McsfRestoreLogger.WriteLog("[unzipFiles]: strZipFile is " + strZipFile);
            string rootPath = "";
            ZipInputStream zipStream = null;
            try
            {
                if (File.Exists(strZipFile))
                {
                    bool bUnzipDir = false;
                    if (!Directory.Exists(strDir))
                        bUnzipDir = (Directory.CreateDirectory(strDir) != null);
                    else
                        bUnzipDir = true;
                    if (bUnzipDir)
                    {

                        zipStream = new ZipInputStream(File.OpenRead(strZipFile));
                       
                        if (zipStream != null)
                        {
                            RestoreInfo restoreInfo = new RestoreInfo();
                            restoreInfo.IsDecompress = true;
                            restoreInfo.Size = 0;
                            restoreInfo.TotalSize = totalSize;
                            restoreInfo.Message = "Decompressing .....";
                            ZipEntry zipEntry = null;
                            while ((zipEntry = zipStream.GetNextEntry()) != null)
                            {
                                if (zipEntry.ToString().Contains("\\"))
                                {
                                    rootPath = strDir + @"\" + zipEntry.ToString().Substring(0, zipEntry.ToString().IndexOf("\\"));
                                }
                                else
                                {
                                    rootPath = strDir + @"\";
                                }

                                zipEntry.IsUnicodeText = true;
                                //McsfRestoreLogger.WriteLog("[unzipFiles]: zipEntry.Name is " + zipEntry.Name);
                                string strUnzipFile = strDir + "//" + zipEntry.Name;
                                string strFileName = Path.GetFileName(strUnzipFile);
                                string strDirName = Path.GetDirectoryName(strUnzipFile);

                                if (!string.IsNullOrEmpty(strDirName))
                                    Directory.CreateDirectory(strDirName);

                                if (!string.IsNullOrEmpty(strFileName))
                                {
                                    using (FileStream unzipFileStream = new FileStream(strUnzipFile, FileMode.Create))
                                    {
                                        //McsfRestoreLogger.WriteLog(string.Format("[unzipFiles]: zip file name:{0},zip stream file name :{1}", strZipFile, strUnzipFile));
                                        if (unzipFileStream != null)
                                        {
                                            byte[] buf = new byte[bufferSize];
                                            int size = 0;
                                            long remainSize = zipStream.Length;

                                            while (true)
                                            {
                                                if (remainSize > bufferSize)
                                                {
                                                    size = zipStream.Read(buf, 0, bufferSize);
                                                    unzipFileStream.Write(buf, 0, size - 0);
                                                    remainSize = remainSize - bufferSize;
                                                }
                                                else
                                                {
                                                    size = zipStream.Read(buf, 0, Convert.ToInt32(remainSize));
                                                    unzipFileStream.Write(buf, 0, size - 0);
                                                    break;
                                                }
                                                restoreInfo.Size = size;
                                                restoreAciton(restoreInfo);   
                                            }
                                            unzipFileStream.Flush();
                                            unzipFileStream.Close();
                                        }
                                    }
                                }
                            }
                            zipStream.Close();
                            McsfRestoreLogger.WriteLog("[unzipFiles]: Decompress zip file successfully." );
                            return true;
                        }
                        else
                        {
                            McsfRestoreLogger.WriteLog("[unzipFiles]:  Zip folder is not exist." );
                            return false;
                        }
                    }
                    else
                    {
                        McsfRestoreLogger.WriteLog("[unzipFiles]: Decompress zip file is not exist.");
                        return false;
                    }
                }
                else
                {
                    McsfRestoreLogger.WriteLog("[unzipFiles]: Zip file is not exist.");
                    return false;
                }
            }
            catch (Exception Ex)
            {
                zipStream.Close();
                McsfRestoreLogger.WriteLog("[unzipFiles] exception: " + Ex.Message);
                return false;
            }
            finally
            {
                var piece = (long)(addSize * 0.1);
                RestoreInfo restoreInfo = new RestoreInfo();
                restoreInfo.IsDecompress = true;
                restoreInfo.TotalSize = totalSize;
                restoreInfo.Message = "Decompressing .....";
                restoreAciton(restoreInfo);
                //0.1
                DeleteSpecialFile(strDir);
                restoreInfo.Size = piece;
                restoreAciton(restoreInfo);
                //0.1
                if (File.Exists(rootPath + @"\\" + tempFileForFolderTime))
                {
                    RestoreFolderTime(rootPath, rootPath, rootPath + @"\\" + tempFileForFolderTime);
                    File.Delete(rootPath + @"\\" + tempFileForFolderTime);
                }
                restoreInfo.Size = piece;
                restoreAciton(restoreInfo);
                //0.1
                if (File.Exists(rootPath + @"\\" + tempFileForFileTime))
                {
                    DirectoryInfo dire = new DirectoryInfo(rootPath);
                    FileInfo[] files = dire.GetFiles("*", SearchOption.AllDirectories);
                    foreach (FileInfo file in files)
                    {
                        RestoreFileTime(rootPath, file.FullName, rootPath + @"\\" + tempFileForFileTime);
                    }

                    File.Delete(rootPath + @"\\" + tempFileForFileTime);
                }
                restoreInfo.Size = piece;
                restoreAciton(restoreInfo);

                //0.1
                if (File.Exists(strDir + @"\\" + tempFileForFolderTime))
                {
                    RestoreFolderTime(strDir, strDir, strDir + @"\\" + tempFileForFolderTime);
                    File.Delete(strDir + @"\\" + tempFileForFolderTime);
                }
                restoreInfo.Size = piece;
                restoreAciton(restoreInfo);
                //0.6
                if (File.Exists(strDir + @"\\" + tempFileForFileTime))
                {
                    var remain = 6 * piece;
                    long slice = 0;
                    DirectoryInfo dire = new DirectoryInfo(strDir);
                    FileInfo[] files = dire.GetFiles("*", SearchOption.AllDirectories);
                    if (remain> remain / files.Length && files.Length > 0)
                    {
                        slice = remain / files.Length;
                    }
                    
                    foreach (FileInfo file in files)
                    {
                        RestoreFileTime(strDir, file.FullName, strDir + @"\\" + tempFileForFileTime);
                        restoreInfo.Size = slice;
                        restoreAciton(restoreInfo);
                    }
                    File.Delete(strDir + @"\\" + tempFileForFileTime);

                }
            }
        }

        public long getTotalSize(string strZipFile)
        {
            McsfRestoreLogger.WriteLog("[getTotalSize]: strZipFile is " + strZipFile);
            string rootPath = "";
            ZipInputStream zipStream = null;
            long totalSize = 0;
            try
            {
                if (File.Exists(strZipFile))
                {
                    var bkpFileInof = new FileInfo(strZipFile);

                    zipStream = new ZipInputStream(File.OpenRead(strZipFile));

                    if (zipStream != null)
                    {
                        ZipEntry zipEntry = null;
                        while ((zipEntry = zipStream.GetNextEntry()) != null)
                        {
                            zipEntry.IsUnicodeText = true;
                            //McsfRestoreLogger.WriteLog("[getTotalSize]: zipEntry Name is " + zipEntry.Name);
                            byte[] buf = new byte[bufferSize];
                            int size = 0;
                            long remainSize = zipStream.Length;
                            while (true)
                            {
                                if (remainSize > bufferSize)
                                {
                                    size = zipStream.Read(buf, 0, bufferSize);
                                    remainSize = remainSize - bufferSize;
                                }
                                else
                                {
                                    size = zipStream.Read(buf, 0, Convert.ToInt32(remainSize));

                                    break;
                                }
                                totalSize += size;

                            }
                        }
                        zipStream.Close();
                        McsfRestoreLogger.WriteLog("[unzipFiles]: Decompress zip file successfully.total size is " + totalSize);
                        return totalSize;
                    }
                    else
                    {
                        McsfRestoreLogger.WriteLog("[getTotalSize]: zipStream is null.");
                        return totalSize;
                    }
                }
                else
                {
                    McsfRestoreLogger.WriteLog("[getTotalSize]: Zip file is not exist.");
                    return totalSize;
                }
            }
            catch (Exception Ex)
            {
                zipStream.Close();
                McsfRestoreLogger.WriteLog("[getTotalSize] exception: " + Ex.Message);
                return totalSize;
            }
            finally
            {

                if (File.Exists(rootPath + @"\\" + tempFileForFolderTime))
                {
                    RestoreFolderTime(rootPath, rootPath, rootPath + @"\\" + tempFileForFolderTime);
                    File.Delete(rootPath + @"\\" + tempFileForFolderTime);
                }
                if (File.Exists(rootPath + @"\\" + tempFileForFileTime))
                {
                    DirectoryInfo dire = new DirectoryInfo(rootPath);
                    FileInfo[] files = dire.GetFiles("*", SearchOption.AllDirectories);
                    foreach (FileInfo file in files)
                    {
                        RestoreFileTime(rootPath, file.FullName, rootPath + @"\\" + tempFileForFileTime);
                    }

                    File.Delete(rootPath + @"\\" + tempFileForFileTime);
                }

            }
        }



        /// <summary>
        /// Decompress zip file.
        /// </summary>   
        /// <param name="strZipFile">Zip file path</param>   
        /// <param name="strDir">The path of the directory after decompress</param>   
        /// <returns>true:Decompress successfully.
        ///          failed:Decompress failed. 
        public bool unzipFiles(string strZipFile, string strDir, string version, out string excep)
        {
            string rootPath = string.Empty;
            try
            {
                if (File.Exists(strZipFile))
                {
                    bool bUnzipDir = false;
                    if (!Directory.Exists(strDir))
                        bUnzipDir = (Directory.CreateDirectory(strDir) != null);
                    else
                        bUnzipDir = true;
                    if (bUnzipDir)
                    {
                        ZipInputStream zipStream = new ZipInputStream(File.OpenRead(strZipFile));
                        if (zipStream != null)
                        {
                            ZipEntry zipEntry = null;
                            while ((zipEntry = zipStream.GetNextEntry()) != null)
                            {
                                if (zipEntry.ToString().Contains("\\"))
                                {
                                    rootPath = strDir + @"\" + zipEntry.ToString().Substring(0, zipEntry.ToString().IndexOf("\\"));
                                }
                                else
                                {
                                    rootPath = strDir + @"\";
                                }

                                zipEntry.IsUnicodeText = true;
                                string strUnzipFile = strDir + "//" + zipEntry.Name;
                                string strFileName = Path.GetFileName(strUnzipFile);
                                string strDirName = Path.GetDirectoryName(strUnzipFile);

                                if (!string.IsNullOrEmpty(strDirName))
                                    Directory.CreateDirectory(strDirName);

                                if (!string.IsNullOrEmpty(strFileName))
                                {
                                    try
                                    {
                                        using (var unzipFileStream = new FileStream(strUnzipFile, FileMode.Create))
                                        {
                                            if (unzipFileStream != null)
                                            {
                                                byte[] buf = new byte[bufferSize];
                                                int size = 0;
                                                long remainSize = zipStream.Length;
                                                while (true)
                                                {
                                                    if (remainSize > bufferSize)
                                                    {
                                                        size = zipStream.Read(buf, 0, bufferSize);
                                                        unzipFileStream.Write(buf, 0, size);
                                                        remainSize = remainSize - bufferSize;
                                                    }
                                                    else
                                                    {
                                                        size = zipStream.Read(buf, 0, Convert.ToInt32(remainSize));
                                                        unzipFileStream.Write(buf, 0, size);
                                                        break;
                                                    }

                                                    alreadyDecompressFileSize = beforeDecompressFileSize + zipStream.Length - remainSize;
                                                }

                                                unzipFileStream.Flush();
                                                unzipFileStream.Close();
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (IsSkipCompressFile(version))
                                        {
                                            McsfRestoreLogger.WriteLog(string.Format("[unzipFiles]: Decompress exception in Folder:{0} , bkp version :{1}, exception:{2}",
                                                Path.GetDirectoryName(strUnzipFile), version, ex.Message));
                                            continue;
                                        }
                                        throw ex;
                                    }
                                }
                                beforeDecompressFileSize = alreadyDecompressFileSize;
                            }

                            zipStream.Close();
                            excep = "Decompress zip file successfully.";

                            return true;
                        }
                        else
                        {
                            excep = "Zip folder is not exist.";
                            return false;
                        }
                    }
                    else
                    {
                        excep = "Decompress zip file is not exist.";
                        return false;
                    }
                }
                else
                {
                    excep = "Zip file is not exist.";
                    return false;
                }
            }
            catch (Exception Ex)
            {
                excep = Ex.Message;
                McsfRestoreLogger.WriteLog(string.Format("[unzipFiles]: zip file catch exception :{0}", Ex.Message));
                return false;
            }
            finally
            {
                DeleteSpecialFile(strDir);
                if (File.Exists(rootPath + @"\\" + tempFileForFolderTime))
                {
                    RestoreFolderTime(rootPath, rootPath, rootPath + @"\\" + tempFileForFolderTime);
                    File.Delete(rootPath + @"\\" + tempFileForFolderTime);
                }
                if (File.Exists(rootPath + @"\\" + tempFileForFileTime))
                {

                    DirectoryInfo dire = new DirectoryInfo(rootPath);
                    FileInfo[] files = dire.GetFiles("*", SearchOption.AllDirectories);
                    foreach (FileInfo file in files)
                    {
                        RestoreFileTime(rootPath, file.FullName, rootPath + @"\\" + tempFileForFileTime);
                    }

                    File.Delete(rootPath + @"\\" + tempFileForFileTime);
                }
            }
        }


        private bool IsSkipCompressFile(string version)
        {
            if (-1 != version.IndexOf("XXXXXX"))
            {
                return true;
            }
            string[] sVersionSplits = version.Split('.');
            if (sVersionSplits.Length <= 6)
            {
                return true;
            }
            if (Convert.ToInt32(sVersionSplits[3]) < 61)
            {
                return true;
            }
            else if (Convert.ToInt32(sVersionSplits[3]) == 61 &&
                Convert.ToInt32(sVersionSplits[4]) < 4 && Convert.ToInt32(sVersionSplits[5]) < 20140930)
            {
                return true;
            }
            return false;
        }

        private string GetCurrentTimeFormat()//for zip use
        {
            string currnetNow = DateTime.Now.ToShortDateString();
            string str_China = DateTime.Now.ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            string str_America = DateTime.Now.ToString("MM/dd/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            string str_Europe = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            McsfRestoreLogger.WriteLog("[GetCurrentTimeFormat]: start " + currnetNow);
            if (currnetNow.Equals(str_China))
            {
                return flag_China;
            }
            if (currnetNow.Equals(str_America))
            {
                return flag_America;
            }
            if (currnetNow.Equals(str_Europe))
            {
                return flag_Europe;
            }
            return "";
        }
        public void RestoreFileTime(string root, string file, string timeFile)
        {
            if (!root.EndsWith(@"\"))
            {
                root += @"\";
            }
            McsfRestoreLogger.WriteLog("[RestoreFileTime]: RestoreFileTime begin.");
            FileInfo fi = new FileInfo(file);

            FileStream fs = File.OpenRead(timeFile);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("gb2312"));
            string sLine = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            sr.Dispose();
            fs.Dispose();

            McsfRestoreLogger.WriteLog("[RestoreFileTime]: TimesLine: " + sLine);
            string[] fileTimes = sLine.Substring(1).Split('*');
            string year = "";
            string month = "";
            string day = "";
            string time = "";

            foreach (string fileTime in fileTimes)
            {
                string particalFilePathName = fileTime.Substring(0, fileTime.IndexOf("|"));
                string f = new FileInfo(root + particalFilePathName).FullName;
                if (fi.FullName.Equals(f))
                {
                    string filetimeFormat = fileTime.Split('|')[1];
                    string fileLastWriteTime = fileTime.Split('|')[2];
                    string currentTimeFormat = GetCurrentTimeFormat();

                    McsfRestoreLogger.WriteLog("[RestoreFileTime]: filetimeFormat: " + filetimeFormat);

                    var dateTimeString = string.Empty;
                    if (filetimeFormat.Equals(flag_America))
                    {
                        month = fileLastWriteTime.Split('/')[0];
                        day = fileLastWriteTime.Split('/')[1];
                        year = fileLastWriteTime.Split('/')[2].Substring(0, 4);
                        time = fileLastWriteTime.Split('/')[2].Substring(fileLastWriteTime.Split('/')[2].Length - 8);
                        dateTimeString = year + "/" + month + "/" + day + " " + time;
                    }
                    if (filetimeFormat.Equals(flag_Europe))
                    {
                        day = fileLastWriteTime.Split('/')[0];
                        month = fileLastWriteTime.Split('/')[1];
                        year = fileLastWriteTime.Split('/')[2].Substring(0, 4);
                        time = fileLastWriteTime.Split('/')[2].Substring(fileLastWriteTime.Split('/')[2].Length - 8);
                        dateTimeString = year + "/" + month + "/" + day + " " + time;
                    }

                    if (filetimeFormat.Equals(flag_China))
                    {
                        dateTimeString = fileLastWriteTime;
                    }
                    DateTime date = new DateTime();
                    DateTime.TryParseExact(dateTimeString, "yyyy/MM/dd HH:mm:ss",
                                               System.Globalization.CultureInfo.InvariantCulture,
                                               System.Globalization.DateTimeStyles.None,
                                               out date);
                    File.SetLastWriteTime(fi.FullName, date);
                }
            }

        }
        public void RestoreFolderTime(string root, string dir, string timeFile)
        {
            if (!root.EndsWith(@"\"))
            {
                root += @"\";
            }

            DirectoryInfo theFolder = new DirectoryInfo(dir);
            DirectoryInfo[] dirs = theFolder.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                DirectoryInfo dd = dirs[i] as DirectoryInfo;

                FileStream fs = File.OpenRead(timeFile);
                StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("gb2312"));
                string sLine = sr.ReadToEnd();
                sr.Close();
                fs.Close();
                sr.Dispose();
                fs.Dispose();

                McsfRestoreLogger.WriteLog("[RestoreFolderTime]: sLine: " + sLine);

                string[] folderTimes = sLine.Substring(1).Split('*');

                string year = "";
                string month = "";
                string day = "";
                string time = "";

                foreach (string folderTime in folderTimes)
                {
                    string particalFolderPathName = folderTime.Substring(0, folderTime.IndexOf("|"));
                    if (dd.FullName.Equals(root + particalFolderPathName))
                    {
                        string foldertimeFormat = folderTime.Split('|')[1];
                        string folderLastWriteTime = folderTime.Split('|')[2];
                        string currentTimeFormat = GetCurrentTimeFormat();

                        McsfRestoreLogger.WriteLog("[RestoreFolderTime]: foldertimeFormat: " + foldertimeFormat);

                        string dateTimeString = string.Empty;
                        if (foldertimeFormat.Equals(flag_America))
                        {
                            month = folderLastWriteTime.Split('/')[0];
                            day = folderLastWriteTime.Split('/')[1];
                            year = folderLastWriteTime.Split('/')[2].Substring(0, 4);
                            time = folderLastWriteTime.Split('/')[2].Substring(folderLastWriteTime.Split('/')[2].Length - 8);
                            dateTimeString = year + "/" + month + "/" + day + " " + time;
                        }
                        if (foldertimeFormat.Equals(flag_Europe))
                        {
                            day = folderLastWriteTime.Split('/')[0];
                            month = folderLastWriteTime.Split('/')[1];
                            year = folderLastWriteTime.Split('/')[2].Substring(0, 4);
                            time = folderLastWriteTime.Split('/')[2].Substring(folderLastWriteTime.Split('/')[2].Length - 8);
                            dateTimeString = year + "/" + month + "/" + day + " " + time;
                        }

                        if (foldertimeFormat.Equals(flag_China))
                        {
                            dateTimeString = folderLastWriteTime;
                        }
                        DateTime date = new DateTime();
                        DateTime.TryParseExact(dateTimeString, "yyyy/MM/dd HH:mm:ss",
                                                   System.Globalization.CultureInfo.InvariantCulture,
                                                   System.Globalization.DateTimeStyles.None,
                                                   out date);
                        Directory.SetLastWriteTime(dd.FullName, date);
                    }
                }

                if (dd != null)
                {
                    RestoreFolderTime(root, dd.FullName, timeFile);
                }
            }
        }
        /// <summary>
        /// Compress folder
        /// </summary>
        /// <param name="directoryToZip">The folder is need to compressed</param>
        /// <param name="zipedDirectory">The path of the directory after compress</param>
        /// <returns>true:Compress successfully.
        ///          failed:Compress failed.
        /// </returns>
        public bool ZipDirectory(string directoryToZip, string zipedDirectory, out string excep)
        {
            McsfRestoreLogger.WriteLog("[ZipDirectory]: ZipDirectory begin.");
            string tempFile = directoryToZip + @"\" + tempFileForFolderTime;
            string tempFileForFile = directoryToZip + @"\" + tempFileForFileTime;

            try
            {

                DirectoryInfo dd = new DirectoryInfo(directoryToZip);

                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
                if (File.Exists(tempFileForFile))
                {
                    File.Delete(tempFileForFile);
                }

                foreach (DirectoryInfo dir in dd.GetDirectories())
                {
                    HandleDirestory(directoryToZip, dir.FullName, tempFile);

                }


                AddSpecialFile2EmptyFolder(directoryToZip);



                FileInfo[] files = dd.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    HandleFile(directoryToZip, file.FullName, tempFileForFile);
                }

                FileStream fileStreamZipedDirectory = File.Create(zipedDirectory);

                using (ZipOutputStream zipStream = new ZipOutputStream(fileStreamZipedDirectory))
                {
                    ArrayList fileList = GetFileList(directoryToZip);

                    var parentDir = Directory.GetParent(directoryToZip);
                    if (null == parentDir)
                    {
                        excep = "can not compress root directory.";
                        McsfRestoreLogger.WriteLog("[ZipDirectory]: exce: " + excep);
                        return false;
                    }
                    int directoryNameLength = (parentDir).ToString().Length;
                    directoryNameLength = directoryNameLength > 3 ? ++directoryNameLength : directoryNameLength;
                    zipStream.SetLevel(6);
                    ZipEntry zipEntry = null;
                    FileStream fileStream = null;


                    foreach (string fileName in fileList)
                    {
                        string file = fileName.Remove(0, directoryNameLength);
                        zipEntry = new ZipEntry(file);
                        zipEntry.IsUnicodeText = true;
                        zipStream.PutNextEntry(zipEntry);
                        if (!fileName.EndsWith(@"/"))
                        {

                            byte[] buffer = new byte[bufferSize];
                            fileStream = File.OpenRead(fileName);
                            long fileLength = fileStream.Length;
                            int readCount = (0 == Convert.ToInt32(fileLength % bufferSize))
                                          ? (int)Math.Ceiling((double)(fileLength / bufferSize))
                                          : (int)Math.Ceiling((double)(fileLength / bufferSize)) + 1;
                            int tempCount = 0;
                            long remainSize = fileLength;
                            do
                            {
                                int read;
                                if (remainSize > bufferSize)
                                {
                                    read = fileStream.Read(buffer, 0, bufferSize);
                                    if (read > 0)
                                    {
                                        zipStream.Write(buffer, 0, read);
                                    }
                                    remainSize = remainSize - bufferSize;
                                }
                                else
                                {
                                    read = fileStream.Read(buffer, 0, Convert.ToInt32(remainSize));
                                    if (read > 0)
                                    {
                                        zipStream.Write(buffer, 0, read);
                                    }
                                }

                                alreadyCompressFileSize = beforeCompressFileSize + fileLength - remainSize;
                                tempCount++;
                            }
                            while (tempCount < readCount);
                            beforeCompressFileSize = alreadyCompressFileSize;
                        }

                        fileStream.Close();
                        fileStream.Dispose();
                    }

                    zipStream.Close();
                    zipStream.Dispose();
                }

                fileStreamZipedDirectory.Close();
                fileStreamZipedDirectory.Dispose();
                excep = "Compress folder successfully.";
                McsfRestoreLogger.WriteLog("[ZipDirectory]: " + excep);
                return true;
            }
            catch (Exception Ex)
            {
                excep = Ex.Message;
                McsfRestoreLogger.WriteLog("[ZipDirectory]: exceptiom: " + excep);
                return false;
            }
            finally
            {
                DeleteSpecialFile(directoryToZip);//压缩后源文件夹也需要删除临时文件
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
                if (File.Exists(tempFileForFile))
                {
                    File.Delete(tempFileForFile);
                }
            }
        }

        public bool HandleFile(string rootPath, string file, string fileToWriteTime)
        {
            GetFileTime(rootPath, file, fileToWriteTime);
            return true;
        }
        public bool HandleDirestory(string rootPath, string directory, string fileToWriteTime)
        {
            if (!Directory.Exists(directory))
            {
                return false;
            }
            GetDirectoryTime(rootPath, directory, fileToWriteTime);
            DirectoryInfo theFolder = new DirectoryInfo(directory);

            DirectoryInfo[] dirs = theFolder.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {

                DirectoryInfo dir = dirs[i] as DirectoryInfo;
                if (dir != null)
                {
                    HandleDirestory(rootPath, dirs[i].FullName, fileToWriteTime);
                }
            }
            return true;
        }
        public void GetFileTime(string rootPath, string file, string fileToWriteTime)
        {
            string fileName = file.Substring(rootPath.Length);
            string fileLastWriteTime = File.GetLastWriteTime(file).ToString();
            string head = "*" + fileName + "|" + GetCurrentTimeFormat() + "|" + fileLastWriteTime + "|";

            FileStream fs = new FileStream(fileToWriteTime, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(head);
            sw.Close();
            fs.Close();
            sw.Dispose();
            fs.Dispose();

        }
        public void GetDirectoryTime(string rootPath, string direstory, string fileToWriteTime)
        {
            string direName = direstory.Substring(rootPath.Length);

            string direLastWriteTime = Directory.GetLastWriteTime(direstory).ToString();

            string head = "*" + direName + "|" + GetCurrentTimeFormat() + "|" + direLastWriteTime + "|";

            FileStream fs = new FileStream(fileToWriteTime, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(head);
            sw.Close();
            fs.Close();
            sw.Dispose();
            fs.Dispose();
        }
        /// <summary>
        /// Add 1!2@3#4$5%6^7&.a1b2c3 file in all empty sub folders in the specified directory
        /// </summary>
        /// <param name="path">The path of the specified directory</param>
        public bool AddSpecialFile2EmptyFolder(string path)
        {
            try
            {
                bool isEmpty = true;
                string[] files = Directory.GetFiles(path);
                if (files.Length >= 1)
                {
                    isEmpty = false;
                }

                string[] dirs = Directory.GetDirectories(path);
                if (dirs.Length >= 1)
                {
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        bool result = AddSpecialFile2EmptyFolder(dirs[i]);
                        isEmpty = isEmpty && result;
                    }
                }
                if (isEmpty)
                {
                    File.Create(Path.Combine(path, "1!2@3#4$5%6^7&.a1b2c3")).Close();
                }
                return isEmpty;
            }
            catch (Exception ex)
            {
                McsfRestoreLogger.WriteLog("[AddSpecialFile2EmptyFolder]: exception is " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Delete 1!2@3#4$5%6^7&.a1b2c3 file in all sub folders in the specified directory
        /// </summary>
        /// <param name="path">The path of the specified directory</param>
        public bool DeleteSpecialFile(string path)
        {
            try
            {
                DirectoryInfo dirDirectory = new DirectoryInfo(path);
                FileInfo[] specialFileArray = dirDirectory.GetFiles("*.a1b2c3", SearchOption.AllDirectories);
                foreach (FileInfo file in specialFileArray)
                {
                    if ("1!2@3#4$5%6^7&.a1b2c3" == file.Name)
                        file.Delete();
                }
                return true;
            }
            catch (Exception ex)
            {
                McsfRestoreLogger.WriteLog("[DeleteSpecialFile]: exception: " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Get all files of the specified directory.
        /// </summary>
        /// <param name="directory">The path of the specified directory</param>
        /// <returns></returns>
        public static ArrayList GetFileList(string directory)
        {
            ArrayList fileList = new ArrayList();
            bool isEmpty = true;
            foreach (string file in Directory.GetFiles(directory))
            {
                fileList.Add(file);
                isEmpty = false;
            }
            if (isEmpty)
            {
                if (Directory.GetDirectories(directory).Length == 0)
                {
                    fileList.Add(directory + @"/");
                }
            }
            foreach (string dirs in Directory.GetDirectories(directory))
            {
                foreach (object obj in GetFileList(dirs))
                {
                    fileList.Add(obj);
                }
            }
            return fileList;
        }




    }
}
