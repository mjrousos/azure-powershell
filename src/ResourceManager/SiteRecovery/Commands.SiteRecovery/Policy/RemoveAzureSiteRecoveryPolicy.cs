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
using System.ComponentModel;
using System.Management.Automation;
using Microsoft.Azure.Management.SiteRecovery.Models;
using Microsoft.Azure.Portal.RecoveryServices.Models.Common;
using Properties = Microsoft.Azure.Commands.SiteRecovery.Properties;

namespace Microsoft.Azure.Commands.SiteRecovery
{
    /// <summary>
    /// Creates Azure Site Recovery Policy object in memory.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzureRmSiteRecoveryPolicy")]
    [Alias("Remove-AzureRmSiteRecoveryProtectionProfile")]
    public class RemoveAzureSiteRecoveryPolicy : SiteRecoveryCmdletBase
    {
        #region Parameters

        /// <summary>
        /// Gets or sets Name of the Policy.
        /// </summary>
        [Parameter(Mandatory = true)]
        public ASRPolicy Policy { get; set; }

        #endregion Parameters

        /// <summary>
        /// ProcessRecord of the command.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            try
            {
                LongRunningOperationResponse responseBlue =  RecoveryServicesClient.DeletePolicy(this.Policy.Name);
                
                JobResponse jobResponseBlue =
                    RecoveryServicesClient
                    .GetAzureSiteRecoveryJobDetails(PSRecoveryServicesClient.GetJobIdFromReponseLocation(responseBlue.Location));

                WriteObject(new ASRJob(jobResponseBlue.Job));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
    }
}
