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

using Microsoft.Azure.Batch;
using Microsoft.Azure.Commands.Batch.Models;
using Microsoft.Azure.Test;
using Microsoft.WindowsAzure.Commands.ScenarioTest;
using System.Collections.Generic;
using System.Management.Automation;
using Xunit;
using Constants = Microsoft.Azure.Commands.Batch.Utils.Constants;

namespace Microsoft.Azure.Commands.Batch.Test.ScenarioTests
{
    public class PoolTests : WindowsAzure.Commands.Test.Utilities.Common.RMTestBase
    {
        private const string commonAccountName = ScenarioTestHelpers.SharedAccount;
        private const string testPoolId = ScenarioTestHelpers.SharedPool;

        // Get from WATaskOSFamilyVersions table, which lags behind https://azure.microsoft.com/en-us/documentation/articles/cloud-services-guestos-update-matrix/
        private const string specificOSVersion = "WA-GUEST-OS-4.22_201507-01"; 

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestNewPool()
        {
            BatchController controller = BatchController.NewInstance;
            controller.RunPsTest(string.Format("Test-NewPool '{0}'", commonAccountName));
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestGetPoolById()
        {
            BatchController controller = BatchController.NewInstance;
            string poolId = "testGetPool";
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-GetPoolById '{0}' '{1}'", commonAccountName, poolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId, 0);
                },
                () =>
                {
                    ScenarioTestHelpers.DeletePool(controller, context, poolId);
                },
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestListPoolsByFilter()
        {
            BatchController controller = BatchController.NewInstance;
            string poolId1 = "testFilter1";
            string poolId2 = "testFilter2";
            string poolId3 = "thirdFilterTest";
            string poolPrefix = "testFilter";
            int matches = 2;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-ListPoolsByFilter '{0}' '{1}' '{2}'", commonAccountName, poolPrefix, matches) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId1, 0);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId2, 0);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId3, 0);
                },
                () =>
                {
                    ScenarioTestHelpers.DeletePool(controller, context, poolId1);
                    ScenarioTestHelpers.DeletePool(controller, context, poolId2);
                    ScenarioTestHelpers.DeletePool(controller, context, poolId3);
                },
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestGetAndListPoolsWithSelect()
        {
            BatchController controller = BatchController.NewInstance;
            controller.RunPsTest(string.Format("Test-GetAndListPoolsWithSelect '{0}' '{1}'", commonAccountName, testPoolId));
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestListPoolsWithMaxCount()
        {
            BatchController controller = BatchController.NewInstance;
            string poolId1 = "testMaxCount1";
            string poolId2 = "testMaxCount2";
            string poolId3 = "thirdMaxCount";
            int maxCount = 1;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-ListPoolsWithMaxCount '{0}' '{1}'", commonAccountName, maxCount) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId1, 0);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId2, 0);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId3, 0);
                },
                () =>
                {
                    ScenarioTestHelpers.DeletePool(controller, context, poolId1);
                    ScenarioTestHelpers.DeletePool(controller, context, poolId2);
                    ScenarioTestHelpers.DeletePool(controller, context, poolId3);
                },
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestListAllPools()
        {
            BatchController controller = BatchController.NewInstance;
            string poolId1 = "testList1";
            string poolId2 = "testList2";
            string poolId3 = "thirdTestList";
            int beforeAddCount = 0;
            int afterAddCount = 0;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-ListAllPools '{0}' '{1}'", commonAccountName, afterAddCount) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    beforeAddCount = ScenarioTestHelpers.GetPoolCount(controller, context);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId1, 0);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId2, 0);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId3, 0);
                    afterAddCount = beforeAddCount + 3;
                },
                () =>
                {
                    ScenarioTestHelpers.DeletePool(controller, context, poolId1);
                    ScenarioTestHelpers.DeletePool(controller, context, poolId2);
                    ScenarioTestHelpers.DeletePool(controller, context, poolId3);
                },
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestUpdatePool()
        {
            BatchController controller = BatchController.NewInstance;
            string poolId = "testUpdate";

            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-UpdatePool '{0}' '{1}'", commonAccountName, poolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId, 0);
                },
                () =>
                {
                    ScenarioTestHelpers.DeletePool(controller, context, poolId);
                },
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestDeletePool()
        {
            BatchController controller = BatchController.NewInstance;
            string poolId = "testDelete";

            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-DeletePool '{0}' '{1}' '0'", commonAccountName, poolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId, 0);
                },
                null,
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestDeletePoolPipeline()
        {
            BatchController controller = BatchController.NewInstance;
            string poolId = "testDeletePipe";

            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-DeletePool '{0}' '{1}' '1'", commonAccountName, poolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.CreateTestPool(controller, context, poolId, 0);
                },
                null,
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestResizePoolById()
        {
            BatchController controller = BatchController.NewInstance;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-ResizePoolById '{0}' '{1}'", commonAccountName, testPoolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                },
                null,
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestResizePoolByPipeline()
        {
            BatchController controller = BatchController.NewInstance;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-ResizePoolByPipeline '{0}' '{1}'", commonAccountName, testPoolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                },
                null,
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestStopResizePoolById()
        {
            BatchController controller = BatchController.NewInstance;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-StopResizePoolById '{0}' '{1}'", commonAccountName, testPoolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                },
                null,
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestStopResizePoolByPipeline()
        {
            BatchController controller = BatchController.NewInstance;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-StopResizePoolByPipeline '{0}' '{1}'", commonAccountName, testPoolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                },
                null,
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestEnableAutoScaleById()
        {
            BatchController controller = BatchController.NewInstance;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-EnableAutoScale '{0}' '{1}' '0'", commonAccountName, testPoolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                    ScenarioTestHelpers.DisableAutoScale(controller, context, testPoolId);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                },
                () =>
                {
                    ScenarioTestHelpers.DisableAutoScale(controller, context, testPoolId);
                },
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestEnableAutoScaleByPipeline()
        {
            BatchController controller = BatchController.NewInstance;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-EnableAutoScale '{0}' '{1}' '1'", commonAccountName, testPoolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                    ScenarioTestHelpers.DisableAutoScale(controller, context, testPoolId);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                },
                () =>
                {
                    ScenarioTestHelpers.DisableAutoScale(controller, context, testPoolId);
                },
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestDisableAutoScaleById()
        {
            BatchController controller = BatchController.NewInstance;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-DisableAutoScale '{0}' '{1}' '0'", commonAccountName, testPoolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                    ScenarioTestHelpers.EnableAutoScale(controller, context, testPoolId);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                },
                null,
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestDisableAutoScaleByPipeline()
        {
            BatchController controller = BatchController.NewInstance;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-DisableAutoScale '{0}' '{1}' '1'", commonAccountName, testPoolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                    ScenarioTestHelpers.EnableAutoScale(controller, context, testPoolId);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                },
                null,
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestEvaluateAutoScaleById()
        {
            BatchController controller = BatchController.NewInstance;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-EvaluateAutoScale '{0}' '{1}' '0'", commonAccountName, testPoolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.EnableAutoScale(controller, context, testPoolId);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                },
                () =>
                {
                    ScenarioTestHelpers.DisableAutoScale(controller, context, testPoolId);
                },
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestEvaluateAutoScaleByPipeline()
        {
            BatchController controller = BatchController.NewInstance;
            BatchAccountContext context = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-EvaluateAutoScale '{0}' '{1}' '1'", commonAccountName, testPoolId) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    ScenarioTestHelpers.EnableAutoScale(controller, context, testPoolId);
                    ScenarioTestHelpers.WaitForSteadyPoolAllocation(controller, context, testPoolId);
                },
                () =>
                {
                    ScenarioTestHelpers.DisableAutoScale(controller, context, testPoolId);
                },
                TestUtilities.GetCallingClass(),
                TestUtilities.GetCurrentMethodName());
        }

        [Fact(Skip = "WATaskOSFamilyVersions table appears to be out of date, re-record when updated")]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestChangeOSVersionById()
        {
            TestChangeOSVersion(false);
        }

        [Fact(Skip = "WATaskOSFamilyVersions table appears to be out of date, re-record when updated")]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestChangeOSVersionPipeline()
        {
            TestChangeOSVersion(true);
        }

        private void TestChangeOSVersion(bool usePipeline)
        {
            BatchController controller = BatchController.NewInstance;
            BatchAccountContext context = null;
            string newTargetOSVersion = null;
            controller.RunPsTestWorkflow(
                () => { return new string[] { string.Format("Test-ChangeOSVersion '{0}' '{1}' '{2}' '{3}'", commonAccountName, testPoolId, newTargetOSVersion, usePipeline ? 1 : 0) }; },
                () =>
                {
                    context = ScenarioTestHelpers.GetBatchAccountContextWithKeys(controller, commonAccountName);
                    string currentTargetOSVersion = ScenarioTestHelpers.WaitForOSVersionChange(controller, context, testPoolId);
                    newTargetOSVersion = currentTargetOSVersion == "*" ? specificOSVersion : "*";
                },
                null,
                TestUtilities.GetCallingClass(),
                usePipeline ? "TestChangeOSVersionPipeline" : "TestChangeOSVersionById");
        }
    }
}
