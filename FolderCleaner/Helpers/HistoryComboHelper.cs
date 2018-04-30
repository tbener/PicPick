using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PicPick.Properties;

namespace PicPick.Helpers
{
    /*
     * Class is not completed
     * 
     * Todo:
     * -----
     * 1. Support any Control (that supports AutoComplete)
     * 2. Use shared AutoCompleteStringCollection as a CustomSource
     *      2.1. It has Changed event
     * */
    class HistoryComboHelper
    {
        const string DEFAULT_SETTING_KEY = "HistoryList";

        public int MaxItems = 10;

        private ComboBox _comboBox;
        private string _settKey = DEFAULT_SETTING_KEY;
        private char[] _delimiter = new char[] { ';' };

        private static Dictionary<ComboBox, HistoryComboHelper> _dictHistoryHelpers = new Dictionary<ComboBox, HistoryComboHelper>();

        public static Dictionary<string, AutoCompleteStringCollection> _dictStringCollectionSources = new Dictionary<string, AutoCompleteStringCollection>();
        

        public HistoryComboHelper(ComboBox cb, string customKey)
        {
            _comboBox = cb;
            _settKey = customKey;
            Init();
        }

        public HistoryComboHelper(ComboBox cb) : this(cb, DEFAULT_SETTING_KEY)
        { }

        public static HistoryComboHelper CreateHistoryHelper(ComboBox cb, string customKey = DEFAULT_SETTING_KEY)
        {
            if (!_dictHistoryHelpers.Keys.Contains(cb))
            {
                HistoryComboHelper historyHelper = new HistoryComboHelper(cb, customKey);
            }
            return _dictHistoryHelpers[cb];
        }

        

        public void Init()
        {
            _comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            _comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            if (Settings.Default.SettingsKey.Contains(_settKey))
            {
                string[] strArray = Settings.Default[_settKey].ToString().Split(_delimiter, StringSplitOptions.RemoveEmptyEntries);

                AutoCompleteStringCollection sc = new AutoCompleteStringCollection();
                

                if (strArray != null)
                {
                    // fill the drop down combo
                    this._comboBox.Items.AddRange(strArray);
                    if (this._comboBox.Items.Count > 0)
                        _comboBox.SelectedIndex = 0;
                }
            }
        }

        public static void AddCurrent(ComboBox cb)
        {
            string text = cb.Text;
            string key = _dictHistoryHelpers[cb]._settKey;

            _dictHistoryHelpers[cb].AddCurrent();
            
            // todo: align other components...
            //List<HistoryComboHelper> list = _dictHistoryHelpers.Values.Where(h => h._settKey == key).ToList();
            //list.ForEach(h => h.Add(text));

        }

        private void Refresh()
        {

        }

        private void Add(string text)
        {
            //////RemoveCurrent(false);
            _comboBox.Text = text;

            _comboBox.AutoCompleteCustomSource.Insert(0, text);
            _comboBox.Items.Insert(0, text);

            while (_comboBox.Items.Count > MaxItems)
            {
                _comboBox.AutoCompleteCustomSource.RemoveAt(MaxItems);
                _comboBox.Items.RemoveAt(MaxItems);
            }

            Save();
        }

        public void AddCurrent()
        {
            string text = _comboBox.Text;

            // to make it first, we remove it from its current place
            if (_comboBox.Items.Contains(text))
            {
                _comboBox.Items.Remove(text);
            }

            // then add it as first
            _comboBox.Items.Insert(0, text);

            // make sure not to hold more than MaxItems
            while (_comboBox.Items.Count > MaxItems)
            {
                _comboBox.Items.RemoveAt(MaxItems);
            }

            Save();
        }

        public void RemoveCurrent(bool initIfEmpty)
        {
            if (_comboBox.AutoCompleteCustomSource.Contains(_comboBox.Text))
            {
                _comboBox.AutoCompleteCustomSource.Remove(_comboBox.Text);
                _comboBox.Items.Remove(_comboBox.Text);
            }

            if (initIfEmpty)
                if (_comboBox.Items.Count == 0)
                {
                    Save();
                    Init();
                }
        }

        private void Save()
        {
            string strSave = string.Empty;
            foreach (string item in _comboBox.Items)
	        {
                strSave += _delimiter[0].ToString() + item;
	        }
            Settings.Default[_settKey] = strSave;
            Settings.Default.Save();
        }
    }

    
}
