using System.Diagnostics;
using TechTalk.SpecLog.Commands;
using TechTalk.SpecLog.Common;
using TechTalk.SpecLog.Common.Commands;
using TechTalk.SpecLog.Entities;
using TechTalk.SpecLog.Server.Services.PluginInfrastructure;

namespace SpecLog.GitPlugin.Server
{
    [Plugin(PluginName, ContainerSetupType = typeof(GitPluginContainerSetup))]
    public class GitPlugin : ServerPlugin
    {
        public const string PluginName = "SpecLog.GitPlugin";
        public const string GitGherkinFileProviderType = PluginName;

        public GitGherkinLinkProvider GitGherkinLinkProvider { get; private set; }

        public override IGherkinLinkProvider GherkinLinkProvider
        {
            get { return GitGherkinLinkProvider; }
        }

        public GitPlugin(IGherkinFileGitPollingSynchronizerFactory gherkinFileGitPollingSynchronizerFactory, IEntityRepository entityRepository)
        {
            GitGherkinLinkProvider = new GitGherkinLinkProvider(entityRepository, gherkinFileGitPollingSynchronizerFactory, this);
        }

        public override void OnStart()
        {
            var config = GetConfiguration<GitPluginConfiguration>();
            GitGherkinLinkProvider.Start(config);
            Log(TraceEventType.Information, "The plugin '{0}' started successfully.", PluginName);
        }

        public override void OnStop()
        {
            GitGherkinLinkProvider.Stop();
            Log(TraceEventType.Information, "The plugin '{0}' stopped successfully.", PluginName);
        }

        public override void PerformCommand(string commandVerb)
        {
            base.PerformCommand(commandVerb);

            switch (commandVerb)
            {
                case PluginCommands.SynchronizeGherkinFilesVerb:
                    GitGherkinLinkProvider.GherkinFilePoller.TriggerUpdate();
                    break;
                default:
                    Log(TraceEventType.Warning, "The command '{0}' is not spported");
                    break;
            }
        }

        public override void BeforeApplyCommand(RepositoryInfo repository, Command command) { /* SKIP */ }

        public override void AfterApplyCommand(RepositoryInfo repository, Command command)
        {
            if (command.CommandName == CommandName.CreateGherkinLink)
            {
                GitGherkinLinkProvider.CreateGherkinLink(repository, command);
            }
        }

        public override void BeforeUndoCommand(RepositoryInfo repository, Command command) { /* SKIP */ }

        public override void AfterUndoCommand(RepositoryInfo repository, Command command) { /* SKIP */ }
    }
}
