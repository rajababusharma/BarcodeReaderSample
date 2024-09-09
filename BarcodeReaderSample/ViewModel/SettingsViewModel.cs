using BarcodeReaderSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeReaderSample.ViewModel
{
    internal class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            
        }
        private string _filechoise = Preferences.Get(Constants.FILEFORMAT,".csv");
        public string Selection
        {
            get
            {
                return _filechoise;
            }
            set
            {
                _filechoise = value;
                Preferences.Set(Constants.FILEFORMAT, value);
                NotifyPropertyChanged("Selection");
            }
        }

        private bool _ischecked = Preferences.Get(Constants.ISDUPLICATE, false);
        public bool IsChecked
        {
            get
            {
                return _ischecked;
            }
            set
            {
                _ischecked = value;
                Preferences.Set(Constants.ISDUPLICATE, value);
                NotifyPropertyChanged("IsChecked");
            }
        }
    }
}
