using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicPick.Views.UserControls1
{
    public partial class PatternComboBox : ComboBox
    {
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (Items.Count == 0)
                Items.AddRange(new string[]{ "YYYY\\MM", "YYYY-MM"});
        }
    }
}
