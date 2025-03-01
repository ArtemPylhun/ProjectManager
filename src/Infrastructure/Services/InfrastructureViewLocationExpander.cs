using Microsoft.AspNetCore.Mvc.Razor;

namespace Infrastructure.Services;

public class InfrastructureViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
        // No values to populate in this case
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        // Add custom view locations for the Infrastructure project
        var customLocations = new[]
        {
            "wwwroot/Views/Emails/{0}.cshtml",
        };

        return viewLocations.Concat(customLocations);
    }
}