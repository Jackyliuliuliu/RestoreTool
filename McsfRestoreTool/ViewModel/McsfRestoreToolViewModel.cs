using McsfRestoreTool.RepairMysql;
using McsfRestoreTool.ViewModel;
using Microsoft.Practices.Prism.Commands;
using MysqlRestoreTool.Common;
using MysqlRestoreTool.Restore;
using RepairWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml;
using UIH.Mcsf.Restore.Logger;



namespace MysqlRestoreTool.ViewModel
{
    
    public class MysqlRestoreToolViewModel : NotificationObject
    {
        private DispatcherTimer dispatcherTimerDecrease = new DispatcherTimer();
        private DispatcherTimer dispatcherTimerIncrease = new DispatcherTimer();
        private Action<bool> repairAction;
        private Action<string> checkAction;
        private Action<RestoreInfo> restoreAction;
        private long completeSize;

        private List<string> _packList;
        public List<string> PackList
        {
            set { _packList = value; RaisePropertyChanged("PackList"); }
            get { return _packList ?? (_packList = new List<string>()); }
        }


        private bool _isFirstVisible = true;
        public bool IsFirstVisible
        {
            set { _isFirstVisible = value; RaisePropertyChanged("IsFirstVisible"); }
            get { return _isFirstVisible; }
        }

        private bool _isLastVisible = false;
        public bool IsLastVisible
        {
            set { _isLastVisible = value; RaisePropertyChanged("IsLastVisible"); }
            get { return _isLastVisible; }
        }

        private string _bkpFiePath;
        public string BkpFilePath
        {
            set { _bkpFiePath = value; RaisePropertyChanged("BkpFilePath"); }
            get { return _bkpFiePath; }
        }

        private int _checkNumber;
        public int CheckNumber
        {
            set { _checkNumber = value; RaisePropertyChanged("CheckNumber"); }
            get { return _checkNumber; }
        }

        private string _checkAnimation = "";
        public string CheckAnimation
        {
            set{ _checkAnimation = value; RaisePropertyChanged("CheckAnimation"); }
            get { return _checkAnimation; }
        }

        private bool _isFirstNextEnable = true;
        public bool IsFirstNextEnable
        {
            set { _isFirstNextEnable = value; RaisePropertyChanged("IsFirstNextEnable"); }
            get { return _isFirstNextEnable; }
        }


        private ObservableCollection<CheckResultItem> _checkResultList;
        public ObservableCollection<CheckResultItem> CheckResultList
        {
            get { return _checkResultList ?? (_checkResultList = new ObservableCollection<CheckResultItem>()); }
            set { _checkResultList = value; RaisePropertyChanged("CheckResultList"); }
        }

        private CheckResultItem _selectResultItem;
        public CheckResultItem SelectResultItem
        {
            get { return _selectResultItem; }
            set { _selectResultItem = value; RaisePropertyChanged("SelectResultItem"); }
        }

        private string _currentModuleName;
        public string CurrentModuleName
        {
            get { return _currentModuleName; }
            set { _currentModuleName = value; RaisePropertyChanged("CurrentModuleName"); }
        }

        public List<GroupConfigItem> GroupList;
        private List<GroupConfigItem> _groupList
        {
            get { return _groupList; }
            set { _groupList = value; RaisePropertyChanged("GroupList"); }
        }

        private RestoreInfo _currentRestoreInfo;
        public RestoreInfo CurrentRestoreInfo
        {
            set { _currentRestoreInfo = value; RaisePropertyChanged("CurrentRestoreInfo"); }
            get { return _currentRestoreInfo ??(_currentRestoreInfo = new RestoreInfo()); }
        }

        private bool _isRestore = true;
        public bool IsRestore
        {
            get { return _isRestore; }
            set { _isRestore = value; RaisePropertyChanged("IsRestore"); }
        }

        private DelegateCommand _browseCommand;
        public DelegateCommand BrowseCommand
        {
            get { return _browseCommand ?? (_browseCommand = new DelegateCommand(BrowseClickExecute)); }
        }

        private DelegateCommand _restoreCommand;
        public DelegateCommand RestoreCommand
        {
            get { return _restoreCommand ?? (_restoreCommand = new DelegateCommand(RestoreClickExcute)); }
        }

