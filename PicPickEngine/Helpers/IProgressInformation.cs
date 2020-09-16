using PicPick.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace PicPick.Helpers
{
    public interface IProgressInformation : INotifyPropertyChanged
    {
        CancellationToken CancellationToken { get; set; }
        int CurrentOperationTotal { get; }
        Exception Exception { get; set; }
        string Header { get; set; }
        bool IsWorking { get; }
        int Maximum { get; set; }
        bool OperationCancelled { get; set; }
        IProgress<ProgressInformation> Progress { get; set; }
        string ProgressPercentsText { get; }
        Dictionary<FILE_STATUS, List<string>> Summary { get; set; }
        string Text { get; set; }
        int Value { get; set; }

        void Advance(int step = 1);
        void AdvanceWithCancellationToken(int step = 1);
        void Cancel();
        void Finished();
        void RenewToken();
        void Report();
        void Reset();
    }
}