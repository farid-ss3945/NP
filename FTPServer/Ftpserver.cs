using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Win32.SafeHandles;
using FileStream = System.IO.FileStream;

namespace FTPServer;

public class Ftpserver
{
    private TcpListener _listener;
    private bool _isListening;
    private TcpClient? _client;

    public Ftpserver(IPEndPoint endPoint)
    {
        _listener = new TcpListener(endPoint);
    }
    
    public async Task StartAsync()
    {
        if (!_isListening)
        {
            _listener.Start();
            _isListening = true;
            _client = await _listener.AcceptTcpClientAsync();

            Console.WriteLine("Accepted!");

            await Task.Run(() =>
            {
                string? path = null;
                using var stream = _client.GetStream();
                using var streamReader = new StreamReader(stream);
                using var streamWriter = new StreamWriter(stream);
                using var binaryWriter = new BinaryWriter(stream);
                using var binaryReader = new BinaryReader(stream);
                FileStream? fileStream = null;

                byte[] buffer = new byte[1024];

                while (_isListening) 
                {
                    if (path is not null)
                    {
                        var read = fileStream!.Read(buffer);
                        Console.WriteLine($"Read: {read}");
                        
                        if (read <= 0)
                        {
                            streamWriter.WriteLine("DONE");
                            streamWriter.Flush();

                            // path = null; 
                            Console.WriteLine("Transfer finished.");

                            // continue;
                        
                            break;
                        }

                        //fileStream.ReadExactly(buffer, 0, read);
                        
                        // binaryWriter.Write(buffer, 0, read);
                        // binaryWriter.Flush();
                    }
                    
                    else
                    {
                        var result = streamReader.ReadLine();

                        if (IsDownloadAction(result, out path))
                        {
                            var parts = path.Split(' ');
                            string clientRoot = parts[0];     
                            string fileName = parts[1];       

                            string rootDirectory = Path.Combine(AppContext.BaseDirectory, fileName);

                            fileStream = new FileStream(rootDirectory, FileMode.Open,FileAccess.Read);
                            Console.WriteLine($"Client requested file: {fileName} from root: {clientRoot}");
                        }
                        else if (IsUploadAction(result, out path))
                        {
                            var parts = path.Split(' ');
                            string clientRoot = parts[0];
                            string fileName = parts[1];

                            string savePath = Path.Combine(AppContext.BaseDirectory,clientRoot, fileName);
                            
                            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
                            fileStream = new FileStream(savePath,FileMode.Create, FileAccess.Write);

                            Console.WriteLine($"Client uploads file: {fileName} to root: {clientRoot}");
                            
                            int read;
                            
                            while ((read = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                fileStream.Write(buffer, 0, read);
                                Console.WriteLine($"Read: {read}");
                                
                            }
                            fileStream.Flush();
                            Console.WriteLine("UPLOAD COMPLETED ON SERVER!");
                            
                        }
                        else
                        {
                            continue;
                        }
                    }
                    break;
                }
                
                // fileStream?.Dispose();
            });
        }
    }
    
    private const string DownloadAction = "DOWNLOAD";

    public static bool IsDownloadAction(string? action, [NotNullWhen(true)] out string? path)
    {
        path = null;

        if (action is null)
        {
            return false;
        }

        try
        {
            var split = action.Split(' ');

            if (split.First().ToUpper() == DownloadAction)
            {
                path = split[1]+" "+split[2];

                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
    
    public static bool IsUploadAction(string? action, [NotNullWhen(true)] out string? path)
    {
        path = null;

        if (action is null)
        {
            return false;
        }

        try
        {
            var split = action.Split(' ');

            if (split.First().ToUpper() == "UPLOAD")
            {
                path = split[1] + " " + split[2];

                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public Task StopAsync()
    {
        if (_isListening)
        {
            _listener.Stop();
        }

        return Task.CompletedTask;
    }
}