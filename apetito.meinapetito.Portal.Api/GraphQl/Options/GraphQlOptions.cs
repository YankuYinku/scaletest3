namespace apetito.meinapetito.Portal.Api.GraphQl.Options
{
    public class GraphQlOptions
    {
        public GraphQlOptions()
        {
            PlaygroundApiPath = string.Empty;
            GraphQlEndpointPath = string.Empty;
            PlaygroundgraphQlEndpointPath = string.Empty;
        }

        public string PlaygroundgraphQlEndpointPath { get; set; } 
        public string GraphQlEndpointPath { get; set; }
        public string PlaygroundApiPath { get; set; }
    }
}