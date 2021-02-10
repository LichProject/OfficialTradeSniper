using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace ImGuiSniperHost.Common
{
    public class JsonSettings
    {
        protected JsonSettings(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path);
            }

            FilePath = path;
            LoadFrom(FilePath);
        }

        public bool HasProperty(string name)
        {
            return name != null && !(GetType().GetProperty(name) == null);
        }

        public object GetProperty(string name)
        {
            return GetType().GetProperty(name)?.GetValue(this);
        }

        public void SetProperty(string name, object value)
        {
            GetType().GetProperty(name)?.SetValue(this, value);
        }

        [JsonIgnore]
        public string FilePath { get; internal set; }

        public static string AssemblyPath => Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
        public static string SettingsPath => Path.Combine(AssemblyPath, "Settings");

        public void Save()
        {
            SaveAs(FilePath);
        }

        protected T GetDefaultValue<T>(Expression<Func<T>> exp)
        {
            if (exp.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException("Value must be a lambda expression", nameof(exp));
            }

            if (!(exp.Body is MemberExpression))
            {
                throw new ArgumentException("The body of the expression must be a memberref", nameof(exp));
            }

            if (((MemberExpression) exp.Body).Member.GetCustomAttributes(typeof(DefaultValueAttribute), true).FirstOrDefault() is
                DefaultValueAttribute defaultValueAttribute)
            {
                return (T) defaultValueAttribute.Value;
            }

            return default;
        }

        public virtual bool Validate(out List<string> errors)
        {
            errors = null;
            return true;
        }

        public void SaveAs(string file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            lock (_fileLock)
            {
                if (!File.Exists(file))
                {
                    string directoryName = Path.GetDirectoryName(file);
                    if (directoryName != null && !Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                }

                string contents = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(file, contents);
            }
        }

        protected void LoadFrom(string file)
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                if (customAttributes.Length == 0)
                {
                    continue;
                }

                foreach (DefaultValueAttribute defaultValueAttribute in customAttributes)
                {
                    if (propertyInfo.GetSetMethod() != null)
                    {
                        propertyInfo.SetValue(this, defaultValueAttribute.Value, null);
                    }
                }
            }

            lock (_fileLock)
            {
                if (!File.Exists(file))
                {
                    return;
                }

                string text = File.ReadAllText(file);
                try
                {
                    JsonConvert.PopulateObject(text, this);
                }
                catch
                {
                    File.Move(file, $"{file}.{Environment.TickCount}.invalid");
                }
            }
        }

        protected static string GetAbsoluteFilePath(params string[] subPathParts)
        {
            var list = new List<string> {SettingsPath};
            list.AddRange(subPathParts);
            return Path.Combine(list.ToArray());
        }

        readonly object _fileLock = new object();
    }
}