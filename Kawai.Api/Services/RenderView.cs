using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Text;
using DinkToPdf;
using DinkToPdf.Contracts;
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

    //public byte[] ConvertHtmlToPdf(string html)
    //{
    //    var doc = new HtmlToPdfDocument()
    //    {
    //        GlobalSettings = {
    //        ColorMode = DinkToPdf.ColorMode.Color,
    //        Orientation = DinkToPdf.Orientation.Portrait,
    //        PaperSize = DinkToPdf.PaperKind.A4,
    //        Margins = new MarginSettings {
    //            Top = 10,
    //            Bottom = 10,
    //            Left = 10,
    //            Right = 10
    //        }
    //    },
    //        Objects = {
    //        new ObjectSettings {
    //            HtmlContent = html,
    //            WebSettings = {
    //                DefaultEncoding = "utf-8",
    //                LoadImages = true
    //            }
    //        }
    //    }
    //    };

    //    return _converter.Convert(doc);
    //}

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

