using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services;

public class EmailViewRenderer : IEmailViewRenderer
{
    private readonly ICompositeViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public EmailViewRenderer(
        ICompositeViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public string RenderView<TModel>(string viewName, TModel model, string email, string subject)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var routeData = new Microsoft.AspNetCore.Routing.RouteData();
        var actionDescriptor = new ActionDescriptor
        {
            DisplayName = viewName // Provide a display name for debugging
        };

        // Create a minimal HttpContext with services from the application’s DI
        var httpContext = new DefaultHttpContext
        {
            RequestServices = scopedServices
        };

        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        // Generate HTML content using Razor
        var viewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
        if (viewResult.View == null)
        {
            throw new InvalidOperationException($"Could not find the {viewName} view. Searched locations: {string.Join(", ", viewResult.SearchedLocations ?? Enumerable.Empty<string>())}");
        }

        using var writer = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
            new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
            writer,
            new HtmlHelperOptions()
        );

        // Ensure the ViewContext has access to the application’s services
        viewContext.HttpContext.RequestServices = scopedServices;

        try
        {
            viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to render the {viewName} view: {ex.Message}", ex);
        }

        var htmlBody = writer.ToString();

        return htmlBody;
    }
}