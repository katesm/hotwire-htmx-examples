using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace hotwire_turbo_stimulus_demo.Controllers;

internal static class ControllerRenderingExtensions
{
    public static string RenderViewToString(this Controller controller, string viewName, ViewDataDictionary viewData)
    {
        var viewEngine = controller.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
        var result = viewEngine.FindView(controller.ControllerContext, viewName, false);
        if (!result.Success)
        {
            throw new InvalidOperationException($"View '{viewName}' was not found.");
        }

        using var writer = new StringWriter();
        var context = new ViewContext(
            controller.ControllerContext,
            result.View,
            viewData,
            controller.TempData,
            writer,
            new HtmlHelperOptions());

        result.View.RenderAsync(context).GetAwaiter().GetResult();
        return writer.ToString();
    }
}
