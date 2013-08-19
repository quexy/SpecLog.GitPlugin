using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using TechTalk.SpecLog.Application.Common.Dialogs;
using TechTalk.SpecLog.Application.Common.PluginsInfrastructure;
using TechTalk.SpecLog.Common;
using System.ComponentModel;
using System.Windows;

namespace SpecLog.GitPlugin.Client
{
    public class GitPluginConfigurationDialogViewModel : PluginConfigurationDialogViewModel<GitPluginConfiguration>, INotifyPropertyChanged
    {
        public readonly IDialogService dialogService;
        public GitPluginConfigurationDialogViewModel(IDialogService dialogService, string config, bool enabled)
            : base(config, enabled)
        {
            this.dialogService = dialogService;
            ClearUserCommand = new DelegateCommand(ClearUser);
            ChangeUserCommand = new DelegateCommand(ChangeUser);
        }

        public override string Caption
        {
            get { return "Git plugin configuration"; }
        }

        public string RemotePath
        {
            get { return configuration.RemoteRepository; }
            set { configuration.RemoteRepository = Trim(value); SaveCommand.RaiseCanExecuteChanged(); }
        }

        public string LocalPath
        {
            get { return configuration.LocalRepository; }
            set { configuration.LocalRepository = Trim(value); SaveCommand.RaiseCanExecuteChanged(); }
        }

        public string Branch
        {
            get { return configuration.Branch; }
            set { configuration.Branch = Trim(value); SaveCommand.RaiseCanExecuteChanged(); }
        }

        public int UpdateIntervalMinutes
        {
            get { return configuration.UpdateIntervalMinutes; }
            set { configuration.UpdateIntervalMinutes = value; SaveCommand.RaiseCanExecuteChanged(); }
        }

        public string ConfiguredUser
        {
            get { return configuration.Username; }
            set { configuration.Username = Trim(value); SaveCommand.RaiseCanExecuteChanged(); }
        }

        public string DisplayedUser
        {
            get
            {
                if (string.IsNullOrEmpty(configuration.Username))
                    return "no user";
                else return ConfiguredUser;
            }
        }

        public DelegateCommand ClearUserCommand { get; private set; }
        public void ClearUser()
        {
            ConfiguredUser = null;
            sensitiveConfig.Remove(GitPluginConfiguration.PasswordKey);
            NotifyPropertyChanged("DisplayedUser");
            NotifyPropertyChanged("ClearVisibility");
        }

        public Visibility ClearVisibility
        {
            get { return string.IsNullOrWhiteSpace(ConfiguredUser) ? Visibility.Collapsed : Visibility.Visible; }
        }

        public DelegateCommand ChangeUserCommand { get; private set; }
        public void ChangeUser()
        {
            var result = dialogService.ShowDialog(new ChangeUserDialogViewModel(ConfiguredUser)) as ChangeUserDialogResult;
            if (result != null)
            {
                ConfiguredUser = result.UserName;
                sensitiveConfig[GitPluginConfiguration.PasswordKey] = result.Password;
                NotifyPropertyChanged("DisplayedUser");
                NotifyPropertyChanged("ClearVisibility");
            }
        }

        public override bool CanSave()
        {
            return !IsEnabled
                || (!string.IsNullOrWhiteSpace(RemotePath)
                && !string.IsNullOrWhiteSpace(LocalPath)
                && UpdateIntervalMinutes > 0);
        }
    }
}
