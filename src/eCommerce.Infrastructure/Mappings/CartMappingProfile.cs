using AutoMapper;
using eCommerce.Core.DTOs.Cart;
using eCommerce.Core.Models;

namespace eCommerce.Infrastructure.Mappings
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.Images));
        }
    }
} 