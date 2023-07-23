using System.Data.SqlClient;

namespace ConsoleApp1;

public class DB
{
    SqlConnection connection = new SqlConnection();
    public string ConnectionString = "";
    
    public delegate void TaskDelegate();
    //public static TaskDelegate? WhileConnected;

    private bool isConnected;
    public bool IsConnected { get => isConnected; }

    public DB(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public DB() : this(@"Server=MSI-85CBVIK\SQLEXPRESS;Database=NS_T1;User ID=user;Password=password") { }

    public bool Connect()
    {
        connection = new SqlConnection(ConnectionString);
        try
        {
            connection.Open();
            Console.WriteLine("Connection open!!");
            isConnected = true;
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public void Disconnect()
    {
        if (isConnected)
        {
            connection.Close();
            isConnected = false;
        }
    }

    public void Execute(string cmd)
    {
        SqlCommand command = new SqlCommand(cmd, connection);
        command.ExecuteNonQuery();
    }

    public SqlDataReader Read(string cmd)
    {
        SqlCommand command = new SqlCommand(cmd, connection);
        SqlDataReader reader = command.ExecuteReader();
        return reader;
    }

    public int GetVehiclesAmount()
    {
        SqlDataReader reader = Read("SELECT COUNT(*) FROM pojazdy");
        reader.Read();
        int count = Convert.ToInt32(reader[0].ToString());
        reader.Close();
        return count;
    }

    public List<string> GetSettings()
    {
        int i = 1;
        string cmd = "DECLARE @count INT, @i INT\r\nSET @count = (SELECT COUNT(*) FROM pojazdy)\r\nSET @i = 1\r\n\r\nWHILE(@i <= @count)\r\nBEGIN\r\n\r\n\t/*SELECT * FROM pojazdy WHERE @i=pojazdy.id_pojazdu*/\r\n\r\n\tSELECT \r\n\t\ttypy_ustawien.id_typu_ustawienia as id_ust, \r\n\t\tustawienia_pojazdow.wartosc as war\r\n\tFROM pojazdy\r\n\tJOIN ustawienia_pojazdow \r\n\t\tON ustawienia_pojazdow.id_pojazdu = pojazdy.id_pojazdu\r\n\tJOIN typy_ustawien \r\n\t\tON typy_ustawien.id_typu_ustawienia = ustawienia_pojazdow.id_typu_ustawienia\r\n\tWHERE pojazdy.id_pojazdu = @i\r\n\tFOR JSON PATH,\r\n\t\tINCLUDE_NULL_VALUES,\r\n\t\tWITHOUT_ARRAY_WRAPPER\r\n\tSET @i = @i+1\r\nEND\r\n";
        List<string> settings = new List<string>();

        SqlDataReader reader = Read(cmd);
        while(reader.Read())
        {
            //Console.WriteLine(reader[0].ToString()); 
            settings.Add(reader[0].ToString());
            if (!reader.NextResult()) break;
        }
            
        reader.Close();
        return settings;
    }
    public List<string> GetVehicles()
    {
        string cmd = "SELECT \r\n\tid_pojazdu, \r\n\tnazwa, \r\n\tdbo.typy_pojazdow.nazwa_typu\r\nFROM dbo.pojazdy \r\nJOIN dbo.typy_pojazdow\r\n\tON dbo.pojazdy.id_typu_pojazdu = dbo.typy_pojazdow.id_typu";
        List<string> list = new List<string>();

        SqlDataReader reader = Read(cmd);
        while (reader.Read())
        {
            list.Add(reader[0].ToString() + "," + reader[1].ToString() + "," + reader[2].ToString());
        }
        reader.Close();
        return list;
    }

    public int GetVehicleID(string name)
    {
        string cmd = "SELECT \r\n\tid_pojazdu, \r\n\tnazwa, \r\n\tdbo.typy_pojazdow.nazwa_typu\r\nFROM dbo.pojazdy \r\nJOIN dbo.typy_pojazdow\r\n\tON dbo.pojazdy.id_typu_pojazdu = dbo.typy_pojazdow.id_typu";

        int id = 0;
        SqlDataReader reader = Read(cmd);
        while (reader.Read())
        {
            if (reader[1].ToString() == name)
            {
                id = Convert.ToInt32(reader[0].ToString());
                break;
            }  
        }
        reader.Close();
        return id;
    }

}
