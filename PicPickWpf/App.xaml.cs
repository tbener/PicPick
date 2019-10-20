using Microsoft.Extensions.DependencyInjection;
using PicPick.Models;
using PicPick.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism.Events;
using System.IO;
using log4net;

//[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace PicPick
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //MainWindowViewModel vm;

        protected override void OnStartup(StartupEventArgs e)
        {
            
            log4net.GlobalContext.Properties["LogFileFolder"] = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PicPick");
            log4net.Config.XmlConfigurator.Configure();

            _log.Info("----------------------------------------");
            _log.Info("Starting...");

            base.OnStartup(e);

            PicPick.Helpers.EventAggregatorHelper.EventAggregator = Helpers.ApplicationService.Instance.EventAggregator;

            MainWindowViewModel vm = new MainWindowViewModel();
            MainWindow view = new MainWindow();
            view.Closing += (s, e1) => {
                if (vm.CurrentProject.IsDirty)
                {
                    var result = MessageBox.Show("Project is not saved. Would you like to save before exiting?", "PicPick", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Cancel)
                    {
                        e1.Cancel = true;
                        return;
                    }
                    if (result == MessageBoxResult.Yes)
                        ProjectLoader.Save();
                }
                vm.Dispose();
                vm = null;
            };
            
            view.DataContext = vm;
            view.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _log.Info("Finishing...");

            base.OnExit(e);

            _log.Info("----------------------------------------");
        }
    }
}
