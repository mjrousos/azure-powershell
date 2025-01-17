﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Azure.Commands.SiteRecovery.Properties;
using Microsoft.Azure.Management.RecoveryServices.Models;

namespace Microsoft.Azure.Commands.SiteRecovery
{
    /// <summary>
    /// Used to initiate a vault create operation.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureRmSiteRecoveryVault")]
    public class CreateAzureSiteRecoveryVault : SiteRecoveryCmdletBase
    {
        #region Parameters

        /// <summary>
        /// Gets or sets the vault name
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the resouce group name
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string ResouceGroupName { get; set; }

        /// <summary>
        /// Gets or sets the location of the vault
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Location { get; set; }

        #endregion

        /// <summary>
        /// ProcessRecord of the command.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            try
            {
                this.WriteWarningWithTimestamp(
                    string.Format(
                    Properties.Resources.SiteRecoveryVaultTypeWillBeDeprecatedSoon));

                VaultCreateArgs vaultCreateArgs = new VaultCreateArgs();
                vaultCreateArgs.Location = this.Location;
                vaultCreateArgs.Properties = new VaultProperties();
                vaultCreateArgs.Properties.Sku = new VaultSku();
                vaultCreateArgs.Properties.Sku.Name = "standard";

                VaultCreateResponse response = RecoveryServicesClient.CreateVault(this.ResouceGroupName, this.Name, vaultCreateArgs);

                this.WriteObject(new ASRVault(response));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
    }
}
