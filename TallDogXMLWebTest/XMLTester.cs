using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using log4net;

namespace TallDogXMLWebTest
{
    /// <summary>
    /// Read an XML file of test definitions and execute each web-based test
    /// </summary>
    public class XMLTester
    {
        // class default values
        private string _configFile = @"ConfigTest.xml";
        private string _testFileDir = @"..\..\TestData";
        private string _testFilePattern = "Test*.xml";


        private WebDriver _webDriver = null;
        private Emailer _emailSuccess = null;
        private Emailer _emailFailure = null;

        private readonly log4net.ILog _log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        #region Constructors & Initializers

        public XMLTester()
        {
            //default constructor. Use member defaults as defined.
            InitializePlace(_testFileDir, _configFile);

            //Default to a headless browser.
            InitializeWebDriver(WebDriverType.Headless);
        }
        public XMLTester(string testFileDir)
        {
            InitializePlace(testFileDir);
        }
        public XMLTester(WebDriverType driverToUse, string testFileDir, string configFile)
        {
            InitializePlace(testFileDir, configFile);
            InitializeWebDriver(driverToUse);
        }
 
        private void InitializePlace(string testFileDir)
        {
            if (Directory.Exists(testFileDir))
            {
                _testFileDir = testFileDir;
            }
            else
            {
                string msg = "Directory " + testFileDir + " does not exist.";
                _log.Error(msg);
                throw new FileNotFoundException(msg);
            }
        }
        private void InitializePlace(string testFileDir, string configFile)
        {
            InitializePlace(testFileDir);
            if (File.Exists(Path.Combine(testFileDir,configFile)))
            {
                _configFile = configFile;
            }
            else
            {
                string msg = string.Format("File {0} does not exist in {1}",configFile, testFileDir);
                _log.Error(msg);
                throw new FileNotFoundException(msg);
            }
        }

        private void InitializeWebDriver(WebDriverType driverToUse)
        {
            try
            {
                _webDriver = new WebDriver(driverToUse);
            }
            catch (Exception ex)
            {
                _log.FatalFormat("Could not instatiate driver {0}. Did you download the required executables? \n{1}",
                    driverToUse.ToString(), ex.Message);
                throw (ex);
            }
        }

        #endregion


        /// <summary>
        /// Using a pre-defined configuration file and test directory,
        ///  traverse each test file and execute the contained tests.
        /// </summary>
        public void RunTests()
        {
            GetGlobalTestConfig();
            TraverseXMLTestDir();

        }


        #region private methods

        /// <summary>
        /// Pull a list of tag values from an XML Document
        /// </summary>
        /// <param name="xmlDoc">Loaded XML document</param>
        /// <param name="tag">full tag path</param>
        /// <returns>list of tag values</returns>
        private List<string> PullFromXML(XmlDocument xmlDoc, string tag)
        {
            List<string> tagValueList = new List<string>();
            foreach (XmlNode node in xmlDoc.SelectNodes(tag))
            {
                tagValueList.Add(node.InnerText);
            }
            return tagValueList;
        }

        /// <summary>
        /// Determine and instantiate the specified web driver.
        /// Default to headless if no driver is defined.
        /// </summary>
        /// <param name="xmlDoc">loaded xml document to scan for driver definition.</param>
        private void DefineDriver(XmlDocument xmlDoc)
        {
            List<string> xmlDriver = PullFromXML(xmlDoc, "//test-attributes/web-driver");
            if (xmlDriver.Count() == 0)
                _webDriver = new WebDriver();
            else
            {
                string driverToUse = xmlDriver[0].ToLower();
                if (0 == string.Compare(driverToUse, "chrome"))
                    _webDriver = new WebDriver(WebDriverType.Chrome);
                else if (0 == string.Compare(driverToUse, "ie32"))
                    _webDriver = new WebDriver(WebDriverType.Firefox);
                else if (0 == string.Compare(driverToUse, "headless"))
                    _webDriver = new WebDriver(WebDriverType.Headless);
                else if (0 == string.Compare(driverToUse, "ie32"))
                    _webDriver = new WebDriver(WebDriverType.IE32);
                else if (0 == string.Compare(driverToUse, "safari"))
                    _webDriver = new WebDriver(WebDriverType.Safari);
                else
                {
                    _log.WarnFormat("Did not understand driver type {0}. Using default.", driverToUse);
                    _webDriver = new WebDriver();
                }
            }
        }
        #endregion

