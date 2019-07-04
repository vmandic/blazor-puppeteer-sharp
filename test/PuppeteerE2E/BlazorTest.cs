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

  public class Documentation
    {
        [Fact]
        public async Task download_Chromium()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
        }

        [Fact]
        public async Task<Browser> Browser()
        {
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            return browser;
        }

        [Fact]
        public async Task close_Browser()
        {
            var browser = await Browser();

            await browser.CloseAsync();
        }

        [Fact]
        public async Task using_Browser()
        {
            var options = new LaunchOptions { Headless = true };

            using (var browser = await Puppeteer.LaunchAsync(options))
            {
                // ...
            }
        }

        [Fact]
        public async Task<Page> Page()
        {
            var browser = await Browser();

            var page = await browser.NewPageAsync();

            return page;
        }

        [Fact]
        public async Task close_Page()
        {
            var page = await Page();

            await page.CloseAsync();
        }

        [Fact]
        public async Task using_Page()
        {
            var browser = await Browser();

            using (var page = await browser.NewPageAsync())
            {
                // ...
            }
        }

        [Fact]
        public async Task navigation()
        {
            var page = await Page();

            await page.GoToAsync("https://github.com/kblok/puppeteer-sharp");
            await page.GoBackAsync();
            await page.GoForwardAsync();
            await page.ReloadAsync();
        }

        [Fact]
        public async Task timeout()
        {
            var page = await Page();

            var timeout = TimeSpan.FromSeconds(30).Milliseconds; // default value

            page.DefaultNavigationTimeout = timeout;
            //page.DefaultWaitForTimeout = timeout;

            var options = new NavigationOptions { Timeout = timeout };

            await page.GoToAsync("https://github.com/kblok/puppeteer-sharp", options);
            await page.GoBackAsync(options);
            await page.GoForwardAsync(options);
            await page.ReloadAsync(options);
        }

        [Fact]
        public async Task wait()
        {
            var page = await Page();

            var timeout = TimeSpan.FromSeconds(3).Milliseconds;

            var request = page.WaitForRequestAsync("https://github.com/kblok/puppeteer-sharp", new WaitForOptions { Timeout = timeout });
            var response = page.WaitForResponseAsync("https://github.com/kblok/puppeteer-sharp", new WaitForOptions { Timeout = timeout });

            await page.GoToAsync("https://github.com/kblok/puppeteer-sharp");

            await Task.WhenAll(request, response);

            await page.ClickAsync("h1 > strong > a");
            await page.WaitForNavigationAsync(new NavigationOptions { Timeout = timeout });

            await page.WaitForExpressionAsync("1 + 1 === 2", new WaitForFunctionOptions { Timeout = timeout });
            await page.WaitForFunctionAsync("() => window.location.href === 'https://github.com/kblok/puppeteer-sharp'", new WaitForFunctionOptions { Timeout = timeout });
            await page.WaitForSelectorAsync("#readme", new WaitForSelectorOptions { Timeout = timeout });
            await page.WaitForXPathAsync("//*[@id='readme']", new WaitForSelectorOptions { Timeout = timeout });

            await page.WaitForTimeoutAsync(timeout);

            // WaitUntilNavigation
            new NavigationOptions().WaitUntil = new[]
            {
                WaitUntilNavigation.Load,
                WaitUntilNavigation.DOMContentLoaded,
                WaitUntilNavigation.Networkidle0,
                WaitUntilNavigation.Networkidle2
            };

            // Frame
            var frame = page.MainFrame;

            await frame.WaitForExpressionAsync("1 + 1 === 2", new WaitForFunctionOptions { Timeout = timeout });
            await frame.WaitForFunctionAsync("() => window.location.href === 'https://github.com/kblok/puppeteer-sharp'", new WaitForFunctionOptions { Timeout = timeout });
            await frame.WaitForSelectorAsync("#readme", new WaitForSelectorOptions { Timeout = timeout });
            await frame.WaitForXPathAsync("//*[@id='readme']", new WaitForSelectorOptions { Timeout = timeout });

            await frame.WaitForTimeoutAsync(timeout);
        }

        [Fact]
        public async Task values_from_Page()
        {
            var page = await Page();

            var url = page.Url;
            var title = await page.GetTitleAsync();
            var content = await page.GetContentAsync();
            var cookies = await page.GetCookiesAsync();
        }

        [Fact]
        public async Task form()
        {
            var page = await Page();

            await page.GoToAsync("http://toolsqa.com/automation-practice-form/");

            // input / text
            await page.TypeAsync("input[name='firstname']", "Puppeteer");

            // input / radio
            await page.ClickAsync("#exp-6");

            // input / checkbox
            await page.ClickAsync("#profession-1"); // TODO: Check / Uncheck

            // select / option
            await page.SelectAsync("#continents", "Europe");

            // input / file
            var file = await page.QuerySelectorAsync("#photo");
            await file.UploadFileAsync(@"c:\temp\test.jpg");

            // button
            await page.ClickAsync("#submit");
        }

        [Fact]
        public async Task query()
        {
            var page = await Page();

            await page.GoToAsync("https://github.com/kblok/puppeteer-sharp");

            var element = await page.QuerySelectorAsync("h1 > strong > a");
            var elements = await page.QuerySelectorAllAsync("a");

            Assert.NotNull(element);
            Assert.NotEmpty(elements);

            var missingElement = await page.QuerySelectorAsync("a#missing-link");
            var missingElements = await page.QuerySelectorAllAsync("a.missing-link");

            Assert.Null(missingElement);
            Assert.Empty(missingElements);

            var innerHtml = await element.GetPropertyAsync("innerHTML").Result.JsonValueAsync<string>();
            var outerHtml = await element.GetPropertyAsync("outerHTML").Result.JsonValueAsync<string>();
            var url = await element.GetPropertyAsync("href").Result.JsonValueAsync<string>();

            Assert.Equal("puppeteer-sharp", innerHtml);
            Assert.Equal("<a data-pjax=\"#js-repo-pjax-container\" href=\"/kblok/puppeteer-sharp\">puppeteer-sharp</a>", outerHtml);
            Assert.Equal("https://github.com/kblok/puppeteer-sharp", url);

            var innerText = await page.QuerySelectorAsync("h1 > strong > a").EvaluateFunctionAsync<string>("node => node.innerText");
            var outerText = await page.QuerySelectorAsync("h1 > strong > a").EvaluateFunctionAsync<string>("node => node.outerText");
            var textContent = await page.QuerySelectorAsync("h1 > strong > a").EvaluateFunctionAsync<string>("node => node.textContent");
            var hasContent = await page.QuerySelectorAsync("h1 > strong > a").EvaluateFunctionAsync<bool>("(node, value) => node.textContent.includes(value)", "puppeteer-sharp");

            Assert.Equal("puppeteer-sharp", innerText);
            Assert.Equal("puppeteer-sharp", outerText);
            Assert.Equal("puppeteer-sharp", textContent);
            Assert.True(hasContent);

            innerHtml = await page.QuerySelectorAsync("h1 > strong > a").EvaluateFunctionAsync<string>("element => element.innerHTML");
            outerHtml = await page.QuerySelectorAsync("h1 > strong > a").EvaluateFunctionAsync<string>("element => element.outerHTML");
            url = await page.QuerySelectorAsync("h1 > strong > a").EvaluateFunctionAsync<string>("element => element.getAttribute('href')");

            Assert.Equal("puppeteer-sharp", innerHtml);
            Assert.Equal("<a data-pjax=\"#js-repo-pjax-container\" href=\"/kblok/puppeteer-sharp\">puppeteer-sharp</a>", outerHtml);
            Assert.Equal("/kblok/puppeteer-sharp", url);

            innerText = await page.EvaluateFunctionAsync<string>("node => node.innerText", element);
            outerText = await page.EvaluateFunctionAsync<string>("node => node.outerText", element);
            textContent = await page.EvaluateFunctionAsync<string>("node => node.textContent", element);

            Assert.Equal("puppeteer-sharp", innerText);
            Assert.Equal("puppeteer-sharp", outerText);
            Assert.Equal("puppeteer-sharp", textContent);
        }
    }
}