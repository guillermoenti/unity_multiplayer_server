using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

public class Network_Manager
{
    public static Network_Manager instance;


    private TcpListener serverListener;
    private List<Client> clients;
    private List<Client> disconnectClient;
    private int lastTimePing;
    private Mutex clientListMutex;
    public Network_Manager()
    {
        if(instance == null)
        {
            instance = this;
        }

        this.clients = new List<Client>();
        this.serverListener = new TcpListener(IPAddress.Any, 6543);
        this.clientListMutex = new Mutex();
        this.lastTimePing = Environment.TickCount;
        this.disconnectClient = new List<Client>();
    }

    public void Start_Network_Service()
    {
        try
        {
            this.serverListener.Start();
            StartListening();
        }catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private void ManagerData(Client client, string data)
    {
        string[] parameters = data.Split('/');
        switch (parameters[0])
        {
            case "Login":
                Login(client, parameters[1], parameters[2]);
                break;
            case "Ping":
                RecievePing(client);
                break;
            case "Register":
                Register(client, parameters[1], parameters[2], int.Parse(parameters[3]));
                break;
            case "RacesData":
                GetRacesData(client);
                break;
            case "GetRace":
                GetRacesOnGame(client, parameters[1], parameters[2]);
                break;
                
        }
    }

    private void SendPing(Client client)
    {
        try
        {
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());
            writer.WriteLine("Ping");
            writer.Flush();
            client.SetWaitingPing(true);
        }
        catch(Exception e)
        {
            Console.WriteLine("Error: " + e.Message  + " con el cliente " + client.GetNick());
        }
    }

    private void Login(Client client, string nick, string password)
    {
        
        try
        {
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());
            Console.WriteLine(Database_Manager.instance.Login(nick, password).ToString());
            writer.WriteLine("Login response/" + Database_Manager.instance.Login(nick, password));
            writer.Flush();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message + " con el cliente " + client.GetNick());
        }

    }

    private void Register(Client client, string nick, string pass, int id_race)
    {

        try
        {
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());
            writer.WriteLine(Database_Manager.instance.Register(nick, pass, id_race).ToString());
            writer.Flush();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message + " con el cliente " + client.GetNick());
        }
    }

    private void GetRacesData(Client client)
    {
        try
        {
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());
            writer.WriteLine("RacesData/" + Database_Manager.instance.GetRacesData().ToString());
            writer.Flush();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message + " con el cliente " + client.GetNick());
        }
    }

    private void GetRacesOnGame(Client client, string nickUser, string nickEnemy)
    {

        try
        {
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());
            writer.WriteLine("RacesResponse/" + Database_Manager.instance.GetRacesOnGame(nickUser, nickEnemy));
            writer.Flush();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message + " con el cliente " + client.GetNick());
        }

    }

    private void StartListening()
    {
        Console.WriteLine("Esperando nuevas conexciones");
        this.serverListener.BeginAcceptTcpClient(AcceptConnection, this.serverListener);
    }

    private void RecievePing(Client client)
    {
        client.SetWaitingPing(false);
    }

    public void CheckConnection()
    {
        if(Environment.TickCount - this.lastTimePing > 5000)
        {
            clientListMutex.WaitOne();
            foreach(Client client in clients)
            {
                if(client.GetWaitingPing() == true)
                {
                    disconnectClient.Add(client);
                }
                else
                {
                    SendPing(client);
                }
            }

            this.lastTimePing = Environment.TickCount;
            clientListMutex.ReleaseMutex();
        }
    }

    public void CheckMessage()
    {
        clientListMutex.WaitOne();
        foreach(Client client in this.clients)
        {
            NetworkStream netStream = client.GetTcpClient().GetStream();
            if (netStream.DataAvailable)
            {
                StreamReader reader = new StreamReader(netStream, true);
                string data = reader.ReadLine();
                if (data != null)
                {
                    ManagerData(client, data);
                }
            }
        }
        clientListMutex.ReleaseMutex();
    }

    private void AcceptConnection(IAsyncResult ar)
    {
        Console.WriteLine("Recibo una conexion");
        TcpListener listener = (TcpListener)ar.AsyncState;
        clientListMutex.WaitOne();
        this.clients.Add(new Client(listener.EndAcceptTcpClient(ar)));
        clientListMutex.ReleaseMutex();
        StartListening();
    }

    public void DisconnectClients()
    {
        clientListMutex.WaitOne();
        foreach(Client client in this.disconnectClient)
        {
            Console.WriteLine("Desconectando usuario");
            client.GetTcpClient().Close();
            this.clients.Remove(client);
        }

        this.disconnectClient.Clear();
        clientListMutex.ReleaseMutex();
    }
}
