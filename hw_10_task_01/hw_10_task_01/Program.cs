using System;
using System.Threading;

class HoneyPot
{
    private int size;
    private readonly int capacity;

    public HoneyPot(int capacity)
    {
        this.capacity = capacity;
    }

    public void Increase()
    {
        Interlocked.Increment(ref size);
    }

    public bool IsFull()
    {
        return size == capacity;
    }

    public void Clear()
    {
        Interlocked.Exchange(ref size, 0);
    }
}

class Program
{
    private static readonly Random random = new Random();

    static void Main(string[] args)
    {
        var honeyPot = new HoneyPot(3);
        var sync = new object();

        // Bear thread
        new Thread(() =>
        {
            while (true)
            {
                lock (sync)
                {
                    while (!honeyPot.IsFull())
                    {
                        Monitor.Wait(sync);
                    }
                    honeyPot.Clear();
                    Console.WriteLine("bear lunch");
                    Monitor.PulseAll(sync);
                }
            }
        }).Start();

        // Bee threads
        for (int i = 0; i < 10; i++)
        {
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(random.Next(100, 200));
                    lock (sync)
                    {
                        while (honeyPot.IsFull())
                        {
                            Monitor.Wait(sync);
                        }
                        honeyPot.Increase();
                        Console.WriteLine("new honey harvest");
                        if (honeyPot.IsFull())
                        {
                            Monitor.PulseAll(sync);
                        }
                    }
                }
            }).Start();
        }
    }
}
