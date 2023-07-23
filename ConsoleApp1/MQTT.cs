using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using static uPLibrary.Networking.M2Mqtt.MqttClient;

namespace ConsoleApp1;

public class MQTT
{
    public string Host { get => "localhost"; }
    private string clientName = "MQTTinCS";
    public string ClientName { get; set; }
    public bool IsConnected { get => client.IsConnected; }
    
    private List<string> subs = new List<string>();
    
    private MqttClient client;

    public MQTT(string clientName)
    {
        ClientName = clientName;
    }

    public MQTT() : this("MQTTinCS") { }
    
    public bool Connect()
    {
        return Connect(Host);
    }
    public bool Connect(string hostName)
    {
        client = new MqttClient(hostName);
        client.Connect(ClientName);
        return client.IsConnected;
    }

    public void Disconnect()
    {
        client.Disconnect();
    }

    public void Publish(string topic, string message)
    {
        client.Publish(topic, Encoding.UTF8.GetBytes(message));
    }

    public void Subscribe(string topic)
    {
        subs.Add(topic);
        byte[] qos = new byte[subs.Count];
        for (int i = 0; i < subs.Count; i++)
            qos[i] = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
        client.Subscribe(subs.ToArray() , qos);
    }

    public void Subscribe(List<string> topics)
    {
        subs.AddRange(topics);
        byte[] qos = new byte[subs.Count];
        for (int i = 0; i < subs.Count; i++)
            qos[i] = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
        client.Subscribe(subs.ToArray(), qos);
    }
    public delegate void MqttMsgRec(object sender, MqttMsgPublishEventArgs e);
    public void AddMqttMsgPublishRecieved(MqttMsgPublishEventHandler function)
    {
        client.MqttMsgPublishReceived += function;
    }
}
