using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

class IPCProgram
{
    static void Main()
    {
        Console.WriteLine("Select Mode:");
        Console.WriteLine("1 - Producer (Send Data)");
        Console.WriteLine("2 - Consumer (Receive Data)");
        Console.Write("Enter your choice (1/2): ");

        string choice = Console.ReadLine();

        if (choice == "1")
        {
            RunProducer();
        }
        else if (choice == "2")
        {
            RunConsumer();
        }
        else
        {
            Console.WriteLine("Invalid choice. Exiting...");
        }
    }

    static void RunProducer()
    {
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("my_pipe", PipeDirection.Out))
        {
            Console.WriteLine("Producer: Waiting for consumer to connect...");
            pipeServer.WaitForConnection();

            using (StreamWriter writer = new StreamWriter(pipeServer))
            {
                writer.AutoFlush = true;

                while (true)
                {
                    Console.Write("Enter message to send (or type 'exit' to stop): ");
                    string message = Console.ReadLine();

                    if (message.ToLower() == "exit")
                    {
                        Console.WriteLine("Producer: Exiting...");
                        break;
                    }

                    writer.WriteLine(message);
                    Console.WriteLine("Producer: Message sent.");
                }
            }
        }
    }

    static void RunConsumer()
    {
        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "my_pipe", PipeDirection.In))
        {
            Console.WriteLine("Consumer: Connecting to producer...");
            pipeClient.Connect();

            using (StreamReader reader = new StreamReader(pipeClient))
            {
                while (true)
                {
                    string message = reader.ReadLine();
                    
                    if (string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine("Consumer: No more messages. Exiting...");
                        break;
                    }

                    Console.WriteLine($"Consumer received: {message}");
                }
            }
        }
    }
}




