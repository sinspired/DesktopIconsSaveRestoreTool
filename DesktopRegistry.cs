using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace IconsSaveRestore.Code
{
    internal class DesktopRegistry
    {
        private const string KeyName = @"Software\Microsoft\Windows\Shell\Bags\1\Desktop";

        public IDictionary<string, string> GetRegistryValues()
        {
            using (var registry = Registry.CurrentUser.OpenSubKey(KeyName))
            {
                return registry.GetValueNames().ToDictionary(n => n, n => GetValue(registry, n));
            }
        }

        private string GetValue(RegistryKey registry, string valueName)
        {
            var value = registry.GetValue(valueName);
            if (value == null)
            { return string.Empty; }

            var json = JsonSerializer.Serialize(value);
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        }

        public void SetRegistryValues(IDictionary<string, string> values)
        {
            using (var registry = Registry.CurrentUser.OpenSubKey(KeyName, true))
            {
                foreach (var item in values)
                {
                    registry.SetValue(item.Key, GetValue(item.Value));
                }
            }
        }

        private object GetValue(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            { return null; }

            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(stringValue));
            return JsonSerializer.Deserialize<object>(json);
        }
    }
}
