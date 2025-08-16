using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace EnumaElish.Client.Desktop;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		//MudBlazor

		builder.Services.AddMudServices(options =>
		{
			options.SnackbarConfiguration.PreventDuplicates = true;
			options.SnackbarConfiguration.ShowCloseIcon = true;
			options.SnackbarConfiguration.VisibleStateDuration = 3000;
			options.SnackbarConfiguration.SnackbarVariant = MudBlazor.Variant.Filled;
		});


        return builder.Build();
	}
}
