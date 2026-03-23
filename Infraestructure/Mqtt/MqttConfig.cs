namespace MqttLogger.Infraestructure.Mqtt;

public class MqttConfig
{
    public string Host { get; set; } = Environment.GetEnvironmentVariable("MQTT_HOST") ?? "";
    public int Port { get; set; } = int.TryParse(Environment.GetEnvironmentVariable("MQTT_PORT"), out int p) ? p : 8883;
    public string Username { get; set; } = Environment.GetEnvironmentVariable("MQTT_USERNAME") ?? "";
    public string Password { get; set; } = Environment.GetEnvironmentVariable("MQTT_PASSWORD") ?? "";
    public string Topic { get; set; } = Environment.GetEnvironmentVariable("MQTT_TOPIC") ?? "";

}