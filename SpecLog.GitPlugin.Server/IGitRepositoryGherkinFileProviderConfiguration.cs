using System;

namespace SpecLog.GitPlugin.Server
{
    public interface IGitRepositoryGherkinFileProviderConfiguration
    {
        string RemoteRepository { get; }
        string LocalRepository { get; }
        string Branch { get; }

        TimeSpan UpdateInterval { get; }

        string Username { get; }
        string Password { get; }
    }
}
