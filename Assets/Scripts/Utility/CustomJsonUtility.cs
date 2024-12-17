using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Base;
using Command;
using Registry;

namespace Utility
{
    public class CustomJsonUtility
    {

        private static readonly Dictionary<string, Type> CommandToParameterTypeMapping;

        static CustomJsonUtility()
        {
            CommandToParameterTypeMapping = new Dictionary<string, Type>
            {
                { "ShowDialog", typeof(ShowDialogParameters) },
                { "SwitchScene", typeof(SwitchSceneParameters) }
            };
        }

        public static T FromJson<T>(string json)
        {
            // 1. Parse JSON into a dictionary (or list) using your chosen JSON loader (e.g., MiniJson).
            var root = MiniJson.Deserialize(json);
            
            if (root is Dictionary<string, object> dict)
            {
                return (T)ParseObject(typeof(T), dict);
            }
            else if (root is List<object> list)
            {
                return (T)ParseArray(typeof(T), list);
            }
            else
            {
                throw new Exception("Invalid JSON structure: root must be an object or array.");
            }
        }

        private static object ParseArray(Type targetType, List<object> rawList)
        {
            if (!targetType.IsGenericType || targetType.GetGenericTypeDefinition() != typeof(List<>))
            {
                throw new Exception($"Expected a List<> type, but got {targetType} instead.");
            }

            var elementType = targetType.GetGenericArguments()[0];
            var listInstance = Activator.CreateInstance(targetType) as IList;

            foreach (var item in rawList)
            {
                if (item is Dictionary<string, object> itemDict)
                {
                    listInstance.Add(ParseObject(elementType, itemDict));
                }
                else if (item is List<object> nestedList)
                {
                    listInstance.Add(ParseArray(elementType, nestedList));
                }
                else
                {
                    listInstance.Add(Convert.ChangeType(item, elementType));
                }
            }

            return listInstance;
        }

        private static object ParseObject(Type targetType, Dictionary<string, object> rawJson)
        {
            if (targetType.IsAbstract)
            {
                targetType = FindUniqueMatchingSubtype(targetType, rawJson);
            }
            
            var instance = Activator.CreateInstance(targetType);
            
            PopulateFields(instance, rawJson);
            return instance;
        }

        private static Type FindUniqueMatchingSubtype(Type baseType, Dictionary<string, object> rawJson)
        {
            var possibleTypes = new List<Type>();
            var allTypes = baseType.Assembly.GetTypes();

            foreach (var t in allTypes)
            {
                if (!t.IsAbstract && t.IsSubclassOf(baseType))
                {
                    if (FieldsMatch(t, rawJson))
                    {
                        possibleTypes.Add(t);
                    }
                }
            }

            if (possibleTypes.Count == 0)
                throw new Exception($"No matching subtype of {baseType.Name} for the given JSON fields.");
            if (possibleTypes.Count > 1)
                throw new Exception(
                    $"Multiple subtypes of {baseType.Name} match the JSON fields: {string.Join(", ", possibleTypes)}");

            return possibleTypes[0];
        }

        private static bool FieldsMatch(Type type, Dictionary<string, object> rawJson)
        {
            var fields =
                type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            if (fields.Length != rawJson.Count)
                return false;

            var jsonKeysLower = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in rawJson)
            {
                jsonKeysLower.Add(kvp.Key);
            }

            foreach (var f in fields)
            {
                if (!jsonKeysLower.Contains(f.Name))
                    return false;
            }

            return true;
        }

        private static void PopulateFields(object instance, Dictionary<string, object> rawJson)
        {
            var targetType = instance.GetType();
            var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            var caseInsensitiveMap = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in rawJson)
            {
                caseInsensitiveMap[kvp.Key] = kvp.Value;
            }

            foreach (var field in fields)
            {
                if (caseInsensitiveMap.TryGetValue(field.Name, out var fieldValue))
                {
                    if (fieldValue is Dictionary<string, object> nestedDict)
                    {
                        var nestedObj = ParseObject(field.FieldType, nestedDict);
                        field.SetValue(instance, nestedObj);
                    }
                    else if (fieldValue is List<object> nestedList)
                    {
                        if (!field.FieldType.IsGenericType ||
                            field.FieldType.GetGenericTypeDefinition() != typeof(List<>))
                        {
                            throw new Exception($"Field {field.Name} expects a List<> type.");
                        }

                        var elementType = field.FieldType.GetGenericArguments()[0];
                        var listInstance = Activator.CreateInstance(field.FieldType) as IList;

                        foreach (var item in nestedList)
                        {
                            if (item is Dictionary<string, object> itemDict)
                                listInstance.Add(ParseObject(elementType, itemDict));
                            else if (item is List<object> subList)
                                listInstance.Add(ParseArray(elementType, subList));
                            else
                                listInstance.Add(Convert.ChangeType(item, elementType));
                        }

                        field.SetValue(instance, listInstance);
                    }
                    else
                    {
                        field.SetValue(instance, Convert.ChangeType(fieldValue, field.FieldType));
                    }
                }
            }
        }

    }

}