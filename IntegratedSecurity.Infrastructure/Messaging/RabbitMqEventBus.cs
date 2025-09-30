// RabbitMqEventBus using RabbitMQ.Client.
// Durable topic exchange "security.events"; JSON serialize app events; basic publish/subscribe.
// Configure with IOptions<RabbitMqOptions> (HostName, Port, UserName, Password, VHost).
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using IntegratedSecurity.Application.Abstractions;

namespace IntegratedSecurity.Infrastructure.Messaging;

public sealed class RabbitMqOptions
{
	public string HostName { get; init; } = "localhost";
	public int Port { get; init; } = 5672;
	public string UserName { get; init; } = "guest";
	public string Password { get; init; } = "guest";
	public string VirtualHost { get; init; } = "/";
}

public sealed class RabbitMqEventBus : IEventBus, IDisposable
{
	private const string Exchange = "security.events";
	private readonly IConnection _conn;
	private readonly IModel _ch;
	private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

	public RabbitMqEventBus(IOptions<RabbitMqOptions> opt)
	{
		var o = opt.Value;
		var factory = new ConnectionFactory { HostName = o.HostName, Port = o.Port, UserName = o.UserName, Password = o.Password, VirtualHost = o.VirtualHost };
		_conn = factory.CreateConnection();
		_ch = _conn.CreateModel();
		_ch.ExchangeDeclare(Exchange, ExchangeType.Topic, durable: true);
	}

	public Task PublishAsync<TEvent>(TEvent evt, CancellationToken ct = default) where TEvent : IEvent
	{
		var key = typeof(TEvent).Name;
		var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt, _json));
		_ch.BasicPublish(Exchange, key, basicProperties: null, body);
		return Task.CompletedTask;
	}

	public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler) where TEvent : IEvent
	{
		var key = typeof(TEvent).Name;
		var q = _ch.QueueDeclare($"{key}-q", durable: true, exclusive: false, autoDelete: false);
		_ch.QueueBind(q.QueueName, Exchange, key);
		var consumer = new EventingBasicConsumer(_ch);
		consumer.Received += async (_, ea) =>
		{
			var json = Encoding.UTF8.GetString(ea.Body.ToArray());
			var evt = JsonSerializer.Deserialize<TEvent>(json, _json)!;
			await handler(evt, CancellationToken.None);
			_ch.BasicAck(ea.DeliveryTag, false);
		};
		_ch.BasicConsume(q.QueueName, autoAck: false, consumer);
	}

	public void Dispose() { _ch?.Dispose(); _conn?.Dispose(); }
}
