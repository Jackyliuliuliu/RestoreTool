using McsfRestoreTool.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MysqlRestoreTool.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using UIH.Mcsf.Restore.Logger;

namespace McsfRestoreTool.RepairMysql
{
    public class RepairMysqlVM : NotificationObject
    {
        private Action<string> recheckAction;
        private Action<bool> repairActon;
        private ObservableCollection<CheckResultItem> checkResultItems = new ObservableCollection<CheckResultItem>();
        private List<GroupConfigItem> groupConfigItems = new List<GroupConfigItem>();

        private DispatcherTimer dispatcherTimerDecrease = new DispatcherTimer();
        private DispatcherTimer dispatcherTimerIncrease = new DispatcherTimer();

        private string _port;
        public string Port
        {
            set { _port = value; RaisePropertyChanged("Port"); }
            get { return _port; }
        }

        private string _resultText;
        public string ResultText
        {
            set { _resultText = value; RaisePropertyChanged("ResultText"); }
            get { return _resultText; }
        }

        private bool _isSuccess;
        public bool IsSuccess
        {
            set { _isSuccess = value; RaisePropertyChanged("IsSuccess"); }
            get { return _isSuccess; }
        }

        private string _repairAnimation;
        public string RepairAnimation
        {
            set { _repairAnimation = value; RaisePropertyChanged("RepairAnimation"); }
            get { return _repairAnimation; }
        }


        private DelegateCommand _recheckCommand;
        public DelegateCommand RecheckCommand
        {
            get { return _recheckCommand ?? (_recheckCommand = new DelegateCommand(RecheckExcute)); }
        }


        public RepairMysqlVM(Action<bool> repairActon, ObservableCollection<CheckResultItem> checkResultItems, List<GroupConfigItem> groupConfigItems)
        {
            this.repairActon = repairActon;
            this.checkResultItems = checkResultItems;
            this.groupConfigItems = groupConfigItems;
            recheckAction += recheckActionExcute;
        }


        private void InitialTimer()
        {
            this.RepairAnimation = "ReCheck ";
            dispatcherTimerIncrease.Interval = TimeSpan.FromMilliseconds(300);
            dispatcherTimerIncrease.Tick += DispatcherTimerIncrease_Tick;
            dispatcherTimerIncrease.Start();
            dispatcherTimerDecrease.Interval = TimeSpan.FromMilliseconds(300);
            dispatcherTimerDecrease.Tick += DispatcherTimerDecrease_Tick;
        }

        private void StopTimer()
        {
            this.dispatcherTimerIncrease.Stop();
            this.dispatcherTimerDecrease.Stop();
            this.RepairAnimation = "";
        }

        private void RecheckExcute()
        {
            McsfRestoreLogger.WriteLog("[RecheckExcute]: begin");
            if (string.IsNullOrWhiteSpace(Port))
            {
                McsfRestoreLogger.WriteLog("[RecheckExcute]: port is null. ");
                return;
            }
            if (this.groupConfigItems == null)
            {
                McsfRestoreLogger.WriteLog("[RecheckExcute]: groupConfigItems is null. ");
                return;
            }

            GroupConfigItem group = this.groupConfigItems.FirstOrDefault(item => item.Module == "MCSF_Mysql");
            if (group == null)
            {
                McsfRestoreLogger.WriteLog("[BtnRecheck_Click]: group is null. ");
                return;
            }
            var checkItem = this.checkResultItems.FirstOrDefault(item => item.ModuleName == "MCSF_Mysql");


            if (checkItem == null)
            {
                McsfRestoreLogger.WriteLog("[RecheckExcute]: group is null. ");
                return;
            }
            this.InitialTimer();
            new Thread(() =>
            {
                int port;
                string response = "";
                DllRefection reflection = new DllRefection();
                var checkResult = checkItem.Result;
                var ret = int.TryParse(Port, out port);
                if (!ret)
                {
                    McsfRestoreLogger.WriteLog("[RecheckExcute]: port parse int failed and port is " + Port);
                    return;
                }
                McsfRestoreLogger.WriteLog(string.Format("[RecheckExcute]: port is {0} and check result is {1}", port, checkResult));
                var handleNameSpace = group.Repair.Substring(0, group.Repair.Length - 4);
                var result = reflection.DLLReflection2(group.Repair, handleNameSpace, "McsfRestoreRepair", "RepairHandler", new object[] { checkResult, port, recheckAction }, out response);
                StopTimer();
                this.IsSuccess = (int)result == 0;
                if (IsSuccess)
                {
                    var filePath = Path.GetFullPath(@"../config/port");
                    File.WriteAllText(filePath, Port);
                    this.ResultText = "repair success.";
                }
                else
                {
                    this.ResultText = "repair failed.";
                }
                var status = (EnumMySQLStatus)result;
                McsfRestoreLogger.WriteLog(string.Format("[RecheckExcute]: RepairHandler result is  {0}, mysql status is {1} and reponse is {2}", result, status, response));
                repairActon(IsSuccess);
                McsfRestoreLogger.WriteLog("[RecheckExcute]: end.");
            }).Start();

        }


        private void recheckActionExcute(string ret)
        {
            McsfRestoreLogger.WriteLog("[recheckActionExcute]: recheckActionExcute result is  " + ret);
        }

        private void DispatcherTimerDecrease_Tick(object sender, EventArgs e)
        {
            //"ReCheck ...";
            if (this.RepairAnimation.Length == 17)
            {
                dispatcherTimerIncrease.Stop();
            }

            if (this.RepairAnimation.Length > 8 && this.RepairAnimation.Length <= 17)
            {
                var temp = this.RepairAnimation;
                this.RepairAnimation = temp.Substring(0, this.RepairAnimation.Length - 3);
            }
            if (this.RepairAnimation.Length == 8)
            {
                dispatcherTimerIncrease.Start();
            }
        }

        private void DispatcherTimerIncrease_Tick(object sender, EventArgs e)
        {
            if (this.RepairAnimation.Length == 8)
            {
                dispatcherTimerDecrease.Stop();
            }
            if (this.RepairAnimation.Length >= 8 && this.RepairAnimation.Length < 17)
            {
                var temp = new StringBuilder(this.RepairAnimation);
                this.RepairAnimation = temp.Append("  .").ToString();
            }
            if (this.RepairAnimation.Length == 17)
            {
                dispatcherTimerDecrease.Start();
            }
        }


    }
}
