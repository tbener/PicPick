using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicPick.Models;

namespace PicPick
{
    public static class ContainerConfig
    {
        static ContainerBuilder _builder;
        static IContainer _container;

        static ContainerConfig()
        {
            _builder = new ContainerBuilder();
        }

        public static void RegisterInstances(PicPickProject project, PicPickProjectActivity activity)
        {
            _builder.RegisterInstance(project);
            _builder.RegisterInstance(activity);
            _container = _builder.Build();
        }

        public static IContainer Container
        {
            get => _container;
        }
    }
}
