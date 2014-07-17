/* Marcus Jackson, mjackson@access-softek.com
 * Selenium Tests for Orpheus
 */

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

namespace Automation_Example
{

    [TestFixture]
    public class Driver
    {
        IWebDriver driver;
            



                //General Constants
            
        // The wait time for wait.Until calls
        public const int WAIT_UNTIL_TIME = 30;

        // The amount of time for the thread to sleep between each function
        public const int SLEEP_TIME = 2000;

        // Toggles whether sleeping between function calls is on to "slow" down the test
        public const bool SLEEP_ON = true;




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



                // General Helper functions

        // Logs into Orpheus with the given information.
        private void site_login(String user, String answer, String pass, Actions action)
        {
            WebDriverWait wait;
            Console.WriteLine(driver.FindElement(By.Name("version")).GetAttribute("content"));

            //Login Screen
            //Type in the username and proceed
            action.SendKeys(user).Perform();

            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            action.SendKeys(Keys.Enter).Perform();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.XPath("//button [text()='Continue']")));

            //Login Screen
            //Checks if it asks for security question or not
            if (driver.Title.Contains("question")) 
            {
                //Type in security question answer
                action.SendKeys(answer).Perform();

                if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

                action.SendKeys(Keys.Enter).Perform();
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
                wait.Until(d => d.FindElement(By.Name("Password")));
            }
            
