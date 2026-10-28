Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Фигуры — Factory Method + Prototype ===");
var creator = new RandomFigureCreator();
var figures = new List<Figure>();

for (int i = 0; i < 6; i++)
{
    Figure figure = creator.CreateFigure();
    figures.Add(figure);
    Console.WriteLine($"{i + 1}. {figure.Name}; клеток: {figure.CellCount}; тип: {(figure.IsSuper ? "суперфигура" : "обычная фигура")}");
}

Console.WriteLine("\nСоздаём копию первой фигуры с помощью Prototype:");
Figure original = figures[0];
Figure copy = original.Clone();
Console.WriteLine($"Оригинал: {original.Name}, клеток: {original.CellCount}, ID объекта: {original.Id}");
Console.WriteLine($"Копия:    {copy.Name}, клеток: {copy.CellCount}, ID объекта: {copy.Id}");
Console.WriteLine($"Это разные объекты: {!ReferenceEquals(original, copy)}");
Console.WriteLine($"Характеристики совпадают: {original.Name == copy.Name && original.CellCount == copy.CellCount && original.IsSuper == copy.IsSuper}");

abstract class Figure
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }
    public int CellCount { get; }
    public bool IsSuper { get; }
    protected Figure(string name, int cellCount, bool isSuper) { Name = name; CellCount = cellCount; IsSuper = isSuper; }
    public abstract Figure Clone();
}
sealed class SquareFigure : Figure
{
    public SquareFigure(int cells, bool isSuper) : base(isSuper ? "Суперквадрат" : "Квадрат", cells, isSuper) { }
    public override Figure Clone() => new SquareFigure(CellCount, IsSuper);
}
sealed class LineFigure : Figure
{
    public LineFigure(int cells, bool isSuper) : base(isSuper ? "Суперлиния" : "Линия", cells, isSuper) { }
    public override Figure Clone() => new LineFigure(CellCount, IsSuper);
}
sealed class LFigure : Figure
{
    public LFigure(int cells, bool isSuper) : base(isSuper ? "Суперфигура L" : "Фигура L", cells, isSuper) { }
    public override Figure Clone() => new LFigure(CellCount, IsSuper);
}

// Factory Method: подкласс определяет, какой конкретный объект создать.
abstract class FigureCreator { public abstract Figure CreateFigure(); }
sealed class RandomFigureCreator : FigureCreator
{
    private readonly Random _random = new();
    public override Figure CreateFigure()
    {
        bool isSuper = _random.Next(0, 4) == 0; // вероятность суперфигуры — 25%
        int cells = isSuper ? _random.Next(6, 10) : _random.Next(3, 6);
        return _random.Next(0, 3) switch
        {
            0 => new SquareFigure(cells, isSuper),
            1 => new LineFigure(cells, isSuper),
            _ => new LFigure(cells, isSuper)
        };
    }
}
