using Microsoft.Extensions.Logging;
using Vaultify.Application.Interfaces;
using Vaultify.Application.Services;
using Vaultify.Infrastructure.Repositories;
using Vaultify.Infrastructure.Services;

namespace Vaultify;

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

        // Register services
        builder.Services.AddSingleton<IPasswordService, PasswordService>();
        builder.Services.AddSingleton<IPasswordRepository, PasswordRepository>();
        builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
        builder.Services.AddSingleton<IUserSettingsService, UserSettingsService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}