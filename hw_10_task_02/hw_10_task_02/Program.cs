
class Map
{
    public Map(int size)
    {
        sheep_locations = new Dictionary<int, (int x, int y)>();
        wolf.x = 0;
        wolf.y = 0; // starting wolf location 
        n = size;
        numOfSheep = 3;
        map = new int[size, size];
        Random random = new Random();
        sheep_locations.Add(1, (random.Next(0, size), random.Next(0, size))); // random locations for each sheep
        sheep_locations.Add(2, (random.Next(0, size), random.Next(0, size)));
        sheep_locations.Add(3, (random.Next(0, size), random.Next(0, size)));
    }

    public void MoveWolf(int direction)
    {
        switch (direction)
        {
            case 1: wolf.y = Math.Min(wolf.y + 1, n - 1); break; // up
            case 2: wolf.y = Math.Max(wolf.y - 1, 0); break; // down
            case 3: wolf.x = Math.Max(wolf.x - 1, 0); break; // left
            case 4: wolf.x = Math.Min(wolf.x + 1, n - 1); break; // right
        }
    }

    public int SheepMove(int sheepNumber, int direction)
    {
        var sheep = sheep_locations[sheepNumber];
        if (sheep == wolf) // check if there are both sheep and wolf in one cell
        {
            Console.WriteLine($"Sheep died at {sheep.x},{sheep.y}");
            map[sheep.x, sheep.y]--; // sheep is dead 
            sheep_locations.Remove(sheepNumber);
            return -1;
        }
        map[sheep.x, sheep.y]--;
        switch (direction)
        {
            case 1: sheep.y = Math.Min(sheep.y + 1, n - 1); break;
            case 2: sheep.y = Math.Max(sheep.y - 1, 0); break;
            case 3: sheep.x = Math.Max(sheep.x - 1, 0); break;
            case 4: sheep.x = Math.Min(sheep.x + 1, n - 1); break;
        }
        map[sheep.x, sheep.y]++;
        if (map[sheep.x, sheep.y] >= 2) // we can born new sheep
        {
            map[sheep.x, sheep.y]++; // new sheep was born!
            sheep_locations.Add(++numOfSheep, (sheep.x, sheep.y));
            return numOfSheep;
        }
        else
        {
            if (sheep == wolf)
            {
                Console.WriteLine($"Sheep died at {sheep.x},{sheep.y}");
                map[sheep.x, sheep.y]--; // died sheep
                sheep_locations.Remove(sheepNumber);
                return -1;
            }
            return 0;
        }
    }

    private Dictionary<int, (int x, int y)> sheep_locations;
    private (int x, int y) wolf;
    private int n;
    private int numOfSheep;
    private int[,] map;
}

class Program
{
    static Random random = new Random();

    static int RandomMove()
    {
        int dir = random.Next(1, 5);
        return dir;
    }

    static Thread BornNewSheep(int SheepNum, Map map)
    {
        return new Thread(() =>
        {
            while (true)
            {
                Thread.Sleep((random.Next() % 100) + 100);
                lock (map)
                {
                    var result = map.SheepMove(SheepNum, RandomMove());
                    if (result == -1)
                    {
                        return;
                    }
                    if (result > 0)
                    {
                        Console.WriteLine("new sheep was born");
                        BornNewSheep(result, map).Start();
                    }
                }
            }
        });
    }

    static void Main(string[] args)
    {
        Map map = new Map(3);
        var threads = new List<Thread>();
        (new Thread(() =>
        {
            while (true)
            {
                Thread.Sleep((random.Next() % 100) + 100);
                lock (map)
                {
                    map.MoveWolf(RandomMove());
                }
            }
        })).Start();
        BornNewSheep(1, map).Start();
        BornNewSheep(2, map).Start();
        BornNewSheep(3, map).Start();
    }
}
