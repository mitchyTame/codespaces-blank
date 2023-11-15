namespace MappingObjects.Mappers;

using AutoMapper;
using AutoMapper.Internal;
using MappingObjects.Models;

public static class CartToSummaryMapper
{
    public static MapperConfiguration GetMapperConfiguration()
    {
        MapperConfiguration config = new(cfg =>
        {


            //Resolve issue with .Net 7 and MaxInteger method.
            cfg.Internal().MethodMappingEnabled = false;

            cfg.CreateMap<Cart, Summary>().ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
            string.Format("{0} {1}",
            src.Customer.FirstName,
            src.Customer.LastName)
            )).ForMember(dest=>dest.Total, opt=> opt.MapFrom(
                src=>src.Items.Sum(item => item.UnitPrice * item.Quantity)));
        });

        return config;
    }
};


