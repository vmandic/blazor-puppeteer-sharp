using System;
using System.Threading.Tasks;
using FluentAssertions;
using PuppeteerSharp;
using Xunit;

namespace PuppeteerE2E
{
  public class BlazorTest
  {
    [Fact]
    public async Task Should_navigate_click_naviagte_and_increment_counter()
    {
      await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
      var browser = await Puppeteer.LaunchAsync(new LaunchOptions
      {
        Headless = true
      });

      var page = await browser.NewPageAsync();
      await page.GoToAsync("https://localhost:5001");

      await page.ClickAsync("a[href=counter]");

      page.Url.Should().EndWith("counter");

      await page.ClickAsync(".btn-primary");

      var pElement = await page.QuerySelectorAsync(".content p");
      pElement.Should().NotBeNull();

      var prop = await pElement.GetPropertyAsync("innerHTML").Result.JsonValueAsync();
      prop.Should().Be("Current count: 1");

      await browser.CloseAsync();
    }
  }
}