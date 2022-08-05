using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestModellerCSharp.Pages.Samples.Calculator;
using Tests;
using Utilities;
using CuriositySoftware.RunResult.Entities;

namespace Tests
{
    [TestFixture]
    public class Calculator_DefaultProfile : TestBase
    {
		

		
       [Test]
        [TestModellerId("b450fa3d-7c01-4344-8422-591d704389aa")]
        public void OpenApplicationClickByAutomationIdCloseApplication()
        {
            app = Application.LaunchStoreApp("Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
        

        TestModellerCSharp.Pages.DesktopActions	 _DesktopActions	= new TestModellerCSharp.Pages.DesktopActions (app);
    TestModellerLogger.SetLastNodeGuid("02be1d5d-1d02-4f3a-8b5a-18bb63e7ac3c");
    _DesktopActions.OpenApplication("Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");

    TestModellerLogger.SetLastNodeGuid("d46ad858-a797-4007-9ac5-dec8bfe38dff");
    _DesktopActions.ClickByAutomationId("num7Button");

    TestModellerLogger.SetLastNodeGuid("f1a2be17-dcc3-4a0a-a73d-4db5d8daa085");
    _DesktopActions.CloseApplication();

        }

    }
}