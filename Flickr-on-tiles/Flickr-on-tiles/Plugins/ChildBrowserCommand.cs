﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using System.Runtime.Serialization;
using WP7CordovaClassLib.Cordova;
using WP7CordovaClassLib.Cordova.Commands;
using WP7CordovaClassLib.Cordova.JSON;
using Microsoft.Phone.Shell;
using Flickr_on_tiles;
using Microsoft.Phone.Tasks;

namespace WP7CordovaClassLib.Cordova.Commands
{
    [DataContract]
    public class BrowserOptions
    {
        [DataMember]
        public string url;

        [DataMember]
        public bool isGeolocationEnabled;
    }

    public class ChildBrowserCommand : BaseCommand
    {

        private static WebBrowser browser;
        private static ApplicationBarIconButton backButton;
        private static ApplicationBarIconButton fwdButton;

        // Display an inderminate progress indicator
        public void showWebPage(string options)
        {
            BrowserOptions opts = JSON.JsonHelper.Deserialize<BrowserOptions>(options);

            Uri loc = new Uri(opts.url);

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (browser != null)
                {
                    browser.IsGeolocationEnabled = opts.isGeolocationEnabled;
                    browser.Navigate(loc);
                }
                else
                {
                    PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
                    if (frame != null)
                    {
                        PhoneApplicationPage page = frame.Content as PhoneApplicationPage;
                        if (page != null)
                        {
                            Grid grid = page.FindName("LayoutRoot") as Grid;
                            if (grid != null)
                            {
                                browser = new WebBrowser();
                                browser.Navigate(loc);

                                browser.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(browser_LoadCompleted);

                                browser.Navigating += new EventHandler<NavigatingEventArgs>(browser_Navigating);
                                browser.NavigationFailed += new System.Windows.Navigation.NavigationFailedEventHandler(browser_NavigationFailed);
                                browser.Navigated += new EventHandler<System.Windows.Navigation.NavigationEventArgs>(browser_Navigated);
                                browser.IsScriptEnabled = true;
                                browser.IsGeolocationEnabled = opts.isGeolocationEnabled;
                                grid.Children.Add(browser);
                            }

                            ApplicationBar bar = new ApplicationBar();
                            if ((Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible)
                            {
                                bar.BackgroundColor = Colors.Black;
                            }
                            else
                            {
                                bar.BackgroundColor = Colors.White;
                            }
                            //bar.BackgroundColor = Colors.Black;
                            //bar.BackgroundColor = Colors.White;
                            bar.IsMenuEnabled = false;

                            backButton = new ApplicationBarIconButton();
                            backButton.Text = "Back";
							if( (Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ) 
							{
								backButton.IconUri = new Uri("/Images/dark/appbar.back.rest.png", UriKind.Relative);
							} 
							else 
							{
								backButton.IconUri = new Uri("/Images/light/appbar.back.rest.png", UriKind.Relative);
							}
                            //backButton.IconUri = new Uri("/Images/appbar.back.rest.png", UriKind.Relative);
                            backButton.Click += new EventHandler(backButton_Click);
                            backButton.IsEnabled = false;
                            bar.Buttons.Add(backButton);


                            fwdButton = new ApplicationBarIconButton();
                            fwdButton.Text = "Forward";
							if( (Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ) 
							{
								fwdButton.IconUri = new Uri("/Images/dark/appbar.next.rest.png", UriKind.Relative);
							} 
							else 
							{
								fwdButton.IconUri = new Uri("/Images/light/appbar.next.rest.png", UriKind.Relative);
							}
                            //fwdButton.IconUri = new Uri("/Images/appbar.next.rest.png", UriKind.Relative);
                            fwdButton.Click += new EventHandler(fwdButton_Click);
                            fwdButton.IsEnabled = false;
                            bar.Buttons.Add(fwdButton);

                            ApplicationBarIconButton closeBtn = new ApplicationBarIconButton();
                            closeBtn.Text = "Close";
							if( (Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ) 
							{
								closeBtn.IconUri = new Uri("/Images/dark/appbar.close.rest.png", UriKind.Relative);
							} 
							else 
							{
								closeBtn.IconUri = new Uri("/Images/light/appbar.close.rest.png", UriKind.Relative);
							}
                            //closeBtn.IconUri = new Uri("/Images/appbar.close.rest.png", UriKind.Relative);
                            closeBtn.Click += new EventHandler(closeBtn_Click);
                            bar.Buttons.Add(closeBtn);

                            page.ApplicationBar = bar;
                        }

                    }
                }
            });
        }

        void browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            
        }

        void fwdButton_Click(object sender, EventArgs e)
        {
            if (browser != null)
            {
                try
                {
                    browser.InvokeScript("execScript", "history.forward();");
                }
                catch(Exception)
                {

                }
            }
        }

        void backButton_Click(object sender, EventArgs e)
        {
            if (browser != null)
            {
                try
                {
                    browser.InvokeScript("execScript", "history.back();");
                }
                catch (Exception)
                {

                }
            }
        }

        void closeBtn_Click(object sender, EventArgs e)
        {
            this.close();
        }


        public void close(string options="")
        {
            if (browser != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
                    if (frame != null)
                    {
                        PhoneApplicationPage page = frame.Content as PhoneApplicationPage;
                        if (page != null)
                        {
                            Grid grid = page.FindName("LayoutRoot") as Grid;
                            if (grid != null)
                            {
                                grid.Children.Remove(browser);
                            }
                            //page.ApplicationBar = null;
                            ApplicationBar appbar = new ApplicationBar();
                            appbar.Opacity = 1;
                            appbar.IsVisible = true;
                            appbar.IsMenuEnabled = true;
                            /*Mail AppBar*/
                            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
                            if ((Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible)
                            {
                                button1.IconUri = new Uri("/Images/dark/appbar.feature.email.rest.png", UriKind.Relative);
                            }
                            else
                            {
                                button1.IconUri = new Uri("/Images/light/appbar.feature.email.rest.png", UriKind.Relative);
                            }
                            button1.Text = "Mail";
                            appbar.Buttons.Add(button1);
                            button1.Click += new EventHandler(email_Click);

                            /*Facebook Appbar*/
                            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
                            if ((Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible)
                            {
                                button2.IconUri = new Uri("/Images/dark/appbar.share.rest.png", UriKind.Relative);
                            }
                            else
                            {
                                button2.IconUri = new Uri("/Images/light/appbar.share.rest.png", UriKind.Relative);
                            }
                            button2.Text = "Share";
                            appbar.Buttons.Add(button2);
                            button2.Click += new EventHandler(fb_Click);

                            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
                            menuItem1.Text = "Share this app with your Friends";
                            appbar.MenuItems.Add(menuItem1);
                            page.ApplicationBar = appbar;
                        }
                    }
                    browser = null;
                });
            }
        }

        void browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            string message = "{\"type\":\"locationChanged\", \"location\":\"" + e.Uri.AbsoluteUri + "\"}";
            PluginResult result = new PluginResult(PluginResult.Status.OK, message);
            result.KeepCallback = true;
            this.DispatchCommandResult(result);
        }

