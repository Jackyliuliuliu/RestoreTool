using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McsfRestoreTool.ViewModel
{
    public class GroupConfigItem : NotificationObject
    {
        private string _package;
        public string Package
        {
            set { _package = value; RaisePropertyChanged("Package"); }
            get { return _package; }
        }
        private string _module;
        public string Module
        {
            set { _module = value; RaisePropertyChanged("Module"); }
            get { return _module; }
        }
        private string _check;
        public string Check
        {
            set { _check = value; RaisePropertyChanged("Check"); }
            get { return _check; }
        }
        private string _repair;
        public string Repair       
        {
            set { _repair = value; RaisePropertyChanged("Repair"); }
            get { return _repair; }
        }

        private bool _isWindow;
        public bool IsWindow 
        {
            set { _isWindow = value; RaisePropertyChanged("IsWindow"); }
            get { return _isWindow; }
        }

        private string _className;
        public string ClassName     
        {
            set { _className = value; RaisePropertyChanged("ClassName"); }
            get { return _className; }
        }
    }
}
