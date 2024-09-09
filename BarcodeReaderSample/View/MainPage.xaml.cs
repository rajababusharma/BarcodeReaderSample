using BarcodeReaderSample.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BarcodeReaderSample
{
    public partial class MainPage : ContentPage
    {
        LoginViewModel viewModel;
        public MainPage()
        {
            InitializeComponent();
            viewModel = new LoginViewModel();
            BindingContext = viewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            PermissionStatus status1 = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            if (status1 != PermissionStatus.Granted)
            {
                status1 = await Permissions.RequestAsync<Permissions.StorageRead>();
            }
        }
    }
}
