using System.Windows;
using Aggravation.Shell.Interfaces;
using Aggravation.Infrastructure.Events;
using Microsoft.Practices.Prism.Events;

namespace Aggravation.Game.GameControls.Views
{
    public partial class GameControls : IGameControls
    {
        readonly IGameControlsViewModel _vm;

        public GameControls(IGameControlsViewModel vm, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this._vm = vm;
            this.DataContext = this._vm;
            this.Loaded += new RoutedEventHandler(GameControls_Loaded);
            eventAggregator.GetEvent<EnableShowInstructionsEvent>().Subscribe(EnableShowInstructionsEventHandler, ThreadOption.UIThread, true);
            eventAggregator.GetEvent<DisableShowInstructionsEvent>().Subscribe(DisableShowInstructionsEventHandler, ThreadOption.UIThread, true);
            eventAggregator.GetEvent<EnableResetEvent>().Subscribe(EnableResetEventHandler, ThreadOption.UIThread, true);
            eventAggregator.GetEvent<DisableResetEvent>().Subscribe(DisableResetEventHandler, ThreadOption.UIThread, true);
        }

        protected void GameControls_Loaded(object sender, RoutedEventArgs e)
        {
            this.btnInstructions.Click += new RoutedEventHandler(btnInstructions_Click);
            this.btnReset.Click += new RoutedEventHandler(btnReset_Click);
        }

        protected void btnReset_Click(object sender, RoutedEventArgs e)
        {
            _vm.ResetClicked();
        }

        protected void btnInstructions_Click(object sender, RoutedEventArgs e)
        {
            _vm.InstructionsClicked();
        }

        protected void EnableShowInstructionsEventHandler(EnableShowInstructionsData e)
        {
            this.btnInstructions.IsEnabled = true;
        }

        protected void DisableShowInstructionsEventHandler(DisableShowInstructionsData e)
        {
            this.btnInstructions.IsEnabled = false;
        }

        protected void EnableResetEventHandler(EnableResetData e)
        {
            this.btnReset.IsEnabled = true;
        }

        protected void DisableResetEventHandler(DisableResetData e)
        {
            this.btnReset.IsEnabled = false;
        }
    }
}
