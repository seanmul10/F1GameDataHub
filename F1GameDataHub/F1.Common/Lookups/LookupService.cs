using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace F1.Common.Lookups
{
    public class LookupService<T> : ILookupService<T> where T : LookupItem
    {
        private readonly Lazy<IReadOnlyList<T>> _items;

        private static readonly JsonSerializerOptions _cachedOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private const string _basePath = "StaticData";

        public LookupService(string file)
        {
            var path = Path.Combine(AppContext.BaseDirectory, _basePath, file);
            _items = new Lazy<IReadOnlyList<T>>(() =>
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<T>>(json, _cachedOptions)
                       ?? throw new InvalidOperationException($"Failed to load data from {path}");
            });
        }

        public IReadOnlyList<T> GetAll() => _items.Value;

        public T? GetById(int id) => _items.Value.FirstOrDefault(i => i.Id == id);
    }
}
