using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TailwindTraders.Mobile.Services;
using TailwindTraders.Mobile.Views;
using MonkeyCache.SQLite;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
using TailwindTraders.Mobile.Helpers;
using System.Collections.Generic;
using Microsoft.AppCenter.Push;
using Xamarin.Essentials;
using Plugin.Toasts;

namespace TailwindTraders.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            Barrel.ApplicationId = "tailwinddata";

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<ProductCategoryDataStore>();
            DependencyService.Register<WishlistDataStore>();

            MainPage = new AppShell();
        }

        protected async override void OnStart()
        {
            SetupPushNotifications();

            // Handle when your app starts
            AppCenter.Start($"ios={AppCenterConstants.iOSAppSecret};" +
                  $"android={AppCenterConstants.AndroidAppSecret}",
                  typeof(Analytics), typeof(Crashes), typeof(Push));
           
            // Check to see if app crashed during last run
            if (await Crashes.HasCrashedInLastSessionAsync())
            {
                var report = await Crashes.GetLastSessionCrashReportAsync();

                Crashes.TrackError(report.Exception, new Dictionary<string, string> { { "RecoverFromCrash", "true" } });
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private void SetupPushNotifications()
        {
            if (!AppCenter.Configured)
            {
                Push.PushNotificationReceived += (sender, e) =>
                {
                    // Add the notification message and title to the message
                    var summary = $"Push notification received:" +
                                        $"\n\tNotification title: {e.Title}" +
                                        $"\n\tMessage: {e.Message}";

                    // If there is custom data associated with the notification,
                    // print the entries
                    if (e.CustomData != null)
                    {
                        summary += "\n\tCustom data:\n";
                        foreach (var key in e.CustomData.Keys)
                        {
                            summary += $"\t\t{key} : {e.CustomData[key]}\n";
                        }
                    }

                    // Send the notification summary to debug output
                    System.Diagnostics.Debug.WriteLine(summary);
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {                        
                        var toast = new NotificationOptions
                        {
                            Title = "Message from Tailwind Traders",
                            Description = e.Message,
                            ClearFromHistory = true,
                            IsClickable = false
                        };

                        var notification = DependencyService.Get<IToastNotificator>();

                        await notification.Notify(toast);
                    });
                };
            }

        }
    }
}
