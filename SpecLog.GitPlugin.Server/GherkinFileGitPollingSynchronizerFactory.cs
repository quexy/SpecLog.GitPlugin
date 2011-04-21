using TechTalk.SpecLog.CommandBuilding;
using TechTalk.SpecLog.DataAccess.Boundaries;
using TechTalk.SpecLog.DataAccess.Repositories;
using TechTalk.SpecLog.GherkinSynchronization;
using TechTalk.SpecLog.Logging;

namespace SpecLog.GitPlugin.Server
{
    public interface IGherkinFileGitPollingSynchronizerFactory
    {
        IGherkinFileSynchronizer CreateSynchronizer(IGherkinFilePollerConfiguration pollerConfiguration, IGitRepositoryGherkinFileProviderConfiguration providerConfiguration);
    }

    public class GherkinFileGitPollingSynchronizerFactory : IGherkinFileGitPollingSynchronizerFactory
    {
        private readonly IGherkinFileRepository gherkinFileRepository;
        private readonly ICommandBuilderFactory commandBuilderFactory;
        private readonly IBoundaryFactory boundaryFactory;
        private readonly IBoundary boundary;
        private readonly ILogger logger;
        public GherkinFileGitPollingSynchronizerFactory(IGherkinFileRepository gherkinFileRepository, ICommandBuilderFactory commandBuilderFactory, IBoundaryFactory boundaryFactory, IBoundary boundary, ILogger logger)
        {
            this.gherkinFileRepository = gherkinFileRepository;
            this.commandBuilderFactory = commandBuilderFactory;
            this.boundaryFactory = boundaryFactory;
            this.boundary = boundary;
            this.logger = logger;
        }

        public IGherkinFileSynchronizer CreateSynchronizer(IGherkinFilePollerConfiguration pollerConfiguration, IGitRepositoryGherkinFileProviderConfiguration providerConfiguration)
        {
            var gherkinFileProvider = new GitRepositoryGherkinFileProvider(logger, providerConfiguration);
            return new GherkinFilePollingSynchronizer
            (
                pollerConfiguration,
                gherkinFileProvider,
                gherkinFileRepository,
                commandBuilderFactory,
                boundaryFactory,
                boundary,
                logger
            );
        }
    }
}
