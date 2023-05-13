using Newtonsoft.Json;

namespace Core
{
    public static class Extensions
    {
        public static T Deserialize<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch { return default; }
        }

        public static string Serialize(this object Object) =>
           JsonConvert.SerializeObject(Object);
    }
}
