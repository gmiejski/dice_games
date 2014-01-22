using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Threading;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTest
{
    [TestClass]
    public class SeleniumGuiTest
    {

        /**
         * Pierwszy gracz zaklada gre, drugi sprawdza czy parametry utworzenia sa wlasciwe,
         * nastepnie do niej dolacza, kazdy z graczy rzuca co najmniej raz
         * Nastepnie jeden z graczy opuszcza gre, a drugi zostaje o tym poinformowany
         * Po czym drugi takze opuszcza gre
         */
        [TestMethod]
        public void TestCreateGameAndJoinAndLeave()
        {
            using (RemoteWebDriver ffDriver = new FirefoxDriver(), chDriver = new ChromeDriver())
            {
                ffDriver.Navigate().GoToUrl("http://localhost:55910/Pages/Login.aspx");
                LoginToPage(ffDriver, "host");
                ffDriver.FindElement(By.CssSelector("input[value=\"Załóż grę\"]")).Click();
                Assert.IsNotNull(ffDriver.FindElement(By.Id("NewGameTable")));

                // zakładanie nowej gry
                ffDriver.FindElement(By.Id("NewGameName")).SendKeys("test1");

                SelectElement select = new SelectElement(ffDriver.FindElement(By.Id("NewGameType")));

                select.SelectByValue("NPlus");

                ffDriver.FindElement(By.Id("NewGamePlayers")).SendKeys("2");
                ffDriver.FindElement(By.Id("NewGameBots")).SendKeys("0");

                ffDriver.FindElement(By.Id("BotLevelHard")).Click();
                
                ffDriver.FindElement(By.Id("StartGame")).Click();


                // drugi gracz wchodzi do gry
                chDriver.Navigate().GoToUrl("http://localhost:55910/Pages/Login.aspx");
                LoginToPage(chDriver, "user1");

                // sprawdzamy poprawność danych
                var gameName = chDriver.FindElement(By.XPath("//*[@id=\"AvailableGamesTable\"]/tbody/tr[2]/td[1]")).Text;
                var ownerName = chDriver.FindElement(By.XPath("//*[@id=\"AvailableGamesTable\"]/tbody/tr[2]/td[2]")).Text;
                var type = chDriver.FindElement(By.XPath("//*[@id=\"AvailableGamesTable\"]/tbody/tr[2]/td[5]")).Text;
                var botLevel = chDriver.FindElement(By.XPath("//*[@id=\"AvailableGamesTable\"]/tbody/tr[2]/td[6]")).Text;

                Assert.AreEqual("test1", gameName);
                Assert.AreEqual("host", ownerName);
                Assert.AreEqual("NPlus", type);
                Assert.AreEqual("Hard", botLevel);

                // dolaczamy do gry
                chDriver.FindElement(By.Id("AvailableGamesTable_joinGame_0")).Click();

                Thread.Sleep(2000);

                // wykonujemy ruch
                if (chDriver.FindElement(By.Id("ThrowDice")).Enabled)
                {
                    ThrowAllDice(chDriver);
                    ThrowAllDice(ffDriver);
                }
                else
                {
                    ThrowAllDice(ffDriver);
                    ThrowAllDice(chDriver);
                }

                // nie ma zadnych nierzuconych kostek
                Assert.IsTrue(0 == ffDriver.FindElements(By.CssSelector("img [src=\"../Images/0.png\"]")).Count);
                Assert.IsTrue(0 == chDriver.FindElements(By.CssSelector("img [src=\"../Images/0.png\"]")).Count);

                // gracz ff wychodzi z gry
                ffDriver.FindElement(By.Id("LeaveGame")).Click();

                ffDriver.SwitchTo().Alert().Accept();

                Thread.Sleep(100);

                // sprawdzamy czy chrome zobaczył zakończenie gry
                Assert.AreEqual("zakończona", chDriver.FindElement(By.Id("GameState")).Text);

                // ff wylogowuje sie
                ffDriver.FindElement(By.Id("LogoutButton")).Click();
                
                // chrome wylogowuje sie
                chDriver.FindElement(By.Id("LeaveGame")).Click();
                chDriver.FindElement(By.Id("LogoutButton")).Click();
            }
        }

        /**
         * Jeden z graczy zaklada gre na trzech graczy, czeka na dolaczenie innych
         * 
         */
        [TestMethod]
        public void TestCreateJoinAndRunBeforeStart()
        {
            using (RemoteWebDriver ffDriver = new FirefoxDriver(), chDriver = new ChromeDriver())
            {
                ffDriver.Navigate().GoToUrl("http://localhost:55910/Pages/Login.aspx");
                LoginToPage(ffDriver, "host");
                ffDriver.FindElement(By.CssSelector("input[value=\"Załóż grę\"]")).Click();
                Assert.IsNotNull(ffDriver.FindElement(By.Id("NewGameTable")));

                // zakładanie nowej gry
                ffDriver.FindElement(By.Id("NewGameName")).SendKeys("test1");

                SelectElement select = new SelectElement(ffDriver.FindElement(By.Id("NewGameType")));

                select.SelectByValue("NPlus");

                ffDriver.FindElement(By.Id("NewGamePlayers")).SendKeys("4");
                ffDriver.FindElement(By.Id("NewGameBots")).SendKeys("0");

                ffDriver.FindElement(By.Id("BotLevelHard")).Click();

                ffDriver.FindElement(By.Id("StartGame")).Click();


                // drugi gracz wchodzi do gry
                chDriver.Navigate().GoToUrl("http://localhost:55910/Pages/Login.aspx");
                LoginToPage(chDriver, "user1");

                Thread.Sleep(1000);
                // dolaczamy do gry
                chDriver.FindElement(By.Id("AvailableGamesTable_joinGame_0")).Click();

                // gracz udaje ze mysli
                Thread.Sleep(1000);

                // gracz rozmysla sie
                chDriver.FindElement(By.Id("LeaveGame")).Click();
                chDriver.SwitchTo().Alert().Accept();
                Thread.Sleep(500);
                // sprawdzamy czy gra nie zakonczyla sie, a jedynie usunela jednego gracza
                var rows = ffDriver.FindElements(By.CssSelector("#AwaitingPlayersList tr")).Count;
                Assert.AreEqual(2, rows); // sa tylko dwa wiersze, naglowek i gracz ff

                // gracz ff wychodzi
                ffDriver.FindElement(By.Id("LeaveGame")).Click();
                ffDriver.SwitchTo().Alert().Accept();

                // wylogowuja sie
                ffDriver.FindElement(By.Id("LogoutButton")).Click();
                chDriver.FindElement(By.Id("LogoutButton")).Click();
            }
        }

        [TestMethod]
        public void TestCreateAndBack()
        {
            using (RemoteWebDriver ffDriver = new FirefoxDriver())
            {
                ffDriver.Navigate().GoToUrl("http://localhost:55910/Pages/Login.aspx");
                LoginToPage(ffDriver, "host");
                ffDriver.FindElement(By.CssSelector("input[value=\"Załóż grę\"]")).Click();
                Assert.IsNotNull(ffDriver.FindElement(By.Id("NewGameTable")));

                ffDriver.FindElement(By.Id("LeaveNewGame")).Click();
                Assert.AreEqual("Lista gier", ffDriver.Title);

                ffDriver.FindElement(By.Id("LogoutButton")).Click();
            }
        }

        public void ThrowAllDice(RemoteWebDriver driver)
        {
            foreach (var die in driver.FindElements(By.ClassName("userDiceSet")))
            {
                die.Click();
            }
            driver.FindElement(By.Id("ThrowDice")).Click();
            Thread.Sleep(100);
        }

        public void LoginToPage(RemoteWebDriver driver, string username)
        {
            driver.FindElement(By.Id("LoginName")).SendKeys(username);
            driver.FindElement(By.Id("LoginConfirm")).Click();
        }
    }
}
