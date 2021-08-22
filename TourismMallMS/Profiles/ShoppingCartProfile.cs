using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Dtos;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Profiles
{
    public class ShoppingCartProfile : Profile
    {
        public ShoppingCartProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<LineItem, LineItemDto>();
        }
    }
}
