Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Лифт в здании — Singleton ===");
Building building1 = Building.Instance;
Building building2 = Building.Instance;
Console.WriteLine($"Здание одно и то же: {ReferenceEquals(building1, building2)}");
Console.WriteLine($"Лифт один и тот же: {ReferenceEquals(building1.Lift, building2.Lift)}");

building1.AddFloor(new Floor(1, new[] { "Квартира 1", "Квартира 2" }));
building1.AddFloor(new Floor(2, new[] { "Офис 201", "Офис 202" }));
Console.WriteLine($"Количество этажей: {building1.Floors.Count}");
foreach (Floor floor in building1.Floors)
    Console.WriteLine($"Этаж {floor.Number}: {string.Join(", ", floor.Rooms)}");

building1.Lift.MoveTo(2);
building1.Lift.MoveTo(1);

sealed class Building
{
    private static readonly Lazy<Building> _instance = new(() => new Building());
    public static Building Instance => _instance.Value;
    public List<Floor> Floors { get; } = new();
    public Lift Lift { get; } = Lift.Instance;
    private Building() { }
    public void AddFloor(Floor floor)
    {
        if (Floors.Any(f => f.Number == floor.Number)) throw new InvalidOperationException("Этаж с таким номером уже существует.");
        Floors.Add(floor);
        Floors.Sort((a, b) => a.Number.CompareTo(b.Number));
    }
}

sealed class Lift
{
    private static readonly Lazy<Lift> _instance = new(() => new Lift());
    public static Lift Instance => _instance.Value;
    public int CurrentFloor { get; private set; } = 1;
    private Lift() { }
    public void MoveTo(int floor)
    {
        if (floor < 1) { Console.WriteLine("Ошибка: номер этажа должен быть положительным."); return; }
        Console.WriteLine($"Лифт перемещается с этажа {CurrentFloor} на этаж {floor}.");
        CurrentFloor = floor;
        Console.WriteLine($"Лифт находится на этаже {CurrentFloor}.");
    }
}

sealed record Floor(int Number, IReadOnlyList<string> Rooms);
