using System;
using System.Net.Mime;
using System.Windows.Forms;

namespace TalUtils
{
    public class Msg
    {
        public static string Caption { get; set; }

        static Msg()
        {

            Caption = Application.ProductName;
            //  For WPF
            //Caption = MediaTypeNames.Application.ProductName;
        }

        #region Show normal message overloads

        public static void Show(string str, params object[] args)
        {
            MessageBox.Show(string.Format(str, args), Caption,MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void Show(string str)
        {
            Show(str, new object());
        }

        #endregion

        #region Show Question message overloads

        public static bool ShowQ(string str, params object[] args)
        {
            return (MessageBox.Show(string.Format(str, args), Caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK);
        }
        public static bool ShowQ(string str)
        {
            return ShowQ(str, new object());
        }

        #endregion

        #region Show Error message overloads

        public static void ShowE(string str)
        {
            MessageBox.Show(str, Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        public static void ShowE(Exception ex)
        {
            ShowE(ex.ToString());
        }
        public static void ShowE(Exception ex, string strBefore)
        {
            ShowE(new Exception(strBefore, ex));
        }
        public static void ShowE(Exception ex, string strBefore, params object[] args)
        {
            ShowE(new Exception(string.Format(strBefore, args), ex));
        }

        public static void ShowE(string str, params object[] args)
        {
            ShowE(string.Format(str, args));
        }
        #endregion
    }
}
