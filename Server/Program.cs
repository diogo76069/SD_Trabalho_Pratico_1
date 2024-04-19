using Server.Model;
using System.Net;
using System.Net.Sockets;
using System.Text;

class MyTcpListener
{
    public static void Main()
    {
        TcpListener server = null;
        try
        {
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);
            server.Start();


            while (true)
            {
                Console.Write("Waiting for a connection... ");

                using TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected.");

                NetworkStream stream = client.GetStream();

                // Método para comunicar com o cliente
                HandleClient(client, stream);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server.Stop();
        }
        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }
    
    static void HandleClient(TcpClient client, NetworkStream stream) 
    {
        Tarefa clientTask = new Tarefa();

        Byte[] buffer = new Byte[256];

        //Manda mensagem "100 OK" inicial estabalecida no protocolo
        SendMessage("100 OK", stream);

        //Recebe o Id
        int received = stream.Read(buffer, 0, buffer.Length);     
        var receivedMessage = Encoding.ASCII.GetString(buffer, 0, received);
        Console.WriteLine("Received: {0}", receivedMessage);

        //Recebe um Client se encontrar e null se não encontrar
        var currentClient = FindClient(receivedMessage);

        if (currentClient != null)
        {
            if(currentClient.Service.Any())
            {
                SendMessage($"You're client {currentClient.Id}! \n Service: {currentClient.Service}", stream); //Mudar para task
            }
            else
            {
                SendMessage($"You're client {currentClient.Id}! \n Service: Unassigned", stream);
            }
        }
        else
        {
            SendMessage("Unrecognized client. 400 BYE", stream);
            client.Close();
            stream.Close();
            return;
        }
  

        while ((received = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            receivedMessage = Encoding.ASCII.GetString(buffer, 0, received);
            Console.WriteLine("Received: {0}", receivedMessage);

            string[] comando = receivedMessage.Split(' ');

            switch (comando[0])
            {
                case "TASK":
                    if (receivedMessage == "TASK COMPLETED")
                    {
                        //clientTask.UpdateTask("COMPLETED")
                    }
                    else
                    {
                        if (receivedMessage != "TASK NEW")
                        {
                            // ERRO
                        }

                        // Procurar tarefa disponível. É preciso usar uma lista de tarefas
                    }
                    break;
                case "QUIT":
                    SendMessage("400 BYE", stream);
                    client.Close();
                    stream.Close();
                    return;
                default:
                    SendMessage("UNRECOGNISED COMMAND", stream);
                    break;
            }
        }
    }

    static void SendMessage(string message, NetworkStream stream) //Metodo para mandar mensagem ao cliente
    {
        byte[] msgBytes = Encoding.ASCII.GetBytes(message);
        stream.Write(msgBytes, 0, msgBytes.Length);
        Console.WriteLine("Sent: {0}", message);
    }

    static Client? FindClient(string clientId)
    {
        string[] lines = File.ReadAllLines(@"D:\Universidade\3ºAno\2º Semestre\Sistemas Distribuidos\SD_Trabalho_Pratico_1\Server\Data\Alocacao_Cliente_Servico.csv");

        string clientLine = lines.FirstOrDefault(line => line.StartsWith($"{clientId},"));

        if (clientLine != null)
        {
            string[] columns = clientLine.Split(',');

            Client foundClient = new Client(columns[0], columns[1]);

            return foundClient;
        }
        else
        {
            return null;
        }
    }
}