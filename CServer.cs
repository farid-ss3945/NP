using System.Net;
using System.Net.Sockets;
using System.Text;

TcpListener _listener=new TcpListener(IPAddress.Parse("127.0.0.1"), 7777);

_listener.Start();
Console.WriteLine("Server started. Waiting for connections...");

while (true)
{
    TcpClient client = _listener.AcceptTcpClient();
    Console.WriteLine("Cliend Connected!");
    NetworkStream stream = client.GetStream();
    byte[] buffer = new byte[1024];
    int bytesRead = stream.Read(buffer, 0, buffer.Length);

    string message = Encoding.UTF8.GetString(buffer, 0, 1);
    string message_2 = Encoding.UTF8.GetString(buffer, 1, 1);
    string message_3 = Encoding.UTF8.GetString(buffer, 2, bytesRead);
    
    if (message=="1")
    {
        if (message_2=="1")
        {
            Console.WriteLine("It's the same🥀🙏");
        }
        else if (message_2 == "2")
        {
            double temp = Convert.ToDouble((message_3));
            temp /= 2;
            message_3 = Convert.ToString(temp);
        }
        else if (message_2 == "3")
        {
            double temp = Convert.ToDouble((message_3));
            temp /= 1.7;
            message_3 = Convert.ToString(temp);
        }
    }
    
    else if (message=="2")
    {
        if (message_2=="2")
        {
            Console.WriteLine("It's the same🥀🙏");
        }
        else if (message_2 == "1")
        {
            double temp = Convert.ToDouble((message_3));
            temp *= 2;
            message_3 = Convert.ToString(temp);
        }
        else if (message_2 == "3")
        {
            double temp = Convert.ToDouble((message_3));
            temp *= 1.16;
            message_3 = Convert.ToString(temp);
        }
    }
    
    else if (message=="3")
    {
        if (message_2=="3")
        {
            Console.WriteLine("It's the same🥀🙏");
        }
        else if (message_2 == "1")
        {
            double temp = Convert.ToDouble((message_3));
            temp *= 1.7;
            message_3 = Convert.ToString(temp);
        }
        else if (message_2 == "2")
        {
            double temp = Convert.ToDouble((message_3));
            temp /= 1.16;
            message_3 = Convert.ToString(temp);
        }
    }
    
    string response = message_3;
    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
    stream.Write(responseBytes, 0, responseBytes.Length);

    client.Close();
}

