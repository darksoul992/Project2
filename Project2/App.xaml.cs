﻿namespace Project2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            //var TabbedPage = new MyTabbedPage();
            //MainPage = TabbedPage;
        }
    }
}