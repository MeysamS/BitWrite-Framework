using Bw.Email.Options;
using Bw.Extensions.Microsoft.DependencyInjection.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bw.Email;

public static class Extensions
{
    public static IServiceCollection AddEmailService(
        this IServiceCollection services,
        IConfiguration configuration,
        EmailProvider provider = EmailProvider.MimKit,
        Action<EmailOptions>? configureOptions = null
    )
    {
        var config = configuration.BindOptions<EmailOptions>(nameof(EmailOptions));
        configureOptions?.Invoke(config ?? new EmailOptions());

        if (provider == EmailProvider.SendGrid)
        {
            services.AddSingleton<IEmailSender, SendGridEmailSender>();
        }
        else
        {
            services.AddSingleton<IEmailSender, MailKitEmailSender>();
        }

        if (configureOptions is { })
        {
            services.Configure(nameof(EmailOptions), configureOptions);
        }
        else
        {
            services
                .AddOptions<EmailOptions>()
                .Bind(configuration.GetSection(nameof(EmailOptions)))
                .ValidateDataAnnotations();
        }

        return services;
    }
}
