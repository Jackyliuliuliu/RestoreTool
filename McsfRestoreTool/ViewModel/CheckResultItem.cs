using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McsfRestoreTool.ViewModel
{
    public class CheckResultItem : NotificationObject
    {
        private bool _isSuccess;
        public bool IsSuccess
        {
            set { _isSuccess = value; RaisePropertyChanged("IsSuccess"); }
            get { return _isSuccess; }
        }

        private int _result;
        public int Result
        {
            set { _result = value; RaisePropertyChanged("Result"); }
            get { return _result; }
        }

        private string _moduleName;
        public string ModuleName
        {
            set { _moduleName = value; RaisePropertyChanged("ModuleName"); }
            get { return _moduleName; }
        }

        private string _description;
        public string Description
        {
            set { _description = value; RaisePropertyChanged("Description"); }
            get { return _description; }
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
