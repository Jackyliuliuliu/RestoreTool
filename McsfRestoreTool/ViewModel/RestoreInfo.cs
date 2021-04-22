using MysqlRestoreTool.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace McsfRestoreTool.ViewModel
{
    public class RestoreInfo: NotificationObject
    {
        public RestoreInfo()
        {

        }

        private string _message;
        public string Message
        {
            set { _message = value;RaisePropertyChanged("Message"); }
            get { return _message; }
        }

        private bool _isDecompress = false;
        public bool IsDecompress
        {
            set { _isDecompress = value;RaisePropertyChanged("IsDecompress"); }
            get { return _isDecompress; }
        }

        private bool _isRestoreFile = false;
        public bool IsRestoreFile
        {
            set { _isRestoreFile = value;RaisePropertyChanged("IsRestoreFile"); }
            get { return _isRestoreFile; }
        }

        private long _totoalSize;
        public long TotalSize
        {
            set { _totoalSize = value;RaisePropertyChanged("TotalSize"); }
            get { return _totoalSize; }
        }

        private long _size;
        public long Size
        {
            set { _size = value; RaisePropertyChanged("Size"); }
            get { return _size; }
        }

        private decimal _percent;
        public decimal Percent
        {
            set { _percent = value; RaisePropertyChanged("Percent"); }
            get { return _percent; }
        }

        private ObservableCollection<string> _restoreTextList;
        public ObservableCollection<string> RestoreTextList
        {
            set { _restoreTextList = value; RaisePropertyChanged("RestoreTextList"); }
            get { return _restoreTextList ?? (_restoreTextList = new ObservableCollection<string>()); }
        }

    }
}
