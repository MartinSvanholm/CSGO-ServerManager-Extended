using CSGO_ServerManager_Extended.Data.DataAccess;
using CSGO_ServerManager_Extended.Models;
using CSGO_ServerManager_Extended.Repositories.CsgoServerRepository;
using CSGO_ServerManager_Extended.Repositories.CsgoServerSettingsRepository;
using CSGO_ServerManager_Extended.Services.CsgoServerService;
using CSGO_ServerManager_Extended.Services.CsgoServerSettingsService;
using CSGO_ServerManager_Extended.Services.SettingsService;
using MudBlazor;
using MudBlazor.Services;
using System.Net.Http.Headers;
using System.Text;

namespace CSGO_ServerManager_Extended;

public static class MauiProgram
{
	static bool DathostAccountIsConnected = false;
	static DathostAccount dathostAccount;
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
        builder.Services.AddSingleton<IDataAccess, DataAccess>(x => new DataAccess(Path.Combine(FileSystem.AppDataDirectory, "ServerManagerDb.db")));
		builder.Services.AddSingleton<ICsgoServerService, CsgoServerService>();
		builder.Services.AddSingleton<ISettingsService>(x => new SettingsService(x.GetRequiredService<HttpClient>(), dathostAccount, DathostAccountIsConnected));
		builder.Services.AddSingleton<IServerSettingsRepository, ServerSettingsRepository>();
		builder.Services.AddSingleton<ICsgoServerRepository, CsgoServerRepository>();
		builder.Services.AddSingleton<IServerSettingsService, ServerSettingsService>();

		var sp = builder.Services.BuildServiceProvider();

        return builder.Build();
	}

	private static HttpClient InitializeClient()
	{
		HttpClient client = new();

		client.DefaultRequestHeaders.Clear();
		client.BaseAddress = new Uri("https://dathost.net");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

		try
		{
            dathostAccount = Task.Run(async () => await GetDathostAccount()).Result;
            DathostAccountIsConnected = true;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
				"Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{dathostAccount.Email}:{dathostAccount.Password}")));
        }
        catch (Exception)
		{
			dathostAccount = new("", "");
            DathostAccountIsConnected = false;
		}

		return client;
    }

	private static async Task<DathostAccount> GetDathostAccount()
	{
		try
		{
			return new DathostAccount(
				await SecureStorage.Default.GetAsync("Dathost_Email"),
				await SecureStorage.Default.GetAsync("Dathost_Password"));
		}
		catch (Exception)
		{
			throw;
		}
	}
}