        void browser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            string message = "{\"type\":\"navigationError\",\"location\":\"" + e.Uri.AbsoluteUri + "\"}";
            PluginResult result = new PluginResult(PluginResult.Status.ERROR, message);
            result.KeepCallback = true;
            this.DispatchCommandResult(result);
        }

        void browser_Navigating(object sender, NavigatingEventArgs e)
        {
            string message = "{\"type\":\"locationAboutToChange\",\"location\":\"" + e.Uri.AbsoluteUri + "\"}";
            PluginResult result = new PluginResult(PluginResult.Status.OK, message);
            result.KeepCallback = true;
            this.DispatchCommandResult(result);
        }
        private void email_Click(object sender, EventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.Subject = "Have you checked this WP App: Flickr-On-Tiles??";
            task.Body = "Hey check out this great WP App named Flickr-On-Tiles. You can find it on http://www.windowsphone.com/s?appid=ece71322-d6c0-42bc-92b3-04e4bc55a2d3";
            task.Show();
        }

        private void fb_Click(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.LinkUri = new Uri("http://www.windowsphone.com/s?appid=ece71322-d6c0-42bc-92b3-04e4bc55a2d3", UriKind.Absolute);
            shareLinkTask.Message = "Have you checked out this WP App: Flickr-On-Tiles??";
            shareLinkTask.Show();
        }
    }
}
