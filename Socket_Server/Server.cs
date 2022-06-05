using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Server
{
    static void Main(string[] args)
    {
        bool bServerOn = true;
        Network_Manager Network_Service = new Network_Manager();
        Database_Manager Database_Service = new Database_Manager();

        StartService();
        while (bServerOn)
        {
            Network_Service.CheckConnection();
            Network_Service.CheckMessage();
            Network_Service.DisconnectClients();
        }

        void StartService()
        {
            //Database manager
            Network_Service.Start_Network_Service();
        }
    }
}
