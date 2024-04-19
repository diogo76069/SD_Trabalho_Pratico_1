//*********************** Cliente ***********************
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    public static void Main(string[] args)
    {
        Connect("127.0.0.1", "Cl_0005");
    }

    static void Connect(String server, String id)
    {
        try
        {
            Int32 port = 13000;

            using TcpClient client = new TcpClient(server, port);
            Console.WriteLine("Connected.");

            NetworkStream stream = client.GetStream();

            // Recebe 100 OK
            Byte[] buffer = new Byte[256];
            var received = stream.Read(buffer, 0, buffer.Length);
            String receivedMessage = Encoding.ASCII.GetString(buffer, 0, received);
            Console.WriteLine("Received: {0}", receivedMessage);

            // Envia o Id para receber a tarefa atual
            var message = id;
            byte[] msgBytes = Encoding.ASCII.GetBytes(message);
            stream.Write(msgBytes, 0, msgBytes.Length);
            Console.WriteLine("Sent: {0}", message);

            while (true)
            {
                received = stream.Read(buffer, 0, buffer.Length);
                receivedMessage = Encoding.ASCII.GetString(buffer, 0, received);
                Console.WriteLine("Received: {0}", receivedMessage);

                if(receivedMessage == "400 BYE")
                {
                    Console.WriteLine("Connection with server ended.");
                    stream.Close();
                    client.Close();
                    return;
                }

                Console.Write("Send: ");
                message = Console.ReadLine();
                msgBytes = Encoding.ASCII.GetBytes(message);
                stream.Write(msgBytes, 0, msgBytes.Length);
            }
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }

        Console.WriteLine("\n Press Enter to continue...");
        Console.Read();
    }
}