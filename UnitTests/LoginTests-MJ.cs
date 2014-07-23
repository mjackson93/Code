using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrpheusUI.General;
using OrpheusUI.Pages;
using OrpheusUI.Pages.Accounts;
using OrpheusUI.Tests.Fundamental;
using OpenQA.Selenium.Support.PageObjects;
using NUnit.Framework;

namespace OrpheusUI.Tests
{
    [TestFixture]
    [Category("Smoke")]
	public class LoginTestsMJ : FundamentalTestConfig
	{
        static LogOnPage logOnPage;
        static SecurityQuestionPage securityQuestionPage;
        static SecurityImagePage securityImagePage;
        static ForgotUsernamePage forgotUsernamePage;
        private OverviewPage overviewPage;

        private const string ZephyrTestCycleName = "AUTOMATION Test Cycle";

        #region TestData
       // private const string Username = User.adminUserName;
      //  private const string Password = User.adminAnswer;
        #endregion
        [TestCase("OFM-807", ZephyrTestCycleName)]
        public void UsernameEntry8(string caseId, string testCycle)
        {
            logOnPage = new LogOnPage(Driver);
            PageFactory.InitElements(Driver, logOnPage);
            logOnPage.Open();
            Assert.IsTrue(logOnPage.IsElementPresent(logOnPage.usernameField), "Step 1: Username entry box is not present");

            securityQuestionPage = logOnPage.SignInAs(User.AdminWeb.UserName);
            securityQuestionPage.WaitForSpinnerToDissapear();
            Assert.AreEqual("Orpheus Develop Mock - Security question", securityQuestionPage.GetPageTitle(), "Step 2: Page Title is wrong");
		}

        [TestCase("OFM-824", ZephyrTestCycleName)]
        public void ForgotUserName1SSN(string caseId, string testCycle)
        {
            //Ignoring for now the step to change settings in Admin Tool
            logOnPage = new LogOnPage(Driver);
            PageFactory.InitElements(Driver, logOnPage);
            logOnPage.Open();

            forgotUsernamePage = logOnPage.iForgotUsernameLinkClick();
            forgotUsernamePage.WaitForSpinnerToDissapear();
            Assert.AreEqual("https://web.orpheusdev.net/Forgot/UserName", Driver.Url, "Step 1: Not on the correct screen");

            forgotUsernamePage.continueTapButton.Click();
            Assert.IsTrue(forgotUsernamePage.IsMsgOnPage("The SSN field is required."), "Step 2: \"The SSN field is required.\" message is not displaying");
            Assert.AreEqual("https://web.orpheusdev.net/Forgot/UserName", Driver.Url, "Step 2: The screen changed to another screen");

            forgotUsernamePage.SsnField.SendKeys("abc   !@#");
            Assert.AreEqual("", forgotUsernamePage.SsnField.GetAttribute("value"), "Step 3: The field accepted non-numeric characters");


        }


	}

            //logOnPage = new LogOnPage(Driver);
            //PageFactory.InitElements(Driver, logOnPage);
            //logOnPage.Open();
            //Assert.AreEqual("Orpheus Develop Mock - Log In", logOnPage.GetPageTitle(), "Step1: Page Title is wrong");

            //securityQuestionPage = logOnPage.SignInAs(User.AdminWeb.UserName);
            //securityQuestionPage.WaitForSpinnerToDissapear();
            //Assert.AreEqual("Orpheus Develop Mock - Security question", securityQuestionPage.GetPageTitle(), "Step2: Page Title is wrong");

            //securityImagePage = securityQuestionPage.SubmitAnswer(User.AdminWeb.ChallengeAnswer);
            //Assert.AreEqual("Orpheus Develop Mock - Security image", securityImagePage.GetPageTitle(), "Step3: Page Title is wrong");

            //overviewPage = securityImagePage.AcceptPassword(User.AdminWeb.Password);
            //Assert.AreEqual("Dashboard", overviewPage.GetPageTitle(), "Step4: OverviewPage Title is wrong");

}
