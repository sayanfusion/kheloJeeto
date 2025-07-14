using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DevCommon
{
    public static class DataConverter
    {

        private static readonly DefaultContractResolver contractResolver =
            new DefaultContractResolver
            {
                //NamingStrategy = new SnakeCaseNamingStrategy()
            };

        private static readonly JsonSerializerSettings serializerSettings =
            new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            };

        private static bool customConvertersAdded = false;

        static DataConverter()
        {
            addCustomConverters();
        }

        public static string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, serializerSettings);
        }

        public static T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, serializerSettings);
        }

        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            return JsonConvert.DeserializeAnonymousType<T>(json, anonymousTypeObject, serializerSettings);
        }

        private static void addCustomConverters()
        {
            if (customConvertersAdded)
                return;
            //serializerSettings.Converters.Add(new BooleanConverter().);
        }
    }
}
