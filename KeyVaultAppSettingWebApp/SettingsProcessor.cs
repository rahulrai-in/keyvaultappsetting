// ***********************************************************************
// Assembly         : KeyVaultAppSettingWebApp
// Author           : rahulrai
// Created          : 01-18-2016
//
// Last Modified By : rahulrai
// Last Modified On : 01-18-2016
// ***********************************************************************
// <copyright file="SettingsProcessor.cs" company="">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

#region

using System.Web;

using KeyVaultAppSettingWebApp;

#endregion

[assembly: PreApplicationStartMethod(typeof(SettingsProcessor), "Start")]

namespace KeyVaultAppSettingWebApp
{
    #region

    using System;
    using System.Configuration;
    using System.Reflection;

    using KeyVaultConfigurationHandler;

    #endregion

    /// <summary>
    ///     Class SettingsProcessor.
    /// </summary>
    public static class SettingsProcessor
    {
        #region Constants

        /// <summary>
        ///     The application setting prefix
        /// </summary>
        private const string AppSettingPrefix = "APPSETTING-";

        /// <summary>
        ///     The SQL server prefix
        /// </summary>
        private const string SqlServerPrefix = "SQLCONNSTR-";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        public static void Start()
        {
            var keys = KeyVaultHandler.GetKeys();
            foreach (var entry in keys)
            {
                var name = entry.Value;
                var val = KeyVaultHandler.GetValue(entry.Key);

                if (name.StartsWith(SqlServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(SqlServerPrefix.Length);
                    SetConnectionString(name, val, "System.Data.SqlClient");
                }
                else if (name.StartsWith(AppSettingPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    //// Update AppSettings with new value.
                    name = name.Substring(AppSettingPrefix.Length);
                    ConfigurationManager.AppSettings[name] = val;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="connString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        private static void SetConnectionString(string name, string connString, string providerName = null)
        {
            var settings = ConfigurationManager.ConnectionStrings[name];
            settings?.SetData(connString, providerName);
        }

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="connString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        private static void SetData(this ConnectionStringSettings settings, string connString, string providerName)
        {
            var readOnlyField = typeof(ConfigurationElement).GetField(
                "_bReadOnly",
                BindingFlags.Instance | BindingFlags.NonPublic);

            readOnlyField.SetValue(settings, false);
            settings.ConnectionString = connString;

            if (providerName != null)
            {
                settings.ProviderName = providerName;
            }
        }

        #endregion
    }
}