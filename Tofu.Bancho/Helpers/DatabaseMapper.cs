using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tofu.Bancho.Helpers {
    public static class DatabaseMapper {
        public static void MapDatabaseResults<pType>(this pType objectToMap, IReadOnlyDictionary<string, object> mappingDictionary) where pType : class, new() {
            PropertyInfo[] properties = typeof(pType).GetProperties();

            Dictionary<string, object> renamedDictionary = mappingDictionary.ToDictionary(keyValuePair => keyValuePair.Key.Replace("_", ""), keyValuePair => keyValuePair.Value);

            foreach (PropertyInfo property in properties) {
                object value = renamedDictionary.GetValueOrDefault(property.Name.ToLower(), null);

                if(value != null)
                    property.SetValue(objectToMap, value);
            }
        }
    }
}
