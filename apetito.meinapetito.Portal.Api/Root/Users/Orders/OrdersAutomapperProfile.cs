using apetito.meinapetito.Portal.Contracts.Root.Users.Orders.Models;
using apetito.meinapetito.Portal.Contracts.Root.Users.Orders.Models.ApetitoOrders;
using apetito.meinapetito.Portal.Contracts.Root.Users.Orders.Models.HawaOrders;
using apetito.Order.Hawa.Contracts.Models;
using AutoMapper;

namespace apetito.meinapetito.Portal.Api.Root.Users.Orders
{
    public class OrdersAutomapperProfile : Profile
    {
        public OrdersAutomapperProfile()
        {
            CreateMap<OrderDto, OrderSummaryDto>()
                .ForMember(d => d.CustomerNumber, s => s.MapFrom(src => src.Customer.Id))
                .ForMember(d => d.DeliveryDate, s => s.MapFrom(src => src.DeliveryDate))
                .ForMember(d => d.OrderPositionCount, s => s.MapFrom(src => src.OrderPositions.Count + src.FailedOrderPositions.Count))
                .ForMember(d => d.OrderDate, s => s.MapFrom(src => src.OrderDate))
                .ForMember(d => d.Supplier, s => s.MapFrom(src => src.Supplier.Name))
                .ForMember(d => d.Status, s => s.MapFrom(src => src.Status.IsSendToSupplier))
                .ForMember(d => d.TotalAmount, s=> s.MapFrom(src => src.Totals.TotalNetPrice))
                .ForMember(d => d.Id, s => s.MapFrom(src => src.OrderNumber))
                .ForMember(d => d.OrderId, s => s.MapFrom(src => src.Id))
                ;
            
            
            CreateMap<ApetitoOrderHeaderDto, OrderSummaryDto>()
                .ForMember(d => d.CustomerNumber, s => s.MapFrom(src => src.CustomerNumber))
                .ForMember(d => d.DeliveryDate, s => s.MapFrom(src => src.DeliveryDate))
                .ForMember(d => d.OrderDate, s => s.MapFrom(src => src.OrderDate))
                .ForMember(d => d.Supplier, s => s.MapFrom(src => ApetitoOrderType))
                .ForMember(d => d.Status, s => s.MapFrom(src => src.Status != true ? OrderStatusEnum.Succeeded.ToString() : OrderStatusEnum.Failed.ToString()))
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.TotalAmount, s => s.MapFrom(src => src.TotalAmount))
                ;

            CreateMap<Order.Contracts.Orders.Models.OrderHeaderDto, ApetitoOrderHeaderDto>();
            CreateMap<Order.Contracts.Orders.Models.OrderDto, ApetitoOrderDto>();
            CreateMap<Order.Contracts.Orders.Models.OrderPositionDto, ApetitoOrderPositionDto>();


            CreateMap<OrderDto, HawaOrderDto>();
            
            CreateMap<CustomerDto, HawaCustomerDto>();
            CreateMap<SupplierDto, HawaSupplierDto>();
            CreateMap<OrderTotalsDto, HawaOrderTotalsDto>();
            CreateMap<OrderStatusDto, HawaOrderStatusDto>();
            CreateMap<OrderPositionDto, HawaOrderPositionDto>();
            
            
        }

        private const string ApetitoOrderType = "apetito";
    }

}
