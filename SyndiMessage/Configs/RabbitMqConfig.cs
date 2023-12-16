using RabbitMQ.Client;

namespace SyndiMessage.Configs;

/// <summary>
/// RabbitMq Broker information class
/// </summary>
public abstract class RabbitMqConfig
{
    public RabbitMqConfig() { }

    public RabbitMqConfig(string host, int port, string userName, string password)
    {
        Host = host;
        Port = port;
        UserName = userName;
        Password = password;
    }

    public RabbitMqConfig(string host, int port, string userName, string password, SslOption sslOption) : this(host, port, userName, password)
    {
        SslOption = sslOption;
    }

    /// <summary>
    /// Broker host name.
    /// </summary>
    /// <value>10.1.1.1</value>
    public string Host { get; set; } = string.Empty;
    /// <summary>
    /// Broker port number.
    /// </summary>
    /// <value>5672</value>
    public int Port { get; set; }
    /// <summary>
    /// Broker user name
    /// </summary>
    /// <value>tests</value>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// Broker password
    /// </summary>
    /// <value>123*123</value>
    public string Password { get; set; } = string.Empty;
    public SslOption? SslOption { get; set; }
}