        #region public interface

        /// <summary>
        /// Read definitions global to all tests within the test file directory.
        /// </summary>
        public void GetGlobalTestConfig()
        {
            GetGlobalTestConfig(Path.Combine(_testFileDir, _configFile));
        }
        /// <summary>
        /// Read definitions global to all tests within the test file directory.
        /// </summary>
        /// <param name="configFileFullPath">Full path of test definition file</param>
        public void GetGlobalTestConfig(string configFileFullPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFileFullPath);

            // Setup emailers for success and failure
            _emailSuccess = new Emailer(
                PullFromXML(xmlDoc, "//log-email/success/email-to"),
                PullFromXML(xmlDoc, "//log-email/success/email-cc"),
                PullFromXML(xmlDoc, "//log-email/success/email-bcc"));
            _emailFailure = new Emailer(
                PullFromXML(xmlDoc, "//log-email/failure/email-to"),
                PullFromXML(xmlDoc, "//log-email/failure/email-cc"),
                PullFromXML(xmlDoc, "//log-email/failure/email-bcc"));
        }

        /// <summary>
        /// Traverse xml test files & execute tests in the pre-defined test directory.
        /// </summary>
        public void TraverseXMLTestDir()
        {
            TraverseXMLTestDir(_testFileDir);
        }
        /// <summary>
        /// Traverse xml test files & execute tests in the specified test directory.
        /// </summary>
        /// <param name="TestFileDir">Directory Path</param>
        public void TraverseXMLTestDir(string TestFileDir)
        {
            try
            {
                // Put all file names in test file directory into array.
                string[] testFiles = Directory.GetFiles(TestFileDir, _testFilePattern);

                // Run tests in each file
                foreach (string filename in testFiles)
                {
                    //Console.WriteLine("Testing with " + filename);
                    XPathTest(filename);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("XPathTest: {0}", ex.Message);
                _log.Error(ex.Message);
            }

        }

        /// <summary>
        /// Execute the tests within the given XML file.
        /// At present, this method only expects target-text tests
        /// See the project subfolder "TestData" for sample XML syntax.
        /// </summary>
        /// <param name="xmlFileName">XML file housing test definition</param>
        public void XPathTest(string xmlFileName)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFileName);

                //Collect web driver to use for this test file
                DefineDriver(xmlDoc);

                // Collect base url on which all tests will run
                XmlNode urlNode = xmlDoc.SelectSingleNode("//address/url");
                string url = urlNode.InnerText;
                _log.InfoFormat("Testing against URL '{0}'", url);


                // Iterate through each test in the XML document
                // Only test type at present is target-text response time
                foreach (XmlNode node in xmlDoc.SelectNodes("//test"))
                {
                    long timeoutLimit = long.Parse(node.SelectSingleNode("timeout-period").InnerText);
                    string page = node.SelectSingleNode("page").InnerText;
                    string expectedText = node.SelectSingleNode("target-text").InnerText;
                    int timeMS = _webDriver.FindTargetTextTime(url, page, expectedText, timeoutLimit);

                    //Console.WriteLine("{0} ms to find '{1}' on page {2}", timeMS, expectedText,page);
                    _log.InfoFormat("{0} ms to find '{1}'  on page {2}", timeMS, expectedText, page);


                    //Failure cases: 
                    //  if time to find target text is zero, the text was not found
                    //  if time to find target text exceeds the threshold then the site is not functioning
                    int timeThreshold = int.Parse(node.SelectSingleNode("time-expected").InnerText);
                    if (timeMS != 0 & timeMS<= timeThreshold)
                    {
                        // response time was within the threshold limit; send
                        // the success message
                        string msg = string.Format("{0} ms to complete '{1}'",
                            timeMS, node.SelectSingleNode("test-name").InnerText);
                        _emailSuccess.SendEmail(
                            "passed: "+node.SelectSingleNode("test-name").InnerText,
                            msg,
                            null
                            );
                    }
                    else
                    {
                        // response time was NOT within the threshold limit; send
                        // the failure message
                        string msg = string.Format("{0} ms to complete '{1}'",
                            timeMS, node.SelectSingleNode("test-name").InnerText);
                        _emailFailure.SendEmail(
                            "failed: "+node.SelectSingleNode("test-name").InnerText,
                            msg,
                            null
                            );

                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("XPathTest: {0}", ex.Message);
                _log.Error(ex.Message);
            }
        }

        #endregion

    }
}
