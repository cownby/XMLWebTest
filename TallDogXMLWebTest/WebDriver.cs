using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using SimpleBrowser.WebDriver;

namespace TallDogXMLWebTest
{
    // Available Selenium web drivers within test framework.
    public enum WebDriverType
    {
        Firefox,
        Chrome,
        IE32,
        Safari,
        Headless
    }

    /// <summary>
    /// This is a polymorphic selenium web driver class for testing purposes.
    /// </summary>
    public class WebDriver
    {
        //web driver which may implement any driver in WebDriverType
        IWebDriver driver;

        #region constructors & destructor

        public WebDriver()
        {
            // use a headless driver by default
            driver = new SimpleBrowserDriver();
        }

        public WebDriver(WebDriverType driverToUse)
        {
            try
            {
                // instantiate with the chosen driver
                switch (driverToUse)
                {
                    case WebDriverType.Firefox:
                        driver = new FirefoxDriver();
                        break;

                    case WebDriverType.Chrome:
                        driver = new ChromeDriver();
                        break;

                    case WebDriverType.IE32:
                        driver = new InternetExplorerDriver();
                        break;

                    case WebDriverType.Safari:
                        driver = new SafariDriver();
                        break;

                    case WebDriverType.Headless:
                    default:
                        driver = new SimpleBrowserDriver();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        ~WebDriver()
        {
            if (null != driver)
                driver.Close();
        }

        #endregion

        /// <summary>
        /// Time loading a web page and ensure a text string is present.
        /// </summary>
        /// <param name="url">base url to load</param>
        /// <param name="page">page to load</param>
        /// <param name="textThatShouldBePresent">text to find on page</param>
        /// <param name="timeoutLimit">timeout limit (not yet used)</param>
        /// <returns>milliseconds to load page and locate target text. Zero upon failure.</returns>
        public int FindTargetTextTime(string url, string page, string textThatShouldBePresent, long timeoutLimit)
        {
            //TODO: implement timeout threshold

            try
            {

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                string fullURL = url + "/" + page;
                driver.Navigate().GoToUrl(fullURL);
                bool textPresent = driver.PageSource.Contains(textThatShouldBePresent);
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                if (textPresent)
                    return ts.Milliseconds;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("FindTargetTextTime: {0}", ex.Message);
                throw (ex);
            }
            return 0;
        }

    }
}
