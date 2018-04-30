using System;
using System.Windows.Forms;

namespace TalUtils
{
    public delegate void TextHandler(string text);

    /// <summary>
    /// Progress Manager Class
    /// 
    /// Provide a ProgressBar and a Text Handler and use this class to easily update the UI.
    /// </summary>
    public class ProgressManager
    {
        public ProgressBar ProgressBar
        {
            get { return _progressBar; }
            set
            {
                _progressBar = value;
                _progressBar.Step = 1;
            }
        }

        public Control TextControl { get; set; }
        public TextHandler TextHandler { get; set; }
        public string TextPrefix { get; set; }

        #region CTOR

        public ProgressManager(Form ownerForm)
        {
            OwnerForm = ownerForm;
        }

        #endregion

        private delegate bool UpdateControlsDelegate(bool promote, int? value, String text);
        private UpdateControlsDelegate _updateControls;
        private Form _ownerForm;

        public void Reset(String text = null)
        {
            _cancelled = false;
            UpdateControls(true, 0, text);
        }
        public void Reset(string text, params object[] args)
        {
            _cancelled = false;
            UpdateControls(true, 0, text, args);
        }


        public bool Promote(int value)
        {
            return UpdateControls(true, ProgressBar.Value + value);
        }

        public bool Promote(int value, string text, params object[] args)
        {
            return UpdateControls(true, value, text, args);
        }

        public bool Promote(string text, params object[] args)
        {
            return UpdateControls(true, null, text, args);
        }

        public bool Promote()
        {
            return UpdateControls();
        }

        public bool Update(int value, string text, params object[] args)
        {
            return UpdateControls(true, value, text, args);
        }

        public bool Update(string text, params object[] args)
        {
            return UpdateControls(false, null, text, args);
        }

        public bool Update(int value)
        {
            return UpdateControls(true, value);
        }

        private bool UpdateControls(bool updateProgressBar = true, int? value = null, String text = null)
        {

            if (CheckCancelled()) return false;

            if (OwnerForm.InvokeRequired)
            {
                OwnerForm.Invoke(_updateControls, new object[] { updateProgressBar, value, text });
                return true;
            }

            if (updateProgressBar)
                PromoteProgress(value);

            if (text != null)
                UpdateText(text);

            return true;
        }
        private bool UpdateControls(bool updateProgressBar, int? value, string text, params object[] args)
        {
            return UpdateControls(updateProgressBar, value, string.Format(text, args));
        }

        private void PromoteProgress(int? value = null)
        {
            if (ProgressBar != null)
                if (value.HasValue)
                {
                    if (value.Value < ProgressBar.Maximum)
                        ProgressBar.Value = value.Value < ProgressBar.Maximum ? value.Value : ProgressBar.Maximum;
                }
                else
                    ProgressBar.PerformStep();

        }

        private void UpdateText(string text)
        {
            text = TextPrefix + text;

            if (TextHandler != null)
                TextHandler(text);

            if (TextControl != null)
                TextControl.Text = text;
        }

        public Form OwnerForm
        {
            get { return _ownerForm; }
            set
            {
                _ownerForm = value;
                _updateControls = UpdateControls;
            }
        }

        public void Cancel()
        {
            _cancelled = true;
            Update("Cancelling...");
        }

        public bool CheckCancelled(bool ask = false, String message=null, params object[] args)
        {
            string msg = "Are you sure you want to stop?";
            if (message != null)
                msg = string.Format(message, args);

            if (_cancelled && ask)
                _cancelled = Msg.ShowQ(msg);
            return _cancelled;
        }

        private bool _cancelled;
        private ProgressBar _progressBar;
    }
}
