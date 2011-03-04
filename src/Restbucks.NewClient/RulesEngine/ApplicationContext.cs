namespace Restbucks.NewClient.RulesEngine
{
    public class ApplicationContext
    {
        private readonly object input;

        public ApplicationContext(object input)
        {
            this.input = input;
        }

        public object Input
        {
            get { return input; }
        }
    }
}