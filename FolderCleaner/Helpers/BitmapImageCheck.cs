using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FolderCleaner.Helpers
{
    /// <summary>
    /// Provides methods for checking whether a file can likely be opened as a BitmapImage, based upon its file extension
    /// </summary>
    public class BitmapImageCheck : IDisposable
    {
        #region class variables
        string baseKeyPath;
        RegistryKey baseKey;
        private const string WICDecoderCategory = "{7ED96837-96F0-4812-B211-F13C24117ED3}";
        private string[] allExtensions;
        private string[] nativeExtensions;
        private string[] customExtensions;
        #endregion

        #region constructors
        public BitmapImageCheck()
        {
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                baseKeyPath = "Wow6432Node\\CLSID";
            }
            else
            {
                baseKeyPath = "CLSID";
            }
            baseKey = Registry.ClassesRoot.OpenSubKey(baseKeyPath, false);
            recalculateExtensions();
        }
        #endregion

        #region properties
        /// <summary>
        /// File extensions that are supported by decoders found elsewhere on the system
        /// </summary>
        public string[] CustomSupportedExtensions
        {
            get
            {
                return customExtensions;
            }
        }

        /// <summary>
        /// File extensions that are supported natively by .NET
        /// </summary>
        public string[] NativeSupportedExtensions
        {
            get
            {
                return nativeExtensions;
            }
        }

        /// <summary>
        /// File extensions that are supported both natively by NET, and by decoders found elsewhere on the system
        /// </summary>
        public string[] AllSupportedExtensions
        {
            get
            {
                return allExtensions;
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Check whether a file is likely to be supported by BitmapImage based upon its extension
        /// </summary>
        /// <param name="extension">File extension (with leading full stop, e.g. ".jpg")</param>
        /// <returns>True if extension appears to contain a supported file extension, false if no suitable extension was found</returns>
        public bool IsExtensionSupported(string extension)
        {
            extension = extension.ToUpper();
            //extension = extension.Insert(0, ".");

            if (AllSupportedExtensions.Contains(extension)) return true;
            return false;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Re-calculate which extensions are available on this system. It's unlikely this ever needs to be called outside of the constructor.
        /// </summary>
        private void recalculateExtensions()
        {
            customExtensions = GetSupportedExtensions().ToArray();
            nativeExtensions = new string[] { ".BMP", ".GIF", ".ICO", ".JPEG", ".PNG", ".TIFF", ".DDS", ".JPG", ".JXR", ".HDP", ".WDP" };

            string[] cse = customExtensions;
            string[] nse = nativeExtensions;
            string[] ase = new string[cse.Length + nse.Length];
            Array.Copy(nse, ase, nse.Length);
            Array.Copy(cse, 0, ase, nse.Length, cse.Length);
            allExtensions = ase;
        }

        /// <summary>
        /// Represents information about a WIC decoder
        /// </summary>
        private struct DecoderInfo
        {
            public string FriendlyName;
            public string FileExtensions;
        }

        /// <summary>
        /// Gets a list of additionally registered WIC decoders
        /// </summary>
        /// <returns></returns>
        private IEnumerable<DecoderInfo> GetAdditionalDecoders()
        {
            var result = new List<DecoderInfo>();

            foreach (var codecKey in GetCodecKeys())
            {
                DecoderInfo decoderInfo = new DecoderInfo();
                decoderInfo.FriendlyName = Convert.ToString(codecKey.GetValue("FriendlyName", ""));
                decoderInfo.FileExtensions = Convert.ToString(codecKey.GetValue("FileExtensions", ""));
                result.Add(decoderInfo);
            }
            return result;
        }

        private List<string> GetSupportedExtensions()
        {
            var decoders = GetAdditionalDecoders();
            List<string> rtnlist = new List<string>();

            foreach (var decoder in decoders)
            {
                string[] extensions = decoder.FileExtensions.Split(',');
                foreach (var extension in extensions) rtnlist.Add(extension);
            }
            return rtnlist;
        }

        private IEnumerable<RegistryKey> GetCodecKeys()
        {
            var result = new List<RegistryKey>();

            if (baseKey != null)
            {
                var categoryKey = baseKey.OpenSubKey(WICDecoderCategory + "\\instance", false);
                if (categoryKey != null)
                {
                    // Read the guids of the registered decoders
                    var codecGuids = categoryKey.GetSubKeyNames();

                    foreach (var codecGuid in GetCodecGuids())
                    {
                        // Read the properties of the single registered decoder
                        var codecKey = baseKey.OpenSubKey(codecGuid);
                        if (codecKey != null)
                        {
                            result.Add(codecKey);
                        }
                    }
                }
            }

            return result;
        }

        private string[] GetCodecGuids()
        {
            if (baseKey != null)
            {
                var categoryKey = baseKey.OpenSubKey(WICDecoderCategory + "\\instance", false);
                if (categoryKey != null)
                {
                    // Read the guids of the registered decoders
                    return categoryKey.GetSubKeyNames();
                }
            }
            return null;
        }

        #endregion


        #region overrides and whatnot

        public override string ToString()
        {
            string rtnstring = "";

            rtnstring += "\nNative support for the following extensions is available: ";
            foreach (var item in nativeExtensions)
            {
                rtnstring += item + ",";
            }
            if (nativeExtensions.Count() > 0) rtnstring = rtnstring.Remove(rtnstring.Length - 1);

            var decoders = GetAdditionalDecoders();
            if (decoders.Count() == 0) rtnstring += "\n\nNo custom decoders found.";
            else
            {
                rtnstring += "\n\nThese custom decoders were also found:";
                foreach (var decoder in decoders)
                {
                    rtnstring += "\n" + decoder.FriendlyName + ", supporting extensions " + decoder.FileExtensions;
                }
            }

            return rtnstring;
        }

        public void Dispose()
        {
            baseKey.Dispose();
        }
        #endregion
    }
}