using System.Deployment.Application;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Pg01.Properties;

namespace Pg01.Models
{

    #region Data Types

    #endregion

    public static class ConfigUtil
    {
        #region singleton pattern

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
        #endregion

        #region public functions

        public static void SaveConfigFile(Config mgs, string configFilePath)
        {
            using (var fs = new FileStream(configFilePath, FileMode.Create))
            {
                Serialize(mgs, fs);
            }
        }

        public static Config LoadConfigFile(string configFilePath)
        {
            Config mgs = null;
            var serializer = new XmlSerializer(typeof(Config));
            if (!File.Exists(configFilePath))
                using (var sw = File.CreateText(configFilePath))
                {
                    sw.Write(Resources.config);
                    sw.Flush();
                }
            using (var fs = new FileStream(configFilePath, FileMode.Open))
            {
                var doc = new XmlDocument {PreserveWhitespace = true};
                doc.Load(fs);
                if (doc.DocumentElement != null)
                {
                    var reader = new XmlNodeReader(doc.DocumentElement);
                    mgs = (Config) serializer.Deserialize(reader);
                }
            }
            return mgs;
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