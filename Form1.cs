using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace HotKeyRepeat
{
    public partial class Form1 : Form
    {
        private static bool DEBUG_CONSOLE = false;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        public Form1()
        {
            InitializeComponent();
        }

        private IntPtr GetAsepriteMainWindowHandle()
        {
            Process[] processList = Process.GetProcesses();

            foreach (Process process in processList)
            {
                if (process.ProcessName == textBoxProcessName.Text)
                {
                    return FindWindow(null, process.MainWindowTitle);
                }
            }

            return IntPtr.Zero;
        }

        private void SendHotKeyToAseprite()
        {
            IntPtr aseprite = GetAsepriteMainWindowHandle();
            IntPtr foreground = GetForegroundWindow();

            // if Aseprite is not in the foreground, it's unlikely there were recent changes anyway
            if (aseprite == foreground)
            {
                debug("Sending hotkey!");
                SendKeys.Send(textBoxHotKey.Text);
            } else
            {
                debug("Aseprite is not in the foreground");
            }
        }

        private void debug(String msg)
        {
            if (DEBUG_CONSOLE)
            {
                Console.WriteLine(msg);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (DEBUG_CONSOLE)
            {
                AllocConsole();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SendHotKeyToAseprite();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTimerEnabled.Checked)
            {
                try
                {
                    Int32 interval = Int32.Parse(textBoxInterval.Text);
                    if (interval > 0)
                    {
                        timer.Interval = interval;
                    } else
                    {
                        checkBoxTimerEnabled.Checked = false;
                        return;
                    }
                } catch (FormatException ex)
                {
                    checkBoxTimerEnabled.Checked = false;
                    return;
                }
            }

            timer.Enabled = checkBoxTimerEnabled.Checked;
            textBoxInterval.Enabled = !checkBoxTimerEnabled.Checked;
            textBoxHotKey.Enabled = !checkBoxTimerEnabled.Checked;
            textBoxProcessName.Enabled = !checkBoxTimerEnabled.Checked;
        }
    }
}
