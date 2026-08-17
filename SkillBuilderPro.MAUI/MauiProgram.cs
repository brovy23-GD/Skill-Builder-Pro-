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
using SkillBuilderPro.MAUI.Services;

namespace SkillBuilderPro.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var apiBaseAddress = ApiEndpointResolver.Resolve();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = apiBaseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
        })
#if DEBUG
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        })
#endif
        ;
        builder.Services.AddHttpClient("AthleteApi", client =>
        {
            client.BaseAddress = apiBaseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
        })
#if DEBUG
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator })
#endif
        ;
        // Authentication and Demo Mode are application-session state. Pages must
        // share one service instance rather than receiving a fresh typed client.
        builder.Services.AddSingleton<IAthleteApiService>(services =>
            new AthleteApiService(
                services.GetRequiredService<IHttpClientFactory>().CreateClient("AthleteApi")));

        builder.Services.AddTransient<DrillApiClient>();
        builder.Services.AddSingleton<ISportVisualService, SportVisualService>();
        //builder.Services.AddTransient<DrillsViewModel>();
        builder.Services.AddTransient<SportListPage>();
        builder.Services.AddTransient<CategoryListPage>();
        builder.Services.AddTransient<DrillListPage>();
        builder.Services.AddTransient<VideoPlayerPage>();
        builder.Services.AddSingleton<DrillsViewModel>();
        builder.Services.AddTransient<DrillLibraryPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<AthleteDashboardPage>(); builder.Services.AddTransient<GoalsPage>(); builder.Services.AddTransient<TrophyRoomPage>(); builder.Services.AddTransient<TrainingPage>(); builder.Services.AddTransient<TrainingRequestsPage>(); builder.Services.AddTransient<NotificationsPage>(); builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<DashboardViewModel>(); builder.Services.AddTransient<GoalsViewModel>(); builder.Services.AddTransient<TrophyViewModel>(); builder.Services.AddTransient<TrainingViewModel>(); builder.Services.AddTransient<RequestsViewModel>(); builder.Services.AddTransient<NotificationsViewModel>(); builder.Services.AddTransient<ProfileViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
