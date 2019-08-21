using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TedsChallenge
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            IWebDriver driver = new ChromeDriver();

            //TODO - this should be in its own class 
            driver.Navigate().GoToUrl("http://yelp.com");

            //Ash's code here 
            driver.Manage().Window.Maximize();

            //Step 5: Enter "Find" **********************
            driver.FindElement(By.CssSelector("#find_desc")).SendKeys("Teds Montana Grill");

            //Step 5:Enter "Location" after clearing preexisting/ default text **********************
            driver.FindElement(By.CssSelector("#dropperText_Mast")).Clear();
            driver.FindElement(By.CssSelector("#dropperText_Mast")).SendKeys("Denver, CO");

            //Step 5:Fire search
            driver.FindElement(By.CssSelector("#header-search-submit")).Click();

            //Step 6: Log the first and third results showing under "All Results" on the UI********************
            //get to "All Results" li element simply by searching for that element that has the text. Go up its ancestor (li)
            //Then, fetch the subsequent 1st element and 3rd li element that follow it (they are li siblings of "All Results" li) and drill down to their link text

            //"All Results" li element:
            IWebElement allResultsHeader = driver.FindElement(By.XPath("//h3[text() = 'All Results']/ancestor::li"));
                
            //Get the 1st result from All Results (i.e, 1st li sibling of All Results and dril down to its link tag (Using XPath because I can search based on text
            IWebElement firstResult = driver.FindElement(By.XPath("//h3[text() = 'All Results']/ancestor::li/following-sibling::li[1]/descendant::a[2]"));
            //extract text from link and log it
            string firstResultText = firstResult.Text;
            Console.WriteLine(firstResultText); //O.P: Ted’s Montana Grill - Larimer Square

            //Get the 3rd result from All Results (i.e, 3rd li sibling of All Results and dril down to its link tag
            IWebElement thirdResult = driver.FindElement(By.XPath("//h3[text() = 'All Results']/ancestor::li/following-sibling::li[3]/descendant::a[2]"));
            //extract text from link and log it
            string thirdResultText= thirdResult.Text;
            Console.WriteLine(thirdResultText); //O.P: Ted’s Montana Grill - Lakewood


            //Step 7: Click the link that says "Ted’s Montana Grill - Aurora" *************************
            //This will need finding the link based on link text
            IWebElement resultLink  = driver.FindElement(By.XPath("//a[text() = 'Ted’s Montana Grill - Aurora']"));
            resultLink.Click();


            /*Step 8: Based on the current time of day create logic to decide if it is time to go to Ted’s for lunch or dinner.*********************
            If the current time is after the time it opens, but before 3p.m. then log a message stating “It is time to go to Ted’s for lunch!”.  
            If the current time is after 3p.m. and before the closing time then log a message stating “It is time to go to Ted’s for dinner!”.  
            If the current time is outside of Ted’s business hours then log a message stating “Bummer, Ted’s is closed.  Ted’s will open in (insert hour until Ted’s opens) hour and (insert minutes until Ted’s opens) minutes.”
            */

            //Get today's open and close time from page
            IWebElement businessHourStart = driver.FindElement(By.CssSelector("li[class = 'biz-hours iconed-list-item'] span[class = 'nowrap']:nth-of-type(1)"));
            IWebElement businessHourClose = driver.FindElement(By.CssSelector("li[class = 'biz-hours iconed-list-item'] span[class = 'nowrap']:nth-of-type(2)"));
            string businessHourStartText = businessHourStart.Text; //11:00 am (as displayed on the web page)
            string businessHourCloseText = businessHourClose.Text; //11:00 pm (as fisplayed on the web page)

            //Get current time. 
            Console.WriteLine("Time now is: " + DateTime.Now); //07/27/2019 23:37:49 (A DateTime Object)

            //Converting the open and close hours to DateTime object seems to be the easier way to make comparison with current time

            bool flag = false;
            int compareWithOpenHour = DateTime.Compare((DateTime.Now), (Convert.ToDateTime(businessHourStartText)));
            int compareWithCloseHour = DateTime.Compare((DateTime.Now), (Convert.ToDateTime(businessHourCloseText)));
            if (compareWithOpenHour > 0 && compareWithCloseHour < 0)
            {
                flag = true; //Ted's is open. Check if it's lunch or dinner time.
            }
            else
            {
                Console.WriteLine("Bummer, Ted’s is closed.");
                //Calculate how much more longer for Ted's to open again
            }

            //we'll determine if it's lunch time or dinner by comparing current time to 3pm
            //Build the 3pm object to compare with
            DateTime today = DateTime.Today; //07/28/2019 00:00:00
            DateTime today3pm = today.AddHours(15); //07/28/2019 15:00:00 (today 3 pm)

            int compareWith3pm = DateTime.Compare(DateTime.Now, today3pm);

            if (flag == true)
            {
                if (compareWith3pm < 0)
                {
                    Console.WriteLine("It is time to go to Ted’s for lunch!");
                }
                else
                {
                    Console.WriteLine("It is time to go to Ted’s for dinner!");
                }
                return; //nothing else to do.
            }

            //Else if flag == 0 => Ted's is closed return the time remaing for Ted to open
            //"Ted’s will open in (insert hour until Ted’s opens) hour and(insert minutes until Ted’s opens) minutes.”

            DateTime tomorrowDayOfWeek = DateTime.Today.AddDays(1); //07/29/2019 00:00:00 (get tomorrow)
            string tomorrowDayofWeekAsString = tomorrowDayOfWeek.DayOfWeek.ToString(); //Monday (get tomorrow DayOfWeek as a string)
            string tomorrowShortForm = tomorrowDayofWeekAsString.Substring(0, 3); //Mon (short for Monday)

            IWebElement matchingDayOpenHours = driver.FindElement(By.XPath("//table[@class = 'table table-simple hours-table'] / descendant::th[text() = '"+ tomorrowShortForm + "'] / parent::tr / descendant::span[1]"));
            string matchingDayOpenHoursText = matchingDayOpenHours.Text;
            DateTime tomorrowBusinessHourStart = Convert.ToDateTime(matchingDayOpenHoursText).AddDays(1);//07/29/2019 11:00:00 (Tomorrow's open time as a DateTime object)

            Console.WriteLine("Ted's opens tomorrow at: " + tomorrowBusinessHourStart); //07/29/2019 11:00:00 (Tomorrow's open time as a DateTime object)

            //Finally calculate the time diff b/w now and tomorrow's open hours!
            TimeSpan diff = tomorrowBusinessHourStart.Subtract(DateTime.Now); //17:00:19.4843829
            int diffHours = diff.Hours; //16
            int diffMinutes = diff.Minutes; //40

            Console.WriteLine("Ted’s will open in " + diffHours + " hours and " + diffMinutes + " minutes.");

            driver.Close();
        }
    }
}

/*****************************************
 * O.P: When open and after 3pm:
 * Ted’s Montana Grill - Larimer Square
   Ted’s Montana Grill - Lakewood
   Time now is: 7/28/2019 6:26:49 PM
   It is time to go to Ted’s for dinner!
 *****************************************
 *  O.P: When closed:
 *  Ted’s Montana Grill - Larimer Square
    Ted’s Montana Grill - Lakewood
    Time now is: 7/28/2019 11:08:55 PM
    Bummer, Ted’s is closed.
    Ted's opens tomorrow at: 7/29/2019 11:00:00 AM
    Ted’s will open in 11 hours and 51 minutes.
 ******************************************  
 */
