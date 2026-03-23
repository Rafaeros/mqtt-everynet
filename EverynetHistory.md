# Everynet History API Backup 🔄

Este documento descreve o uso da **History API da Everynet** como uma camada crítica de redundância para recuperação de dados em sistemas baseados em MQTT (ex: HiveMQ).

---

## 📋 Contexto de Segurança

Embora o sistema MQTT utilize filas offline para garantir entrega de mensagens, existem cenários onde dados podem ser perdidos:

1. Expiração do tempo de retenção no broker MQTT.
2. Limite de mensagens offline excedido.
3. Falhas de configuração em tópicos ou subscribers.
4. Instabilidade de rede no momento da publicação.

➡️ Para mitigar esses riscos, utilizamos o histórico da Everynet como **fonte confiável de recuperação de uplinks**.

---

## 🛰️ Everynet History API (Network Server)

A Everynet mantém um histórico de mensagens LoRaWAN recebidas por seus gateways. Esse histórico pode ser acessado via API REST.

---

## 🔐 Autenticação

A API requer um **Access Token** válido:

Deve ser enviado no header HTTP:

Authorization: Bearer {ACCESS_TOKEN}

---

## 🌐 Endpoint de Consulta

GET https://ns.atc.everynet.io/api/v1.0/history/data.json

---

## ⚙️ Parâmetros da Query

### 🕒 `from`
Timestamp Unix (segundos) - início do intervalo

### 🕒 `to`
Timestamp Unix (segundos) - fim do intervalo

### 🔢 `limit`
Número máximo de registros retornados

### 🔍 `filter`
Permite filtrar dados (ex: dev_eui, app_eui)

---

## 📌 Exemplo de Requisição

GET /api/v1.0/history/data.json?from=1700000000&to=1700003600&limit=100&filter=dev_eui:ABC123456789

Authorization: Bearer SEU_ACCESS_TOKEN

---

## 📦 Exemplo de Resposta

```json
[
  {
    "dev_eui": "ABC123456789",
    "payload": "A1B2C3",
    "metadata": {
      "time": "2026-03-23T12:00:00Z",
      "rssi": -70,
      "snr": 5.2,
      "gateway_id": "gateway-01"
    }
  }
]
```

---

## 🧠 Estratégias de Uso

### 🔄 Recuperação de Gaps
Comparar timestamps e buscar intervalos faltantes

### 📊 Reprocessamento
Reenviar dados para pipeline interno

### 📅 Paginação
Usar `limit` e iterar requisições

---

## ⏱️ Ordenação de Dados

Sempre usar:

metadata.time

---

## ✅ Boas Práticas

- Validar duplicidade
- Armazenar último timestamp
- Retry com backoff
- Monitorar gaps
- Versionar payloads

---

## 🚀 Conclusão

A History API da Everynet garante recuperação confiável de dados mesmo em falhas no MQTT.
