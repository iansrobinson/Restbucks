namespace RestInPractice.RestToolkit.Hypermedia
{
    public interface IUriTemplate
    {
        string RoutePrefix { get; }
        string UriTemplateValue { get; }
    }
}