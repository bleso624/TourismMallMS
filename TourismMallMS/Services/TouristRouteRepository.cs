using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Database;
using TourismMallMS.Dtos;
using TourismMallMS.Helper;
using TourismMallMS.Models;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        private readonly AppDbContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public TouristRouteRepository(
            AppDbContext context,
            IPropertyMappingService propertyMappingService
        )
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public async Task<TouristRoutePicture> GetPictureAsync(int pictureId)
        {
            return await _context.TouristRoutePictures.FirstOrDefaultAsync(p => p.Id == pictureId);
        }

        public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutePictures
                .Where(p => p.TouristRouteId == touristRouteId)
                .ToListAsync();
        }

        public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefaultAsync(n => n.Id == touristRouteId);
        }

        public async Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(
            string keyword, 
            string ratingOperator, 
            int? ratingValue,
            string orderBy,
            int pageSize,
            int pageNumber
        )
        {
            IQueryable<TouristRoute> res = _context
                .TouristRoutes
                .Include(t => t.TouristRoutePictures);
            if(!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                res = res.Where(t => t.Title.Contains(keyword));
            }
            if(ratingValue >= 0)
            {
                res = ratingOperator switch
                {
                    "largerThan" => res.Where(t => t.Rating >= ratingValue),
                    "lessThan" => res.Where(t => t.Rating <= ratingValue),
                    _ => res.Where(t => t.Rating == ratingValue),
                };
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var toristRouteMappingDictionary = _propertyMappingService
                    .GetPropertyMapping<TouristRouteDto, TouristRoute>();

                res = res.ApplySort(orderBy, toristRouteMappingDictionary);
            }
            return await PaginationList<TouristRoute>.CreateAsync(pageNumber, pageSize, res);
        }

        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.AnyAsync(t => t.Id == touristRouteId);
        }

        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if(touristRoute == null)
            {
                throw new ArgumentNullException(nameof(touristRoute));
            }
            _context.TouristRoutes.Add(touristRoute);
        }

        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if (touristRouteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(touristRouteId));
            }
            if(touristRoutePicture == null)
            {
                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            touristRoutePicture.TouristRouteId = touristRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);
        }

        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.Remove(touristRoute);
        }

        public void DeletePicture(TouristRoutePicture touristRoutePicture)
        {
            _context.Remove(touristRoutePicture);
        }

        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdsAsync(IEnumerable<Guid> Ids)
        {
            return await _context.TouristRoutes.Where(t => Ids.Contains(t.Id)).ToListAsync();
        }

        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.RemoveRange(touristRoutes);
        }
    }
}
