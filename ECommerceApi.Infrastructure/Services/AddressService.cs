using AutoMapper;
using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Address;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Services;

public class AddressService : IAddressService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AddressService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResult<List<AddressDto>>> GetUserAddressesAsync(int userId)
    {
        var addresses = await _context.Addresses.AsNoTracking()
            .Where(a => a.UserId == userId).OrderByDescending(a => a.IsDefault).ToListAsync();
        return ServiceResult<List<AddressDto>>.Ok(_mapper.Map<List<AddressDto>>(addresses));
    }

    public async Task<ServiceResult<AddressDto>> CreateAsync(int userId, CreateAddressRequest request)
    {
        if (request.IsDefault)
            await UnsetDefaultAddresses(userId);

        var address = _mapper.Map<Address>(request);
        address.UserId = userId;
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
        return ServiceResult<AddressDto>.Ok(_mapper.Map<AddressDto>(address));
    }

    public async Task<ServiceResult<AddressDto>> UpdateAsync(int addressId, int userId, UpdateAddressRequest request)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
        if (address == null) return ServiceResult<AddressDto>.Fail("Address not found.");

        if (request.IsDefault && !address.IsDefault)
            await UnsetDefaultAddresses(userId);

        _mapper.Map(request, address);
        await _context.SaveChangesAsync();
        return ServiceResult<AddressDto>.Ok(_mapper.Map<AddressDto>(address));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int addressId, int userId)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
        if (address == null) return ServiceResult<bool>.Fail("Address not found.");
        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    private async Task UnsetDefaultAddresses(int userId)
    {
        var defaults = await _context.Addresses.Where(a => a.UserId == userId && a.IsDefault).ToListAsync();
        defaults.ForEach(a => a.IsDefault = false);
    }
}
