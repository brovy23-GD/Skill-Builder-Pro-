using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

using SkillBuilderPro.Client.ApiClients;
using SkillBuilderPro.Client.Services;
using SkillBuilderPro.MAUI.ViewModels;
using SkillBuilderPro.MAUI.Views;

namespace SkillBuilderPro.MAUI;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            var baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:5001/"
                : "https://localhost:5001/";

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
#if DEBUG
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        })
#endif
        ;

        builder.Services.AddTransient<DrillApiClient>();
        //builder.Services.AddTransient<DrillsViewModel>();
        builder.Services.AddTransient<SportListPage>();
        builder.Services.AddTransient<CategoryListPage>();
        builder.Services.AddTransient<DrillListPage>();
        builder.Services.AddTransient<VideoPlayerPage>();
        builder.Services.AddSingleton<DrillsViewModel>();
        builder.Services.AddTransient<DrillLibraryPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}