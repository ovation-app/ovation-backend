namespace Ovation.Persistence.Observability.Interface
{
    public interface ISentryService
    {
        ISpan? StartSpan(string operation, string description);
        void AddBreadcrumb(string message, string? category = null, Dictionary<string, string>? data = null);
        void SetUser(SentryUser user);
        void AddContext(string key, object context);
        void AddTag(string key, string value);
        void WithScope(Action<Scope> configureScope);
    }
}
