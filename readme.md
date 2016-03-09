# XML Web Test

This is a test suite which uses selenium to measure response and test interaction of web applications. 

## Scheduing Tests (to the minute)

Copy the bin/Debug folder to the desired location. This folder will have a TestDirectory in it already which will be used by default. Optionally, you can put the folder of XML tests, named "TestFiles", anywhere of your choosing and tell the scheduled task to run from there.

### Schedule the Task
The account that runs the task must have a password.

* Open the Task Scheduler from the start menu: Start > Right-click on Computer > Manage > Task Scheduler
* Select "Create Basic Task" from the right pane & follow the prompts. Set the trigger to run daily; multiple runs/day are defined later. You want to execute "TestDriver.exe" in the Debug folder.
* To see the custom task you've scheduled, you must find it the Task Scheduler Library.
* Select the task you just created.
* Select "properties" from the right click menu
* Select the Triggers tab
* Edit the trigger. There is an option to repeat every X minutes.

## References Setup Summary
NuGet will automatically update the packages required upon Visual Studio startup.

### Test Driver
 * log4net 

### WebTester 
 * log4net 
 * selenium web driver and the selenium webdriver support classes 
 * selenium chrome driver
 * selenium firefox driver
 * selenium IE driver
 * simple browser and simple browser web driver


## History
This is to document what I did to get this going.

## References
 * http://www.anujchaudhary.com/2012/11/selenium-automating-ui-testing-for.html
 * https://www.youtube.com/watch?v=2ac4hKSfQ9s
 * http://geekswithblogs.net/MarkPearl/archive/2012/01/30/log4net-basics-with-a-console-application-c.aspx

### Test Driver
The test driver requires log4net: I installed through nuget.
http://geekswithblogs.net/MarkPearl/archive/2012/01/30/log4net-basics-with-a-console-application-c.aspx

 
### Firefox Setup

This was easy!  From https://www.youtube.com/watch?v=2ac4hKSfQ9s.

1. install nuget package manager
1. create console application project
1. right click on project & select nuget package manager.
1. do online search for selenium web driver; install that and the selenium webdriver support classes.
1. add to program.cs:
    + using OpenQA.Selenium;
    + using OpenQA.Selenium.Firefox;


### IE Setup 

From http://www.anujchaudhary.com/2012/11/selenium-automating-ui-testing-for.html

Download:

+ Newtonsoft.Json.dll from [http://www.dllme.com/dll/files/newtonsoft_json_dll.html] 
+ Ionic.Zip.dll from [http://www.dllme.com/dll/files/ionic_zip_dll.html]
+ Selenium Chrome web driver from [https://sites.google.com/a/chromium.org/chromedriver/downloads]
+ from [http://docs.seleniumhq.org/download/]:
    - Selenium IE 32 bit web driver: IEDriverServer_Win32_2.47.0.zip (Use this even if you are on a 64 bit OS)
    - Selenium C# driver: selenium-dotnet-2.47.0 

1. Put in a folder named "Dependencies" under the XMLWebTest project: chromedriver.exe,  IEDriverServer.exe, WebDriver.dll, Newtonsoft.Json.dll, Ionic.Zip.dll
2. Add references to the XMLWebTest project: WebDriver.dll, System.Drawing
3. Add references to the TestDRiver project: XMLWebTest
