using Microsoft.Extensions.DependencyInjection;
using MqttLogger.Infraestructure.Mqtt;
using MqttLogger.Features.SaveMqttLog;

DotNetEnv.Env.TraversePath().Load();

var services = new ServiceCollection();

services.AddSingleton<MqttConfig>();
services.AddSingleton<SaveMqttLogHandler>();
services.AddSingleton<MqttConnectionManager>();

var serviceProvider = services.BuildServiceProvider();

var mqttManager = serviceProvider.GetRequiredService<MqttConnectionManager>();
await mqttManager.StartAsync();

Console.WriteLine("Aplicação rodando... Pressione [Enter] para sair.");
Console.ReadLine();