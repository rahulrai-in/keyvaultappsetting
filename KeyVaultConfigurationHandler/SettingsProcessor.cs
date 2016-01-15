#region

using System.Web;

using KeyVaultConfigurationHandler;

#endregion

[assembly: PreApplicationStartMethod(typeof(SettingsProcessor), "Start")]

namespace KeyVaultConfigurationHandler
{
    #region

    using System;
    using System.Collections;
    using System.Configuration;
    using System.Reflection;

    #endregion

    public static class SettingsProcessor
    {
        #region Constants

        private const string AppSettingPrefix = "APPSETTING_";

        private const string CustomPrefix = "CUSTOMCONNSTR_";

        private const string MySqlServerPrefix = "MYSQLCONNSTR_";

        private const string SqlAzureServerPrefix = "SQLAZURECONNSTR_";

        private const string SqlServerPrefix = "SQLCONNSTR_";

        #endregion

        #region Public Methods and Operators

        public static void Start()
        {
            // Go through all the environment variables and process those that start
            // with one of our prefixes
            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                var name = (string)entry.Key;
                var val = (string)entry.Value;

                if (name.StartsWith(MySqlServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(MySqlServerPrefix.Length);
                    SetConnectionString(name, val, "MySql.Data.MySqlClient");
                }
                else if (name.StartsWith(SqlAzureServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(SqlAzureServerPrefix.Length);
                    SetConnectionString(name, val, "System.Data.SqlClient");
                }
                else if (name.StartsWith(SqlServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(SqlServerPrefix.Length);
                    SetConnectionString(name, val, "System.Data.SqlClient");
                }
                else if (name.StartsWith(CustomPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(CustomPrefix.Length);
                    SetConnectionString(name, val);
                }
                else if (name.StartsWith(AppSettingPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(AppSettingPrefix.Length);
                    ConfigurationManager.AppSettings[name] = val;
                }
            }
        }

        #endregion

        #region Methods

        private static void SetConnectionString(string name, string connString, string providerName = null)
        {
            // Set the value of the connection string if it exists. Note that we don't
            // create a new one if it doesn't, as the app must already have some logic
            // that tries to use this connection string.
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];
            if (settings != null)
            {
                settings.SetData(connString, providerName);
            }
        }

        private static void SetData(this ConnectionStringSettings settings, string connString, string providerName)
        {
            // Note: we need to use reflection to be able to modify connection strings
            // This is an unfortunate framework limitation, and we are working with
            // the ASP.NET team to relax that restriction in the next framework
            var readOnlyField = typeof(ConfigurationElement).GetField(
                "_bReadOnly",
                BindingFlags.Instance | BindingFlags.NonPublic);

            readOnlyField.SetValue(settings, value: false);
            settings.ConnectionString = connString;

            // we should leave the original provider name if a new provider name is not specified
            if (providerName != null)
            {
                settings.ProviderName = providerName;
            }
        }

        #endregion
    }
}