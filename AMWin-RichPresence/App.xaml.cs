﻿using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;

namespace AMWin_RichPresence {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        private TaskbarIcon? taskbarIcon;
        private AppleMusicScraper amScraper;
        private AppleMusicDiscordClient discordClient;
        
        public App() {

            // start Discord RPC
            discordClient = new(Constants.DiscordClientID, enabled: false, subtitleOptions: (AppleMusicDiscordClient.RPSubtitleDisplayOptions)AMWin_RichPresence.Properties.Settings.Default.RPSubtitleChoice);

            // start scraper
            amScraper = new(Constants.RefreshPeriod, (newInfo) => {
                // disable RPC when Apple Music is paused or not open
                if (newInfo != null && ((AppleMusicInfo)newInfo).HasSong) {
                    discordClient.Enable();
                    discordClient.SetPresence((AppleMusicInfo)newInfo);
                } else {
                    discordClient.Disable();
                }
            });
        }
        
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            taskbarIcon = (TaskbarIcon)FindResource("TaskbarIcon");
        }

        private void Application_Exit(object sender, ExitEventArgs e) {
            taskbarIcon?.Dispose();
            discordClient.Disable();
        }

        internal void UpdateRPSubtitleDisplay(AppleMusicDiscordClient.RPSubtitleDisplayOptions newVal) {
            discordClient.subtitleOptions = newVal;
        }
    }
}
