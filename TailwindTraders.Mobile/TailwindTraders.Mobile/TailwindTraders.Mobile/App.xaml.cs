﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TailwindTraders.Mobile.Services;
using TailwindTraders.Mobile.Views;
using MonkeyCache.SQLite;

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

        protected override void OnStart()
        {
            // Handle when your app starts

            
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
