﻿using Microsoft.Extensions.DependencyInjection;
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

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

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

            PicPick.Helpers.EventAggregatorHelper.EventAggregator = Helpers.ApplicationService.Instance.EventAggregator;

            MainWindowViewModel vm = new MainWindowViewModel();
            MainWindow view = new MainWindow();
            view.DataContext = vm;
            view.Show();
        }
    }
}
