using System;
using System.Threading;

class BankAccount
{
    public int Id { get; }
    private double balance;
    private readonly object lockObject = new object();
    private Mutex mutex = new Mutex();

    public BankAccount(int id, double initialBalance)
    {
        Id = id;
        balance = initialBalance;
    }

    public void Deposit(double amount)
    {
        balance += amount;
        Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Deposited {amount:C}, New Balance: {balance:C}");
    }

    public void Withdraw(double amount)
    {
        if (balance >= amount)
        {
            balance -= amount;
            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Withdrew {amount:C}, New Balance: {balance:C}");
        }
        else
        {
            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Insufficient funds!");
        }
    }

    public void SafeDeposit(double amount)
    {
        mutex.WaitOne();
        balance += amount;
        Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Deposited {amount:C} (Thread-Safe), New Balance: {balance:C}");
        mutex.ReleaseMutex();
    }

    public void SafeWithdraw(double amount)
    {
        mutex.WaitOne();
        if (balance >= amount)
        {
            balance -= amount;
            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Withdrew {amount:C} (Thread-Safe), New Balance: {balance:C}");
        }
        else
        {
            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Insufficient funds!");
        }
        mutex.ReleaseMutex();
    }
}

class Program
{
    static void Main()
    {
        BankAccount accountA = new BankAccount(1, 1000);
        int numberOfCustomers = 5; // Adjust this for more stress testing

        while (true)
        {
            Console.WriteLine("\nSelect which phase to run:");
            Console.WriteLine("1. Basic Multi-Threading (Multiple Customers)");
            Console.WriteLine("2. Resource Protection (Mutex)");
            Console.WriteLine("3. Exit");

            Console.Write("Enter your choice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    // Run multiple customer threads
                    Thread[] customerThreads = new Thread[numberOfCustomers];

                    for (int i = 0; i < numberOfCustomers; i++)
                    {
                        customerThreads[i] = new Thread(() =>
                        {
                            accountA.Deposit(100);
                            accountA.Withdraw(50);
                        });
                        customerThreads[i].Start();
                    }

                    foreach (Thread t in customerThreads)
                    {
                        t.Join();
                    }

                    Console.WriteLine("All customer transactions completed.");
                    break;

                case 2:
                    // Safe deposit/withdraw using Mutex
                    Thread[] safeThreads = new Thread[numberOfCustomers];

                    for (int i = 0; i < numberOfCustomers; i++)
                    {
                        safeThreads[i] = new Thread(() =>
                        {
                            accountA.SafeDeposit(100);
                            accountA.SafeWithdraw(50);
                        });
                        safeThreads[i].Start();
                    }

                    foreach (Thread t in safeThreads)
                    {
                        t.Join();
                    }

                    Console.WriteLine("All safe transactions completed.");
                    break;

                case 3:
                    Console.WriteLine("Exiting program...");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }
        }
    }
}
