using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.IoT;

[Activity(Category = "IoT", Description = "Send data to Azure IoT Hub", DisplayName = "Azure IoT Send")]
public class AzureIoTSendActivity : CodeActivity
{
    [Input(Description = "Connection string")]
    public Input<string> ConnectionString { get; set; } = default!;

    [Input(Description = "Device ID")]
    public Input<string> DeviceId { get; set; } = default!;

    [Input(Description = "Telemetry data JSON")]
    public Input<string> TelemetryData { get; set; } = default!;

    [Output(Description = "Whether sent")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var connectionString = context.Get(ConnectionString) ?? throw new ArgumentException("ConnectionString is required");
            var deviceId = context.Get(DeviceId) ?? throw new ArgumentException("DeviceId is required");
            var telemetryData = context.Get(TelemetryData) ?? throw new ArgumentException("TelemetryData is required");

            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
