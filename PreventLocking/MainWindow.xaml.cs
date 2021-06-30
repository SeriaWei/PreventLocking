using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace PreventLocking
{
    [FlagsAttribute]
    public enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
        // Legacy flag, should not be used.
        // ES_USER_PRESENT = 0x00000004
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
        private readonly Timer _timer;
        private Delegate DisplayTimeDelegate;
        public MainWindow()
        {
            InitializeComponent();

            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);

            App.Current.Exit += new ExitEventHandler((sender, e) =>
            {
                SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            });
            _timer = new Timer(TimerCallback);
            _timer.Change(1000, 1000);
            DisplayTimeDelegate = new Action(DisplayTime);
        }

        public void TimerCallback(object state)
        {
            Label_Time.Dispatcher.BeginInvoke(DisplayTimeDelegate);
        }
        public void DisplayTime()
        {
            Label_Time.Content = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
