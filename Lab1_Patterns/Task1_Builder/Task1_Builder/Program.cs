using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

// Формат article.txt:
// Заголовок: Название статьи
// Авторы: Иванов, Петров
// Текст:
// Текст статьи в одной или нескольких строках.
// Хеш: <SHA-256 в шестнадцатеричном виде>

const string inputPath = "article.txt";
const string outputPath = "article.json";

if (!File.Exists(inputPath))
{
    const string title = "Порождающие паттерны проектирования";
    string body = "Паттерны проектирования помогают организовывать программный код.\n" +
                  "Builder позволяет пошагово создавать сложные объекты.";
    string hash = CalculateSha256(body);
    File.WriteAllText(inputPath,
        $"Заголовок: {title}\nАвторы: Иванов, Петров\nТекст:\n{body}\nХеш: {hash}\n",
        new UTF8Encoding(false));
    Console.WriteLine($"Создан пример входного файла: {inputPath}");
}

try
{
    string source = File.ReadAllText(inputPath, Encoding.UTF8);
    Article article = ArticleParser.Parse(source);
    string actualHash = CalculateSha256(article.Text);
    bool hashIsValid = string.Equals(actualHash, article.Hash, StringComparison.OrdinalIgnoreCase);

    Console.WriteLine($"Статья: {article.Title}");
    Console.WriteLine($"Авторы: {string.Join(", ", article.Authors)}");
    Console.WriteLine($"Проверка SHA-256: {(hashIsValid ? "УСПЕШНО" : "ОШИБКА — хеш не совпадает")}");
    Console.WriteLine($"Ожидаемый хеш: {article.Hash}");
    Console.WriteLine($"Вычисленный хеш: {actualHash}");

    // Конвертируем документ в JSON даже при неверном хеше, но явно сохраняем результат проверки.
    var builder = new ArticleJsonBuilder();
    string json = builder.SetTitle(article.Title)
        .SetAuthors(article.Authors)
        .SetText(article.Text)
        .SetHash(article.Hash)
        .SetHashValid(hashIsValid)
        .Build();

    File.WriteAllText(outputPath, json, new UTF8Encoding(false));
    Console.WriteLine($"JSON сохранён в файл: {Path.GetFullPath(outputPath)}");
    if (!hashIsValid) Environment.ExitCode = 1;
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка обработки файла: {ex.Message}");
    Environment.ExitCode = 1;
}

static string CalculateSha256(string text)
{
    byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
    return Convert.ToHexString(bytes).ToLowerInvariant();
}

record Article(string Title, List<string> Authors, string Text, string Hash);

static class ArticleParser
{
    public static Article Parse(string source)
    {
        string normalized = source.Replace("\r\n", "\n").Replace('\r', '\n');
        string[] lines = normalized.Split('\n');
        if (lines.Length < 4 || !lines[0].StartsWith("Заголовок: ") || !lines[1].StartsWith("Авторы: ") || lines[2] != "Текст:")
            throw new FormatException("Ожидается формат: Заголовок:, Авторы:, Текст:, текст, Хеш:.");

        int hashLine = Array.FindLastIndex(lines, line => line.StartsWith("Хеш: "));
        if (hashLine < 3) throw new FormatException("Не найдена строка 'Хеш:'.");
        string title = lines[0]["Заголовок: ".Length..].Trim();
        List<string> authors = lines[1]["Авторы: ".Length..]
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
        string text = string.Join("\n", lines[3..hashLine]);
        string hash = lines[hashLine]["Хеш: ".Length..].Trim();
        if (string.IsNullOrWhiteSpace(title)) throw new FormatException("Заголовок не может быть пустым.");
        if (authors.Count == 0) throw new FormatException("Укажите хотя бы одного автора.");
        if (string.IsNullOrWhiteSpace(text)) throw new FormatException("Текст статьи не может быть пустым.");
        if (hash.Length != 64 || !hash.All(Uri.IsHexDigit)) throw new FormatException("Хеш должен содержать 64 шестнадцатеричных символа SHA-256.");
        return new Article(title, authors, text, hash);
    }
}

// Builder: поэтапно собирает JSON-представление статьи.
sealed class ArticleJsonBuilder
{
    private string _title = "";
    private List<string> _authors = new();
    private string _text = "";
    private string _hash = "";
    private bool _hashValid;

    public ArticleJsonBuilder SetTitle(string value) { _title = value; return this; }
    public ArticleJsonBuilder SetAuthors(List<string> value) { _authors = value; return this; }
    public ArticleJsonBuilder SetText(string value) { _text = value; return this; }
    public ArticleJsonBuilder SetHash(string value) { _hash = value; return this; }
    public ArticleJsonBuilder SetHashValid(bool value) { _hashValid = value; return this; }
    public string Build() => JsonSerializer.Serialize(new
    {
        title = _title,
        authors = _authors,
        text = _text,
        hash = _hash,
        hashAlgorithm = "SHA-256",
        hashValid = _hashValid
    }, new JsonSerializerOptions { WriteIndented = true });
}
