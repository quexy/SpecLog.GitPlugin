using TechTalk.SpecLog.Application.Common.Dialogs;
using TechTalk.SpecLog.Application.Common.PluginsInfrastructure;
using TechTalk.SpecLog.Common;
using TechTalk.SpecLog.Entities;

namespace SpecLog.GitPlugin.Client
{
    [Plugin(PluginName)]
    public class GitPlugin : IClientPlugin
    {
        public const string PluginName = "SpecLog.GitPlugin";
        public const string GitGherkinFileProviderType = PluginName;

        public string Name
        {
            get { return PluginName; }
        }

        public string Description
        {
            get { return "Gherkin file synchronisation provider for git repositories."; }
        }

        public bool IsConfigurable(RepositoryMode repositoryMode) { return repositoryMode == RepositoryMode.ClientServer; }
        public IDialogViewModel GetConfigDialog(RepositoryMode repositoryMode, bool isEnabled, string config)
        {
            return new GitPluginConfigurationDialogViewModel(config, isEnabled);
        }

        public bool IsGherkinLinkProvider(RepositoryMode repositoryMode) { return repositoryMode == RepositoryMode.ClientServer; }
        public IGherkinLinkProviderViewModel GetGherkinLinkViewModel(RepositoryMode repositoryMode)
        {
            return new GitPluginGherkinLinkProviderViewModel();
        }

        public bool IsGherkinStatsProvider(RepositoryMode repositoryMode) { return false; }
        public IGherkinStatsProvider CreateStatsProvider(RepositoryMode repositoryMode, string configuration, IGherkinStatsRepository statsRepository) { return null; }

        public System.Collections.Generic.IEnumerable<PluginCommand> GetSupportedCommands(RepositoryMode repositoryMode)
        {
            return new[]
            {
                new PluginCommand { CommandVerb = PluginCommands.SynchronizeGherkinFilesVerb, DisplayText = "Synchronize Gherkin Files" }
            };
        }
    }
}
