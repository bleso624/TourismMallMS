using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourismMallMS.Dtos;
using TourismMallMS.Helper;
using TourismMallMS.Models;
using TourismMallMS.Models.Entities;
using TourismMallMS.Services;

namespace TourismMallMS.Controllers
{
    [ApiController]
    [Route("api/shoppingCart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public ShoppingCartController(
            IHttpContextAccessor httpContextAccessor,
            IShoppingCartRepository shoppingCartRepository,
            ITouristRouteRepository touristRouteRepository,
            IOrderRepository orderRepository,
            IMapper mapper
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _shoppingCartRepository = shoppingCartRepository;
            _touristRouteRepository = touristRouteRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetShoppinCart")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetShoppinCart()
        {
            var userId = _httpContextAccessor.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCart = await _shoppingCartRepository.GetShoppingCartByUserIdAsync(userId);

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }

        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddShoppingCartItem(
            [FromBody] AddShoppingCartItemDto addShoppingCartItemDto
        )
        {
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCart = await _shoppingCartRepository
                .GetShoppingCartByUserIdAsync(userId);

            var touristRoute = await _touristRouteRepository
                .GetTouristRouteAsync(addShoppingCartItemDto.TouristRouteId);
            if (touristRoute == null)
            {
                return NotFound("旅游路线不存在");
            }

            var lineItem = new LineItem()
            {
                TouristRouteId = addShoppingCartItemDto.TouristRouteId,
                ShoppingCartId = shoppingCart.Id,
                OriginalPrice = touristRoute.OriginalPrice,
                DiscountPresent = touristRoute.DiscountPresent
            };

            await _shoppingCartRepository.AddShoppingCartItemAsync(lineItem);
            await _shoppingCartRepository.SaveAsync();

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }

        [HttpDelete("items/{itemId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int itemId)
        {
            var lineItem = await _shoppingCartRepository
                .GetShoppingCartItemByItemIdAsync(itemId);
            if (lineItem == null)
            {
                return NotFound("购物车商品找不到");
            }

            _shoppingCartRepository.DeleteShoppingCartItem(lineItem);
            await _shoppingCartRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("items/({itemIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> RemoveShoppingCartItems(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            [FromRoute] IEnumerable<int> itemIDs
        )
        {
            var lineitems = await _shoppingCartRepository
                .GeshoppingCartItemsByIdListAsync(itemIDs);

            _shoppingCartRepository.DeleteShoppingCartItems(lineitems);
            await _shoppingCartRepository.SaveAsync();

            return NoContent();
        }

        [HttpPost("checkout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Checkout()
        {
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCart = await _shoppingCartRepository.GetShoppingCartByUserIdAsync(userId);

            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                State = OrderState.Pending,
                OrderItems = shoppingCart.ShoppingCartItems,
                CreateDateUTC = DateTime.UtcNow,
            };

            shoppingCart.ShoppingCartItems = null;

            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveAsync();

            return Ok(_mapper.Map<OrderDto>(order));
        }
    }
}
