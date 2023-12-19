using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProdTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime _startTime = DateTime.Now;
        private DateTime _lastTime = DateTime.Now;
        private TimerStates _timerState = TimerStates.Stopped;
        private Task _timerTask = Task.FromResult(0);
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public LogData LogData = new(@"Timer log... \n");

        public delegate void UpdateTimerDelegate();
        public delegate void StartTimerDelegate();

        private void UpdateTimer()
        {
            var currentTime = _lastTime - _startTime;
            var textTime = currentTime.ToString(@"hh\:mm\:ss");
            //var textTime = currentTime.ToString(@"mm\:ss\.ff");
            TimerBox.Text = textTime;
        }

        private void InvokeUpdateTimer(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(1000);
                _lastTime = DateTime.Now;
                TimerBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateTimerDelegate(UpdateTimer));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            var logDataBinding = new Binding("data")
            {
                Source = LogData
            };
            LogView.SetBinding(TextBlock.TextProperty, logDataBinding);
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }

            if (e.ChangedButton == MouseButton.Right)
            {
                ContextMenu? contextMenu = this.FindName("MainContextMenu") as ContextMenu;
                if (contextMenu == null)
                {
                    throw new InvalidOperationException("Context Menu not found!");
                }
                contextMenu.PlacementTarget = this;
                contextMenu.IsOpen = true;
            }

        }

        private void MenuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StartStopResumeButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_timerState)
            { 
                case TimerStates.Stopped:
                    _timerState = TimerStates.Running;
                    var diffTime = _lastTime - _startTime;
                    _lastTime = DateTime.Now;
                    _startTime = _lastTime - diffTime;
                    StartStopResumeButton.Content = "Stop";
                    LogData.Data += @"\nStarted timer.";
                    break;
                case TimerStates.Running:
                    _cancellationTokenSource.Cancel();
                    _timerState = TimerStates.Stopped;
                    StartStopResumeButton.Content = "Resume";
                    break;
            }
            if (_timerState == TimerStates.Running)
            {
                _cancellationTokenSource = new();
                var cancellationToken = _cancellationTokenSource.Token;
                _timerTask = Task.Run(() => this.InvokeUpdateTimer(cancellationToken), cancellationToken);
            }

            UpdateTimer();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _timerState = TimerStates.Stopped;
            _startTime = DateTime.Now;
            _lastTime = _startTime;
            StartStopResumeButton.Content = "Start";
            UpdateTimer();
        }

        private void LogExpander_Expanded(object sender, RoutedEventArgs e)
        {
            LogExpander.Height = 175;
        }

        private void LogExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            LogExpander.Height = 24;
        }
    }
}
