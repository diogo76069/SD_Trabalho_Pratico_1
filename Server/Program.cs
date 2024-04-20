using Server.Model;
using System.Net;
using System.Net.Sockets;
using System.Text;

class MyTcpListener
{
    private static Mutex mutex_ficheiro = new Mutex();
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

                //Thread clientThread = new Thread(() => Main());
                //clientThread.Start();
                HandleClient(client);
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

    static void HandleClient(TcpClient client) 
    {

        NetworkStream stream = client.GetStream();
        string message = "100 OK";

        //Manda mensagem "100 OK" inicial estabalecida no protocolo
        SendMessage(message, stream);

        //Recebe o Id
        Byte[] buffer = new Byte[256];
        int received = stream.Read(buffer, 0, buffer.Length);     
        var receivedMessage = Encoding.ASCII.GetString(buffer, 0, received);
        Console.WriteLine("Received: {0}", receivedMessage);

        // É passado o id recebido pelo cliente como parâmetro
        // Recebe um Client se encontrar e null se não encontrar 
        var currentClient = FindClient(receivedMessage);
        Tarefa? clientTask = null;

        // Se o cliente existir mostra tarefa atual (se tiver) senão termina conexão
        if (currentClient != null)
        {
            if(currentClient.Service.Any())
            {
                // Verifica se tem alguma tarefa em curso
                if((clientTask = currentClient.FindCurrentTask()) != null)
                {
                    message = $"Current task: {clientTask.Description}";
                }
                else
                {
                    message = "Current task: Unassigned. Use 'TASK NEW' to receive a new task.";
                }
            }
            else
            {
                // É associado ao serviço com menos clientes se não tiver alocado
                AssignService(currentClient);
                message = $"Assigned to service {currentClient.Service}. User 'TASK NEW' to receive your first task.";
            }

            SendMessage(message, stream);
        }
        else // Termina conexão se não reconhecer o cliente
        {
            SendMessage("Unrecognized client. 400 BYE", stream);
            client.Close();
            stream.Close();
            return;
        }
  
        // Após validar o id e associar serviço se necessário, aguarda comandos
        while ((received = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            receivedMessage = Encoding.ASCII.GetString(buffer, 0, received);
            Console.WriteLine("Received: {0}", receivedMessage);

            message = string.Empty;

            switch (receivedMessage)
            {
                case "TASK NEW":
                    // Para pedir uma nova tarefa não pode ter nenhuma em curso
                    if(clientTask == null)
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
                    // Apenas pode declarar uma tarefa como concluida se tiver uma tarefa em curso
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
                    //Manda mensagem 400 BYE pedida no protocolo
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

    // Metodo para mandar mensagem ao cliente
    static void SendMessage(string message, NetworkStream stream)
    {
        byte[] msgBytes = Encoding.ASCII.GetBytes(message);
        stream.Write(msgBytes, 0, msgBytes.Length);
        Console.WriteLine("Sent: {0}", message);
    }

    // Metodo para verificar se o cliente existe, procurando pelo id recebido
    static Client? FindClient(string clientId)
    {
        mutex_ficheiro.WaitOne();

        string workingDirectory = Environment.CurrentDirectory;
        string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string filePath = @$"{projectDirectory}\Data\Alocacao_Cliente_Servico.csv";
        string[] lines = File.ReadAllLines(filePath);

        string clientLine = lines.FirstOrDefault(line => line.StartsWith($"{clientId},"));

        if (clientLine != null) // Se o cliente existir é retornado um objeto da classe cliente
        {
            string[] columns = clientLine.Split(',');

            Client foundClient = new Client(columns[0], columns[1]);

            mutex_ficheiro.ReleaseMutex();
            return foundClient;
        }
        else // Se o cliente não existir retorna null
        {
            mutex_ficheiro.ReleaseMutex();
            return null;
        }     
    }

    // Metodo para alocar um serviço ao cliente, caso não esteja alocado
    static int AssignService(Client userClient)
    {
        string service = GetLeastAssignedService();

        string workingDirectory = Environment.CurrentDirectory;
        string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string filePath = @$"{projectDirectory}\Data\Alocacao_Cliente_Servico.csv";

        string[] lines = File.ReadAllLines(filePath);

        int clientLine = Array.FindIndex(lines, line => line.StartsWith($"{userClient.Id},"));

        if (clientLine != -1)
        {
            string line = lines[clientLine];

            string[] colunas = line.Split(",");

            colunas[1] = service;

            userClient.Service = colunas[1];

            line = string.Join(",", colunas);
            lines[clientLine] = line;

            File.WriteAllLines(filePath, lines);

            return 0;
        }
        return 1;
    }

    // Este método devolve uma string com o servico que tem menos clientes
    static string GetLeastAssignedService()
    {
        string service = "Servico_A";

        string filePath = @"D:\Universidade\3ºAno\2º Semestre\Sistemas Distribuidos\SD_Trabalho_Pratico_1\Server\Data\Alocacao_Cliente_Servico.csv";
        string[] lines = File.ReadAllLines(filePath);

        int cA = 0;
        int cB = 0;
        int cC = 0;
        int cD = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string?[] columns = line.Split(",");

            int len = columns.Length;

            if (columns.Length == 2)
            {
                switch (columns[1])
                {
                    case "Servico_A":
                        cA = cA + 1;
                        break;
                    case "Servico_B":
                        cB = cB + 1;
                        break;
                    case "Servico_C":
                        cC = cC + 1;
                        break;
                    case "Servico_D":
                        cD = cD + 1;
                        break;
                    default:
                        break;
                }
            }
        }
        int min = Math.Min(Math.Min(Math.Min(cA, cB), cC), cD);

        if (min == cB)
            service = "Servico_B";

        if (min == cC)
            service = "Servico_C";

        if (min == cD)
            service = "Servico_D";

        return service;
    }
}