            //Password Screen
            //Type in the password and proceed
            action.SendKeys(pass).Perform();

            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            action.SendKeys(Keys.Enter).Perform();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.XPath("//div [contains (@class, 'accounts-wrapper')]/div[2]/div")));
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.XPath("//a [contains(@href, '/payments/index')]")));
        }




                // Test functions


        //***Verify History Sorted Test***

                //Specific Helper functions

        private List<String> collect_elem_fields()
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            WebDriverWait wait;


            List<String> topmost_fields = new List<String>();
            //IWebElement elem = driver.FindElement(By.XPath("//td [contains(@data-field, 'AccountId')]"));
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
            wait.Until(d => d.FindElement(By.XPath("//td [contains(@data-field, 'AccountId')]")));
            try
            {
                topmost_fields.Add((String)js.ExecuteScript("return arguments[0].textContent",
                    driver.FindElement(By.XPath("//td [contains(@data-field, 'AccountId')]"))));
            }
            catch
            {
                System.Threading.Thread.Sleep(500);
                topmost_fields.Add((String)js.ExecuteScript("return arguments[0].textContent",
                    driver.FindElement(By.XPath("//td [contains(@data-field, 'AccountId')]"))));
            }
            //elem = driver.FindElement(By.XPath("//td [contains(@data-field, 'Date')]"));
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
            wait.Until(d => d.FindElement(By.XPath("//td [contains(@data-field, 'Date')]")));
            try
            {
                topmost_fields.Add((String)js.ExecuteScript("return arguments[0].textContent",
                    driver.FindElement(By.XPath("//td [contains(@data-field, 'Date')]"))));
            }
            catch
            {
                System.Threading.Thread.Sleep(500);
                topmost_fields.Add((String)js.ExecuteScript("return arguments[0].textContent",
                    driver.FindElement(By.XPath("//td [contains(@data-field, 'Date')]"))));
            }
            //elem = driver.FindElement(By.XPath("//td [contains(@data-field, 'Description')]"));
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
            wait.Until(d => d.FindElement(By.XPath("//td [contains(@data-field, 'Description')]")));
            try
            {
                topmost_fields.Add((String)js.ExecuteScript("return arguments[0].textContent",
                    driver.FindElement(By.XPath("//td [contains(@data-field, 'Description')]"))));
            }
            catch
            {
                System.Threading.Thread.Sleep(500);
                topmost_fields.Add((String)js.ExecuteScript("return arguments[0].textContent",
                    driver.FindElement(By.XPath("//td [contains(@data-field, 'Description')]"))));
            }
            return topmost_fields;
        }


        private bool areElemsEqual(List<String> elem1, List<String> elem2)
        {
            for (int i = 0; i < elem1.Count; i++)
            {
                if (!elem1[i].Equals(elem2[i]))//(elem1[i] != elem2[i])
                {
                    return false;
                }
            }
            return true;
        }


        private void print_element_when_comp(List<String> elem1, List<String> elem2)
        {
            Console.Write("elem1: ");
            foreach (String str in elem1)
            {
                Console.Write(str + ", ");
            }
            Console.WriteLine();

            Console.Write("elem2: ");
            foreach (String str in elem2)
            {
                Console.Write(str + ", ");
            }
            Console.WriteLine();
        }


        private IWebElement get_topmost(IReadOnlyCollection<IWebElement> elems)
        {
            IEnumerator<IWebElement> ienum = elems.GetEnumerator();
            ienum.MoveNext();
            return ienum.Current;
        }


        private void process_new_dates(List<String> dates)
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            IReadOnlyCollection<IWebElement> elems = driver.FindElements(By.XPath("//td [contains(@data-field, 'Date')]"));
            IEnumerator<IWebElement> ienum = elems.GetEnumerator();
            while (ienum.MoveNext())
            {
                dates.Add((String)js.ExecuteScript("return arguments[0].textContent", ienum.Current));
                //dates.Add((ienum.Current).Text);
            }
        }

        //Returns true if the first date comes before the second.
        private bool isEarlier(String date1, String date2)
        {

            return true;
        }


        private void verify_sorted(List<String> dates)
        {

        }


        /* web_verify_history_sorted
         * -------------------------
         * 
         */
        [Test]
        public void web_verify_history_sorted()
        {
            String url = "https://web.orpheusdev.net/";
            driver.Navigate().GoToUrl(url);
            Actions action = new Actions(driver);
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            WebDriverWait wait;
            IWebElement elem;

            //Bypasses the first three screens
            site_login("admin", "cat", "1234", action);

            //Dashboard screen
            //Click History
            elem = driver.FindElement(By.XPath("//a [contains(@href, '/payments/index')]"));
            elem.Click();
            elem = driver.FindElement(By.XPath("//a [contains(@href, '/accounts/history')]"));
            elem.Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
            wait.Until(d => d.FindElement(By.XPath("//div [contains(@class, 'k-scrollbar k-scrollbar-vertical')]")));
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
            wait.Until(d => d.FindElement(By.XPath("//td [contains(@data-field, 'Date')]")));

            //
            IWebElement bar = driver.FindElement(By.XPath("//div [contains(@class, 'k-scrollbar k-scrollbar-vertical')]"));
            List<String> dates = new List<String>();
            List<String> top_fields = collect_elem_fields();


            //System.Threading.Thread.Sleep(6000);
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));
            bool inf_looping = false;
            for (int i = 0; i < 5; i++)
            {
                process_new_dates(dates);
                Console.WriteLine("Dates size: " + dates.Count);
                foreach (String date in dates)
                {
                    Console.WriteLine("\"" + date + "\"");
                }
                Console.WriteLine("End.");

                int loop_check = 0;
                List<String> test_fields = collect_elem_fields();
                while (areElemsEqual(top_fields, test_fields))
                {
                    for (int j = 0; j < 1; j++)
                    {
                        bar.SendKeys(Keys.ArrowDown);
                    }
                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
                    wait.Until(d => d.FindElement(By.XPath("//td [contains(@data-field, 'Date')]")));
                    loop_check++;
                    if (loop_check > 80)
                    {
                        Console.WriteLine("Infinite loop");
                        inf_looping = true;
                        break;
                    }
                    test_fields = collect_elem_fields();
                    //System.Threading.Thread.Sleep(500);
                }
                if (inf_looping) i = 2;
                top_fields = collect_elem_fields();
            }

            //IReadOnlyCollection<IWebElement> elems = driver.FindElements(By.XPath("//td [contains(@data-field, 'Date')]"));
            //process_new_dates(dates, elems.GetEnumerator());

            Console.WriteLine("Dates size: " + dates.Count);
            foreach (String date in dates)
            {
                Console.WriteLine("\"" + date + "\"");
            }
            Console.WriteLine("End.");

            verify_sorted(dates);
        }




        // ***Admin Add Comment Test***

                //Specific Helper functions

        // Navigates from the Overview screen to the Admin Notes screen in Orchard.
        private void navigate_to_admin_notes(int user)
        {
            WebDriverWait wait;
            IWebElement elem;

            // Clicks Dashboard to go to the Orchard page
            driver.FindElement(By.XPath("//a [contains(@href, '/Admin')]")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.XPath("//a [contains(@href, '/Admin/Chorus.Admin.UserManagement')]")));
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Navigates to the User Management page
            driver.FindElement(By.XPath("//a [contains(@href, '/Admin/Chorus.Admin.UserManagement')]")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.XPath("//div [contains (@class, 'k-grid-content') and contains (@data-role, 'virtualscrollable')]/div/table/tbody/tr[1]/td[1]/input")));
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Clicks the checkbox for the designated number user
            elem = driver.FindElement(By.XPath("//div [contains (@class, 'k-grid-content') and contains (@data-role, 'virtualscrollable')]/div/table/tbody/tr[" + user + "]/td[1]/input"));
            while (!elem.Displayed)
            {
                driver.FindElement(By.XPath("//div [contains(@id, 'GridContainer')]/div/div[3]/div[2]")).SendKeys(Keys.Down);
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
                wait.Until(d => d.FindElement(By.XPath("//div [contains (@class, 'k-grid-content') and contains (@data-role, 'virtualscrollable')]/div/table/tbody/tr[" + user + "]/td[1]/input")));
                elem = driver.FindElement(By.XPath("//div [contains (@class, 'k-grid-content') and contains (@data-role, 'virtualscrollable')]/div/table/tbody/tr[" + user + "]/td[1]/input"));
            }
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);
            driver.FindElement(By.XPath("//div [contains (@class, 'k-grid-content') and contains (@data-role, 'virtualscrollable')]/div/table/tbody/tr[" + user + "]/td[1]/input")).Click();
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Clicks the Open Detail View button
            driver.FindElement(By.Name("DetailView")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.XPath("//div [contains(@id, 'dUserDetailTabs') and contains(@data-role, 'tabstrip')]/ul/li[3]/a")));
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Clicks the Admin Notes button
            driver.FindElement(By.XPath("//div [contains(@id, 'dUserDetailTabs') and contains(@data-role, 'tabstrip')]/ul/li[3]/a")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.Id("btnAddComment")));
        }


                // Specific Constants

        /* A constant for which Orpheus website to use in the following test */
        public const string ORPHEUS_URL_1 = "https://web.orpheusdev.net/";

        /* Determines which users in the list is selected to add a comment to (Must range from 1-100) */
        public const int USER_TO_ADD_COMMENT = 25;

        /*  orchard_create_comment_test()
         *  -----------------------------
         *  An automation example test.
         *  Logs into Orpheus (version determined by the above constant) then goes to Orchard by clicking Dashboard. Next, goes 
         *  to User Management and clicks on the nth user in the list (n determined by the above constant). Next, clicks 
         *  Open Detail View, and navigates to the Admin Notes section where it adds a comment for the user based on the time 
         *  then asserts that the comment appears and appears as entered.
         */
        //[Test]
        public void orchard_create_comment_test()
        {
            // Setup
            driver.Navigate().GoToUrl(ORPHEUS_URL_1);
            Actions action = new Actions(driver);
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            WebDriverWait wait;
            IWebElement elem;

            // Logs in as admin
            site_login("admin", "cat", "1234", action);
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Goes from the Dashboard to the Admin Notes screen for the nth user in the list.
            navigate_to_admin_notes(USER_TO_ADD_COMMENT);
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Clicks the Add comment button
            driver.FindElement(By.Id("btnAddComment")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.Name("AdminNoteText")));
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Enters a comment into the textbox based on the current time
            String comment = "The quick brown fox jumps over the lazy dog: " + DateTime.Now.ToString();
            driver.FindElement(By.Name("AdminNoteText")).SendKeys(comment);
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Clicks the save button
            driver.FindElement(By.XPath("//div [contains(@class, 'k-ext-dialog-buttons') and contains(@style, 'position:absolute; bottom:10px; text-align:center; width:366px;')]/div/button[1]")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.XPath("//button [text()='OK']")));
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            //Clicks the Ok button
            elem = driver.FindElement(By.XPath("//button [text()='OK']"));
            elem.Click();

            // Asserts that the comment exists and that it displays the proper text
            Assert.AreNotEqual(0, driver.FindElements(By.XPath("//div [contains(@id, 'AdminNotesGrid')]/div/div[3]/div/table/tbody/tr[1]/td[4]")).Count);
            String text = driver.FindElement(By.XPath("//div [contains(@id, 'AdminNotesGrid')]/div/div[3]/div/table/tbody/tr[1]/td[4]")).Text;
            Assert.AreEqual(comment, text);
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Clicks the Logout link
            driver.FindElement(By.XPath("//a [contains(@href, '/Users/Account/LogOff')]")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.Name("UserName")));

            //Signs in again
            site_login("admin", "cat", "1234", action);

            // Goes from the Dashboard to the Admin Notes screen for the nth user in the list.
            navigate_to_admin_notes(USER_TO_ADD_COMMENT);

            // Asserts that the comment exists and that it displays the proper text
            Assert.AreNotEqual(0, driver.FindElements(By.XPath("//div [contains(@id, 'AdminNotesGrid')]/div/div[3]/div/table/tbody/tr[1]/td[4]")).Count);
            text = driver.FindElement(By.XPath("//div [contains(@id, 'AdminNotesGrid')]/div/div[3]/div/table/tbody/tr[1]/td[4]")).Text;
            Assert.AreEqual(comment, text);
        }




        // ***Transfers Test***

                // Specific Helper functions

        // Navigates from the Overview page to the Transfers & Payments screen.
        private void navigate_to_transfers()
        {
            WebDriverWait wait;

            // Clicks the Transfers & Payments button
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);
            driver.FindElement(By.XPath("//a [contains(@href, '/payments/index')]")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.XPath("//table [contains(@class, 'table-general table-bordered')]/tbody/tr/td[1]/div[1]/div[1]")));
        }


        // Returns the available balance of the specified account. Returns -1 if the account is not found.
        private Double get_acc_avail_balance(String acc_name)
        {
            int count = driver.FindElements(By.XPath("//div [contains (@class, 'accounts-wrapper')]/div[2]/div")).Count();
            //Console.WriteLine("count: " + count);
            for (int i = 1; i <= count; i++)
            {
                String acc_path = "//div [contains (@class, 'accounts-wrapper')]/div[2]/div[" + i + "]";
                String curr_name = driver.FindElement(By.XPath(acc_path + "/div/section/div[2]/span/span")).Text;
                //Console.WriteLine("acc: " + acc_name + ", curr: " + curr_name);
                if (acc_name.Equals(curr_name))
                {
                    // Acquires the available balance from the XPath and converts to double
                    String bal = driver.FindElement(By.XPath(acc_path + "/div/section/div[4]")).Text;
                    bal = bal.Substring(bal.IndexOf('$')).Trim();
                    bal = bal.Replace('$', ' ').Trim();
                    //Console.WriteLine("bal: " + bal);
                    return Convert.ToDouble(bal);
                }
            }
            return -1;
        }


        // Based on the boolean, transfers the specified amount from saving to checkings or vice versa.
        private void make_transfer(bool sav_to_chk, double amount)
        {
            WebDriverWait wait;
            IWebElement elem;

            // Determines whether to click the savings or classic checking buttons in each column.
            int button1 = 2;
            int button2 = 1;
            if (sav_to_chk)
            {
                button1 = 1;
                button2 = 2;
            }

            // Clicks a button in the first column.
            driver.FindElement(By.XPath("//table [contains(@class, 'table-general table-bordered')]/tbody/tr/td[1]/div[1]/div[" + button1 + "]")).Click();
            //if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Clicks a button in the second column.
            driver.FindElement(By.XPath("//table [contains(@class, 'table-general table-bordered')]/tbody/tr/td[2]/div[1]/div[" + button2 + "]")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.XPath("//table [contains(@class, 'table-general table-bordered')]/tbody/tr/td[3]/div/div/form/div[1]/div/div[1]")));
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Clicks the send now button.
            driver.FindElement(By.XPath("//table [contains(@class, 'table-general table-bordered')]/tbody/tr/td[3]/div/div/form/div[1]/div/div[1]/span/span/label")).Click();
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Sends the amount to the payment amount field.
            driver.FindElement(By.XPath("//table [contains(@class, 'table-general table-bordered')]/tbody/tr/td[3]/div/div/form/div[2]/span/span/input[1]")).SendKeys(Convert.ToString(amount));
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Sends text to the memo field.
            driver.FindElement(By.XPath("//table [contains(@class, 'table-general table-bordered')]/tbody/tr/td[3]/div/div/form/div[3]/input")).SendKeys("Test");
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Clicks the Transfer Now button
            driver.FindElement(By.XPath("//table [contains(@class, 'table-general table-bordered')]/tbody/tr/td[3]/div/div/form/div[5]/input")).Click();
            elem = driver.FindElement(By.XPath("//button [text()='Confirm transfer']"));
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until<bool>((d) => { return elem.Displayed == true; });
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Clicks the Confirm transfer button
            elem.Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until<bool>((d) => { return elem.Displayed == false; });
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Goes to Overview screen
            driver.FindElement(By.XPath("//a [contains(@href, '/accounts/overview')]")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.XPath("//div [contains (@class, 'accounts-wrapper')]/div[2]/div")));
        }

        // Asserts that the transfer took place properly based on the given parameters. Must be called from the overview page.
        private void assert_transferred(double amount, double prev_sav_bal, double prev_chk_bal, bool sav_to_chk)
        {
            // Asserts that the transfer took place properly.
            Double curr_sav_bal = get_acc_avail_balance("SAVINGS");
            Double curr_chk_bal = get_acc_avail_balance("CLASSIC CHECKING");

            //Console.WriteLine("amount: " + amount + ";;; prev_sav: " + prev_sav_bal + ", curr_sav: " + curr_sav_bal + ";;; prev_chk: " + prev_chk_bal + ", curr_chk: " + curr_chk_bal);

            if (sav_to_chk)
            {
                Assert.AreEqual(Math.Round(prev_sav_bal - amount, 2), curr_sav_bal);
                Assert.AreEqual(Math.Round(prev_chk_bal + amount, 2), curr_chk_bal);
            }
            else
            {
                Assert.AreEqual(prev_sav_bal + amount, curr_sav_bal);
                Assert.AreEqual(prev_chk_bal - amount, curr_chk_bal);
            }

        }


                // Specific Constants

        /* A constant for which Orpheus website to use in the following test */
        public const string ORPHEUS_URL_2 = "https://qa.orpheusdev.net/";

        /* A constant for the amount of money to transfer. Must have at most 2 decimal places. */
        public const double AMOUNT_TO_TRANSFER = 2345.67;


        /* transfers_test
         * --------------
         * An automation example test.
         * Logs into Orpheus (version determined by the above constant). First, checks if the savings account
         * has enough in its balance for the specified amount of money to be transferred (based on constant).
         * Next, goes to the Transfers & Payments screen and transfers the specified amount from savings to 
         * checkings. Next, it goes back to the dashboard to make sure that the balance has changed to reflect
         * the transfer. Next, it logs out then back in to ensure the change has remained. Now, it goes back
         * and transfers the specified amount back from checkings to savings then asserts that the balances have
         * changed to reflect the transfer.
         */
        //[Test]
        public void transfers_test()
        {
            // Setup
            driver.Navigate().GoToUrl(ORPHEUS_URL_2);
            Actions action = new Actions(driver);
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            WebDriverWait wait;
            IWebElement elem;

            // Logs in as admin
            site_login("admin", "cat", "1234", action);

            // Asserts that the balance of the savings account is enough to transfer the specified amount
            Double orig_sav_bal = get_acc_avail_balance("SAVINGS");
            Double orig_chk_bal = get_acc_avail_balance("CLASSIC CHECKING");
            Assert.GreaterOrEqual(orig_sav_bal, AMOUNT_TO_TRANSFER);
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Goes to the Transfers & Payments screen
            navigate_to_transfers();
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Transfers the specified amount from savings to checkings.
            make_transfer(true, AMOUNT_TO_TRANSFER);
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Asserts that the transfer took place properly.
            assert_transferred(AMOUNT_TO_TRANSFER, orig_sav_bal, orig_chk_bal, true);

            // Logs Out
            driver.FindElement(By.XPath("//a [contains(@href, '/Users/Account/LogOff?ReturnUrl=%2Fspa%3FcontentUrl%3Daccounts%2Foverview')]")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_UNTIL_TIME));
            wait.Until(d => d.FindElement(By.Name("UserName")));

            //Signs in again
            site_login("admin", "cat", "1234", action);

            // Asserts that the transfer took place properly.
            assert_transferred(AMOUNT_TO_TRANSFER, orig_sav_bal, orig_chk_bal, true);
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Asserts that the balance of the checking account is enough to transfer the specified amount
            Double next_sav_bal = get_acc_avail_balance("SAVINGS");
            Double next_chk_bal = get_acc_avail_balance("CLASSIC CHECKING");
            Assert.GreaterOrEqual(next_chk_bal, AMOUNT_TO_TRANSFER);

            // Goes to the Transfers & Payments screen
            navigate_to_transfers();
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Transfers the specified amount from savings to checkings.
            make_transfer(false, AMOUNT_TO_TRANSFER);
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            // Asserts that the transfer took place properly.
            assert_transferred(AMOUNT_TO_TRANSFER, next_sav_bal, next_chk_bal, false);

            // Asserts that the current balances are equal to the original balances before any transfers.
            Assert.AreEqual(orig_sav_bal, get_acc_avail_balance("SAVINGS"));
            Assert.AreEqual(orig_chk_bal, get_acc_avail_balance("CLASSIC CHECKING"));
        }

    }
}
