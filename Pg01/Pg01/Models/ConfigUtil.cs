using System.Deployment.Application;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Pg01.Properties;

namespace Pg01.Models
{

    #region Data Types

    #endregion

    public static class ConfigUtil
    {
        #region public functions

        public static string GetConfigFilePath()
        {
            var configFilePath = "";
            var dataDirectory = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            if (ApplicationDeployment.IsNetworkDeployed)
                dataDirectory = ApplicationDeployment.CurrentDeployment.DataDirectory;
            if (dataDirectory != null)
                configFilePath = Path.Combine(
                    dataDirectory,
                    "config.xml");

            return configFilePath;
        }

        public static void SaveConfigFile(Config mgs, string configFilePath)
        {
            using (var fs = new FileStream(configFilePath, FileMode.Create))
            {
                Serialize(mgs, fs);
            }
        }

        public static Config LoadDefaultConfigFile()
        {
            var configFilePath = GetConfigFilePath();
#if true
            if (!File.Exists(configFilePath))
#endif
            using (var writer = File.CreateText(configFilePath))
            {
                writer.Write(Resources.config);
                writer.Flush();
            }
            return LoadConfigFile(configFilePath);
        }

        public static Config LoadConfigFile(string configFilePath)
        {
            Config config;
            using (var sr = new StreamReader(configFilePath, new UTF8Encoding(false)))
            {
                var serializer = new XmlSerializer(typeof(Config));
                config = (Config) serializer.Deserialize(sr);
            }
            return config;
        }

#endregion

#region public static functions

        public static void Serialize(Config ags, Stream fs)
        {
            var serializer = new XmlSerializer(typeof(Config));
            serializer.Serialize(fs, ags);
        }

        public static Config Deserialize(string str)
        {
            using (var stream = new StringReader(str))
            {
                return Deserialize(stream);
            }
        }

        public static Config Deserialize(TextReader stream)
        {
            var serializer = new XmlSerializer(typeof(Config));
            var doc = new XmlDocument {PreserveWhitespace = true};
            doc.Load(stream);
            if (doc.DocumentElement == null) return null;
            var reader = new XmlNodeReader(doc.DocumentElement);
            return (Config) serializer.Deserialize(reader);
        }

#endregion
    }
}