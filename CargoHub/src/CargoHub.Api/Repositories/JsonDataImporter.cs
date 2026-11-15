
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using CargoHub.Api.Data;

namespace CargoHub.Api.Repositories;

public class JsonDataImporter
{
    private readonly AppDbContext _context;

    public JsonDataImporter(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> ImportAsync<T>(string filePath)
        where T : class
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"JSON import file was not found: {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        var records = JsonSerializer.Deserialize<List<T>>(json, options);

        if (records == null || records.Count == 0)
        {
            Console.WriteLine($"No records found in {filePath}");
            return 0;
        }

        int imported = 0;
        int skipped = 0;
        int recordNumber = 0;

        var idProperty = typeof(T).GetProperty("Id");

        foreach (var record in records)
        {
            recordNumber++;

            try
            {
                // Let PostgreSQL generate a new primary key.
                if (idProperty != null)
                {
                    if (idProperty.PropertyType == typeof(int))
                    {
                        idProperty.SetValue(record, 0);
                    }
                    else if (idProperty.PropertyType == typeof(long))
                    {
                        idProperty.SetValue(record, 0L);
                    }
                }

                _context.Set<T>().Add(record);

                await _context.SaveChangesAsync();

                imported++;
            }
            catch (DbUpdateException ex)
            {
                // Detach the failed record so the next one can be imported.
                _context.Entry(record).State = EntityState.Detached;

                skipped++;

                Console.WriteLine(
                    $"Skipped {typeof(T).Name} record #{recordNumber}: " +
                    $"{ex.InnerException?.Message ?? ex.Message}");
            }
        }

        Console.WriteLine($"Import completed: {typeof(T).Name}");
        Console.WriteLine($"Successfully imported: {imported}");
        Console.WriteLine($"Skipped: {skipped}");
        Console.WriteLine($"Total records: {records.Count}");

        return imported;
    }
}
