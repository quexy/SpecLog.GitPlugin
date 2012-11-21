using Microsoft.Practices.Unity;
using TechTalk.SpecLog.CommandBuilding;
using TechTalk.SpecLog.Common;
using TechTalk.SpecLog.Common.Commands;
using TechTalk.SpecLog.DataAccess.Repositories;

namespace SpecLog.GitPlugin.Server
{
    public class GitPluginContainerSetup : IPluginContainerSetup
    {
        public void Setup(IUnityContainer container)
        {
            container
                .RegisterType<IGherkinFileRepository, GherkinFileRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<ICommandBuilderFactory, CommandBuilderFactory>(new ContainerControlledLifetimeManager())
                .RegisterType<IEntityRepository, GenomeHelper>(new ContainerControlledLifetimeManager())
                .RegisterType<IGherkinFileGitPollingSynchronizerFactory, GherkinFileGitPollingSynchronizerFactory>(new ContainerControlledLifetimeManager())
            ;
        }
    }
}
