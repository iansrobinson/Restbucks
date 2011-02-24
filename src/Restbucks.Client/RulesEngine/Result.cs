namespace Restbucks.Client.RulesEngine
{
    public class Result<T> where T : class
    {
        private readonly bool isSuccessful;
        private readonly T value;

        public Result(bool isSuccessful, T value)
        {
            this.isSuccessful = isSuccessful;
            this.value = value;
        }

        public bool IsSuccessful
        {
            get { return isSuccessful; }
        }

        public T Value
        {
            get { return value; }
        }
    }
}