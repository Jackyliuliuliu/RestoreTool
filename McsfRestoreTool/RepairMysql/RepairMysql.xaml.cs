using McsfRestoreTool.ViewModel;
using MysqlRestoreTool.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UIH.Mcsf.Restore.Logger;

namespace McsfRestoreTool.RepairMysql
{
    /// <summary>
    /// RepairMysql.xaml 的交互逻辑
    /// </summary>
    public partial class RepairMysql : Window
    {
        public RepairMysql(Action<bool> repairActon, ObservableCollection<CheckResultItem> checkResultItems, List<GroupConfigItem> groupConfigItems)
        {
            InitializeComponent();
            this.DataContext = new RepairMysqlVM(repairActon, checkResultItems, groupConfigItems);
            var mainWnd = Application.Current.MainWindow;
            this.Owner = mainWnd;

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove(); 
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnRecheck_Click(object sender, RoutedEventArgs e)
        {
            //McsfRestoreLogger.WriteLog("[BtnRecheck_Click]: begin");
            //string response = "";
            //DllRefection reflection = new DllRefection();
            //if (this.groupConfigItems == null)
            //{
            //    McsfRestoreLogger.WriteLog("[BtnRecheck_Click]: groupConfigItems is null. ");
            //    return;
            //}

            //GroupConfigItem group = this.groupConfigItems.FirstOrDefault(item => item.Module == "MCSF_Mysql");
            //if (group == null)
            //{
            //    McsfRestoreLogger.WriteLog("[BtnRecheck_Click]: group is null. ");
            //    return;
            //}
            //var checkItem = this.checkResultItems.FirstOrDefault(item => item.ModuleName == "MCSF_Mysql");


            //if (checkItem == null)
            //{
            //    McsfRestoreLogger.WriteLog("[BtnRecheck_Click]: group is null. ");
            //    return;
            //}
            //var checkResult = checkItem.Result;


            //var handleNameSpace = group.Repair.Substring(0, group.Repair.Length - 4);
            //var result = reflection.DLLReflection2(group.Repair, handleNameSpace, "McsfRestoreRepair", "RepairHandler", new object[] { checkResult, 3333, recheckAction }, out response);
            //McsfRestoreLogger.WriteLog("[BtnRecheck_Click]: port is and check ret is and result is ");
            //McsfRestoreLogger.WriteLog("[BtnRecheck_Click]: RepairHandler result is  " + result);
            //repairActon((int)result == 0);

        }


    }
}
