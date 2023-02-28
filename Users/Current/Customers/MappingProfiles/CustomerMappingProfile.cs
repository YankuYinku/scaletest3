using apetito.meinapetito.Portal.Contracts.Root.Users.Customers;
using AutoMapper;

namespace apetito.meinapetito.Portal.Application.Root.Users.Current.Customers.MappingProfiles;

public class CustomerMappingProfile : Profile
{

    public CustomerMappingProfile()
    {
        CreateMap<apetito.Customers.Contracts.CustomersOfUser.Models.CustomersOfUserDto, CustomersOfUserDto>();
        CreateMap<apetito.Customers.Contracts.CustomersOfUser.Models.CustomerOfUserDto, CustomerDto>()
            .ForMember(a => a.EffectiveOrderSystems, opts => opts.MapFrom(a => new List<string>()
            {
                a.OrderSystem
            }));
        CreateMap<apetito.Customers.Contracts.CustomersOfUser.Models.SortimentDto, Contracts.Root.Users.Sortiments.SortimentDto>();
    }

}
