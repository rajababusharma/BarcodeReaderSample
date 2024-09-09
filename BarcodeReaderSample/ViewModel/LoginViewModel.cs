using BarcodeReaderSample.DB;
using BarcodeReaderSample.Models;
using BarcodeReaderSample.View;
using BarcodeReaderSample.Views;
using System;
using System.Collections.Generic;
using System.Text;


namespace BarcodeReaderSample.ViewModels
{
    
    public class LoginViewModel : BaseViewModel
    {
        DatabaseController database;
        public Command LOGIN { get; set; }
        public Command CreateUsers { get; set; }

        public string AppVersion { get; set; }

        public LoginViewModel()
        {
            database = new DatabaseController();
            string appVersion = AppInfo.VersionString;
            AppVersion = "App Version: " + appVersion;

            LOGIN = new Command(() => 
            {
                Login();
            });

            CreateUsers = new Command(async() => 
            {
                await App.Current.MainPage.Navigation.PushAsync(new CreateUsers());
            });
        }

        private string _username;
        public string USERNAME
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                NotifyPropertyChanged("USERNAME");
            }
        }

        private string _password;
        public string PASSWORD
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                NotifyPropertyChanged("PASSWORD");
            }
        }

        private async void Login()
        {
            Users user = new Users { username = USERNAME, password = PASSWORD};
            if (!string.IsNullOrWhiteSpace(USERNAME) && string.IsNullOrWhiteSpace(PASSWORD))
            {
                await App.Current.MainPage.DisplayAlert("User name and password are Required", "Please enter a username ans password.", "OK");
                return;
            }
            int result = await database.Authenticate(user);
            if (result != 1)
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Invalid credentials, please try again", "OK");
            }
            else
            {
                Preferences.Set(Constants.username, USERNAME);
                Preferences.Set(Constants.password, PASSWORD);
               // await App.Current.MainPage.DisplayAlert("Alert", "Success", "OK");
                // await Shell.Current.GoToAsync(nameof(MainPage));
                 await App.Current.MainPage.Navigation.PushAsync(new Scan());
            }
        }
    }
}
