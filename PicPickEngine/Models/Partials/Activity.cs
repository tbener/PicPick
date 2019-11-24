﻿using PicPick.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TalUtils;
using PicPick.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using PicPick.Core;
using PicPick.Models.Interfaces;

namespace PicPick.Models
{

    /// <summary>
    /// This class extends PicPickProjectActivity and includes:
    /// 1. Extension properties
    /// 2. Execution methods
    /// </summary>
    public partial class PicPickProjectActivity : ICloneable, IActivity
    {

        public event CopyEventHandler OnCopyStatusChanged;

        private ObservableCollection<PicPickProjectActivityDestination> _destinationList = null;

        private Dictionary<string, CopyFilesHandler> _mapping = new Dictionary<string, CopyFilesHandler>();
        private Dictionary<string, PicPickFileInfo> _dicFiles = new Dictionary<string, PicPickFileInfo>();
        private List<string> _errorFiles = new List<string>();
        private bool _isRunning;
        private ActivityFileMapping _fileMapping;
        private Runner _runner;

        public PicPickProjectActivity(string name)
        {
            Name = name;
            Source = new PicPickProjectActivitySource();
        }



        /// <summary>
        /// Use this list rather than the Destination Array for easyer manipulations and editing.
        /// This will be converted back to the Destination Array in ConfigurationHelper.Save()
        /// </summary>
        [XmlIgnore]
        public ObservableCollection<PicPickProjectActivityDestination> DestinationList
        {
            get
            {
                if (_destinationList == null)
                {
                    if (this.Destination == null)
                        this.Destination = new PicPickProjectActivityDestination[0];
                    _destinationList = new ObservableCollection<PicPickProjectActivityDestination>();

                    foreach (PicPickProjectActivityDestination dest in this.Destination)
                    {
                        _destinationList.Add(dest);
                    }
                }
                return _destinationList;
            }
        }



        [XmlIgnore]
        public bool Initialized { get; set; }

        [XmlIgnore]
        public FileExistsResponseEnum FileExistsResponse { get; set; }

        [XmlIgnore]
        public Dictionary<string, CopyFilesHandler> Mapping { get; set; }

        [XmlIgnore]
        [IsDirtySupport.IsDirtyIgnore]
        public Dictionary<string, PicPickFileInfo> FilesInfo { get; set; }

        [XmlIgnore]
        [IsDirtySupport.IsDirtyIgnore]
        public ActivityFileMapping FileMapping
        {
            get
            {
                if (_fileMapping == null)
                    _fileMapping = new ActivityFileMapping(this);
                return _fileMapping;
            }
            set => _fileMapping = value;
        }

        [XmlIgnore]
        [IsDirtySupport.IsDirtyIgnore]
        public Runner Runner
        {
            get
            {
                if (_runner == null)
                    _runner = new Runner(this, ProjectLoader.Project.Options);
                return _runner;
            }
            set => _runner = value;
        }

        [XmlIgnore]
        public bool IsRunning
        {
            get => _isRunning; set
            {
                _isRunning = value;
                RaisePropertyChanged("IsRunning");
            }
        }

        #region ICloneable

        public object Clone()
        {
            PicPickProjectActivity newTask = new PicPickProjectActivity()
            {
                Name = Name,
                DeleteSourceFiles = DeleteSourceFiles
            };
            if (Source != null)
                newTask.Source = new PicPickProjectActivitySource()
                {
                    Path = Source.Path,
                    Filter = Source.Filter
                };

            if (Destination != null)
                newTask.Destination = (PicPickProjectActivityDestination[])Destination.Clone();

            return newTask;

        }

        #endregion
    }

}
