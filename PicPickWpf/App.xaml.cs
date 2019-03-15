using Microsoft.Extensions.DependencyInjection;
using PicPick.Project;
using PicPick.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PicPick
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindowViewModel vm = new MainWindowViewModel();
            MainWindow view = new MainWindow();
            view.DataContext = vm;
            view.Show();
        }
    }
}
