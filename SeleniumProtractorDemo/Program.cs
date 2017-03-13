using System;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Firefox;
using Protractor;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Internal;
using System.Timers;

namespace ConsoleApplication1
{
    public class Program
    {
        private static System.Timers.Timer aTimer;
        public static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();

            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);
            var ngDriver = new NgWebDriver(driver, "[ng-app = 'UiApp']");


            ngDriver.Navigate().GoToUrl("http://www.glintinc.com/pulsepreview/");

            //determines progress bar values
            var progress = ngDriver.FindElement(By.ClassName("lightText"));
            var valueMax = progress.GetAttribute("aria-valuemax");
            var valueNow = progress.GetAttribute("aria-valuenow");
            var ctrNow = int.Parse(valueNow);
            var ctr = int.Parse(valueMax);
            Console.WriteLine(valueMax);
            Console.WriteLine(valueNow);

            Random rnd = new Random();

            var incrementer = ctr - 1;
            var nextPage = ngDriver.FindElement(By.ClassName("icon-nextquestion"));
            nextPage.Click();

            for (int i = 0; i < ctr; i++)
            {
                var stopIndex = ctr - 1;
                //var progress = ngDriver.FindElement(By.ClassName("lightText"));
                //var valueCurrently = progressInit.GetAttribute("aria-valuenow");
                var progressLoop = ngDriver.FindElement(By.ClassName("lightText"));
                var valueNowLoop = progress.GetAttribute("aria-valuenow");
                Console.WriteLine("The progress bar current value is: " + valueNowLoop);


                int choice = rnd.Next(1, 7);
                if (i == 0)
                {
                    NgWebElement question = ngDriver.FindElements(NgBy.Model("answer.questionAnswerValue"))[1];
                    question.SendKeys("{ ENTER }");
                    Console.WriteLine("Answer Value Clicked");
                }

                if (i > 0 && i < stopIndex)
                {
                    new WebDriverWait(ngDriver, TimeSpan.FromSeconds(30)).Until(ExpectedConditions.ElementExists(NgBy.Repeater("question in section.activeQuestions")));
                    var questionResponse = ngDriver.FindElements(NgBy.Repeater("question in section.activeQuestions"))[i];
                    var elementGo = questionResponse.FindElements(NgBy.Model("answer.questionAnswerValue"))[i];
                    Console.WriteLine("Question Element Found");

                    if (elementGo != null)
                    {

                        Actions builder = new Actions(ngDriver);
                        var mouse = builder.MoveToElement(elementGo).Build();
                        mouse.Perform();
                        //ngDriver.WaitForAngular();
                        new WebDriverWait(ngDriver, TimeSpan.FromSeconds(15)).Until(ExpectedConditions.ElementExists(By.ClassName("addComment")));
                        var commentBox = questionResponse.FindElement(By.ClassName("addComment"));
                        //var commentAnswer = question2.FindElement(NgBy.Model("answer.questionAnswerComment"));
                        //Actions builder1 = new Actions(ngDriver);
                        var mouse1 = builder.MoveToElement(commentBox).Click().Build();
                        mouse1.Perform();
                        //SetTimer();
                        Console.WriteLine("Comment Clicked");
                        new WebDriverWait(ngDriver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementExists(NgBy.Model("answer.questionAnswerComment")));
                        questionResponse.FindElement(NgBy.Model("answer.questionAnswerComment")).SendKeys("Test");
                        new WebDriverWait(ngDriver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementExists(By.XPath(".//*[contains(text(),'Save')]")));
                        new WebDriverWait(ngDriver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementToBeClickable(questionResponse.FindElement(By.XPath(".//*[contains(text(),'Save')]"))));
                        questionResponse.FindElement(By.XPath(".//*[contains(text(),'Save')]")).Click();
                        Console.WriteLine("Comment submitted");
                        //ngDriver.WaitForAngular();
                        //IWebElement element = ngDriver.WrappedDriver.FindElement(By.XPath(".//*[contains(text(),'Save')]"));
                        //element.Click();

                    }

                }

                if(i == stopIndex)
                {

                    NgWebElement questionComment = ngDriver.FindElements(NgBy.Repeater("question in section.activeQuestions"))[i];
                    questionComment.FindElement(NgBy.Model("answer.questionAnswerComment")).SendKeys("Look at this!");
                    Console.WriteLine("Comment added");
                    //Puts comments in question 7

                    new WebDriverWait(ngDriver, TimeSpan.FromSeconds(15)).Until(ExpectedConditions.ElementToBeClickable(questionComment.FindElement(By.XPath(".//*[contains(text(),'Save')]"))));
                    questionComment.FindElement(By.XPath(".//*[contains(text(),'Save')]")).Click();
                    Console.WriteLine("Comment submitted");
                }

                //var valueMin = progress.GetAttribute("aria-valuemin");
                //Console.WriteLine("Progress Bar Value: " + valueMin);

            }


            new WebDriverWait(ngDriver, TimeSpan.FromSeconds(15)).Until(ExpectedConditions.ElementExists(By.ClassName("submitBtnOuter")));
            var submitFinal = ngDriver.FindElement(By.ClassName("submitBtnOuter"));
            //ngDriver.WaitForAngular();
            submitFinal.Click();

            var progressFinal = ngDriver.FindElement(By.ClassName("lightText"));
            var valueNowFinal = progress.GetAttribute("aria-valuenow");
            Console.WriteLine("The progress bar final value is: " + valueNowFinal);
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }
    }
}