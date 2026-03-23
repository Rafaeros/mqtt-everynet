


using System.Text;
using MqttLogger.Features.SaveMqttLog;
using MQTTnet;
using MQTTnet.Formatter;

namespace MqttLogger.Infraestructure.Mqtt;

public class MqttConnectionManager
{
  private readonly MqttConfig _config;
  private readonly SaveMqttLogHandler _saveLogHandler;
  private readonly IMqttClient _mqttClient;

  public MqttConnectionManager(MqttConfig config, SaveMqttLogHandler saveLogHandler)
  {
    _config = config;
    _saveLogHandler = saveLogHandler;
    _mqttClient = new MqttClientFactory().CreateMqttClient();
  }

  public async Task StartAsync()
  {
    var options = new MqttClientOptionsBuilder()
        .WithTcpServer(_config.Host, _config.Port)
        .WithCredentials(_config.Username, _config.Password)
        .WithTlsOptions(o => o.UseTls())
        .WithClientId($"LogReceiver") // Precisa ser ID Fixo para não perder a sessão e os logs armazenados.
        .WithCleanSession(false) // Mantém a sessão mesmo se desconectar, criando a fila de mensagens não entregues.
        .Build();

    _mqttClient.ApplicationMessageReceivedAsync += async e =>
    {
      var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
      await _saveLogHandler.HandleAsync(e.ApplicationMessage.Topic, payload);
    };


    _mqttClient.DisconnectedAsync += async e =>
    {
      Console.WriteLine("Desconectado, reconectando...");
      await Task.Delay(5000);
      await ConnectAsync(options);
    };

    await ConnectAsync(options);



  }


  private async Task ConnectAsync(MqttClientOptions options)
  {
    Console.WriteLine($"[Config] Host: {_config.Host}:{_config.Port}");
    Console.WriteLine($"[Config] Tópico configurado: '{_config.Topic}'");

    if (string.IsNullOrWhiteSpace(_config.Topic))
    {
      Console.WriteLine("❌ ERRO CRÍTICO: O tópico está vazio! Verifique seu arquivo .env.");
      return;
    }

    try
    {

      await _mqttClient.ConnectAsync(options);
      Console.WriteLine("✅ Conectado ao broker MQTT (HiveMQ)!");

      var subOptions = new MqttClientFactory().CreateSubscribeOptionsBuilder()
          .WithTopicFilter(f => f
            .WithTopic(_config.Topic)
            .WithAtLeastOnceQoS() // Garante que a mensagem seja entregue pelo menos uma vez
          )
          .Build();

      await _mqttClient.SubscribeAsync(subOptions);
      Console.WriteLine($"✅ Inscrito e ouvindo o tópico: {_config.Topic}");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Falha ao conectar no Broker: {ex.Message}");
    }
  }
}