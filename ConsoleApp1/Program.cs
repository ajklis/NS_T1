using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net;

namespace ConsoleApp1;
class Program
{
    static void Main(string[] args)
    {
        DB database = new DB();
        database.Connect();
        MQTT publisher = new MQTT();
        publisher.Connect();

        //MQTT.Publish("test_topic", "im in");

        CreateTopicsAndSubs(publisher, database);


        database.Disconnect();
        publisher.Disconnect();
    }


    
    

    static void CreateTopicsAndSubs(MQTT mqtt, DB database)
    {
        List<string> vehicles = database.GetVehicles();
        List<string> settings = database.GetSettings();

        foreach(string vehicle in vehicles)
        {
            string[] split = vehicle.Split(",");
            int id_pojazdu = Convert.ToInt32(split[0]);

            string topic = split[2].Split(" ")[0] + "/" + split[1]; // żeby spacje usunąć
            Console.WriteLine(topic + " = " + settings[id_pojazdu-1]);
            mqtt.Publish(topic, settings[id_pojazdu-1]);
        }
    }

    

}
