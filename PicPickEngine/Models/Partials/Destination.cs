﻿using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models.IsDirtySupport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using TalUtils;

namespace PicPick.Models
{
    public partial class PicPickProjectActivityDestination
    {
        public event CopyEventHandler OnCopyStatusChanged;


        [XmlIgnore]
        [IsDirtyIgnore]
        public Dictionary<string, CopyFilesHandler> Mapping { get; set; }

        [XmlIgnore]
        public bool Move { get; set; }

        [XmlIgnore]
        public bool HasTemplate { get => !string.IsNullOrEmpty(Template); }

        [XmlIgnore]
        public string PathAbsolute { get; set; }

        public string GetTemplatePath(DateTime dt)
        {
            return HasTemplate ? dt.ToString(Template) : string.Empty;
        }

        //public string GetTemplatePath(string file)
        //{
        //    return HasTemplate ? GetTemplatePath(GetFileDate(file, true)) : string.Empty;
        //}

        //public DateTime GetFileDate(string file, bool usePicDateTaken)
        //{
        //    if (usePicDateTaken)
        //        return GetDateTaken(file);
        //    else
        //        return File.GetLastWriteTime(file);
        //}

        //public static DateTime GetDateTaken(string inFullPath)
        //{
        //    DateTime returnDateTime = DateTime.MinValue;
        //    FileStream picStream = null;
        //    try
        //    {
        //        picStream = new FileStream(inFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        BitmapSource bitSource = BitmapFrame.Create(picStream);
        //        BitmapMetadata metaData = (BitmapMetadata)bitSource.Metadata;
        //        returnDateTime = DateTime.Parse(metaData.DateTaken);

        //        //JpegBitmapDecoder decoder = new JpegBitmapDecoder(picStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
        //        //BitmapMetadata metaData = new BitmapMetadata("jpg");
        //        //BitmapFrame frame = BitmapFrame.Create(decoder.Frames[0]);

        //    }
        //    catch(Exception ex)
        //    {
        //        Debug.Print($"{System.IO.Path.GetFileName(inFullPath)} - {ex.Message}");
        //        returnDateTime = File.GetLastWriteTime(inFullPath);
        //    }
        //    finally
        //    {
        //        picStream?.Close();
        //    }
        //    return returnDateTime;
        //}

        public string GetFullPath(DateTime dt)
        {
            return PathHelper.GetFullPath(Path, GetTemplatePath(dt), false);
        }

        internal string GetPath(string path, bool buildPath = false)
        {
            return PathHelper.GetFullPath(PathAbsolute, path, buildPath);
        }

        public void Execute()
        {
            foreach (var kv in Mapping)
            {
                CopyEventArgs e = new CopyEventArgs(kv.Value);

                // get the full path and CREATE it if not exists
                string fullPath = GetPath(kv.Key, true);
                CopyFilesHandler map = kv.Value;
                map.SetStart();
                OnCopyStatusChanged?.Invoke(this, e);

                try
                {

                    // The operation is done on a banch of files at once!
                    if (Move)
                    {
                        Debug.Print("Moving {0} files to {1}", map.FileList.Count(), fullPath);
                        ShellFileOperation.MoveItems(map.FileList, fullPath);
                    }
                    else
                    {
                        Debug.Print("Copying {0} files to {1}", map.FileList.Count(), fullPath);
                        ShellFileOperation.CopyItems(map.FileList, fullPath);
                    }
                    map.SetFinished();
                }
                catch (Exception ex)
                {
                    map.SetError(ex);
                    throw;
                }

                OnCopyStatusChanged?.Invoke(this, e);
            }
        }


    }


}