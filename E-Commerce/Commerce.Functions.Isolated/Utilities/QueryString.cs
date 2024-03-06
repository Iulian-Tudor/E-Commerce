using System.Web;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Commerce.Functions.Isolated;

public class QueryString
{
    public static T Parse<T>(string query) where T : new()
    {
        var instance = new T();
        var properties = typeof(T).GetProperties();
        var parsedQuery = HttpUtility.ParseQueryString(query);

        foreach (var property in properties)
        {
            var queryValue = Convert(property, parsedQuery);
            if (queryValue != null)
            {
                property.SetValue(instance, queryValue);
            }
        }

        return instance;
    }

    private static object Convert(PropertyInfo property, NameValueCollection query)
    {
        var key = property.Name[..1].ToLower() + property.Name[1..];
        var isCollectionType = property.PropertyType != typeof(string) && property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable));

        var queryKey = isCollectionType ? $"{key}[]" : key;
        var queryValues = query.GetValues(queryKey);

        if (queryValues is null || !queryValues.Any())
        {
            return null;
        }

        var itemType = isCollectionType
            ? property.PropertyType.GetGenericArguments().First()
            : property.PropertyType;

        var converter = TypeDescriptor.GetConverter(itemType);
        var convertedValues = queryValues.Select(v => converter.ConvertFromString(v));

        if (!isCollectionType)
        {
            return convertedValues.First();
        }

        var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast), BindingFlags.Static | BindingFlags.Public);
        var toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList), BindingFlags.Static | BindingFlags.Public);

        var genericCast = castMethod.MakeGenericMethod(itemType);
        var genericToList = toListMethod.MakeGenericMethod(itemType);

        var listAfterCast = genericCast.Invoke(null, new object[] { convertedValues });
        var genericList = genericToList.Invoke(null, new[] { listAfterCast });

        return genericList;
    }
}