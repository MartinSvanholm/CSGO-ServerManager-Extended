using CSGO_ServerManager_Extended.Services.CsgoServerService;
using Microsoft.AspNetCore.Components.WebView.Maui;
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
        builder.Services.AddMudServices();
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