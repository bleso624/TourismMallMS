using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Dtos;
using TourismMallMS.Models.Entities;
using TourismMallMS.Services;

namespace TourismMallMS.Controllers
{
    [Route("api/touristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePictureController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;

        public TouristRoutePictureController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper
        )
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetPictureListForTouristRoute")]
        public async Task<IActionResult> GetPictureListForTouristRoute(Guid touristRouteId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"旅游路线{touristRouteId}不存在");
            }

            var picturesFromRepo = await _touristRouteRepository.GetPicturesByTouristRouteIdAsync(touristRouteId);
            if (picturesFromRepo == null || picturesFromRepo.Count() <= 0)
            {
                return NotFound("照片不存在");
            }
            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(picturesFromRepo));
        }

        [HttpGet("{pictureId}", Name = "GetPicture")]
        public async Task<IActionResult> GetPicture(
            [FromRoute]Guid touristRouteId, 
            [FromRoute]int pictureId
        )
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"旅游路线{touristRouteId}不存在");
            }

            var pictureFromRepo = await _touristRouteRepository.GetPictureAsync(pictureId);
            if (pictureFromRepo == null)
            {
                return NotFound("相片不存在");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
        }

        [HttpPost(Name = "CreateTouristRoutePicture")]
        public async Task<IActionResult> CreateTouristRoutePicture(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto
        )
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"旅游路线{touristRouteId}不存在");
            }
            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
            _touristRouteRepository.AddTouristRoutePicture(touristRouteId, pictureModel);
            await _touristRouteRepository.SaveAsync();
            var pictureToReturn = _mapper.Map<TouristRoutePictureDto>(pictureModel);
            return CreatedAtRoute("GetPicture",
                new
                {
                    touristRouteId = pictureModel.TouristRouteId,
                    pictureId = pictureModel.Id
                },
                pictureToReturn
            );
        }

        [HttpDelete("{touristRoutePictureId}")]
        public async Task<IActionResult> DeleteTouristRoutePciture(
            [FromRoute] Guid touristRouteId,
            [FromRoute] int touristRoutePictureId
        )
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"旅游路线{touristRouteId}不存在");
            }
            var touristRoutePicture = await _touristRouteRepository.GetPictureAsync(touristRoutePictureId);
            _touristRouteRepository.DeletePicture(touristRoutePicture);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
    }
}
