using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProdTimer
{
    public class LogData: INotifyPropertyChanged
    {
        public LogData(string data)
        {
            Data = data;
        }

        private string _data;

        public string Data { get => _data; set => OnPropertyChanged(nameof(_data)); }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)  => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
