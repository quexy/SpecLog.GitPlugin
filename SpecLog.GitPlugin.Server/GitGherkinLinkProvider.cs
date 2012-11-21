using System;
using System.Diagnostics;
using TechTalk.Genome;
using TechTalk.SpecLog.Commands;
using TechTalk.SpecLog.Common.Commands;
using TechTalk.SpecLog.DataAccess.Boundaries;
using TechTalk.SpecLog.Entities;
using TechTalk.SpecLog.GherkinSynchronization;
using TechTalk.SpecLog.Logging;
using TechTalk.SpecLog.Server.Services.PluginInfrastructure;

namespace SpecLog.GitPlugin.Server
{
    public class GitGherkinLinkProvider : IGherkinLinkProvider
    {
        private readonly IEntityRepository entityRepository;
        private readonly IGherkinFileGitPollingSynchronizerFactory gherkinFileGitPollingSynchronizerFactory;
        private readonly ILogger logger;
        public IGherkinFileSynchronizer GherkinFilePoller { get; private set; }

        public string ProviderType
        {
            get { return GitPlugin.GitGherkinFileProviderType; }
        }

        public string ProviderDisplayName
        {
            get { return "Git Repository"; }
        }

        public GitGherkinLinkProvider(IEntityRepository entityRepository, IGherkinFileGitPollingSynchronizerFactory gherkinFileGitPollingSynchronizerFactory, ILogger logger)
        {
            this.entityRepository = entityRepository;
            this.gherkinFileGitPollingSynchronizerFactory = gherkinFileGitPollingSynchronizerFactory;
            this.logger = logger;
        }

        internal void Start(GitPluginConfiguration config)
        {
            GherkinFilePoller = gherkinFileGitPollingSynchronizerFactory.CreateSynchronizer(config, config);
            GherkinFilePoller.Start();
        }

        internal void Stop()
        {
            GherkinFilePoller.Stop();
            GherkinFilePoller = null;
        }

        internal void CreateGherkinLink(RepositoryInfo repositoryInfo, Command command)
        {
            var args = (AppendToCollectionCommandArgs)command.CommandArgs;
            var gherkinFile = (GherkinFile)entityRepository.Load(repositoryInfo, args.CollectionMember);
            if (gherkinFile.FileProviderType != ProviderType)
                return; // this is not for us

            var updateBoundary = new Boundary(Context.Current);
            try
            {
                GherkinFilePoller.TriggerUpdate(gherkinFile, updateBoundary);
            }
            catch (Exception ex)
            {
                logger.Log(TraceEventType.Error, "An exception has occurred during updating a gherkin file link: {0}", ex);
            }
        }
    }
}