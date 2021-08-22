using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Dtos;
using TourismMallMS.Helper;
using TourismMallMS.Models.Entities;
using TourismMallMS.ResourceParameters;
using TourismMallMS.Services;

namespace TourismMallMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        public TouristRoutesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IPropertyMappingService propertyMappingService
        )
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _propertyMappingService = propertyMappingService;
        }

        private string GenerateTouristRouteResourceURL(
            TouristRouteResourceParameters searchParameter,
            PaginationResourceParameters paginationParameter,
            ResourceUrlType type
        )
        {
            return type switch
            {
                ResourceUrlType.PreviousPage => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = searchParameter.Fields,
                        keyword = searchParameter.Keyword,
                        rating = searchParameter.Rating,
                        orderBy = searchParameter.OrderBy, 
                        pageNumber = paginationParameter.PageNumber - 1,
                        pageSize = paginationParameter.PageSize
                    }),
                ResourceUrlType.NextPage => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = searchParameter.Fields,
                        keyword = searchParameter.Keyword,
                        rating = searchParameter.Rating,
                        orderBy = searchParameter.OrderBy,
                        pageNumber = paginationParameter.PageNumber + 1,
                        pageSize = paginationParameter.PageSize
                    }),
                _ => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = searchParameter.Fields,
                        keyword = searchParameter.Keyword,
                        rating = searchParameter.Rating,
                        orderBy = searchParameter.OrderBy,
                        pageNumber = paginationParameter.PageNumber,
                        pageSize = paginationParameter.PageSize
                    })
            };
        }

        [HttpGet(Name = "GetTouristRoutes")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourceParameters searchParameter,
            [FromQuery] PaginationResourceParameters paginationParameter,
            [FromHeader(Name = "Accept")] string mediaType
        )
        {
            if(!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }
            if (!_propertyMappingService.IsMappingExists<TouristRouteDto, TouristRoute>(searchParameter.OrderBy))
            {
                return NotFound("请输入正确排序参数");
            }
            if (!_propertyMappingService.IsPropertiesExists<TouristRoute>(searchParameter.Fields))
            {
                return NotFound("请输入正确塑形参数");
            }
            var touristRoutesFromRepo = await _touristRouteRepository
                .GetTouristRoutesAsync(
                    searchParameter.Keyword,
                    searchParameter.RatingOptrator,
                    searchParameter.RatingValue,
                    searchParameter.OrderBy,
                    paginationParameter.PageSize,
                    paginationParameter.PageNumber
                );
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("没有旅游路线");
            }

            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            var previousPageLink = touristRoutesFromRepo.HasPrevious
                ? GenerateTouristRouteResourceURL(
                    searchParameter,
                    paginationParameter,
                    ResourceUrlType.PreviousPage
                  ) 
                : null;

            var nextPageLink = touristRoutesFromRepo.HasNext
                ? GenerateTouristRouteResourceURL(
                    searchParameter,
                    paginationParameter,
                    ResourceUrlType.NextPage
                )
                : null;
            var paginationMetadata = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRoutesFromRepo.TotalCount,
                pageSize = touristRoutesFromRepo.PageSize,
                curentPage = touristRoutesFromRepo.CurrentPage,
                totalPages = touristRoutesFromRepo.TotalPages
            };

            Response.Headers.Add("x-pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var shapedDtoList = touristRoutesDto.ShapeData(searchParameter.Fields);
            if(parsedMediaType.MediaType == "application/vnd.bleso.hateoas+json")
            {
                var linkDtos = CreateLinkForTouristRouteList(searchParameter, paginationParameter);
                var shapedDtoWithLinkList = shapedDtoList.Select(t =>
                {
                    var touristRouteDictionary = t as IDictionary<string, object>;
                    var links = CreateLinkForTouristRoute((Guid)touristRouteDictionary["Id"], null);
                    touristRouteDictionary.Add("links", links);
                    return touristRouteDictionary;
                });
                var res = new
                {
                    value = shapedDtoWithLinkList,
                    links = linkDtos
                };
                return Ok(res);
            }
            return Ok(shapedDtoList);
        }

        [HttpGet("{touristRouteId:Guid}", Name = "GetTouristRouteById")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRouteById(
            [FromRoute] Guid touristRouteId,
            [FromQuery] string fields
        )
        {
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound($"旅游路线{touristRouteId}找不到");
            }
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            var linkDtos = CreateLinkForTouristRoute(touristRouteId, fields);
            var res = touristRouteDto.ShapeData(fields) as IDictionary<string, object>;
            res.Add("links", linkDtos);
            return Ok(res);
        }

        [HttpPost(Name = "CreateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();

            var links = CreateLinkForTouristRoute(touristRouteModel.Id, null);
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);

            var res = touristRouteToReturn.ShapeData(null) as IDictionary<string, object>;
            res.Add("links", links);
            return CreatedAtRoute("GetTouristRouteById",
                new { touristRouteId = res["Id"] },
                res
            );
        }

        [HttpPut("{touristRouteId}", Name = "UpdateTouristRoute")]
        public async Task<IActionResult> UpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto
        )
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线找不到");
            }
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{touristRouteId}", Name = "PartiallyUpdateTouristRoute")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument
        )
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线找不到");
            }
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPatch, ModelState);

            if (!TryValidateModel(touristRouteToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{touristRouteId}", Name = "DeleteTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"旅游路线{touristRouteId}找不到");
            }
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("({touristRouteIds})")]
        public async Task<IActionResult> DeleteTouristsByIds(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] [FromRoute] IEnumerable<Guid> touristRouteIds
        )
        {
            if(touristRouteIds == null)
            {
                return BadRequest();
            }
            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIdsAsync(touristRouteIds);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinkForTouristRoute(Guid touristRouteId, string fields)
        {
            var links = new List<LinkDto>();
            links.Add(
                new LinkDto(
                    Url.Link("GetTouristRouteById", new { touristRouteId, fields }),
                    "self",
                    "GET"
                    )
                );
            links.Add(
                new LinkDto(
                    Url.Link("UpdateTouristRoute", new { touristRouteId }),
                    "partially_update",
                    "PATCH"
                    )
                );
            links.Add(
                new LinkDto(
                    Url.Link("DeleteTouristRoute", new { touristRouteId }),
                    "delete",
                    "DELETE"
                    )
                );
            links.Add(
                new LinkDto(
                    Url.Link("GetPictureListForTouristRoute", new { touristRouteId }),
                    "get_pictures",
                    "GET"
                    )
                );

            links.Add(
                new LinkDto(
                    Url.Link("CreateTouristRoutePicture", new { touristRouteId }),
                    "create_picture",
                    "POST"
                    )
                );

            return links;
        }
        private IEnumerable<LinkDto> CreateLinkForTouristRouteList(
            TouristRouteResourceParameters searchParameter,
            PaginationResourceParameters paginationParameter
        )
        {
            var links = new List<LinkDto>();
            links.Add(new LinkDto(
                GenerateTouristRouteResourceURL(
                    searchParameter,
                    paginationParameter,
                    ResourceUrlType.CurrnetPage
                ),
                "self",
                "GET"
            ));
            links.Add(new LinkDto(
                Url.Link("CreateTouristRoute", null),
                "create_tourist_route",
                "POST"
            ));
            return links;
        }
    }
}
