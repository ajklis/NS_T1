

using ConsoleApp1;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Listener;

class ListenerService
{
    private readonly System.Timers.Timer _timer;

    private MQTT client;
    private DB database;

    public ListenerService()
    {
        _timer = new System.Timers.Timer(10000) { AutoReset = true };
        _timer.Elapsed += TimerElapsed;

        client = new MQTT("ListenerClient1");
        client.Connect();
        database = new DB();
        database.Connect();

        client.Subscribe(GetAllTopics());

        //reaction to message recieved
        client.AddMqttMsgPublishRecieved(MqttClient_MqttMsgPublishReceived);

        Console.WriteLine("Client connected?: " + client.IsConnected);
    }
    private List<string> GetAllTopics()
    {
        List<string> vehicles = database.GetVehicles();
        List<string> topics = new List<string>();

        foreach (string s in vehicles)
        {
            string[] split = s.Split(",");
            topics.Add(split[2].Split(" ")[0] + "/" + split[1]); //removing spaces
        }
        foreach (string topic in topics)
            Console.WriteLine(topic);
        return topics;
    }

    private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        new Task(() =>
        {
            //getting new settings
            string name = e.Topic.Split("/")[1];
            string message = Encoding.UTF8.GetString(e.Message);

            string[] split = message.Split(new char[] { ',', '{', '}', '=', ':', '"' });

            List<string> list = new List<string>();
            foreach (string s in split)
            {
                if (s != "" && s != "id_ust" && s != "war")
                {
                    list.Add(s);
                }
            }

            List<int> type_id = new List<int>();
            List<string> val = new List<string>();
            for (int i = 0; i < list.Count; i += 2)
            {
                type_id.Add(Convert.ToInt32(list[i]));
                val.Add(list[i + 1]);
            }

            //saving new setting to database
            int id = database.GetVehicleID(name);
            for (int i = 0; i < type_id.Count; i++)
            {
                string cmd = "UPDATE ustawienia_pojazdow " +
                            $"SET wartosc = \'{val[i]}\' " + 
                            $"WHERE id_pojazdu = {id} AND id_typu_ustawienia = {type_id[i]}";
                Console.WriteLine(cmd);
                database.Execute(cmd);
            }

        }).Start();
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e) // does nothing rn
    {
        
    }

    public void Start()
    {
        _timer.Start();
        //client.Connect(clientName);
    }

    public void Stop()
    {
        client.Disconnect();
        database.Disconnect();
        _timer.Stop();
    }
}