        private DelegateCommand _pageDownCommand;
        public DelegateCommand PageDownCommand
        {
            get { return _pageDownCommand ?? (_pageDownCommand = new DelegateCommand(PageDownCommandExcute)); }
        }

        private DelegateCommand _completeCommand;
        public DelegateCommand CompleteCommand
        {
            get { return _completeCommand ?? (_completeCommand = new DelegateCommand(CompleteExcute)); }
        }

        private DelegateCommand _secondPageUpCommand;
        public DelegateCommand SecondPageUpCommand
        {
            get { return _secondPageUpCommand ?? (_secondPageUpCommand = new DelegateCommand(SecondPageUpExcute)); }
        }

        private DelegateCommand _repairCommand;
        public DelegateCommand RepairCommand
        {
            get { return _repairCommand ?? (_repairCommand = new DelegateCommand(RepairExucte)); }
        }


        private DelegateCommand _contentRenderedCommand
;
        public DelegateCommand ContentRenderedCommand
        {
            get { return _contentRenderedCommand ?? (_contentRenderedCommand = new DelegateCommand(ContentRenderedExcute)); }
        }

       

        public void Initialize()
        {
            GetRestoreInfo();
            repairAction += RepairCallBackExcute;
            checkAction += CheckCallBackExcute;
            restoreAction += RestoreCallBackExcute;
        }

        private  void InitialTimer()
        {
            dispatcherTimerIncrease.Interval = TimeSpan.FromMilliseconds(300);
            dispatcherTimerIncrease.Tick += DispatcherTimerIncrease_Tick;
            dispatcherTimerIncrease.Start();
            dispatcherTimerDecrease.Interval = TimeSpan.FromMilliseconds(300);
            dispatcherTimerDecrease.Tick += DispatcherTimerDecrease_Tick;
        }


