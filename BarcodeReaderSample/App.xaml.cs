namespace BarcodeReaderSample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		NavigationPage navigationPage = new NavigationPage(new MainPage());
/*		navigationPage.BarBackgroundColor = Colors.Maroon;
		navigationPage.BarTextColor = Colors.White;*/
        MainPage = navigationPage;
	}
}
