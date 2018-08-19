using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PicPick.UserControls
{
    public partial class CopyActionDisplay : UserControl
    {
        private readonly Color BackColorDefault;
        public CopyActionDisplay()
        {
            InitializeComponent();

            BackColorDefault = BackColor;
            WireAllControls(this);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        private void WireAllControls(Control cont)
        {
            foreach (Control ctl in cont.Controls)
            {
                ctl.Click += (s, e) => this.InvokeOnClick(this, e);
                ctl.MouseEnter += (s, e) => SetBackColor(true);
                ctl.MouseLeave += (s, e) => SetBackColor(false);

                if (ctl.HasChildren)
                {
                    WireAllControls(ctl);
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }

        public ImageInfo ImageInfo { get => imageInfoControl; }

        public string Header { get => lblHeader.Text; set => lblHeader.Text = value; }
        public string Details { get => lblText.Text; set => lblText.Text = value; }

        public bool ImagePaneVisible { get => imageInfoControl.Visible; set => imageInfoControl.Visible = value; }

        bool _mouseOver;

        private void SetBackColor(bool active)
        {
            if (active)
            {
                if (_mouseOver) return;
                _mouseOver = true;
                SuspendDrawing(this);
                BackColor = Color.LightSkyBlue;
                ResumeDrawing(this);
            }
            else if (!IsMouseOver())
            {
                _mouseOver = false;
                BackColor = SystemColors.Control;
            }
        }

        private void CopyActionDisplay_MouseEnter(object sender, EventArgs e)
        {
            SetBackColor(true);
        }

        private void CopyActionDisplay_MouseLeave(object sender, EventArgs e)
        {
            SetBackColor(false);
        }


        private bool IsMouseOver()
        {
            return this.GetChildAtPoint(this.PointToClient(MousePosition)) != null;
        }
    }
}
