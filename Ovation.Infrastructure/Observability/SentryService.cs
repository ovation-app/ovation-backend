using Ovation.Persistence.Observability.Interface;

namespace Ovation.Persistence.Observability
{
    internal class SentryService : ISentryService
    {
        public void AddBreadcrumb(string message, string? category = null, Dictionary<string, string>? data = null)
        {
            SentrySdk.AddBreadcrumb(message, category, data: data);
        }

        public void AddContext(string key, object context)
        {
            SentrySdk.ConfigureScope(scope => scope.Contexts[key] = context);
        }

        public void AddTag(string key, string value)
        {
            SentrySdk.ConfigureScope(scope => scope.SetTag(key, value));
        }

        public void SetUser(SentryUser user)
        {
            SentrySdk.ConfigureScope(scope => scope.User = user);
        }

        public ISpan? StartSpan(string operation, string description)
        {
            try
            {
                ISpan? span = null;
                SentrySdk.ConfigureScope(scope =>
                {
                    var transaction = scope.Transaction;

                    span = transaction?.StartChild(operation, description);
                });

                 return span;
            }
            catch (Exception _)
            {
                return null;
            }
                        
        }

        public void WithScope(Action<Scope> configureScope)
        {
            throw new NotImplementedException();
        }
    }
}
