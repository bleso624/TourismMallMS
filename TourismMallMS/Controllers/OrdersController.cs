using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourismMallMS.Dtos;
using TourismMallMS.Models.Entities;
using TourismMallMS.ResourceParameters;
using TourismMallMS.Services;

namespace TourismMallMS.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrdersController(
            IHttpContextAccessor httpContextAccessor,
            IOrderRepository orderRepository,
            IMapper mapper
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }
        [HttpGet(Name = "GetOrders")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrders(
            [FromQuery] PaginationResourceParameters paginationParameter
        )
        {
           var userId =  _httpContextAccessor.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier).Value;

            var orders = await _orderRepository.GetOrdersByUserIdAsync(
                userId, 
                paginationParameter.PageSize,
                paginationParameter.PageNumber
            );

            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        [HttpGet("{orderId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GerOrderById([FromRoute] Guid orderId)
        {
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var order = await _orderRepository.GetOrderByOrderIdAsync(orderId);

            return Ok(_mapper.Map<OrderDto>(order));
        }
    }
}
