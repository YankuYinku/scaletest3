using System.Reflection;
using apetito.DependencyInjection.Services;
using apetito.Exceptions.Infrastructure;
using HotChocolate.Execution.Configuration;
using HotChocolate.Execution.Options;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.GraphQl;

public class GraphQlServicesRegistrar  : IServicesRegistrar
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var graphQlTypes = GraphQlExtensions.GetTypes();
        
        services.AddGraphQLServer()
            .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
            .RegisterQueries(graphQlTypes[GraphQlExtensions.Query])
            .RegisterMutations(graphQlTypes[GraphQlExtensions.Mutation])
            .SetRequestOptions(_ => new RequestExecutorOptions
            {
                ExecutionTimeout = TimeSpan.FromMinutes(10)
            })
            .AddProjections()
            .AddSorting()
            .AddFiltering();
        
        services.AddErrorFilter<GraphQlErrorFilter>();
        
        services.AddOptions<GraphQlOptions>().Bind(configuration.GetSection("GraphQL"));
    }
}

