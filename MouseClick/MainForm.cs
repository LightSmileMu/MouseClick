using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseClick
{
    public partial class MainForm : Form
    {
        private readonly KeyboardHook _kHook = new KeyboardHook();
        private KeyboardHook.KeyboardHookCallback _myKeyEventHandeler; //按键钩子

        public MainForm()
        {
            InitializeComponent();
        }

        private void StartListen()
        {
            try
            {
                _myKeyEventHandeler = hook_KeyDown;
                _kHook.KeyDown += _myKeyEventHandeler; //钩住键按下
                _kHook.Install(); //安装键盘钩子
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void StopListen()
        {
            if (_myKeyEventHandeler != null)
            {
                _kHook.KeyDown -= _myKeyEventHandeler; //取消按键事件
                _myKeyEventHandeler = null;
                _kHook.Install(); //关闭键盘钩子
            }
        }

        private void hook_KeyDown(KeyboardHook.VKeys key)
        {
            if (key == KeyboardHook.VKeys.F8)
            {
                tbX.Text = MousePosition.X.ToString();
                tbY.Text = MousePosition.Y.ToString();
            }
            else if (key == KeyboardHook.VKeys.F9)
            {
                button1_Click(btnClick,null);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            StartListen();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                StopListen();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            base.OnClosing(e);
        }

        private void DoMouseClick()
        {
            int x;
            int y;

            if (int.TryParse(tbX.Text, out x))
            {
                if (int.TryParse(tbY.Text, out y))
                {
                    if (x >= 0 && y >= 0)
                    {
                        var dx = (int) ((double) x / Screen.PrimaryScreen.Bounds.Width *
                                        65535); //屏幕分辨率映射到0~65535(0xffff,即16位)之间
                        var dy = (int) ((double) y / Screen.PrimaryScreen.Bounds.Height *
                                        0xffff); //转换为double类型运算，否则值为0、1
                        mouse_event(MouseEventFlag.Move | MouseEventFlag.LeftDown | MouseEventFlag.LeftUp |
                            MouseEventFlag.Absolute, dx, dy, 0, new UIntPtr(0)); //点击
                    }
                }
            }
        }

        /// <summary>
        ///     鼠标事件
        /// </summary>
        /// <param name="flags">事件类型</param>
        /// <param name="dx">x坐标值(0~65535)</param>
        /// <param name="dy">y坐标值(0~65535)</param>
        /// <param name="data">滚动值(120一个单位)</param>
        /// <param name="extraInfo">不支持</param>
        [DllImport("user32.dll")]
        private static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);

        private void button1_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                return;
            }

            timer1.Enabled = !timer1.Enabled;
            if (button.Text == @"开始点击(F9)")
            {
                button.Text = @"停止点击(F9)";
            }
            else
            {
                button.Text = @"开始点击(F9)";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DoMouseClick();
        }

        private void tbTimeInterval_TextChanged(object sender, EventArgs e)
        {
            var box = sender as TextBox;
            if (box == null)
            {
                return;
            }
            int interval;
            if (int.TryParse(box.Text, out interval))
            {
                if (interval > 0)
                {
                    timer1.Interval = interval * 1000;
                }
            }
        }
    }

    /// <summary>
    ///     鼠标操作标志位集合
    /// </summary>
    [Flags]
    internal enum MouseEventFlag : uint
    {
        /// <summary>
        ///     鼠标移动事件
        /// </summary>
        Move = 0x0001,

        /// <summary>
        ///     鼠标左键按下事件
        /// </summary>
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        XDown = 0x0080,
        XUp = 0x0100,
        Wheel = 0x0800,
        VirtualDesk = 0x4000,

        /// <summary>
        ///     设置鼠标坐标为绝对位置（dx,dy）,否则为距离最后一次事件触发的相对位置
        /// </summary>
        Absolute = 0x8000
    }
}