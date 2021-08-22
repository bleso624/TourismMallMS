using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Dtos;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Profiles
{
    public class TouristRouteProfile : Profile
    {
        public TouristRouteProfile()
        {
            CreateMap<TouristRoute, TouristRouteDto>()
                .ForMember(
                    dest => dest.Price,
                    opt => opt.MapFrom(src => src.OriginalPrice * (decimal)(src.DiscountPresent ?? 1))
                )
                .ForMember(
                    dest => dest.TravelDays,
                    opt => opt.MapFrom(src => src.TravelDays)
                )
                .ForMember(
                    dest => dest.TripType,
                    opt => opt.MapFrom(src => src.TripType)
                )
                .ForMember(
                    dest => dest.DepartureCity,
                    opt => opt.MapFrom(src => src.DepartureCity)
                );

            CreateMap<TouristRouteForCreationDto, TouristRoute>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => Guid.NewGuid())
                );

            CreateMap<TouristRouteForUpdateDto, TouristRoute>();

            CreateMap<TouristRoute, TouristRouteForUpdateDto>();
        }
    }
}
