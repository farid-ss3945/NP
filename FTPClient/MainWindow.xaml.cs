using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserInfo;

namespace FTPClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }


    private void Register_OnClick(object sender, RoutedEventArgs e)
    {
        User user = new User();
        user._name = Username.Text;
        user._pwd = Password.Text;
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true
        };
            
        if (Username.Text.Length>0 && Password.Text.Length >= 4 && !File.Exists($"C:\\Users\\MSI1\\RiderProjects\\FTPClient\\FTPClient\\bin\\Debug\\net9.0-windows\\{Username.Text}Root\\"+user._name + ".json")) 
        {
            Directory.CreateDirectory(Username.Text+"Root");
            File.WriteAllText($"C:\\Users\\MSI1\\RiderProjects\\FTPClient\\FTPClient\\bin\\Debug\\net9.0-windows\\{Username.Text}Root\\"+user._name + ".json", JsonSerializer.Serialize(user,options));
            Reg_1.Visibility = Visibility.Collapsed;
            Main.Visibility = Visibility.Visible;
        }
        else if (File.Exists($"C:\\Users\\MSI1\\RiderProjects\\FTPClient\\FTPClient\\bin\\Debug\\net9.0-windows\\{Username.Text}Root\\"+user._name + ".json"))
        {
            MessageBox.Show("There is a user with this name,maybe you wanna login?67");
        }
        else
        {
            MessageBox.Show("Incorrect authorization data(Password must have atleast 4 symbols)");
        }
        Username.Clear();
        Password.Clear();
        
    }

    private void Login_OnClick(object sender, RoutedEventArgs e)
    {
        Username.Clear();
        Password.Clear();
        Reg_1.Visibility = Visibility.Collapsed;
        LogView.Visibility = Visibility.Visible;
    }

    private void Login_2_OnClick_OnClick(object sender, RoutedEventArgs e)
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true
        };

        if (!File.Exists($"C:\\Users\\MSI1\\RiderProjects\\FTPClient\\FTPClient\\bin\\Debug\\net9.0-windows\\{Username_2.Text}Root\\"+Username_2.Text + ".json"))
        {
            MessageBox.Show("User not found");
            return;
        }

        string jsonText = File.ReadAllText($"C:\\Users\\MSI1\\RiderProjects\\FTPClient\\FTPClient\\bin\\Debug\\net9.0-windows\\{Username_2.Text}Root\\"+Username_2.Text + ".json");
        var json = JsonSerializer.Deserialize<User>(jsonText, options);

        if (Password_2.Text == json._pwd)
        {
            Username_2.Clear();
            Password_2.Clear();
            LogView.Visibility = Visibility.Collapsed;
            Main.Visibility = Visibility.Visible;
        }
        else
        {
            MessageBox.Show("Incorrect Password");
        }
    }


    private void Back_OnClick(object sender, RoutedEventArgs e)
    {
        Username_2.Clear();
        Password_2.Clear();
        LogView.Visibility = Visibility.Collapsed;
        Reg_1.Visibility = Visibility.Visible;
    }

    private void BackToReg_OnClick(object sender, RoutedEventArgs e)
    {
        Main.Visibility = Visibility.Collapsed;
        Reg_1.Visibility = Visibility.Visible;
    }

    private async void Download_OnClick(object sender, RoutedEventArgs e)
    {
        var serverEndpoint = IPEndPoint.Parse("127.0.0.1:7777");
        var tcpClient = new TcpClient();

        await tcpClient.ConnectAsync(serverEndpoint);
        var fileName = FileName.Text;
        var networkStream = tcpClient.GetStream();
        using var streamWriter = new StreamWriter(networkStream);
        using var binaryReader = new BinaryReader(networkStream);
        using var fileStream = new FileStream($"C:\\Users\\MSI1\\RiderProjects\\FTPClient\\FTPClient\\bin\\Debug\\net9.0-windows\\{Username_3.Text}Root\\"+fileName, FileMode.Create, FileAccess.Write);

        streamWriter.WriteLine($"DOWNLOAD {Username_3.Text.Trim()}Root {fileName}");
        streamWriter.AutoFlush=true;

        var buffer = new byte[1024];

        while (true)
        {
            var read = binaryReader.Read(buffer);

            Console.WriteLine(read);
    
            if (read == 0 || IsDone(buffer, read))
            {
                fileStream.Flush();
                Console.WriteLine("DONE!");
                break;
            }
    
            fileStream.Write(buffer, 0, read);
        }



        bool IsDone(byte[] payload, int read)
        {
            return read == 4 && Encoding.UTF8.GetString(payload, 0, read) == "DONE";
        }
    }

    private async void Upload_OnClick(object sender, RoutedEventArgs e)
    {
        var serverEndpoint = IPEndPoint.Parse("127.0.0.1:7777");
        var tcpClient = new TcpClient();

        await tcpClient.ConnectAsync(serverEndpoint);
        var networkStream = tcpClient.GetStream();
        var binaryWriter = new BinaryWriter(networkStream);

        var fileName = FileName_2.Text.Trim();
        var clientRoot = $"{Username_4.Text.Trim()}Root";
        var localPath = $"C:\\Users\\MSI1\\RiderProjects\\FTPClient\\FTPClient\\bin\\Debug\\net9.0-windows\\{clientRoot}\\{fileName}";

        using var fileStream = new FileStream(localPath, FileMode.Open, FileAccess.Read);

        var streamWriter = new StreamWriter(networkStream);
        streamWriter.AutoFlush = true;
        streamWriter.WriteLine($"UPLOAD {clientRoot} {fileName}");


        var buffer = new byte[1024];
        int read;

        while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            Console.WriteLine($"Read {read}");
            binaryWriter.Write(buffer, 0, read);
        }
        binaryWriter.Flush();
        fileStream.Flush();
        tcpClient.Client.Shutdown(SocketShutdown.Send);
        
    }
}