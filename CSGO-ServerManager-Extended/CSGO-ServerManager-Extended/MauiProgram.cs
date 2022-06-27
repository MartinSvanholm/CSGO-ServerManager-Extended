using CSGO_ServerManager_Extended.Services.CsgoServerService;
using Microsoft.AspNetCore.Components.WebView.Maui;
using MudBlazor;
using MudBlazor.Services;
using System.Net.Http.Headers;
using System.Text;

namespace CSGO_ServerManager_Extended;

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
		#endif

		builder.Services.AddScoped(sp => InitializeClient());
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 5000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
        builder.Services.AddSingleton<ICsgoServerService, CsgoServerService>();

		return builder.Build();
	}

	private static HttpClient InitializeClient()
	{
		HttpClient client = new();

		client.DefaultRequestHeaders.Clear();
		client.BaseAddress = new Uri("https://dathost.net");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
			"Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"ms@hobrovikings.dk:hobrovikings1212")));

		return client;
    }
}