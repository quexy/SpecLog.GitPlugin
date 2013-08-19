using System;
using System.Windows.Input;
using TechTalk.SpecLog.Application.Common;
using TechTalk.SpecLog.Application.Common.Dialogs;
using TechTalk.SpecLog.Common;

namespace SpecLog.GitPlugin.Client
{
    public class OfferResynchronizationViewModel : IDialogViewModel
    {
        private readonly ICommandExecutionService commandExecutionService;
        public OfferResynchronizationViewModel(ICommandExecutionService commandExecutionService)
        {
            this.commandExecutionService = commandExecutionService;

            AcceptCommand = new DelegateCommand(Accept);
            CancelCommand = new DelegateCommand(Cancel);
        }

        public string Caption
        {
            get { return "Resynchronize with Git"; }
        }

        public ICommand AcceptCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public event EventHandler<EventArgs<bool?>> Close = delegate { };
        private void RequestClose(bool? result)
        {
            Close(this, new EventArgs<bool?>(result));
        }

        public void Cancel()
        {
            RequestClose(false);
        }

        public void Accept()
        {
            commandExecutionService.IssueCommand(PluginCommands.SynchronizeGherkinFilesVerb);
            RequestClose(true);
        }

        public IDialogResult GetDialogResultData()
        {
            return null;
        }
    }
}
