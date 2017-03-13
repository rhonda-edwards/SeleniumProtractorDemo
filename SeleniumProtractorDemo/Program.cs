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

            //sets timeouts, webdriver and goes to url
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);
            var ngDriver = new NgWebDriver(driver, "[ng-app = 'UiApp']");
            ngDriver.Navigate().GoToUrl("http://www.glintinc.com/pulsepreview/");

            //determines progress bar values and number of questions
            var progress = ngDriver.FindElement(By.ClassName("lightText"));
            var valueMax = progress.GetAttribute("aria-valuemax");
            var valueNow = progress.GetAttribute("aria-valuenow");
            var ctrNow = int.Parse(valueNow);
            var ctr = int.Parse(valueMax);
            Console.WriteLine("The number of questions in this quiz is " + valueMax);
            Console.WriteLine("The progress bar current value is " + valueNow);

            //clicks on initial button to go to questions
            var incrementer = ctr - 1;
            var nextPage = ngDriver.FindElement(By.ClassName("icon-nextquestion"));
            nextPage.Click();
            Console.WriteLine("Initial blue arrow clicked, proceeding to questions");

            //Answer questions loop
            for (int i = 0; i < ctr; i++)
            {
                //Check progress bar is incrementing
                var stopIndex = ctr - 1;
                var progressLoop = ngDriver.FindElement(By.ClassName("lightText"));
                var valueNowLoop = progress.GetAttribute("aria-valuenow");
                Console.WriteLine("The progress bar current value is: " + valueNowLoop);

                //Click choice on question
                if (i == 0)
                {
                    NgWebElement question = ngDriver.FindElements(NgBy.Model("answer.questionAnswerValue"))[1];
                    question.SendKeys("{ ENTER }");
                    Console.WriteLine("Answer Value Clicked for question");
                }


                //Finds active question
                if (i > 0 && i < stopIndex)
                {
                    new WebDriverWait(ngDriver, TimeSpan.FromSeconds(30)).Until(ExpectedConditions.ElementExists(NgBy.Repeater("question in section.activeQuestions")));
                    var questionResponse = ngDriver.FindElements(NgBy.Repeater("question in section.activeQuestions"))[i];
                    var elementGo = questionResponse.FindElements(NgBy.Model("answer.questionAnswerValue"))[i];
                    Console.WriteLine("Question Element Found for question index = " + i);

                    if (elementGo != null)
                    {
                        //hover over and click add comments selection
                        Actions builder = new Actions(ngDriver);
                        var mouse = builder.MoveToElement(elementGo).Build();
                        mouse.Perform();
                        new WebDriverWait(ngDriver, TimeSpan.FromSeconds(15)).Until(ExpectedConditions.ElementExists(By.ClassName("addComment")));
                        var commentBox = questionResponse.FindElement(By.ClassName("addComment"));
                        var mouse1 = builder.MoveToElement(commentBox).Click().Build();
                        mouse1.Perform();
                        Console.WriteLine("Comment Clicked");
                        //adds comments
                        new WebDriverWait(ngDriver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementExists(NgBy.Model("answer.questionAnswerComment")));
                        questionResponse.FindElement(NgBy.Model("answer.questionAnswerComment")).SendKeys("Test");
                        new WebDriverWait(ngDriver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementExists(By.XPath(".//*[contains(text(),'Save')]")));
                        new WebDriverWait(ngDriver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementToBeClickable(questionResponse.FindElement(By.XPath(".//*[contains(text(),'Save')]"))));
                        questionResponse.FindElement(By.XPath(".//*[contains(text(),'Save')]")).Click();
                        Console.WriteLine("Comment submitted");


                    }

                }

                //finds last question
                if(i == stopIndex)
                {
                    //Puts comments in last questions
                    NgWebElement questionComment = ngDriver.FindElements(NgBy.Repeater("question in section.activeQuestions"))[i];
                    questionComment.FindElement(NgBy.Model("answer.questionAnswerComment")).SendKeys("Look at this!");
                    Console.WriteLine("Comment added");

                    //Saves final comments
                    new WebDriverWait(ngDriver, TimeSpan.FromSeconds(15)).Until(ExpectedConditions.ElementToBeClickable(questionComment.FindElement(By.XPath(".//*[contains(text(),'Save')]"))));
                    questionComment.FindElement(By.XPath(".//*[contains(text(),'Save')]")).Click();
                    Console.WriteLine("Comment submitted");
                }


            }

            //Click final submit button
            new WebDriverWait(ngDriver, TimeSpan.FromSeconds(15)).Until(ExpectedConditions.ElementExists(By.ClassName("submitBtnOuter")));
            var submitFinal = ngDriver.FindElement(By.ClassName("submitBtnOuter"));
            submitFinal.Click();

            var progressFinal = ngDriver.FindElement(By.ClassName("lightText"));
            var valueNowFinal = progress.GetAttribute("aria-valuenow");
            Console.WriteLine("The progress bar final value is: " + valueNowFinal);
        }


    }
}