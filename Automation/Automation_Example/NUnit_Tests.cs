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




        //***Verify History Sorted Test***

                //Specific Helper functions

        /* The amount of time in msec that the test should wait before trying to collect a data field when receiving
         * an error. */
        public const int TRY_AGAIN_WAIT_TIME = 500;

        /* A list of the fields that each transaction has which will be collected for checks to see if transactions are
         * the same. */
        public string[] TRANSACTION_FIELDS = { "AccountId", "Date", "Description", "Note", "Amount", "Balance" };

        /* collect_top_trans_fields
         * ------------------------
         * Collects the transaction fields defined above for the topmost transaction.
         * Due to an Exception Error that appears somewhat randomly which is due to the test trying to find an element
         * that is not quite in view yet, when this error occurs, the test will wait the specified time above then try
         * to access the element again. By the time it waits, the element is ready to be accessed and the error does not
         * occur again allowing the program to proceed forward without problem.
         */
        private List<String> collect_top_trans_fields()
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            WebDriverWait wait;
            List<String> topmost_fields = new List<String>();

            foreach (String field in TRANSACTION_FIELDS)
            {
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
                wait.Until(d => d.FindElement(By.XPath("//td [contains(@data-field, '" + field + "')]")));
                try
                {
                    topmost_fields.Add((String)js.ExecuteScript("return arguments[0].textContent",
                        driver.FindElement(By.XPath("//td [contains(@data-field, '" + field + "')]"))));
                }
                catch
                {
                    System.Threading.Thread.Sleep(TRY_AGAIN_WAIT_TIME);
                    topmost_fields.Add((String)js.ExecuteScript("return arguments[0].textContent",
                        driver.FindElement(By.XPath("//td [contains(@data-field, '" + field + "')]"))));
                }
            }
            return topmost_fields;
        }


        /* The number of transactions that are present at one time. */
        public const int TRANS_PER_PAGINATION = 20;

        /* collect_all_trans_fields
         * --------------------
         * Collects the transaction fields defined above for all of the current transactions present.
         * Due to an Exception Error that appears somewhat randomly which is due to the test trying to find an element
         * that is not quite in view yet, when this error occurs, the test will wait the specified time above then try
         * to access the element again. By the time it waits, the element is ready to be accessed and the error does not
         * occur again allowing the program to proceed forward without problem.
         */
        private List<String>[] collect_all_trans_fields()
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            WebDriverWait wait;
            List<String>[] trans_array = new List<String>[TRANS_PER_PAGINATION];
            for (int i = 0; i < trans_array.Length; i++)
            {
                trans_array[i] = new List<String>();
            }

            foreach (String field in TRANSACTION_FIELDS)
            {
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
                wait.Until(d => d.FindElement(By.XPath("//td [contains(@data-field, '" + field + "')]")));
                try
                {
                    IReadOnlyCollection<IWebElement> elems = driver.FindElements(By.XPath("//td [contains(@data-field, '" + field + "')]"));
                    IEnumerator<IWebElement> ienum = elems.GetEnumerator();
                    for (int i = 0; ienum.MoveNext(); i++) trans_array[i].Add((String)js.ExecuteScript("return arguments[0].textContent", ienum.Current));
                }
                catch
                {
                    System.Threading.Thread.Sleep(TRY_AGAIN_WAIT_TIME);
                    IReadOnlyCollection<IWebElement> elems = driver.FindElements(By.XPath("//td [contains(@data-field, '" + field + "')]"));
                    IEnumerator<IWebElement> ienum = elems.GetEnumerator();
                    for (int i = 0; ienum.MoveNext(); i++) trans_array[i].Add((String)js.ExecuteScript("return arguments[0].textContent", ienum.Current));
                }
            }
            return trans_array;
        }


        /* are_equal_lists
         * ---------------
         * Checks if the two lists have the same strings.
         */
        private bool are_equal_lists(List<String> list1, List<String> list2)
        {
            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                {
                    return false;
                }
            }
            return true;
        }


        /* not_in_list_arr
         * ---------------
         * Returns true if the given list does not have the same contents of any of the lists in the given list array.
         */
        private bool not_in_list_arr(List<String> list, List<String>[] lists)
        {
            for (int i = 0; i < lists.Length; i++)
            {
                if (are_equal_lists(list, lists[i]))
                {
                    return false;
                }
            }
            return true;
        }


        /* collect_displayed_fields
         * ------------------------
         * Collects the fields of the given type and puts them into the given list.
         */
        private void collect_displayed_fields(List<String> collected_fields, String field, List<String>[] new_all_fields, List<String>[] old_all_fields)
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            IReadOnlyCollection<IWebElement> elems = driver.FindElements(By.XPath("//td [contains(@data-field, '" + field + "')]"));
            IEnumerator<IWebElement> ienum = elems.GetEnumerator();
            bool stop_checking = false;
            for (int i = 0; ienum.MoveNext(); i++)
            {
                if (old_all_fields == null || stop_checking || not_in_list_arr(new_all_fields[i], old_all_fields))
                {
                    stop_checking = true;
                    collected_fields.Add((String)js.ExecuteScript("return arguments[0].textContent", ienum.Current));
                }
            }
        }


        /* get_fields
         * ----------
         * Collects a list of fields of the given type of field, scrolling down the screen to have pagination load more until
         * the given number of paginations have occurred.
         */
        private List<String> get_fields(int n, String field)
        {
            IWebElement bar = driver.FindElement(By.XPath("//div [contains(@class, 'k-scrollbar k-scrollbar-vertical')]"));
            List<String> collected_fields = new List<String>();
            List<String>[] new_all_fields = null;
            List<String>[] old_all_fields = null;
            new_all_fields = collect_all_trans_fields();
            collect_displayed_fields(collected_fields, field, new_all_fields, old_all_fields);

            for (int i = 0; i < n; i++)
            {
                List<String> top_trans_fields = collect_top_trans_fields();
                int loop_count = 0;
                bool do_collect = true;
                while (are_equal_lists(top_trans_fields, collect_top_trans_fields()))
                {
                    for (int j = 0; j < DOWN_PRESSES_PER_CHECK; j++) bar.SendKeys(Keys.ArrowDown);
                    loop_count++;
                    if (loop_count >= NUM_LOOPS_BEF_AT_BOTTOM)
                    {
                        do_collect = false;
                        break;
                    }
                }
                old_all_fields = new_all_fields;
                new_all_fields = collect_all_trans_fields();
                if (do_collect) collect_displayed_fields(collected_fields, field, new_all_fields, old_all_fields);
                if (!do_collect) i = n;
            }
            return collected_fields;
        }

        //Console.WriteLine("Fields size: " + collected_fields.Count);
        //    foreach (String f in collected_fields)
        //    {
        //        Console.WriteLine("\"" + f + "\"");
        //    }
        //    Console.WriteLine("End. Fields size: " + collected_fields.Count);


        /* convert_to_double
         * -----------------
         * Takes the given String version of an amount and returns it as a double. Amounts with () are converted to negative numbers.
         */
        private Double convert_to_double(String val)
        {
            if (val.Contains('('))
            {
                int front_ind = val.IndexOf('(');
                int back_ind = val.IndexOf(')');
                val = val.Substring(front_ind + 1, back_ind - front_ind - 1);
                val = val.Replace('$', '-');
            }
            else val = val.Replace('$', ' ').Trim();
            //Console.WriteLine("val: " + val);
            return Convert.ToDouble(val);
        }


        /* verify_sorted
         * -------------
         * Verifies that the given list of fields of the given type are sorted in the order based on the given boolean.
         */
        private void verify_sorted(List<String> fields, String type, bool descend)
        {
            if (type.Equals("Amount"))
            {
                for (int i = 0; i < fields.Count - 1; i++)
                {
                    if (descend) Assert.GreaterOrEqual(convert_to_double(fields[i]), convert_to_double(fields[i + 1]));
                    else Assert.LessOrEqual(convert_to_double(fields[i]), convert_to_double(fields[i + 1]));
                }
                Console.WriteLine("Amounts verified");
            }
            else if (type.Equals("Description"))
            {
                for (int i = 0; i < fields.Count - 1; i++)
                {
                    if (descend) Assert.GreaterOrEqual(fields[i], fields[i + 1]);
                    else Assert.LessOrEqual(fields[i], fields[i + 1]);
                }
                Console.WriteLine("Descriptions verified");
            }
            else if (type.Equals("Date"))
            {
                DateTime[] dates = new DateTime[fields.Count];
                for (int i = 0; i < fields.Count; i++)
                {
                    dates[i] = new DateTime(Convert.ToInt32(fields[i].Substring(6, 2)), 
                        Convert.ToInt32(fields[i].Substring(0, 2)), Convert.ToInt32(fields[i].Substring(3, 2)));
                }
                for (int i = 0; i < dates.Length - 1; i++)
                {
                    if (descend) Assert.GreaterOrEqual(dates[i], dates[i + 1]);
                    else Assert.LessOrEqual(dates[i], dates[i + 1]);
                }
                Console.WriteLine("Dates verified");
            }
        }


                //Specific Constants
        /* A constant for which Orpheus website to use in the following test */
        public const string ORPHEUS_URL_3 = "https://web.orpheusdev.net/";

        /* A constant to determine the max number of paginations the test should have occur before checking if the transactions 
         * are sorted. */
        public const int NUM_PAGINATIONS = 1;

        /* Specifies which fields by which to check if the transactions are sorted. */
        public string[] FIELDS_TO_TEST = { "Date", "Description", "Amount" };

        /* The number of down presses sent before checking if pagination has occurred. */
        public const int DOWN_PRESSES_PER_CHECK = 1;

        /* The number of times the loop to check for pagination should run before assuming the bar has reached the 
         * bottom of the page. */
        public const int NUM_LOOPS_BEF_AT_BOTTOM = 50;

        /* The number of times after verifying a field was sorted correctly in a direction that it scrolls the screen
         * back up before testing the next field or direction. */
        public const int NUM_TIMES_BACK_UP = 3;
        
        /* web_verify_history_sorted
         * -------------------------
         * An automation example test.
         * Logs into Orpheus (version determined by the above constant) then goes to the History screen and the
         * Transactions history section. Starts collecting transaction data by collecting the displayed transactions,
         * then scrolling down until it paginates then collecting the new displayed transactions, and repeating until
         * it has collected the number of transactions specified by the above constant. Then verifies that these
         * transactions are sorted in order by date.
         */
        [Test]
        public void verify_history_sorted()
        {
            // Setup
            driver.Navigate().GoToUrl(ORPHEUS_URL_3);
            Actions action = new Actions(driver);
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            WebDriverWait wait;

            // Logs in as admin
            site_login("admin", "cat", "1234", action);
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            //Dashboard screen
            //Go to History
                        //driver.FindElement(By.XPath("//a [contains(@href, '/payments/index')]")).Click();
                        //if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);
            driver.FindElement(By.XPath("//a [contains(@href, '/accounts/history')]")).Click();
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
            wait.Until(d => d.FindElement(By.XPath("//div [contains(@class, 'k-scrollbar k-scrollbar-vertical')]")));
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
            wait.Until(d => d.FindElement(By.XPath("//td [contains(@data-field, 'Date')]")));
            if (SLEEP_ON) System.Threading.Thread.Sleep(SLEEP_TIME);

            //History screen
            foreach (String field in FIELDS_TO_TEST)
            {
                for (int i = 0; i < NUM_TIMES_BACK_UP; i++) driver.FindElement(By.XPath("//div [contains(@class, 'k-scrollbar k-scrollbar-vertical')]")).SendKeys(Keys.ArrowUp);

                driver.FindElement(By.XPath("//th [contains (@data-title, '" + field + "')]")).Click();
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
                wait.Until(d => d.FindElement(By.XPath("//th [contains(@data-field, '" + field + "') and contains (@aria-sort, 'ascending')]")));
                verify_sorted(get_fields(NUM_PAGINATIONS, field), field, false);

                for (int i = 0; i < NUM_TIMES_BACK_UP; i++) driver.FindElement(By.XPath("//div [contains(@class, 'k-scrollbar k-scrollbar-vertical')]")).SendKeys(Keys.ArrowUp);

                    driver.FindElement(By.XPath("//th [contains (@data-title, '" + field + "')]")).Click();
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
                wait.Until(d => d.FindElement(By.XPath("//th [contains(@data-field, '" + field + "') and contains (@aria-sort, 'descending')]")));
                verify_sorted(get_fields(NUM_PAGINATIONS, field), field, true);
            }
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
