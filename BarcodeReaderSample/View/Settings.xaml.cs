using BarcodeReaderSample.ViewModel;

namespace BarcodeReaderSample.View;

public partial class Settings : ContentPage
{
	SettingsViewModel ViewModel;
	public Settings()
	{
		InitializeComponent();
		ViewModel = new SettingsViewModel();
		BindingContext = ViewModel;

    }
}