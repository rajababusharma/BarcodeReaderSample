using BarcodeReaderSample.DB;
using BarcodeReaderSample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeReaderSample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateUsers : ContentPage
    {
        DatabaseController database;
        public CreateUsers()
        {
            InitializeComponent();
            BindingContext=this;
            database = new DatabaseController();

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(username.Text) && string.IsNullOrWhiteSpace(password.Text) && string.IsNullOrWhiteSpace(confirmpassword.Text))
            {

                await DisplayAlert("User name and password are Required", "Please enter a username ans password.", "OK");
                return;
            }
            if (!password.Text.Equals(confirmpassword.Text))
            {
                await DisplayAlert("Passwords do not match", "Please enter a matching password.", "OK");
                return;
            }
            Users user = new Users { username = username.Text, password = password.Text };
            int status =await  database.Insert_IntoUsers(user);
            if (status == 0)
            {
                await DisplayAlert("Alert", "please try again", "OK");
            }
            else
            {
                await DisplayAlert("Alert", "User created successfully", "OK");
               // await Shell.Current.GoToAsync("..");
            }

        }
    }
}