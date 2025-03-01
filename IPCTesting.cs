using System;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;

class IPCTests
{
    static void Main()
    {
        Console.WriteLine("Select Test:");
        Console.WriteLine("1 - Data Integrity Test");
        Console.WriteLine("2 - Error Handling Test");
        Console.WriteLine("3 - Performance Benchmarking");
        Console.Write("Enter choice (1-3): ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                TestDataIntegrity();
                break;
            case "2":
                TestErrorHandling();
                break;
            case "3":
                TestPerformance();
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }

    static void TestDataIntegrity()
    {
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("my_pipe", PipeDirection.Out))
        {
            Console.WriteLine("Data Integrity Test: Waiting for consumer...");
            pipeServer.WaitForConnection();

            using (StreamWriter writer = new StreamWriter(pipeServer))
            {
                string testMessage = "TEST12345";
                writer.WriteLine(testMessage);
                Console.WriteLine($"Sent: {testMessage}");
            }
        }

        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "my_pipe", PipeDirection.In))
        {
            pipeClient.Connect();

            using (StreamReader reader = new StreamReader(pipeClient))
            {
                string receivedMessage = reader.ReadLine();
                Console.WriteLine($"Received: {receivedMessage}");

                Console.WriteLine(receivedMessage == "TEST12345" ? "✅ Test Passed!" : "❌ Test Failed!");
            }
        }
    }

    static void TestErrorHandling()
    {
        try
        {
            Console.WriteLine("Error Handling Test: Attempting to read from a closed pipe...");
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "non_existent_pipe", PipeDirection.In))
            {
                pipeClient.Connect(1000); // 1-second timeout
                using (StreamReader reader = new StreamReader(pipeClient))
                {
                    string message = reader.ReadLine();
                    Console.WriteLine($"Received: {message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Test Passed! Caught Exception: {ex.Message}");
        }
    }

    static void TestPerformance()
    {
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("my_pipe", PipeDirection.Out))
        {
            Console.WriteLine("Performance Test: Waiting for consumer...");
            pipeServer.WaitForConnection();

            using (StreamWriter writer = new StreamWriter(pipeServer))
            {
                string largeData = new string('A', 1000000); // 1MB of data

                Stopwatch stopwatch = Stopwatch.StartNew();
                writer.WriteLine(largeData);
                stopwatch.Stop();

                Console.WriteLine($" Sent 1MB of data in {stopwatch.ElapsedMilliseconds} ms");
            }
        }

        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "my_pipe", PipeDirection.In))
        {
            pipeClient.Connect();

            using (StreamReader reader = new StreamReader(pipeClient))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                string receivedData = reader.ReadToEnd();
                stopwatch.Stop();

                Console.WriteLine($" Received 1MB of data in {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
