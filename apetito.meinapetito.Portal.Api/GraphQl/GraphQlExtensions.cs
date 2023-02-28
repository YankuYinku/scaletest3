using System.Reflection;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;

namespace apetito.meinapetito.Portal.Api.GraphQl;

public static class GraphQlExtensions
{
    public const string Query = "Query";
    public const string Mutation = "Mutation";

    public static IDictionary<string, IList<Type>> GetTypes()
    {
        var queryTypes = new List<Type>();
        var mutationTypes = new List<Type>();

        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsDefined(typeof(ExtendObjectTypeAttribute)));

        foreach (var type in types)
        {
            var name = type.GetCustomAttributesData()
                .FirstOrDefault(a => a.AttributeType == typeof(ExtendObjectTypeAttribute));

            var value = name?.ConstructorArguments.FirstOrDefault().Value?.ToString();

            switch (value)
            {
                case Query:
                    queryTypes.Add(type);
                    continue;
                case Mutation:
                    mutationTypes.Add(type);
                    continue;
            }
        }

        return new Dictionary<string, IList<Type>>()
        {
            {Query, queryTypes},
            {Mutation, mutationTypes}
        };
    }

    public static IRequestExecutorBuilder RegisterQueries(this IRequestExecutorBuilder builder, IList<Type> queries)
    {
        builder.AddQueryType(q => q.Name(Query));

        builder.RegisterTypes(queries);

        return builder;
    }

    public static IRequestExecutorBuilder RegisterMutations(this IRequestExecutorBuilder builder, IList<Type> mutations)
    {
        builder.AddMutationType(q => q.Name(Mutation));
        
        builder.RegisterTypes(mutations);
       
        return builder;
    }

    private static IRequestExecutorBuilder RegisterTypes(this IRequestExecutorBuilder builder, IList<Type> types)
    {
        foreach (var type in types)
        {
            builder.AddType(type);
        }

        return builder; 
    }
}