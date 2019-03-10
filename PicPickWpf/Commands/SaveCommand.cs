using PicPick.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TalUtils;

namespace PicPick.Commands
{
    public class SaveCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        
        public void Execute(object parameter)
        {
            Save( Convert.ToBoolean(parameter));
        }

        #region Saving methods

        /// <summary>
        /// Save or Save As
        /// If the paramter forceSaveAs=true or there is no filename yet, the SaveAs method will be called.
        /// </summary>
        /// <param name="forceSaveAs">Pass True to prompt the user for file name</param>
        /// <returns></returns>
        public bool Save(bool forceSaveAs=false)
        {
            if (forceSaveAs || ProjectHelper.FileName == null)
                return SaveAs();
            else
                return ProjectHelper.Save();
        }

        private bool Save(string file)
        {
            return ProjectHelper.Save(file);
        }

        private bool SaveAs()
        {
            string file = "";
            if (DialogHelper.BrowseSaveFileByExtensions(new[] { "picpick" }, true, ref file))
            {
                Save(file);
                return true;
            }
            // not saved
            return false;
        } 

        #endregion
    }
}
