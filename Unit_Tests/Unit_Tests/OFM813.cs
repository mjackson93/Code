using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace Unit_Tests
{
    [TestFixture]
    public class Ofm813
    {
        private IWebDriver _driver;

        // NUnit functions
        [SetUp]
        public void setup()
        {
            _driver = new FirefoxDriver();
        }

        [TearDown]
        public void tear_down()
        {
            // _driver.Quit();
        }

            //Constants

        // The wait time for wait.Until calls
        public const int WaitUntilTime = 30;

        // The URL to go to
        public const string OrpheusUrl = "https://web.orpheusdev.net/";

            //Inputs
        // The username to log in as
        public const String Username = "admin";
        // The answer to the security question
        public const String QuestionAnswer = "cat";

            //Page Titles
        // The title of the security question screen
        public const String QuestionScreenTitle = "Security question";
        // The title of the password entry screen
        public const String PasswordScreenTitle = "Security image";

            //Locators
        // Security question answer field css locator
        public const String QuestionField = "#credentialsForm > button[type = 'submit']";   // Temporary finding the continue button until web is back up so I can get the field's locator

        // Password answer field css locator
        public const String PasswordField = "#Password";

        /* 1. Enters the username and checks that the Security Question screen is loaded.
         * 2. Enters the security question answer and checks that the Password screen is loaded.
         */
        [Test]
        private void Run()
        {
            _driver.Navigate().GoToUrl(OrpheusUrl);
            var action = new Actions(_driver);

            action.SendKeys(Username).Perform();
            action.SendKeys(Keys.Enter).Perform();
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(WaitUntilTime));
            wait.Until(d => d.FindElement(By.CssSelector(QuestionField)));
            Assert.IsTrue(_driver.Title.Contains(QuestionScreenTitle), "Not on the Security Question screen");

            action.SendKeys(QuestionAnswer).Perform();
            action.SendKeys(Keys.Enter).Perform();
            wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(WaitUntilTime));
            wait.Until(d => d.FindElement(By.CssSelector(PasswordField)));
            Assert.IsTrue(_driver.Title.Contains(PasswordScreenTitle), "Not on the Password Entry screen");
        }
    }
}