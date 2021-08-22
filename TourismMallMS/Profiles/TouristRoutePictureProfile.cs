using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Dtos;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Profiles
{
    public class TouristRoutePictureProfile : Profile
    {
        public TouristRoutePictureProfile()
        {
            CreateMap<TouristRoutePicture, TouristRoutePictureDto>();
            CreateMap<TouristRoutePictureForCreationDto, TouristRoutePicture>();
            CreateMap<TouristRoutePicture, TouristRoutePictureForCreationDto>();
        }
    }
}
