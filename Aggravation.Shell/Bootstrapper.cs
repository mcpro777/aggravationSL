using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Aggravation.Infrastructure.Strings;
using Aggravation.Shell.Interfaces;
using Aggravation.Shell.ViewModels;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

namespace Aggravation.Shell
{
    public class Bootstrapper : UnityBootstrapper
    {
        readonly IDictionary<String, String> _initParameters;
        readonly String _catalog;

        public Bootstrapper(IDictionary<String, String> initParameters, String catalog)
        {
            this._initParameters = initParameters;
            this._catalog = catalog;
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IGameControlsViewModel, Aggravation.Game.GameControls.ViewModels.GameControlsViewModel>();

            Container.RegisterType<IShellViewModel, ShellViewModel>();
            Container.RegisterType<IShell, Views.Shell>();

            Container.RegisterInstance(ContainerItems.InitParams, this._initParameters, new ContainerControlledLifetimeManager());
        }

        protected override DependencyObject CreateShell()
        {
            var shell = Container.Resolve<IShell>();
            shell.Show();
            return (DependencyObject)shell;
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var catalogStream = new MemoryStream(Encoding.UTF8.GetBytes(_catalog));
            return Microsoft.Practices.Prism.Modularity.ModuleCatalog.CreateFromXaml(catalogStream);
        }
    }
}
