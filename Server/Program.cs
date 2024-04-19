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

        string message = "100 OK";

        //Manda mensagem "100 OK" inicial estabalecida no protocolo
        SendMessage(message, stream);

        //Recebe o Id
        Byte[] buffer = new Byte[256];
        int received = stream.Read(buffer, 0, buffer.Length);     
        var receivedMessage = Encoding.ASCII.GetString(buffer, 0, received);
        Console.WriteLine("Received: {0}", receivedMessage);

        //Recebe um Client se encontrar e null se não encontrar
        var currentClient = FindClient(receivedMessage);
        Tarefa? clientTask = null;

        if (currentClient != null)
        {
            if(currentClient.Service.Any())
            {
                if((clientTask = currentClient.FindCurrentTask()) != null)
                {
                    message = $"Current task: {clientTask.Description}";
                }
                else
                {
                    message = "Current task: Unassigned. Use 'TASK NEW' to assign a new task.";
                }
            }
            else
            {
                message = "No service found"; // Mudar isto para atribuir um serviço novo automaticamente se houver tempo
            }

            SendMessage(message, stream);
        }
        else // Por enquanto termina a conexão se O cliente não existir. Talvez adicionar maneira de criar um novo»
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

            message = string.Empty;

            switch (receivedMessage)
            {
                case "TASK NEW":
                    if(clientTask == null) // O cliente não deve ter uma tarefa em curso
                    {
                        clientTask = new Tarefa();
                        if(clientTask.AssignClient(currentClient) == 0)
                            message = $"New task:{clientTask.Description}";
                        
                        else
                            message = "No tasks available";
                       
                    }
                    else
                        message = "Unable to use command.";

                    break;
                case "TASK COMPLETED":
                    if(clientTask != null)
                    {
                        if (clientTask.FinishTask(currentClient.Service) == 0)
                            message = "Task state updated successfuly.";
                        
                        else
                            message = "An error occurred.";
                        
                    }
                    else
                        message = "You don't have an assigned task. Use TASK NEW to get a new task.";

                    break;
                case "QUIT":
                    SendMessage("400 BYE", stream);
                    client.Close();
                    stream.Close();
                    return;
                default:
                    message = "Unrecognised command.";
                    break;
            }

            SendMessage (message, stream);
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
        string filePath = @"D:\Universidade\3ºAno\2º Semestre\Sistemas Distribuidos\SD_Trabalho_Pratico_1\Server\Data\Alocacao_Cliente_Servico.csv";
        string[] lines = File.ReadAllLines(filePath);

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