using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Playwright;
namespace PlaywrightTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test()
    {
        //Playwright
        using var playwright = await Playwright.CreateAsync();
        //Browser
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new[]
            //disable certain flags that detect Playwright
            {
                "--disable-blink-features=AutomationControlled",
                "--disable-web-security",
                "--no-sandbox"
            }
        });
        //Page
        var page = await browser.NewPageAsync(new BrowserNewPageOptions
        {
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
        });
        //Search for Prometheus Group
        await page.GotoAsync("http://www.google.com/", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
        await page.FillAsync("#APjFqb", "Prometheus Group");
        await page.ClickAsync("text=Google Search");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "googleResultsAfterClick.jpg"
        });

        //Verify search results
        var isExist = await page.Locator("h3:has-text('Prometheus Group')").First.IsVisibleAsync();
        Console.WriteLine("Promethus Group search results found");
        Assert.That(isExist, Is.True);

        //Access Contact Us page
        try
        {
            await page.Locator("text=Contact Us").ClickAsync(new() { Timeout = 5000 }); 
            Console.WriteLine("Successfully navigated by clicking the link.");
        }
        catch (TimeoutException) 
        {
            Console.WriteLine("Could not find the link. Navigating directly to the URL instead.");
            await page.GotoAsync("https://www.prometheusgroup.com/contact-us");
        }

        //disperse promotional popup
        try
        {
            Console.WriteLine("Attempting to close pop-up...");
            await page.Locator("[data-test-id=\"interactive-frame\"]").ContentFrame.GetByRole(AriaRole.Button, new() { Name = "Close" }).ClickAsync(new() { Timeout = 10000 });
            Console.WriteLine("Pop-Up closed.");
        }
        catch (Exception)
        {
           Console.WriteLine("Pop-up didn't appear, proceeding with sign-up."); 
        }
        await page.FillAsync("#firstname-fe70f03d-5bac-4ad3-a698-3e130182d674_5824", "James");
        await page.FillAsync("#lastname-fe70f03d-5bac-4ad3-a698-3e130182d674_5824", "Urani");
        await page.ClickAsync("input[type='submit']");


        //Verify invalid fields >= 4
        var invalidFieldsCount = await page.Locator(".hs-input.invalid.error").CountAsync();
        Console.WriteLine($"Found {invalidFieldsCount} invalid fields.");
        Assert.That(invalidFieldsCount, Is.GreaterThanOrEqualTo(4), $"Expected to find 4 or more invalid fields, but found {invalidFieldsCount}.");

    }
}
