using System.Net;

namespace Restbucks.NewClient.RulesEngine
{
    public class On
    {
        private readonly IsApplicableToStateInfoDelegate isApplicableDelegate;

        public static On Status(HttpStatusCode statusCode)
        {
            return new On((response, context) => response.StatusCode.Equals(statusCode));
        }

        public static On Response(IsApplicableToResponseDelegate isApplicableDelegate)
        {
            return new On((response, context) => isApplicableDelegate(response));
        }

        public static On Response(IsApplicableToStateInfoDelegate isApplicableDelegate)
        {
            return new On(isApplicableDelegate);
        }

        public On(IsApplicableToStateInfoDelegate isApplicableDelegate)
        {
            this.isApplicableDelegate = isApplicableDelegate;
        }

        public StateCreationRule Do(CreateStateDelegate createStateDelegate)
        {
            return new StateCreationRule(isApplicableDelegate, new StateFactory(createStateDelegate));
        }
    }
}