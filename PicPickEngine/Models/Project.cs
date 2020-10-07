﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace PicPick.Models {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class PicPickProject : object, System.ComponentModel.INotifyPropertyChanged {
        
        private PicPickProjectActivity[] activitiesField;
        
        private PicPickProject_options _optionsField;
        
        private string verField;
        
        public PicPickProject() {
            this.verField = "2.0";
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Activity", IsNullable=false)]
        public PicPickProjectActivity[] Activities {
            get {
                return this.activitiesField;
            }
            set {
                this.activitiesField = value;
                this.RaisePropertyChanged("Activities");
            }
        }
        
        /// <remarks/>
        public PicPickProject_options _options {
            get {
                return this._optionsField;
            }
            set {
                this._optionsField = value;
                this.RaisePropertyChanged("_options");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("2.0")]
        public string ver {
            get {
                return this.verField;
            }
            set {
                this.verField = value;
                this.RaisePropertyChanged("ver");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class PicPickProjectActivity : object, System.ComponentModel.INotifyPropertyChanged {
        
        private PicPickProjectActivitySource sourceField;
        
        private PicPickProjectActivityDestination[] destinationField;
        
        private string nameField;
        
        private bool deleteSourceFilesField;
        
        private bool deleteSourceFilesOnSkipField;
        
        private bool activeField;
        
        public PicPickProjectActivity() {
            this.nameField = "My Pictures";
            this.deleteSourceFilesField = true;
            this.deleteSourceFilesOnSkipField = false;
            this.activeField = true;
        }
        
        /// <remarks/>
        public PicPickProjectActivitySource Source {
            get {
                return this.sourceField;
            }
            set {
                this.sourceField = value;
                this.RaisePropertyChanged("Source");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Destination")]
        public PicPickProjectActivityDestination[] Destination {
            get {
                return this.destinationField;
            }
            set {
                this.destinationField = value;
                this.RaisePropertyChanged("Destination");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("My Pictures")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
                this.RaisePropertyChanged("Name");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool DeleteSourceFiles {
            get {
                return this.deleteSourceFilesField;
            }
            set {
                this.deleteSourceFilesField = value;
                this.RaisePropertyChanged("DeleteSourceFiles");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool DeleteSourceFilesOnSkip {
            get {
                return this.deleteSourceFilesOnSkipField;
            }
            set {
                this.deleteSourceFilesOnSkipField = value;
                this.RaisePropertyChanged("DeleteSourceFilesOnSkip");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool Active {
            get {
                return this.activeField;
            }
            set {
                this.activeField = value;
                this.RaisePropertyChanged("Active");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class PicPickProjectActivitySource : object, System.ComponentModel.INotifyPropertyChanged {
        
        private DateComplex fromDateField;
        
        private DateComplex toDateField;
        
        private string pathField;
        
        private string filterField;
        
        private bool includeSubFoldersField;
        
        private bool onlyNewFilesField;
        
        public PicPickProjectActivitySource() {
            this.pathField = "";
            this.filterField = "*.jpg";
            this.includeSubFoldersField = false;
            this.onlyNewFilesField = false;
        }
        
        /// <remarks/>
        public DateComplex FromDate {
            get {
                return this.fromDateField;
            }
            set {
                this.fromDateField = value;
                this.RaisePropertyChanged("FromDate");
            }
        }
        
        /// <remarks/>
        public DateComplex ToDate {
            get {
                return this.toDateField;
            }
            set {
                this.toDateField = value;
                this.RaisePropertyChanged("ToDate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("")]
        public string Path {
            get {
                return this.pathField;
            }
            set {
                this.pathField = value;
                this.RaisePropertyChanged("Path");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("*.jpg")]
        public string Filter {
            get {
                return this.filterField;
            }
            set {
                this.filterField = value;
                this.RaisePropertyChanged("Filter");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool IncludeSubFolders {
            get {
                return this.includeSubFoldersField;
            }
            set {
                this.includeSubFoldersField = value;
                this.RaisePropertyChanged("IncludeSubFolders");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool OnlyNewFiles {
            get {
                return this.onlyNewFilesField;
            }
            set {
                this.onlyNewFilesField = value;
                this.RaisePropertyChanged("OnlyNewFiles");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DateComplex : object, System.ComponentModel.INotifyPropertyChanged {
        
        private System.DateTime dateField;
        
        private bool useField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="date")]
        public System.DateTime Date {
            get {
                return this.dateField;
            }
            set {
                this.dateField = value;
                this.RaisePropertyChanged("Date");
            }
        }
        
        /// <remarks/>
        public bool Use {
            get {
                return this.useField;
            }
            set {
                this.useField = value;
                this.RaisePropertyChanged("Use");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class PicPickProjectActivityDestination : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string pathField;
        
        private string templateField;
        
        private bool activeField;
        
        private bool keepAbsoluteField;
        
        public PicPickProjectActivityDestination() {
            this.pathField = "";
            this.templateField = "yyyy-MM";
            this.activeField = true;
            this.keepAbsoluteField = true;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("")]
        public string Path {
            get {
                return this.pathField;
            }
            set {
                this.pathField = value;
                this.RaisePropertyChanged("Path");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("yyyy-MM")]
        public string Template {
            get {
                return this.templateField;
            }
            set {
                this.templateField = value;
                this.RaisePropertyChanged("Template");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool Active {
            get {
                return this.activeField;
            }
            set {
                this.activeField = value;
                this.RaisePropertyChanged("Active");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool KeepAbsolute {
            get {
                return this.keepAbsoluteField;
            }
            set {
                this.keepAbsoluteField = value;
                this.RaisePropertyChanged("KeepAbsolute");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class PicPickProject_options : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string fileExistsResponseStringField;
        
        public PicPickProject_options() {
            this.fileExistsResponseStringField = "ASK";
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("ASK")]
        public string FileExistsResponseString {
            get {
                return this.fileExistsResponseStringField;
            }
            set {
                this.fileExistsResponseStringField = value;
                this.RaisePropertyChanged("FileExistsResponseString");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
