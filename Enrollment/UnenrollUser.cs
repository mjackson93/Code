using System;
using OrpheusUI.General;
using OrpheusUI.Pages;
using OrpheusUI.Pages.Accounts;
using OrpheusUI.Tests.Fundamental;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.PageObjects;
using NUnit.Framework;

namespace OrpheusUI.Tests
{
    [TestFixture]
    [Category("Smoke")]
    class EnrollmentTest
    {
        IWebDriver driver;

        // NUnit functions
        [SetUp]
        public void setup()
        {
            driver = new FirefoxDriver();
        }

        [TearDown]
        public void tear_down()
        {
            //driver.Quit();
        }

        
        /* UnenrollUser
         * ------------
         * Goes to the QA tools site and un-enrolls the given user by username via the Kill Bill tool.
         */
        [TestCase("test0123")]
        public void UnenrollUser(String username)
        {
            Actions action = new Actions(driver);
            driver.Navigate().GoToUrl("https://qatools.orpheusdev.net/");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(d => d.FindElement(By.XPath("//select [contains(@id, 'cnnId')]")));
            driver.FindElement(By.XPath("//select [contains(@id, 'cnnId')]")).Click();
            for (int i = 0; i < 3; i++) action.SendKeys(Keys.Down).Perform();
            action.SendKeys(Keys.Enter).Perform();
            driver.FindElement(By.XPath("//input [contains(@name, 'param')]")).SendKeys(username);
            driver.FindElement(By.XPath("//button [contains(@id, 'button3')]")).Click();
        }


    }
}
