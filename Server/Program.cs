using Server.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
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
    
    static void HandleClient(TcpClient client, NetworkStream stream) //Qualquer coisa relacionada com comunicação com cliente vai aqui
    {
        Tarefa clientTask = new Tarefa();

        //Enviar mensagem inicial (100 OK) pedido no protocolo
        SendMessage("100 OK", stream);

        // Estas variaveis servem para receber mensagens do cliente
        Byte[] buffer = new Byte[256];
        int received;

        while ((received = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            string receivedMessage = Encoding.ASCII.GetString(buffer, 0, received); //Esta variavel guarda a mensagem do cliente
            Console.WriteLine("Received: {0}", receivedMessage);

            string msgType = receivedMessage.Split(' ').FirstOrDefault();

            //É preciso fazer verificações para ver se a tarefa pode ser concluida
            //Por exemplo para usar TASK COMPLETED é necesssário ter uma tarefa atribuida... etc.
            //Não sei se é melhor fazer isso aqui ou no model

            switch (msgType)
            {
                case "ID":
                    //FindClient("id")
                    //Deve haver uma fução que lê o ficheiro dos clientes e ver se existe
                    //Se existir tem que ver em que serviço está alocado, e alocar se ainda n estiver alocado
                    //Criar metodo FindCurrentTask("idCliente") para encontrar tarefa atual (se estiver alocada)
                    //Enviar mensagem CURRENT TASK: UNASSIGNED/*tarefa atual*
                    break;
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
                    //Ao fechar conexão dá erro, talvez seja preciso fazer um break no while
                    //Nesse caso é preciso meter este código no início do loop
                    //É preciso testar
                    SendMessage("400 BYE", stream);
                    client.Close();
                    stream.Close();
                    break;
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

    static string? FindClient(string idCliente)
    {
        //string[] lines = File.ReadAllLines();

        return null;
    }
}