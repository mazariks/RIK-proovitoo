using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Domain;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using Microsoft.VisualBasic;

namespace AutomatedTests;

public class Tests
{
    private IWebDriver? _driver;

    [SetUp]
    public void Setup()
    {
        string path = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
        ChromeOptions options = new ChromeOptions();
        
        // when tests are run, then additional browser windows are not launched.
        options.AddArgument("headless");
        options.AddArgument("window-size=1920,1080");
        
        _driver = new ChromeDriver(path + @"\drivers\", options);
        _driver.Navigate().GoToUrl("https://localhost:7031/");
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }

    [Test]
    public void Event_CanBeCreated_WhenProperDataIsUsed()
    {
        int eventCountBefore = _driver!.FindElements(By.ClassName("event-main-info")).Count;

        CreateTestEvent("Test-üritus", "08032025", "1200", "RIK");

        int eventCountAfter = _driver!.FindElements(By.ClassName("event-main-info")).Count;

        Assert.That(eventCountAfter - eventCountBefore == 1);
    }

    [Test]
    public void Event_CanBeDeleted_WhenStartsInFuture()
    {
        int eventCountBefore = _driver!.FindElements(By.ClassName("event-main-info")).Count;

        CreateTestEvent("Test-üritus kustutamiseks", "08032025", "1200", "RIK");

        _driver!.FindElement(By.LinkText("Kustuta üritus")).Click();

        _driver!.FindElement(By.Id("eventFinalDeleteButton")).Click();

        int eventCountAfter = _driver!.FindElements(By.ClassName("event-main-info")).Count;

        Assert.AreEqual(eventCountBefore, eventCountAfter);
    }

    [Test]
    public void Event_CannotBeCreated_WhenValidationFails()
    {
        int eventCountBefore = _driver!.FindElements(By.ClassName("event-main-info")).Count;

        _driver!.FindElement(By.LinkText("Lisa üritus")).Click();

        IWebElement eventNamingInput = _driver!.FindElement(By.Name("Event.Naming"));
        IWebElement eventStartsAtInput = _driver!.FindElement(By.Name("Event.StartsAt"));
        IWebElement eventPlaceInput = _driver!.FindElement(By.Name("Event.Place"));
        IWebElement eventAddInfoInput = _driver!.FindElement(By.Name("Event.AdditionalInfo"));

        eventNamingInput.SendKeys("");
        _driver!.FindElement(By.Id("eventCreateSubmit")).Click();

        Assert.True(
            _driver!.FindElement(By.Id("Event_Naming-error")).Text == "Selle välja minimaalne pikkus on 1 märk.");


        eventStartsAtInput.SendKeys("");
        _driver!.FindElement(By.Id("eventCreateSubmit")).Click();
        Assert.True(_driver!.FindElement(By.Id("Event_StartsAt-error")).Text == "See väli on kohustuslik.");

        eventPlaceInput.SendKeys("");
        _driver!.FindElement(By.Id("eventCreateSubmit")).Click();
        Assert.True(_driver!.FindElement(By.Id("Event_Place-error")).Text == "Selle välja minimaalne pikkus on 1 märk.");


        eventAddInfoInput.SendKeys(new string('a', 1100));
        Assert.AreEqual(1000, eventAddInfoInput.GetAttribute("value").Length);

        _driver!.FindElement(By.LinkText("Avalehele")).Click();
        int eventCountAfter = _driver!.FindElements(By.ClassName("event-main-info")).Count;

        Assert.AreEqual(eventCountAfter, eventCountBefore);
    }

    [Test]
    public void Event_CanBeEdited_WhenProperDataIsUsed()
    {
        CreateTestEvent("Test-üritus", "10032025", "1200", "RIK");
        var timestamp = DateTime.Now;
        _driver!.FindElement(By.LinkText("Test-üritus")).Click();

        _driver!.FindElement(By.LinkText("Muuda ürituse andmed")).Click();

        _driver!.FindElement(By.Name("Event.Naming")).SendKeys($" (muudetud - {timestamp})");
        _driver!.FindElement(By.Name("Event.Place")).SendKeys($" (muudetud - {timestamp})");

        _driver!.FindElement(By.Id("eventEditSubmit")).Click();

        Assert.That(_driver!.FindElement(By.LinkText($"Test-üritus (muudetud - {timestamp})")).Displayed);
    }

    [Test]
    public void Participant_CanBeAddedToEvent_WhenProperDataIsUsed()
    {
        CreateTestParticipant('f', "TEST-ERA-EESNIMI", "TEST-ERA-PERENIMI");

        _driver!.FindElement(By.LinkText("Osaleja testimiseks")).Click();

        Assert.True(_driver.FindElements(By.ClassName("participant-table-info")).Count == 1);
    }

    [Test]
    public void ExistingParticipant_CanBeAddedToEvent_WhenNotInEventList()
    {
        CreateTestParticipant('f', "TEST-ERA-EESNIMI", "TEST-ERA-PERENIMI");
        
        CreateTestEvent("Olemasoleva osaleja lisamise testimiseks", "10032025", "1200", "RIK");


        _driver!.FindElement(By.LinkText("Lisa osaleja")).Click();

        SelectElement participant = new SelectElement(_driver.FindElement(By.Name("ParticipantId")));

        participant.SelectByIndex(1);

        _driver.FindElement(By.Id("existingParticipantAddButton")).Click();

        _driver.FindElement(By.LinkText("Olemasoleva osaleja lisamise testimiseks")).Click();

        Assert.True(_driver.FindElements(By.ClassName("participant-table-info")).Count == 1);
    }

    [Test]
    public void ExistingParticipant_CannotBeAddedToEvent_WhenInEventList()
    {
        var code = CreateTestParticipant('f', "TEST-ERA-EESNIMI", "TEST-ERA-PERENIMI");

        _driver!.FindElement(By.LinkText("Lisa osaleja")).Click();

        SelectElement participantId = new SelectElement(_driver.FindElement(By.Name("ParticipantId")));

        participantId.SelectByText($"F - TEST-ERA-EESNIMI, TEST-ERA-PERENIMI - {code}");

        _driver.FindElement(By.Id("existingParticipantAddButton")).Click();

        _driver.FindElement(By.Id("message"));

        Assert.AreEqual("Valitud osaleja on juba üritusele registreeritud.", _driver.FindElement(By.Id("message")).Text);
    }

    [Test]
    public void Participant_CanBeRemovedFromEvent_WhenInEventList()
    {
        //Can remove person from event
        CreateTestParticipant('f', "Test-eraosalea-ees", "Test-eraosalea-pere");

        _driver!.FindElement(By.LinkText("Osaleja testimiseks")).Click();

        _driver!.FindElement(By.LinkText("Kustuta osaleja ürituselt")).Click();

        _driver!.FindElement(By.Id("participantDeleteButton")).Click();

        _driver!.FindElement(By.LinkText("Osaleja testimiseks")).Click();

        Assert.AreEqual("Antud üritusega ei ole ühtegi osalejat seotud.",
            _driver.FindElement(By.Id("missingParticipants")).Text);
    }

    [Test]
    public void Participant_CanBeDeletedFully_WhenCheckboxIsFilled()
    {
        var code = CreateTestParticipant('f', "Test-osalejaees-kustutamiseks", "Test-osalejapere-kustutamiseks");

        _driver!.FindElement(By.LinkText("Osaleja testimiseks")).Click();

        _driver!.FindElement(By.LinkText("Kustuta osaleja ürituselt")).Click();

        _driver!.FindElement(By.Id("FullDelete")).Click();

        _driver!.FindElement(By.Id("participantDeleteButton")).Click();

        _driver!.FindElement(By.LinkText("Osaleja testimiseks")).Click();

        Assert.AreEqual("Antud üritusega ei ole ühtegi osalejat seotud.",
            _driver.FindElement(By.Id("missingParticipants")).Text);

        _driver.FindElement(By.LinkText("Avalehele")).Click();

        _driver!.FindElement(By.LinkText("Lisa osaleja")).Click();

        SelectElement participantId = new SelectElement(_driver.FindElement(By.Name("ParticipantId")));

        try
        {
            participantId.SelectByText($"F - Test-osalejaees-kustutamiseks, Test-osalejapere-kustutamiseks - {code}");
        }
        catch (NoSuchElementException)
        {
            // if there is no such element, then deletion is done correctly. Test passed.
            Assert.Pass();
        }
    }


    [Test]
    public void Event_CannotBeDeleted_WhenStartsInPast()
    {
        DateTime currentTimestamp = DateTime.Now;


        CreateTestEvent("Ürituse kustutamiseks",
            $"{currentTimestamp.Day}{currentTimestamp.Month}{currentTimestamp.Year}",
            $"{currentTimestamp.Hour}{currentTimestamp.Minute}", "RIK");
        
            _driver!.FindElement(By.LinkText("Ürituse kustutamiseks")).Click();
            
            try
            {
                _driver.FindElement(By.LinkText("Kustuta üritus"));
            }
            catch (NoSuchElementException)
            {
                //If needed element was not found, then deletion cannot be performed - PASSED!
                Assert.Pass();
            }
    }

    [Test]
    public void Participant_CanBeEdited_WhenInEventList()
    {
        var code = CreateTestParticipant('f', "Test-osalejaees-muutmiseks", "Test-osalejapere-muutmiseks");

        _driver!.FindElement(By.LinkText("Osaleja testimiseks")).Click();
        _driver.FindElement(By.LinkText("Test-osalejaees-muutmiseks, Test-osalejapere-muutmiseks")).Click();
        _driver.FindElement(By.LinkText("Muuda andmed")).Click();

        _driver.FindElement(By.Name("Participant.FirstName")).SendKeys(" (muudetud)");

        _driver.FindElement(By.Id("participantEditButton")).Click();

        _driver.FindElement(By.LinkText("Osaleja testimiseks")).Click();

        Assert.That(_driver.FindElement(By.LinkText("Test-osalejaees-muutmiseks (muudetud), Test-osalejapere-muutmiseks")).Displayed);
    }


    [Test]
    public void PrivateParticipant_CannotBeCreated_WhenValidationFails()
    {
        CreateTestEvent("Osaleja testimiseks (validatsioon)", "10032025", "1200", "RIK");
        _driver!.FindElement(By.LinkText("Lisa osaleja")).Click();

        _driver.FindElement(By.LinkText("Lisa uus osaleja")).Click();

        SelectElement participantTypePrivate = new SelectElement(_driver.FindElement(By.Name("Participant.Type")));

        participantTypePrivate.SelectByText("Füüsiline");

        _driver.FindElement(By.Name("Participant.FirstName")).SendKeys("");
        _driver.FindElement(By.Id("participantCreateButton")).Click();
        Assert.AreEqual("Eraisiku puhul on väljad 'EESNIMI', 'PERENIMI' ja 'ISIKUKOOD' kohustuslikud.",
            _driver.FindElement(By.Id("message")).Text);
        _driver.FindElement(By.Name("Participant.FirstName")).SendKeys("EESNIMI");

        _driver.FindElement(By.Name("Participant.LastName")).SendKeys("");
        _driver.FindElement(By.Id("participantCreateButton")).Click();
        Assert.AreEqual("Eraisiku puhul on väljad 'EESNIMI', 'PERENIMI' ja 'ISIKUKOOD' kohustuslikud.",
            _driver.FindElement(By.Id("message")).Text);
        _driver.FindElement(By.Name("Participant.LastName")).SendKeys("PERENIMI");

        string code = GenerateRandomCode(11);
        _driver.FindElement(By.Name("Participant.IdentityCode")).SendKeys("");
        _driver.FindElement(By.Id("participantCreateButton")).Click();
        Assert.AreEqual("Eraisiku puhul on väljad 'EESNIMI', 'PERENIMI' ja 'ISIKUKOOD' kohustuslikud.",
            _driver.FindElement(By.Id("message")).Text);

        _driver.FindElement(By.Name("Participant.IdentityCode")).SendKeys($"{code}");

        _driver.FindElement(By.Name("Participant.AdditionalInfo")).SendKeys($"{new string('a', 1600)}");

        _driver.FindElement(By.Id("participantCreateButton")).Click();

        Assert.AreEqual("Lisainfo teksti pikkus on liiga suur. Max = 1500.",
            _driver.FindElement(By.Id("message")).Text);
    }

    [Test]
    public void BusinessParticipant_CannotBeCreated_WhenValidationFails()
    {
        CreateTestEvent("Osaleja testimiseks (validatsioon)", "10032025", "1200", "RIK");
        _driver!.FindElement(By.LinkText("Lisa osaleja")).Click();

        _driver.FindElement(By.LinkText("Lisa uus osaleja")).Click();

        SelectElement participantTypePrivate = new SelectElement(_driver.FindElement(By.Name("Participant.Type")));

        participantTypePrivate.SelectByText("Juriidiline");

        _driver.FindElement(By.Name("Participant.Naming")).SendKeys("");
        _driver.FindElement(By.Id("participantCreateButton")).Click();
        Assert.AreEqual("Juriidilise isiku puhul on väljad 'NIMETUS', 'REGISTRIKOOD' ja 'OSALEJATE ARV' kohustuslikud.",
            _driver.FindElement(By.Id("message")).Text);
        _driver.FindElement(By.Name("Participant.Naming")).SendKeys("NIMETUS");

        _driver.FindElement(By.Name("Participant.RegisterCode")).SendKeys("");
        _driver.FindElement(By.Id("participantCreateButton")).Click();
        Assert.AreEqual("Juriidilise isiku puhul on väljad 'NIMETUS', 'REGISTRIKOOD' ja 'OSALEJATE ARV' kohustuslikud.",
            _driver.FindElement(By.Id("message")).Text);
        _driver.FindElement(By.Name("Participant.RegisterCode")).SendKeys($"{GenerateRandomCode(8)}");

        _driver.FindElement(By.Name("Participant.AmountOfGuests")).SendKeys("");
        _driver.FindElement(By.Id("participantCreateButton")).Click();
        Assert.AreEqual("Juriidilise isiku puhul on väljad 'NIMETUS', 'REGISTRIKOOD' ja 'OSALEJATE ARV' kohustuslikud.",
            _driver.FindElement(By.Id("message")).Text);
        _driver.FindElement(By.Name("Participant.AmountOfGuests")).SendKeys("10");

        _driver.FindElement(By.Name("Participant.AdditionalInfo")).SendKeys($"{new string('a', 5100)}");

        Assert.AreEqual(5000, _driver.FindElement(By.Name("Participant.AdditionalInfo")).GetAttribute("value").Length);
    }


    private void CreateTestEvent(string naming, string date, string time, string place, string? extraInfo = null)
    {
        IWebElement eventCreateButton = _driver!.FindElement(By.LinkText("Lisa üritus"));
        eventCreateButton.Click();

        IWebElement eventNamingInput = _driver!.FindElement(By.Name("Event.Naming"));
        IWebElement eventStartsAtInput = _driver!.FindElement(By.Name("Event.StartsAt"));
        IWebElement eventPlaceInput = _driver!.FindElement(By.Name("Event.Place"));
        IWebElement eventAdditionalInfoInput = _driver!.FindElement(By.Name("Event.AdditionalInfo"));

        eventNamingInput.SendKeys(naming);
        eventStartsAtInput.SendKeys(date);
        eventStartsAtInput.SendKeys(Keys.Tab);
        eventStartsAtInput.SendKeys(time);

        eventPlaceInput.SendKeys(place);
        eventAdditionalInfoInput.SendKeys(extraInfo ?? new string('a', 10));

        _driver!.FindElement(By.Id("eventCreateSubmit")).Click();
    }


    private string CreateTestParticipant(char type, string? firstName = null, string? lastName = null, string? naming = null,
        int? amountOfGuests = null)
    {
        CreateTestEvent("Osaleja testimiseks", "10032025", "1200", "RIK");

        _driver!.FindElement(By.LinkText("Lisa osaleja")).Click();

        _driver.FindElement(By.LinkText("Lisa uus osaleja")).Click();
        string code;
        SelectElement participantType = new SelectElement(_driver.FindElement(By.Name("Participant.Type")));
        if (type == 'f')
        {
            code = GenerateRandomCode(11);
            participantType.SelectByText("Füüsiline");
            _driver.FindElement(By.Name("Participant.FirstName")).SendKeys($"{firstName ?? "Test-eraosalea-ees"}");
            _driver.FindElement(By.Name("Participant.LastName")).SendKeys($"{lastName ?? "Test-eraosalea-pere"}");

            _driver.FindElement(By.Name("Participant.IdentityCode")).SendKeys($"{code}");
            _driver.FindElement(By.Name("Participant.AdditionalInfo")).SendKeys("Test-eraosalea-lisainfo");
        }
        else
        {
            code = GenerateRandomCode(8);
            participantType.SelectByText("Juriidiline");
            _driver.FindElement(By.Name("Participant.Naming")).SendKeys($"{naming ?? "Test-äriosalea-nimetus"}");

            _driver.FindElement(By.Name("Participant.RegisterCode")).SendKeys($"{code}");
            _driver.FindElement(By.Name("Participant.AmountOfGuests")).SendKeys($"{amountOfGuests.ToString() ?? "10"}");
            _driver.FindElement(By.Name("Participant.AdditionalInfo")).SendKeys("Test-äriosalea-lisainfo");
        }


        _driver.FindElement(By.Id("participantCreateButton")).Click();
        return code;
    }

    private string GenerateRandomCode(int length)
    {
        var random = new Random();
        var code = new string[11];
        for (int i = 0; i < length; i++)
        {
            code[i] = random.Next(0, 10).ToString();
        }

        return string.Join("", code);
    }
}