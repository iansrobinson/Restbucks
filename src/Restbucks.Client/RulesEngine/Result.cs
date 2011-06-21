using System.Net;

namespace Restbucks.NewClient.RulesEngine
{
    public class Result
    {
        public static readonly Result Unsuccessful = new Result(false, null);

        private readonly bool isSuccessful;
        private readonly IState state;

        public Result(bool isSuccessful, IState state)
        {
            this.isSuccessful = isSuccessful;
            this.state = state;
        }

        public bool IsSuccessful
        {
            get { return isSuccessful; }
        }

        public IState State
        {
            get { return state; }
        }
    }
}