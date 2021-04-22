using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MysqlRestoreTool.ViewModel
{
    public class NotificationObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        public Action OnPropertyChanged { set; get; }
        #endregion

        #region Private Methods
        protected void RaisePropertyChanged(string propertyName, bool ShouldFirePropertyChangedEvent = true)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
            if (ShouldFirePropertyChangedEvent && OnPropertyChanged != null && !string.Equals(propertyName, "ErrorMessage") && !string.Equals(propertyName, "IsModified"))
            {
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
