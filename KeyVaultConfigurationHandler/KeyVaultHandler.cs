// ***********************************************************************
// Assembly         : KeyVaultConfigurationHandler
// Author           : rahulrai
// Created          : 01-14-2016
//
// Last Modified By : rahulrai
// Last Modified On : 01-18-2016
// ***********************************************************************
// <copyright file="KeyVaultHandler.cs" company="">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace KeyVaultConfigurationHandler
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.KeyVault;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    #endregion

    /// <summary>
    /// Class KeyVaultHandler.
    /// </summary>
    public static class KeyVaultHandler
    {
        #region Static Fields

        /// <summary>
        /// The vault name
        /// </summary>
        private static readonly string VaultName = ConfigurationManager.AppSettings["VaultName"];

        /// <summary>
        /// The kv client
        /// </summary>
        private static readonly KeyVaultClient KvClient = new KeyVaultClient(GetToken);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> GetKeys()
        {
            try
            {
                var result = KvClient.GetSecretsAsync(VaultName).Result;
                return result.Value.ToDictionary(value => value.Id, value => value.Identifier.Name);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public static string GetValue(string key)
        {
            try
            {
                return KvClient.GetSecretAsync(key).Result.Value;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        /// <exception cref="System.InvalidOperationException">No Key Vault token</exception>
        private static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authenticationContext = new AuthenticationContext(authority);
            ////Alternatively, use certificate authentication
            var clientCredential = new ClientCredential(
                ConfigurationManager.AppSettings["ClientID"],
                ConfigurationManager.AppSettings["ClientKey"]);
            var result = await authenticationContext.AcquireTokenAsync(resource, clientCredential);
            if (result == null)
            {
                throw new InvalidOperationException("No Key Vault token");
            }

            return result.AccessToken;
        }

        #endregion
    }
}