using Newtonsoft.Json;
using System.Reflection;

namespace FiniteGroupCalc.PersistableCachers
{
    public class PersistableUlongListCacher
    {
        private static readonly string _cacheFileName = "PersistableUlongListCache.json";
        private static readonly string _appExePath = Path.GetDirectoryName(
            Assembly.GetExecutingAssembly()?.Location
            ?? Environment.CurrentDirectory
            ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) ?? ".";
        private static readonly string _cacheFilePath = Path.Combine(_appExePath, _cacheFileName);
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
        };
        private static readonly object _lock = new object();
        private static Dictionary<string, ulong[]> _cache = new Dictionary<string, ulong[]>();

        public static ulong[] Get(string key) => _cache[key];
        public static void Set(string key, ulong[] value)
        {
            if (_cache.ContainsKey(key) && _cache[key] == value) return;
            lock (_lock)
            {
                _cache[key] = value;
                File.WriteAllText(_cacheFilePath, JsonConvert.SerializeObject(_cache, _jsonSettings));
            }
        }

        public static int[] GetInts(string key) => Get(key).Select(i => (int)i).ToArray();
        public static void SetInts(string key, int[] value) => Set(key, value.Select(i => (ulong)i).ToArray());
        public static void Init()
        {
            lock (_lock)
            {
                if (File.Exists(_cacheFilePath))
                {
                    string json = File.ReadAllText(_cacheFilePath);
                    _cache = JsonConvert.DeserializeObject<Dictionary<string, ulong[]>>(json, _jsonSettings) ?? new Dictionary<string, ulong[]>();

                }
                else _cache = new Dictionary<string, ulong[]>();
            }
        }

        public static bool Contains(string key) => _cache.ContainsKey(key);
    }


}