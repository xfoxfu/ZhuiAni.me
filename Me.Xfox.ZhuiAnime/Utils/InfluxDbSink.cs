using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Serilog;
using Serilog.Configuration;
using Serilog.Sinks.PeriodicBatching;
using SerilogLogEvent = Serilog.Events.LogEvent;

namespace Me.Xfox.ZhuiAnime;

public class InfluxDBSink : IBatchedLogEventSink
{
    public readonly string ServerEndpoint;
    public readonly string Token;
    public readonly string OrganizationId;
    public readonly string BucketName;
    public readonly string MeasurementName;

    public readonly IInfluxDBClient Client;

    public InfluxDBSink(
        string serverEndpoint,
        string token,
        string organizationId,
        string bucketName,
        string measurementName)
    {
        ServerEndpoint = serverEndpoint;
        Token = token;
        OrganizationId = organizationId;
        BucketName = bucketName;
        MeasurementName = measurementName;
        Client = new InfluxDBClient("http://localhost:8086", Token);
    }

    public async Task EmitBatchAsync(IEnumerable<SerilogLogEvent> events)
    {
        try
        {
            IFormatProvider? _formatProvider = null;
            if (events == null) throw new ArgumentNullException(nameof(events));

            var logEvents = events as List<SerilogLogEvent> ?? events.ToList();
            var points = new List<PointData>(logEvents.Count);

            foreach (var logEvent in logEvents)
            {
                var p = PointData.Measurement("zhuianime.logging")
                    .Timestamp(logEvent.Timestamp.UtcDateTime, WritePrecision.Ns);
                foreach (var property in logEvent.Properties)
                {
                    p = p.Field(property.Key, property.Value);
                }

                // Add tags
                if (logEvent.Exception != null) p = p.Tag("ExceptionType", logEvent.Exception.GetType().Name);
                if (logEvent.MessageTemplate != null) p = p.Tag("MessageTemplate", logEvent.MessageTemplate.Text);

                p.Tag("Level", logEvent.Level.ToString());

                // Add rendered message
                p.Field("Message", logEvent.RenderMessage(_formatProvider));

                points.Add(p);
            }

            await Client.GetWriteApiAsync().WritePointsAsync(points, "zhuianime", "030783613ae14cf6");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public Task OnEmptyBatchAsync()
    {
        throw new NotImplementedException();
    }
}

public static class InfluxDBSinkExtensions
{
    public static LoggerConfiguration InfluxDB(
        this LoggerSinkConfiguration loggerConfiguration,
        string serverEndpoint,
        string token,
        string organizationId,
        string bucketName,
        string measurementName)
    {
        return loggerConfiguration.Sink(new PeriodicBatchingSink(new InfluxDBSink(
            serverEndpoint,
            token,
            organizationId,
            bucketName,
            measurementName), new PeriodicBatchingSinkOptions()));
    }
}
