namespace Restbucks.NewClient
{
    public class UnsuccessfulState : IState
    {
        public static readonly IState Instance = new UnsuccessfulState();

        private UnsuccessfulState()
        {
        }
    }
}