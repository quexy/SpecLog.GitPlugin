using TechTalk.SpecLog.Application.Common.Dialogs;
using TechTalk.SpecLog.Application.Common.PluginsInfrastructure;
using TechTalk.SpecLog.Common;
using TechTalk.SpecLog.Entities;

namespace SpecLog.GitPlugin.Client
{
    [Plugin(PluginName)]
    public class GitPlugin : IClientPlugin
    {
        public const string PluginName = "GitPlugin";

        public string Description
        {
            get { return "Gherkin file synchronisation provider for git repositories."; }
        }

        public IDialogViewModel GetConfigDialog(string config, bool isEnabled)
        {
            return new GitPluginConfigurationDialogViewModel(config, isEnabled);
        }

        public IGherkinLinkProviderViewModel GetGherkinLinkViewModel(RepositoryMode repositoryMode)
        {
            return new GitPluginGherkinLinkProviderViewModel();
        }

        public bool IsGherkinLinkProvider(RepositoryMode repositoryMode)
        {
            return repositoryMode == RepositoryMode.ClientServer;
        }
    }
}
