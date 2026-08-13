using System;

namespace GerenciadorProcessos.Shared.Time;

public class BrazilianTimeProvider : TimeProvider
{
    // No Windows, o TZ é E. South America Standard Time
    // No Linux, é America/Sao_Paulo
    private static readonly TimeZoneInfo BrazilTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "E. South America Standard Time" : "America/Sao_Paulo");

    public override TimeZoneInfo LocalTimeZone => BrazilTimeZone;

    public override DateTimeOffset GetUtcNow()
    {
        return DateTimeOffset.UtcNow;
    }
}
