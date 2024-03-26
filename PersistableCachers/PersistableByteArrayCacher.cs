using System.Reflection;

namespace FiniteGroupCalc.PersistableCachers
{
    public class PersistableByteArrayCacher
    {
        private static readonly string _appExePath = Path.GetDirectoryName(
            Assembly.GetExecutingAssembly()?.Location
            ?? Environment.CurrentDirectory
            ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) ?? ".";

        private static readonly object _lock = new object();

        public static sbyte[]? Get(string key)
        {
            var fileName = Path.Combine(_appExePath, "Cache_" + key + ".bin");
            if (File.Exists(fileName))
            {
                return File.ReadAllBytes(fileName).Select(b => (sbyte)b).ToArray();
            }
            return null;
        }
        public static void Set(string key, sbyte[] value)
        {
            lock (_lock)
            {
                var fileName = Path.Combine(_appExePath, "Cache_" + key + ".bin");
                File.WriteAllBytes(fileName, value.Select(b => (byte)b).ToArray());
            }
        }
        public static bool Contains(string key) => File.Exists(Path.Combine(_appExePath, "Cache_" + key + ".bin"));
    }


}