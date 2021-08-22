using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Helper;
using TourismMallMS.Models;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Services
{
    public interface ITouristRouteRepository
    {
        Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(
            string keyword, 
            string ratingOperator, 
            int? ratingValue,
            string orderBy,
            int pageSize,
            int pageNumber
        );
        Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId);
        Task<bool> TouristRouteExistsAsync(Guid touristRouteId);
        Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId);
        Task<TouristRoutePicture> GetPictureAsync(int pictureId);
        void AddTouristRoute(TouristRoute touristRoute);
        Task<bool> SaveAsync();
        void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
        void DeleteTouristRoute(TouristRoute touristRoute);
        void DeletePicture(TouristRoutePicture touristRoutePicture);
        Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdsAsync(IEnumerable<Guid> Ids);
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
    }
}
