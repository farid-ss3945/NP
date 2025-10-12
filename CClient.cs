using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

User user = new User();
user.name = Console.ReadLine();

TcpClient client = new TcpClient();
client.Connect(IPAddress.Parse("127.0.0.1"), 7777);



user.date=DateTime.Now;
 
NetworkStream stream = client.GetStream();

Console.WriteLine("1.AZN\n" +
                  "2.EUR\n" +
                  "3.USD");

string from=Console.ReadLine();

Console.WriteLine("1.AZN\n" +
                  "2.EUR\n" +
                  "3.USD");

string to = Console.ReadLine();

string amount = Console.ReadLine();

string message = from + to + amount;
byte[] data = Encoding.UTF8.GetBytes(message);
stream.Write(data, 0, data.Length);
    
byte[] buffer = new byte[1024];
int bytesRead = stream.Read(buffer, 0, buffer.Length);
string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);



Console.WriteLine("Server Response: "+response);

user.converted = response;  

string filename = user.name + ".json";
string json = JsonSerializer.Serialize(user);
File.WriteAllText(filename, json);


client.Close();


class User
{
    public string name;
    public DateTime date;
    public string converted;
}