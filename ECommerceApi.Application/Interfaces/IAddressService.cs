using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Address;

namespace ECommerceApi.Application.Interfaces;

public interface IAddressService
{
    Task<ServiceResult<List<AddressDto>>> GetUserAddressesAsync(int userId);
    Task<ServiceResult<AddressDto>> CreateAsync(int userId, CreateAddressRequest request);
    Task<ServiceResult<AddressDto>> UpdateAsync(int addressId, int userId, UpdateAddressRequest request);
    Task<ServiceResult<bool>> DeleteAsync(int addressId, int userId);
}
