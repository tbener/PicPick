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
using System.Diagnostics;

namespace PicPick.UserControls 
{
    public partial class CopyActionDisplay : UserControl
    {
        public CopyActionDisplay()
        {
            InitializeComponent();

            WireAllControls(this);

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


        public ImageInfo ImageInfo { get => imageInfoControl; }

        public string Header { get => lblHeader.Text; set => lblHeader.Text = value; }
        public string Details { get => lblText.Text; set => lblText.Text = value; }

        public bool ImagePaneVisible { get => imageInfoControl.Visible; set => imageInfoControl.Visible = value; }

        bool _mouseOver;

        private void SetBackColor(bool active)
        {
            if (active)
            {
                if (_mouseOver)
                {
                    Debug.Print($"{DateTime.Now} Still in");
                    return;
                }
                _mouseOver = true;
                Debug.Print($"{DateTime.Now} Draw");
                BackColor = Color.GhostWhite;
            }
            else if (!IsMouseOver())
            {
                Debug.Print($"{DateTime.Now} Clear");
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
            Debug.Print($"{MousePosition} ===>  {this.PointToClient(MousePosition)}");
            return this.GetChildAtPoint(this.PointToClient(MousePosition)) != null;
        }

    }
}
