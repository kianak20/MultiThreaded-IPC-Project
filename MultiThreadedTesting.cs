using System;
using System.Threading;

class ThreadingTests
{
    static object lockObject = new object();
    static int sharedBalance = 1000;  // Simulated shared resource

    static void Main()
    {
        Console.WriteLine("Select Test:");
        Console.WriteLine("1 - Concurrency Test");
        Console.WriteLine("2 - Synchronization Validation");
        Console.WriteLine("3 - Stress Test");
        Console.Write("Enter choice (1-3): ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ConcurrencyTest();
                break;
            case "2":
                SynchronizationTest();
                break;
            case "3":
                StressTest();
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }

    // **Test 1: Concurrency Test**
    static void ConcurrencyTest()
    {
        Console.WriteLine("Starting Concurrency Test...");
        Thread customer1 = new Thread(SimulateBankCustomer);
        Thread customer2 = new Thread(SimulateBankCustomer);

        customer1.Start();
        customer2.Start();

        customer1.Join();
        customer2.Join();

        Console.WriteLine(" Concurrency Test Completed.");
    }

    static void SimulateBankCustomer()
    {
        Console.WriteLine($"Customer {Thread.CurrentThread.ManagedThreadId} is being served.");
        Thread.Sleep(500);  // Simulate transaction processing time
        Console.WriteLine($"Customer {Thread.CurrentThread.ManagedThreadId} transaction completed.");
    }

    // **Test 2: Synchronization Test (Prevents Race Condition)**
    static void SynchronizationTest()
    {
        Console.WriteLine("Starting Synchronization Test...");

        Thread customer1 = new Thread(WithdrawWithLock);
        Thread customer2 = new Thread(WithdrawWithLock);

        customer1.Start();
        customer2.Start();

        customer1.Join();
        customer2.Join();

        Console.WriteLine($" Final balance after synchronized transactions: ${sharedBalance}");
    }

    static void WithdrawWithLock()
    {
        Random rand = new Random();
        int withdrawalAmount = rand.Next(100, 500);

        lock (lockObject)
        {
            if (sharedBalance >= withdrawalAmount)
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} is withdrawing ${withdrawalAmount}...");
                Thread.Sleep(200);  // Simulate processing time
                sharedBalance -= withdrawalAmount;
                Console.WriteLine($" Withdrawal successful! New balance: ${sharedBalance}");
            }
            else
            {
                Console.WriteLine($" Thread {Thread.CurrentThread.ManagedThreadId} failed to withdraw. Insufficient funds.");
            }
        }
    }

    // **Test 3: Stress Test (Simulates Many Customers)**
    static void StressTest()
    {
        Console.WriteLine("Starting Stress Test...");
        int numCustomers = 20;  // Simulating 20 concurrent customers
        Thread[] customers = new Thread[numCustomers];

        for (int i = 0; i < numCustomers; i++)
        {
            customers[i] = new Thread(SimulateBankCustomer);
            customers[i].Start();
        }

        foreach (Thread t in customers)
        {
            t.Join();
        }

        Console.WriteLine("Stress Test Completed.");
    }
}
