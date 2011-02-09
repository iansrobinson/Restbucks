namespace Restbucks.Client
{
    public interface IAction<T> where T : class
    {
        ActionResult<T> Execute();
    }
}