using System.Text.Json;
using System.Text.Json.Serialization;

namespace PharmacyApi.Services;

public class JsonFileStore<T>
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public JsonFileStore(IWebHostEnvironment environment, string fileName)
    {
        var dataDirectory = Path.Combine(environment.ContentRootPath, "Data");
        Directory.CreateDirectory(dataDirectory);
        _filePath = Path.Combine(dataDirectory, fileName);

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public async Task<List<T>> ReadAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? [];
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task WriteAllAsync(List<T> items)
    {
        await _lock.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(items, JsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _lock.Release();
        }
    }
}
