using System.Net;
using FTPServer;

var serverEndpoint = IPEndPoint.Parse("127.0.0.1:7777");
var fileTransferServer = new Ftpserver(serverEndpoint);

await fileTransferServer.StartAsync();

Console.WriteLine("Done!");