Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Кинопрокат — Abstract Factory ===");
Console.Write("Введите название фильма: ");
string movie = Console.ReadLine()?.Trim() ?? "Интерстеллар";
if (string.IsNullOrWhiteSpace(movie)) movie = "Интерстеллар";

string language = "Русский";
while (true)
{
    Console.WriteLine("\nВыберите язык: 1 — Русский, 2 — Английский, 3 — Французский, 0 — выход");
    Console.Write($"Текущий язык: {language}. Ваш выбор: ");
    string? choice = Console.ReadLine();
    if (choice == "0") break;
    string? selected = choice switch { "1" => "Русский", "2" => "Английский", "3" => "Французский", _ => null };
    if (selected is null) { Console.WriteLine("Неизвестный выбор. Попробуйте ещё раз."); continue; }

    // При смене языка создаётся новая фабрика, которая поставляет согласованную пару ресурсов.
    IMovieLocalizationFactory factory = selected switch
    {
        "Русский" => new RussianLocalizationFactory(),
        "Английский" => new EnglishLocalizationFactory(),
        _ => new FrenchLocalizationFactory()
    };
    IAudioTrack audio = factory.CreateAudioTrack();
    ISubtitles subtitles = factory.CreateSubtitles();
    language = selected;
    Console.WriteLine($"\nФильм: {movie}");
    Console.WriteLine($"Звуковая дорожка: {audio.Language}");
    Console.WriteLine($"Субтитры: {subtitles.Language}");
    Console.WriteLine($"Совместимость: {(audio.Language == subtitles.Language ? "OK" : "ОШИБКА")}");
}

interface IMovieLocalizationFactory
{
    IAudioTrack CreateAudioTrack();
    ISubtitles CreateSubtitles();
}
interface IAudioTrack { string Language { get; } }
interface ISubtitles { string Language { get; } }

sealed class RussianLocalizationFactory : IMovieLocalizationFactory
{
    public IAudioTrack CreateAudioTrack() => new AudioTrack("Русский");
    public ISubtitles CreateSubtitles() => new SubtitleTrack("Русский");
}
sealed class EnglishLocalizationFactory : IMovieLocalizationFactory
{
    public IAudioTrack CreateAudioTrack() => new AudioTrack("Английский");
    public ISubtitles CreateSubtitles() => new SubtitleTrack("Английский");
}
sealed class FrenchLocalizationFactory : IMovieLocalizationFactory
{
    public IAudioTrack CreateAudioTrack() => new AudioTrack("Французский");
    public ISubtitles CreateSubtitles() => new SubtitleTrack("Французский");
}
sealed record AudioTrack(string Language) : IAudioTrack;
sealed record SubtitleTrack(string Language) : ISubtitles;
