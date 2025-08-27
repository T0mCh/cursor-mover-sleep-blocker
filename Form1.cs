using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseMoverApp
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(uint esFlags);
        // 选项所用到的常数
        const uint ES_AWAYMODE_REQUIRED = 0x00000040;
        const uint ES_CONTINUOUS = 0x80000000;
        const uint ES_DISPLAY_REQUIRED = 0x00000002;
        const uint ES_SYSTEM_REQUIRED = 0x00000001;

        public Form1()
        {
            InitializeComponent();
            //points to be travelled when kick-started
            SetupPoints();

            //timer for setting up thread state handling, AKA keep PC awake and NOT to be locked or screen off
            timer1.Interval = 10000;
            timer1.Start();
        }



        protected List<Point> Mode1Points;
        protected List<Point> Mode2Points;
        protected List<Point> Mode3Points;

        /// <summary>
        /// cursor points assignment / pre-setup for points to be travelled
        /// </summary>
        public void SetupPoints()
        {
            Mode1Points = new List<Point>();
            Mode2Points = new List<Point>();
            Mode3Points = new List<Point>();

            Mode1Points.Add(new Point(0, 0));
            Mode1Points.Add(new Point(20, 0));
            Mode1Points.Add(new Point(20, 20));
            Mode1Points.Add(new Point(0, 20));

            Mode2Points.Add(new Point(320, 320));
            Mode2Points.Add(new Point(280, 280));
            Mode2Points.Add(new Point(360, 280));
            Mode2Points.Add(new Point(280, 360));
            Mode2Points.Add(new Point(360, 360));

            Mode3Points.Add(new Point(320, 320));
            Mode3Points.Add(new Point(320, 220));
            Mode3Points.Add(new Point(320, 420));
            ;
        }

        /// <summary>
        /// Mouse cursor action to be simulated by program
        /// check which set of action to be selected, and assign cursor to go to setted locations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartBut_Click(object sender, EventArgs e)
        {
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            trackBar1.Enabled = false;
            TimeTxt.Enabled = false;

            StartBut.Text = "Running...";
            StartBut.Enabled = false;
            

            //begin action
            this.Cursor = new Cursor(Cursor.Current.Handle);

            List<Point> PointSet = new List<Point>();
            if (radioButton1.Checked) { PointSet = Mode1Points; }
            else if (radioButton2.Checked) { PointSet = Mode2Points; }
            else if (radioButton3.Checked) { PointSet = Mode3Points; }

            int idx = 0;
            int rate = trackBar1.Value;
            for (int i = 0; i < int.Parse(TimeTxt.Text)* rate; i++)
            {
                Point p = PointSet[idx];
                Cursor.Position = p;
                idx++;
                if(idx == PointSet.Count) idx = 0;
                System.Threading.Thread.Sleep(1000/ rate);
            }

            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            radioButton3.Enabled = true;
            trackBar1.Enabled = true;
            TimeTxt.Enabled = true;

            StartBut.Text = "Start";
            StartBut.Enabled = true;
        }

        /// <summary>
        /// do action when timer reached the triggering timing
        /// TODO: set thread execution state to keep PC from screen-off or locked
        /// AKA keep PC awake without disconnection to WWW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            SetThreadExecutionState(ES_AWAYMODE_REQUIRED | ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED);
            MsgLbl.Text = "Set Thread Execution State @ " + DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// resize handler: 
        /// hide icon from task bar, show icon in tray bar
        /// show dialog message
        /// https://www.c-sharpcorner.com/UploadFile/f9f215/how-to-minimize-your-application-to-system-tray-in-C-Sharp/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
            }
        }

        /// <summary>
        /// double click tray icon to show dialog back
        /// hide tray bar icon after double clicking 
        /// https://www.c-sharpcorner.com/UploadFile/f9f215/how-to-minimize-your-application-to-system-tray-in-C-Sharp/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        /// <summary>
        /// hide dialog when form is shown
        /// https://stackoverflow.com/a/8486441
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            //to minimize window
            this.WindowState = FormWindowState.Minimized;

            //to hide from taskbar
            this.Hide();
        }
    }
}
