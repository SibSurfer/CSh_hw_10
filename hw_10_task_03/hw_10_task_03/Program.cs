
class BarberShop
{
    private readonly int maxWaitingChairs;
    private readonly Queue<Visitor> waitingChairs;
    private bool isBarberSleeping;

    public BarberShop(int maxWaitingChairs)
    {
        this.maxWaitingChairs = maxWaitingChairs;
        waitingChairs = new Queue<Visitor>();
        isBarberSleeping = true;
    }

    public void EnterBarberShop(Visitor visitor)
    {
        lock (waitingChairs)
        {
            if (waitingChairs.Count < maxWaitingChairs)
            {
                Console.WriteLine($"{visitor.Name} enters and sits in a waiting chair.");
                waitingChairs.Enqueue(visitor);
                if (isBarberSleeping)
                {
                    WakeUpBarber();
                }
            }
            else
            {
                Console.WriteLine($"{visitor.Name} enters but leaves because all chairs are occupied.");
                
            }
        }
    }

    public void WakeUpBarber()
    {
        isBarberSleeping = false;
        Console.WriteLine("Barber wakes up.");
        Thread barberThread = new Thread(StartHaircut);
        barberThread.Start();
    }

    private void StartHaircut()
    {
        while (true)
        {
            Visitor visitor;
            lock (waitingChairs)
            {
                if (waitingChairs.Count > 0)
                {
                    visitor = waitingChairs.Dequeue();
                    Console.WriteLine($"Barber starts haircut for {visitor.Name}.");
                }
                else
                {
                    isBarberSleeping = true;
                    Console.WriteLine("Barber falls asleep.");
                    return;
                }
            }

            Thread.Sleep(TimeSpan.FromSeconds(3)); // Simulating haircut process

            Console.WriteLine($"Barber finishes haircut for {visitor.Name}.");
        }
    }
}

class Visitor
{
    public string Name { get; }

    public Visitor(string name)
    {
        Name = name;
    }
}

class Program
{
    static void Main(string[] args)
    {
        BarberShop barberShop = new BarberShop(3);

        Thread visitorThread = new Thread(() =>
        {
            for (int i = 1; i <= 6; i++)
            {
                barberShop.EnterBarberShop(new Visitor($"Visitor {i}"));
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        });

        visitorThread.Start();

        visitorThread.Join();
    }
}
