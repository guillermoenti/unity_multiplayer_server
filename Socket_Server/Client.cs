using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

class Client
{
    private TcpClient tcp;
    private string nick;
    private bool waitingPing;

    public Client(TcpClient tcp)
    {
        this.tcp = tcp;
        this.nick = "Anonymous";
        this.waitingPing = false;
    }

    public string GetNick()
    {
        return this.nick;
    }

    public void SetWaitingPing(bool waitingPing)
    {
        this.waitingPing = waitingPing;
    }

    public bool GetWaitingPing()
    {
        return waitingPing;
    }

    public TcpClient GetTcpClient()
    {
        return this.tcp;
    }
}