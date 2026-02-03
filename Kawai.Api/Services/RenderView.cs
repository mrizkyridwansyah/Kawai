using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Playwright;

namespace Kawai.Api.Services;

public class RazorViewRenderer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;

    public RazorViewRenderer(
        IServiceProvider serviceProvider,
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider)
    {
        _serviceProvider = serviceProvider;
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
    }

    public async Task<string> RenderAsync<TModel>(string viewName, TModel model)
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = _serviceProvider
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor()
        );

        var viewResult = _viewEngine.GetView(null, viewName, true);
        if (!viewResult.Success)
            throw new InvalidOperationException($"View {viewName} not found");

        await using var sw = new StringWriter();

        var viewData = new ViewDataDictionary<TModel>(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary())
        {
            Model = model
        };

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewData,
            new TempDataDictionary(httpContext, _tempDataProvider),
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }

    public async Task<byte[]> GeneratePdfAsync(string html)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions { Headless = true });

        var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);

        return await page.PdfAsync(new PagePdfOptions
        {
            Format = "A4",
            PrintBackground = true
        });
    }

}

