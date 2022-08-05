using CuriositySoftware.DataAllocation.Engine;
using CuriositySoftware.DataAllocation.Entities;
using CuriositySoftware.RunResult.Entities;
using CuriositySoftware.RunResult.Services;
using CuriositySoftware.Utils;
using FlaUI.Core;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Tests
{
    public class ModellerConfig
    {
        public static String APIUrl = "https://cloud.testinsights.io";

        public static String APIKey = "R-8S1IAoXOTU5ljcb7D8idjXL";

        public static String ServerName = "VIP-James";

        public static ConnectionProfile GetConnectionProfile()
        {
            ConnectionProfile cp = new ConnectionProfile();

            cp.APIKey = ModellerConfig.APIKey;
            cp.Url = ModellerConfig.APIUrl;

            return cp;
        }
    }

    [SetUpFixture]
    public class DataAllocSetup
    {
        [OneTimeSetUp]
        public void performAllocations()
        {
            ConnectionProfile cp = new ConnectionProfile();
            cp.APIKey = ModellerConfig.APIKey;
            cp.Url = ModellerConfig.APIUrl;

            DataAllocationEngine dataAllocationEngine = new DataAllocationEngine(cp);

            // Create a list of all the pools that need allocating
            List<AllocationType> allocationTypes = new List<AllocationType>();

            foreach (Test curTest in TestExecutionContext.CurrentContext.CurrentTest.Tests)
            {
                foreach (Test subTest in curTest.Tests)
                {
                    DataAllocation[] allocAttr = subTest.Method.GetCustomAttributes<DataAllocation>(true);
                    if (allocAttr != null && allocAttr.Length > 0)
                    {
                        foreach (String testType in allocAttr[0].groups)
                        {
                            AllocationType allocationType = new AllocationType(allocAttr[0].poolName, allocAttr[0].suiteName, testType);

                            allocationTypes.Add(allocationType);
                        }
                    }
                }
            }

            if (allocationTypes.Count > 0)
            {
                if (!dataAllocationEngine.ResolvePools(ModellerConfig.ServerName, allocationTypes))
                {
                    throw new Exception("Error - " + dataAllocationEngine.getErrorMessage());
                }
            }
        }
    }

    public class TestBase
    {
        public static TestPathRunEntity testPathRun;

        protected DataAllocationEngine dataAllocationEngine;

        protected Application app;

        [SetUp]
        public void initDriver()
        {
            TestModellerLogger.steps.Clear();

            testPathRun = new TestPathRunEntity();

            ConnectionProfile cp = new ConnectionProfile();
            cp.APIKey = ModellerConfig.APIKey;
            cp.Url = ModellerConfig.APIUrl;

            dataAllocationEngine = new DataAllocationEngine(cp);
        }

        [TearDown]
        public void closerDriver()
        {
            TestModellerId[] attr = TestExecutionContext.CurrentContext.CurrentTest.Method.GetCustomAttributes<TestModellerId>(true);
            if (attr != null && attr.Length > 0)
            {
                String guid = attr[0].guid;

                testPathRun.message = TestExecutionContext.CurrentContext.CurrentResult.Message; 
                testPathRun.runTime = (int) (TestExecutionContext.CurrentContext.CurrentResult.EndTime.ToUniversalTime().Ticks - TestExecutionContext.CurrentContext.CurrentResult.StartTime.ToUniversalTime().Ticks);
                testPathRun.runTimeStamp = DateTime.Now;
                testPathRun.testPathGuid = (guid);
                testPathRun.vipRunId = (TestRunIdGenerator.GenerateGuid());

                if (TestExecutionContext.CurrentContext.CurrentResult.ResultState.Equals(ResultState.Success)) {
                    TestModellerLogger.PassStep(app, "Test Passed");
                    testPathRun.testStatus = (TestPathRunStatus.Passed);
                } else {
                    TestModellerLogger.FailStepWithScreenshot(app, "Test Failed");

                    testPathRun.testStatus = (TestPathRunStatus.Failed);
                }

                testPathRun.testPathRunSteps = TestModellerLogger.steps;

                TestRunService runService = new TestRunService(ModellerConfig.GetConnectionProfile());
                runService.PostTestRun(testPathRun);
            }
        }
    }
}
