using Microsoft.Extensions.Options;

namespace Me.Xfox.ZhuiAnime.Modules.TorrentDirectory;

public class Worker<S> : IHostedService, IDisposable where S : ISource
{
    private ILogger<Worker<S>> Logger { get; init; }
    private S Source { get; init; }
    private IServiceScopeFactory ServiceProvider { get; init; }
    private Timer? Timer { get; set; }
    private IOptionsMonitor<Option> Options { get; set; }

    private TimeSpan IntervalBetweenPages => Options.CurrentValue.IntervalBetweenPages;
    private uint FetchPageThreshold => Options.CurrentValue.FetchPageThreshold;

    public Worker(ILogger<Worker<S>> logger, IServiceScopeFactory serviceProvider, IOptionsMonitor<Option> options)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
        Source = Activator.CreateInstance<S>() ??
            throw new Exception($"Failed to create instance of {nameof(S)}");
        Options = options;
    }

    public Task StartAsync(CancellationToken ct)
    {
        if (Options.CurrentValue.Sources.TryGetValue(Source.Name, out var value) && value == true)
        {
            var span = Options.CurrentValue.IntervalBetweenFetch;
            Timer = new Timer(TimerWork, null, TimeSpan.Zero, span);
            Logger.LogInformation("Started timed service for {@Source} every {@Span}", Source.Name, span);
        }
        else
        {
            Logger.LogInformation("{@Source} is disabled", Source.Name);
        }
        return Task.CompletedTask;
    }

    protected async void TimerWork(object? state)
    {
        try
        {
            await UpdateData();
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            Logger.LogError(e, "Error while updating data.");
        }
    }

    protected async Task UpdateData()
    {
        using var db = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ZAContext>();
        uint nextPage = 1;
        while (nextPage != 0)
        {
            bool hasNonExistent = await Source.GetPage(nextPage, db);
            Logger.LogInformation("{@Source} Got page {@Page}", Source.Name, nextPage);
            if (hasNonExistent)
            {
                Logger.LogInformation(
                    "{@Source} Page {@Page} has unsaved item, continuing after {@Interval}",
                    Source.Name,
                    nextPage,
                    IntervalBetweenPages);
                nextPage += 1;
                if (nextPage > FetchPageThreshold)
                {
                    Logger.LogInformation(
                        "{@Source} Page {@Page} reached threshold, stopping",
                        Source.Name,
                        nextPage);
                    nextPage = 0;
                }
            }
            else
            {
                Logger.LogInformation(
                    "{@Source} Page {@Page} all items saved, stopping",
                    Source.Name,
                    nextPage);
                nextPage = 0;
            }
            await Task.Delay(IntervalBetweenPages);
        }
    }

    public Task StopAsync(CancellationToken ct)
    {
        Logger.LogInformation("Stopped timed service for {@Source}.", Source.Name);
        Timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Timer?.Dispose();
        GC.SuppressFinalize(this);
    }

    public static WebApplicationBuilder ConfigureOn(WebApplicationBuilder builder)
    {
        builder.Services.Configure<Option>(builder.Configuration.GetSection(Option.LOCATION));
        builder.Services.AddHostedService<Worker<S>>();
        return builder;
    }

    public class Option
    {
        public const string LOCATION = "Modules:TorrentDirectory";

        public TimeSpan IntervalBetweenFetch { get; set; }

        public TimeSpan IntervalBetweenPages { get; set; }

        public uint FetchPageThreshold { get; set; }

        public IDictionary<string, bool> Sources { get; set; } = new Dictionary<string, bool>();
    }
}
