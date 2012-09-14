using System;
using System.Diagnostics;
using TechTalk.Genome;
using TechTalk.SpecLog.Commands;
using TechTalk.SpecLog.Common;
using TechTalk.SpecLog.Common.Commands;
using TechTalk.SpecLog.DataAccess.Boundaries;
using TechTalk.SpecLog.Entities;
using TechTalk.SpecLog.GherkinSynchronization;
using TechTalk.SpecLog.Server.Services.PluginInfrastructure;

namespace SpecLog.GitPlugin.Server
{
    [Plugin(PluginName, ContainerSetupType = typeof(GitPluginContainerSetup))]
    public class GitPlugin : ServerPlugin
    {
        public const string PluginName = "GitPlugin";

        private readonly IGherkinFileGitPollingSynchronizerFactory gherkinFileGitPollingSynchronizerFactory;
        private readonly IEntityRepository entityRepository;
        public GitPlugin(IGherkinFileGitPollingSynchronizerFactory gherkinFileGitPollingSynchronizerFactory, IEntityRepository entityRepository)
        {
            this.gherkinFileGitPollingSynchronizerFactory = gherkinFileGitPollingSynchronizerFactory;
            this.entityRepository = entityRepository;
        }

        private IGherkinFileSynchronizer gherkinFileSynchronizer;
        public override void OnStart()
        {
            var config = GetConfiguration<GitPluginConfiguration>();
            gherkinFileSynchronizer = gherkinFileGitPollingSynchronizerFactory.CreateSynchronizer(config, config);
            gherkinFileSynchronizer.Start();
            Log(TraceEventType.Information, "The plugin '{0}' started successfully.", PluginName);
        }

        public override void OnStop()
        {
            gherkinFileSynchronizer.Stop();
            gherkinFileSynchronizer = null;
            Log(TraceEventType.Information, "The plugin '{0}' stopped successfully.", PluginName);
        }

        public override void BeforeApplyCommand(RepositoryInfo repository, Command command) { /* SKIP */ }

        public override void AfterApplyCommand(RepositoryInfo repository, Command command)
        {
            if (command.CommandName == CommandName.CreateGherkinLink)
            {
                var args = (AppendToCollectionCommandArgs)command.CommandArgs;
                var gherkinFile = (GherkinFile)entityRepository.Load(repository, args.CollectionMember);

                var updateBoundary = new Boundary(Context.Current);
                try
                {
                    gherkinFileSynchronizer.TriggerUpdate(gherkinFile, updateBoundary);
                }
                catch (Exception ex)
                {
                    Log(TraceEventType.Error, "An exception has occurred during updating a gherkin file link: {0}", ex);
                }
            }
        }

        public override void BeforeUndoCommand(RepositoryInfo repository, Command command) { /* SKIP */ }

        public override void AfterUndoCommand(RepositoryInfo repository, Command command) { /* SKIP */ }
    }
}
