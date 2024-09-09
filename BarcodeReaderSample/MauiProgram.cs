using BarcodeReaderSample.DB;
using BarcodeReaderSample.View;
using BarcodeReaderSample.ViewModel;
using BarcodeReaderSample.ViewModels;
using BarcodeReaderSample.Views;
using IntelliJ.Lang.Annotations;
using Microsoft.Maui.Controls;

namespace BarcodeReaderSample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
             .RegisterServices()
                .RegisterViewModels()
                .RegisterViews()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("verdana_regular.ttf", "verdana_regular");
            });

		return builder.Build();
	}
    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {

        
        builder.Services.AddSingleton<DatabaseController>();
        builder.Services.AddSingleton<FileFontResolver>();
        return builder;
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<ScanViewModel>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<BaseViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        // More services registered here.

        return builder;
    }
    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<CreateUsers>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<Scan>();
        builder.Services.AddSingleton<Settings>();


        return builder;
    }
}
