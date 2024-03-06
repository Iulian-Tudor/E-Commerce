using System.Web;
using System.Reflection;
using System.Collections;

namespace Commerce.Client;

public static class RouteExtensions
{
    public static string WithQueryParams(this string route, object queryParams)
    {
        if (queryParams == null)
        {
            return route;
        }

        var properties = queryParams.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        var queryBuilder = HttpUtility.ParseQueryString(string.Empty);
        foreach (var property in properties)
        {
            var value = property.GetValue(queryParams);

            if (value == null)
            {
                continue;
            }

            if (value is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    queryBuilder.Add($"{property.Name}[]", item.ToString());
                }

                continue;
            }

            queryBuilder[property.Name] = value.ToString();
        }

        var queryString = queryBuilder.ToString();
        return string.IsNullOrEmpty(queryString) ? route : $"{route}?{queryString}";
    }

    public static string WithRouteParams(this string routeTemplate, object routeParams)
    {
        if (routeParams == null)
        {
            return routeTemplate;
        }

        var properties = routeParams.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        var route = routeTemplate;
        foreach (var property in properties)
        {
            var value = property.GetValue(routeParams)?.ToString();
            if (value != null)
            {
                route = route.ToLower().Replace($"{{{property.Name.ToLower()}}}", value);
            }
        }

        return route;
    }
}