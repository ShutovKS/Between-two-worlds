namespace Dialogue_Converter;

public class CharactersAvatars
{
    public CharactersAvatars(string path)
    {
        foreach (var value in File.ReadAllLines(path + ADDING_TO_PATH))
        {
            var index = value.IndexOf('-');
            _dictionary.Add(value[..index], value[(index + 1)..]);
        }
    }

    private const string ADDING_TO_PATH = @"\CharactersAvatars.txt";
    private readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>();

    public bool TryGetCharacterAvatarPath(string key, out string? avatarPath) => _dictionary.TryGetValue(key, out avatarPath);
}
