# MQTT Everynet Logger 🛰️

Serviço de logging persistente para monitoramento de pacotes MQTT provenientes da Everynet/HiveMQ. O sistema foi projetado para garantir que nenhuma mensagem seja perdida, mesmo durante instabilidades de rede ou queda do serviço local.

## 🚀 Funcionamento MQTT

O sistema se conecta a um Broker MQTT (atualmente HiveMQ) e assina um tópico (ex: `#`) para capturar todas as mensagens recebidas.

### 🔗 Gerenciamento de Conexão
Para garantir a integridade dos dados, a conexão utiliza parâmetros específicos:
- **Client ID Fixo:** Definido como `LogReceiver`. Isso é crucial para que o Broker identifique o cliente em reconexões.
- **Clean Session (False):** Ao definir `CleanSession` como `false`, informamos ao Broker que queremos manter uma sessão ativa. Mesmo que o serviço caia, o Broker continuará "guardando" as mensagens para nós.
- **QoS 1 (At Least Once):** Garante que a mensagem seja entregue pelo menos uma vez, confirmando o recebimento entre Broker e Cliente.

## 📥 Fila e Histórico Offline

O projeto utiliza o conceito de **Persistência no Broker**:

1. **Desconexão:** Se o serviço `mqtt_everynet` perder a conexão com a internet ou for desligado, o HiveMQ Cloud identifica que o `LogReceiver` saiu, mas mantém sua sessão ativa.
2. **Acúmulo:** As novas mensagens que chegam no tópico assinado são colocadas em uma fila interna no Broker.
3. **Flush Automático:** Assim que o serviço volta a ficar online e se reconecta com o mesmo `Client ID`, o Broker realiza um **Flush** (envio em massa) de todas as mensagens acumuladas durante o período offline.
4. **Processamento:** O serviço recebe essas mensagens retroativas e as processa normalmente, salvando-as no log.

## 📝 Armazenamento de Dados

As mensagens recebidas são processadas pelo `SaveMqttLogHandler` e salvas no arquivo:
- `logs.txt`

Cada entrada contém o timestamp local, o tópico de origem e o conteúdo JSON da mensagem.

---

## 🛠️ Configuração (.env)
Certifique-se de configurar as credenciais corretamente no arquivo `.env`:
```env
MQTT_HOST=seu_host_hivemq
MQTT_PORT=8883
MQTT_USERNAME=seu_usuario
MQTT_PASSWORD=sua_senha
MQTT_TOPIC="#"
```

## 💻 Execução
Para rodar o projeto:
```bash
dotnet run
```
