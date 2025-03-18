namespace Application.Common.Interfaces.Services;

public interface IEmailViewRenderer
{
    string RenderView<TModel>(string viewName, TModel model, string email, string subject);
}