using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;
using log4net;


namespace TallDogTestDriver
{
    class Program
    {
        private static string configFile = @"ConfigTest.xml";
        private static string testFileDir = @"..\..\TestData";
        private static TallDogXMLWebTest.WebDriverType driverToTest = TallDogXMLWebTest.WebDriverType.IE32;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///  Execute a set of tests
        /// </summary>
        /// <param name="args">
        /// command line arguments:  [test file directory] [configuration file]
        ///     The configuration file is expected within the test directory.
        /// </param>
        static void Main(string[] args)
        {
            TallDogXMLWebTest.XMLTester tester = null;
            try
            {
                switch (args.Length)
                {
                    case 0:
                        tester = new TallDogXMLWebTest.XMLTester(
                            driverToTest, testFileDir, configFile);
                        break;

                    case 1:
                        tester = new TallDogXMLWebTest.XMLTester(
                            driverToTest, args[0], configFile);
                        break;

                    case 2:
                        tester = new TallDogXMLWebTest.XMLTester(
                            driverToTest, args[0], args[1]);
                        break;

                    default:
                        log.Error("Did not understand command line arguments");
                        break;
                }


                if (tester != null)
                    tester.RunTests();
            }
            catch (Exception ex)
            { 
                log.Error(ex.Message);
                Console.WriteLine("test driver: {0}", ex.Message);
                Pause();
            }
        }


        #region helper methods
        /// <summary>
        /// Shorthand to keep console around long enough to review output.
        /// </summary>
        static void Pause()
        {
            Console.WriteLine("Press ENTER to continue.");
            String readLine = Console.ReadLine();
        }

        #endregion


    }

}
