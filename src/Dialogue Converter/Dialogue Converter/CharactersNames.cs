namespace Dialogue_Converter;

public class CharactersNames
{
    public CharactersNames(string path)
    {
        foreach (var value in File.ReadAllLines(path))
        {
            var index = value.IndexOf('-');
            _dictionary.Add(value[..index], value[(index + 1)..]);
        }
    }

    private readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>();
    public bool TryGetCharacterName(string key, out string? name) => _dictionary.TryGetValue(key, out name);
}
