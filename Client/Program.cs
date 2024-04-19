//*********************** Cliente ***********************
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    public static void Main(string[] args)
    {
        Connect("127.0.0.1", "Cl_0001");
    }

    static void Connect(String server, String id)
    {
        try
        {
            Int32 port = 13000;

            using TcpClient client = new TcpClient(server, port);
            Console.WriteLine("Connected.");

            String receivedMessage = String.Empty;
            Byte[] buffer = new Byte[256];

            NetworkStream stream = client.GetStream();

            var message = id;
            byte[] msgBytes = Encoding.ASCII.GetBytes(message);
            stream.Write(msgBytes, 0, msgBytes.Length);

            while (true)
            {
                var received = stream.Read(buffer, 0, buffer.Length);
                receivedMessage = Encoding.ASCII.GetString(buffer, 0, received);
                Console.WriteLine("Received: {0}", receivedMessage);

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