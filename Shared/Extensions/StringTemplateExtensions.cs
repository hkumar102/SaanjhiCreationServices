using System.Reflection;
using System.Text.RegularExpressions;

namespace Shared.Extensions;

public static class StringTemplateExtensions
{
    /// <summary>
    /// Replaces {PropertyName} placeholders in the template with values from the object's public properties (supports nested properties via dot notation).
    /// </summary>
    public static string ParseTemplateWithObject(this string template, object data)
    {
        if (string.IsNullOrEmpty(template) || data == null)
            return template;

        return Regex.Replace(template, "{([a-zA-Z0-9_.]+)}", match =>
        {
            var propertyPath = match.Groups[1].Value;
            var value = GetPropertyValue(data, propertyPath);
            return value?.ToString() ?? string.Empty;
        });
    }

    private static object? GetPropertyValue(object obj, string propertyPath)
    {
        var props = propertyPath.Split('.');
        object? value = obj;
        foreach (var prop in props)
        {
            if (value == null) return null;
            var type = value.GetType();
            var property = type.GetProperty(prop, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property == null) return null;
            value = property.GetValue(value);
        }
        return value;
    }
}
