namespace MqttLogger.Features.SaveMqttLog;

public class SaveMqttLogHandler
{
    private readonly string _filePath = "logs.txt";
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task HandleAsync(string topic, string payload)
    {
        if (payload.Contains("\"type\": \"error\"") && payload.Contains("Bad message format"))
        {
            Console.WriteLine($"[Ignorado] Mensagem de erro em loop detectada no tópico: {topic}");
            return; 
        }
        
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [Topic]: {topic}] {payload}";

        Console.WriteLine($"[SaveMqttLogFeature]: Processando: {logEntry}");

        await _semaphore.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(_filePath, logEntry + Environment.NewLine);
        }
        finally
        {
            _semaphore.Release();
        }
    }

}