        private void InitializeCheck()
        {
            McsfRestoreLogger.WriteLog("[InitializeCheck]: begin.");
            InitialTimer();

            GroupList = GetGroupList();
            if(GroupList==null || GroupList.Count == 0)
            {
                McsfRestoreLogger.WriteLog("[InitializeCheck]: GroupList is null.");
                return;
            }

            new Thread(() =>
            {
                foreach (var item in GroupList)
                {
                    string response = "";
                    DllRefection reflection = new DllRefection();
                    this.CurrentModuleName = item.Module;
                    var nameSpace = item.Check.Substring(0, item.Check.Length - 4);
                    //var result = reflection.DLLReflection2(item.Check, nameSpace, "McsfRestoreCheck", "CheckHandler", new object[] { checkAction }, out response);
                    var result = 0;
                    if ((int)result != 0)
                    {
                        this.IsFirstNextEnable = false;
                    }
                    //Nothing = -1,Accessed = 0,UnAccessed = 1,NotExisted = 2,Running = 3,Stopped = 4
                    var status = (EnumMySQLStatus)result;
                    McsfRestoreLogger.WriteLog(string.Format("[InitializeCheck]: item.Check  DLLReflection2 ret is {0} and status is {1}", result, status));
                    var checkResultItem = new CheckResultItem()
                    {
                        Description = "test",
                        IsSuccess = (int)result == 0,
                        Result = (int)result,
                        ModuleName = item.Module,
                        IsWindow = item.IsWindow,
                        ClassName = item.ClassName
                    };
           
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        this.CheckResultList.Add(checkResultItem);
                        this.CheckNumber += 1;
                        if (CheckNumber == GroupList.Count)
                        {
                            this.dispatcherTimerDecrease.Stop();
                            this.dispatcherTimerIncrease.Stop();
                            this.CurrentModuleName = "";
                            this.CheckAnimation = "Complete.";
                        }
                    }));

                }

            }).Start();
        }

        private void DispatcherTimerDecrease_Tick(object sender, EventArgs e)
        {
            //"Check: Mysql...";
            if (this.CheckAnimation.Length == 9)
            {
                dispatcherTimerIncrease.Stop();
            }
       
            if (this.CheckAnimation.Length > 0 && this.CheckAnimation.Length <= 9)
            {
                var temp = this.CheckAnimation;
                this.CheckAnimation = temp.Substring(0, this.CheckAnimation.Length - 3);
            }
            if (this.CheckAnimation.Length == 0)
            {
                dispatcherTimerIncrease.Start();
            }
        }

        private void DispatcherTimerIncrease_Tick(object sender, EventArgs e)
        {
            if (this.CheckAnimation.Length == 0)
            {
                dispatcherTimerDecrease.Stop();
            }
            if (this.CheckAnimation.Length >= 0 && this.CheckAnimation.Length < 9)
            {
                var temp = new StringBuilder(this.CheckAnimation);
                this.CheckAnimation = temp.Append("  .").ToString();
            }
            if (this.CheckAnimation.Length == 9)
            {
                dispatcherTimerDecrease.Start();
            }
        }

        private void GetRestoreInfo()
        {
            McsfRestoreLogger.WriteLog("[GetRestoreInfo]: begin.");
            var path = Path.GetFullPath(@"../config/");
            McsfRestoreLogger.WriteLog("[GetRestoreInfo]: path is " + path);
            if (string.IsNullOrWhiteSpace(path))
            {
                McsfRestoreLogger.WriteLog("[GetRestoreInfo]: path is null.");
                return;
            }
            if (!Directory.Exists(path))
            {
                McsfRestoreLogger.WriteLog(string.Format("[GetRestoreInfo]: path {0} is not exist.", path));
                return;
            }
            var configPath = path + "ServiceRestore.xml";
            XmlDocument xDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(configPath, settings);
            xDoc.Load(reader);
            XmlElement root = (XmlElement)xDoc.SelectSingleNode("Root");
            var xmlList = root["Packages"];
            foreach (XmlElement packageItem in xmlList)
            {
                var xmlValue = packageItem.InnerText;
                this.PackList.Add(xmlValue);
            }
            McsfRestoreLogger.WriteLog("[GetRestoreInfo]: end.");
        }

       
    

        public List<GroupConfigItem> GetGroupList()
        {
            McsfRestoreLogger.WriteLog("[GetCheckList]: begin.");
            try
            {
                List<GroupConfigItem> ret = new List<GroupConfigItem>();
                var currentDir = Directory.GetCurrentDirectory();//获取应用程序的当前工作目录
                var xmlPath = Path.GetFullPath(@"../config/ServiceCheck.xml");
                McsfRestoreLogger.WriteLog("[GetCheckList]: xmlPath is "+ xmlPath);
                XmlDocument doc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(xmlPath, settings);
                doc.Load(reader);
                XmlNode root = doc.SelectSingleNode("Root");
                XmlElement rootXml = (XmlElement)root;
                var groups = rootXml["Groups"];
                var groupList = groups["Group"];
                foreach (XmlElement group in groups)
                {
                    var checkConfigItem = new GroupConfigItem();
                    checkConfigItem.Package = group.GetAttribute("Package");
                    checkConfigItem.Module = group.GetAttribute("Module");
                    if (group["Check"] == null || group["Repair"] == null)
                    {
                        McsfRestoreLogger.WriteLog(string.Format("[GetCheckList]: group {0} has no check or restore item.", checkConfigItem.Module));
                        continue;
                    }
                    checkConfigItem.Check = group["Check"].InnerText;
                    checkConfigItem.Repair = group["Repair"].InnerText;
                    checkConfigItem.IsWindow = group["Repair"].GetAttribute("IsWindow").ToLower() == "true";
                    checkConfigItem.ClassName = group["Repair"].GetAttribute("ClassName");
                    ret.Add(checkConfigItem);
                }
                McsfRestoreLogger.WriteLog("[GetCheckList]: end.");
                return ret;
            }
            catch (Exception ex)
            {
                McsfRestoreLogger.WriteLog("[GetCheckList]: exception " + ex.Message);
                return null;
            }
        }

        private void RepairCallBackExcute(bool ret)
        {
            McsfRestoreLogger.WriteLog("[RepairCallBackExcute]: begin.");
            McsfRestoreLogger.WriteLog("[RepairCallBackExcute]: ret is " + ret);
            this.SelectResultItem.IsSuccess = true;
            this.IsFirstNextEnable = true;
        }

        private void CheckCallBackExcute(string ret)
        {
            McsfRestoreLogger.WriteLog("[CheckCallBackExcute]: begin.");
            McsfRestoreLogger.WriteLog("[CheckCallBackExcute]: ret is " + ret);
        }

        private void RestoreCallBackExcute(RestoreInfo restoreInfo)
        {

            new Thread(() =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {

                    this.CurrentRestoreInfo.IsRestoreFile = restoreInfo.IsRestoreFile;
                    this.CurrentRestoreInfo.IsDecompress = restoreInfo.IsDecompress;
                    this.CurrentRestoreInfo.Message = restoreInfo.Message;

                    if (restoreInfo.IsDecompress && restoreInfo.TotalSize!=0)
                    {
                        this.completeSize += restoreInfo.Size;
                        this.CurrentRestoreInfo.Percent = (decimal)this.completeSize / restoreInfo.TotalSize;
                    }
                    if (restoreInfo.IsRestoreFile)
                    {
                        this.CurrentRestoreInfo.RestoreTextList.Add(restoreInfo.Message);
                    }
                }));
            }).Start();

        }
        private void BrowseClickExecute()
        {
            McsfRestoreLogger.WriteLog("[BrowseClickExecute]: begin.");
            var dialog = new OpenFileDialog()
            {
                Filter = "Text documents (.bkp)|*.bkp|All files (*.*)|*.*"
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.BkpFilePath = dialog.FileName;
                McsfRestoreLogger.WriteLog("[BrowseClickExecute]: select file is  " + dialog.FileName);
            }
            else
            {
                McsfRestoreLogger.WriteLog("[BrowseClickExecute]: dialog result is cancel.");
            }
        }

        private void RestoreClickExcute()
        {
            this.CurrentRestoreInfo = null;
            this.completeSize = 0;
            this.IsRestore = false;
            new Thread(() =>
            {
                McsfRestoreLogger.WriteLog("[RestoreClickExcute]: begin.");
                SyncTaskUtility RestoreAsyncTask = new SyncTaskUtility();
                if (string.IsNullOrWhiteSpace(this.BkpFilePath))
                {
                    this.IsRestore = true;
                    McsfRestoreLogger.WriteLog("[RestoreClickExcute]: bapfile path is null.");
                    return;
                }
                if (!File.Exists(this.BkpFilePath))
                {
                    this.IsRestore = true;
                    McsfRestoreLogger.WriteLog(string.Format("[RestoreClickExcute]: bkpfile path {0} is not exists .", this.BkpFilePath));
                    return;
                }
                if (this.PackList == null || this.PackList.Count == 0)
                {
                    this.IsRestore = true;
                    McsfRestoreLogger.WriteLog(string.Format("[RestoreClickExcute]: PackList is null."));
                    return;
                }


                ResultType ret = RestoreAsyncTask.Execute<RestoreMainWorkflow>(this.BkpFilePath, this.PackList, this.restoreAction);
                McsfRestoreLogger.WriteLog("[RestoreClickExcute]: excute result is " + ret);
                McsfRestoreLogger.WriteLog("[RestoreClickExcute]: end.");
                this.IsRestore = true;
            }).Start();
        }

        private void PageDownCommandExcute()
        {
            this.IsLastVisible = true;
            this.IsFirstVisible = false;
        }

        private void CompleteExcute()
        {
            this.IsFirstVisible = true;
            this.IsLastVisible = false;
        }


        private void SecondPageUpExcute()
        {
            this.IsFirstVisible = true;
            this.IsLastVisible = false;
        }

        private void RepairExucte()
        {
            McsfRestoreLogger.WriteLog("[RepairExucte]: begin.");
            if (!this.SelectResultItem.IsWindow)
            {
                McsfRestoreLogger.WriteLog("[RepairExucte]: open  repair mysql window.");
                var tempCRList = CheckResultList;
                var tempGList = GroupList;
                McsfRestoreLogger.WriteLog(string.Format("[RepairExucte]: tempCRList count is {0} and tempGList count is ", tempCRList.Count, tempGList.Count));
                var repairSubWnd = new RepairMysql(repairAction, tempCRList, tempGList);
                repairSubWnd.ShowDialog();
            }
            else
            {
                McsfRestoreLogger.WriteLog("[RepairExucte]: class name is " + this.SelectResultItem.ClassName);
                var testWnd = new RepairWindowTest(this.SelectResultItem.ClassName);
                testWnd.ShowDialog();
            }
            McsfRestoreLogger.WriteLog("[RepairExucte]: end.");
        }

        private void ContentRenderedExcute()
        {
            InitializeCheck();
        }



    }
}
