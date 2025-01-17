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


using Microsoft.Azure.Commands.Compute.Common;
using Microsoft.Azure.Commands.Compute.Extension.AzureDiskEncryption;
using Microsoft.Azure.Commands.Compute.Extension.AzureVMBackup;
using Microsoft.Azure.Commands.Compute.Models;
using Microsoft.Azure.Commands.Compute.StorageServices;
using Microsoft.Azure.Common.Authentication;
using Microsoft.Azure.Common.Authentication.Models;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Azure.Management.Storage;
using Microsoft.WindowsAzure.Commands.Sync.Download;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Compute.Extension.AzureVMBackup
{
    [Cmdlet(
    VerbsCommon.Remove,
    ProfileNouns.AzureVMBackup)]
    [OutputType(typeof(PSComputeLongRunningOperation))]
    public class RemoveAzureVMBackup : VirtualMachineExtensionBaseCmdlet
    {
        [Parameter(
           Mandatory = true,
           Position = 0,
           ValueFromPipelineByPropertyName = true,
           HelpMessage = "The resource group name.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Alias("ResourceName")]
        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The virtual machine name.")]
        [ValidateNotNullOrEmpty]
        public string VMName { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 2,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The tag for this backup.")]
        public string Tag { get; set; }

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            VirtualMachineGetResponse virtualMachineResponse = this.ComputeClient.ComputeManagementClient.VirtualMachines.GetWithInstanceView(this.ResourceGroupName, VMName);
            string currentOSType = virtualMachineResponse.VirtualMachine.StorageProfile.OSDisk.OperatingSystemType;

            if (string.Equals(currentOSType, "Linux", StringComparison.InvariantCultureIgnoreCase))
            {
                AzureVMBackupExtensionUtil util = new AzureVMBackupExtensionUtil();
                AzureVMBackupConfig vmConfig = new AzureVMBackupConfig();
                vmConfig.ResourceGroupName = ResourceGroupName;
                vmConfig.VMName = VMName;
                vmConfig.VirtualMachineExtensionType = VirtualMachineExtensionType;
                util.RemoveSnapshot(vmConfig, Tag, this);
            }
            else
            {
                ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "The VM should be a Linux VM")),
                                                      "InvalidArgument",
                                                      ErrorCategory.InvalidArgument,
                                                      null));
            }
        }
    }
}
