using Aggravation.Infrastructure.Events;
using Aggravation.Infrastructure.Models;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Aggravation.Shell.Interfaces;

namespace Aggravation.Game.GameControls.ViewModels
{
    public class GameControlsViewModel : ViewModelBase, IGameControlsViewModel
    {
        public GameControlsViewModel(IUnityContainer container, IEventAggregator eventAggregator) : base(container, eventAggregator)
        {
        }

        public void InstructionsClicked()
        {
            EventAggregator.GetEvent<ShowInstructionsEvent>().Publish(new ShowInstructionsData());
        }

        public void ResetClicked()
        {
            EventAggregator.GetEvent<ResetEvent>().Publish(new ResetData());
        }
    }
}
