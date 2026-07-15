using CarAllowedApi.Data;
using CarAllowedApi.Dto;
using Microsoft.EntityFrameworkCore;

namespace CarAllowedApi.Services;

public interface IPublicHolidayService
{
    Task<List<PublicHolidayDto>> GetAllAsync(int? year);
    Task<PublicHolidayDto?> GetByIdAsync(int id);
    Task<bool> IsHolidayAsync(DateOnly date);
    Task<(bool Success, string Message, PublicHolidayDto? Data)> CreateAsync(CreatePublicHolidayDto dto);
    Task<(bool Success, string Message, PublicHolidayDto? Data)> UpdateAsync(int id, CreatePublicHolidayDto dto);
    Task<bool> DeleteAsync(int id);
}



public class PublicHolidayService : IPublicHolidayService
{
    private readonly ApplicationDbContext _context;

    public PublicHolidayService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PublicHolidayDto>> GetAllAsync(int? year)
    {
        var query = _context.PublicHolidays.AsQueryable();

        if (year.HasValue)
        {
            query = query.Where(x => x.HolidayDate.Year == year.Value);
        }

        return await query
            .OrderBy(x => x.HolidayDate)
            .Select(x => new PublicHolidayDto
            {
                Id = x.Id,
                HolidayDate = x.HolidayDate,
                HolidayName = x.HolidayName,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<PublicHolidayDto?> GetByIdAsync(int id)
    {
        return await _context.PublicHolidays
            .Where(x => x.Id == id)
            .Select(x => new PublicHolidayDto
            {
                Id = x.Id,
                HolidayDate = x.HolidayDate,
                HolidayName = x.HolidayName,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsHolidayAsync(DateOnly date)
    {
        return await _context.PublicHolidays
            .AnyAsync(x => x.HolidayDate == date && x.IsActive);
    }

    public async Task<(bool Success, string Message, PublicHolidayDto? Data)> CreateAsync(CreatePublicHolidayDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.HolidayName))
            return (false, "กรุณากรอกชื่อวันหยุด", null);

        var exists = await _context.PublicHolidays
            .AnyAsync(x => x.HolidayDate == dto.HolidayDate);

        if (exists)
            return (false, "มีวันหยุดวันที่นี้อยู่แล้ว", null);

        var entity = new PublicHoliday
        {
            HolidayDate = dto.HolidayDate,
            HolidayName = dto.HolidayName.Trim(),
            IsActive = dto.IsActive,
            CreatedAt = DateTime.Now
        };

        _context.PublicHolidays.Add(entity);
        await _context.SaveChangesAsync();

        return (true, "บันทึกข้อมูลสำเร็จ", new PublicHolidayDto
        {
            Id = entity.Id,
            HolidayDate = entity.HolidayDate,
            HolidayName = entity.HolidayName,
            IsActive = entity.IsActive
        });
    }

    public async Task<(bool Success, string Message, PublicHolidayDto? Data)> UpdateAsync(int id, CreatePublicHolidayDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.HolidayName))
            return (false, "กรุณากรอกชื่อวันหยุด", null);

        var entity = await _context.PublicHolidays.FindAsync(id);

        if (entity == null)
            return (false, "ไม่พบข้อมูลวันหยุด", null);

        var duplicate = await _context.PublicHolidays
            .AnyAsync(x => x.HolidayDate == dto.HolidayDate && x.Id != id);

        if (duplicate)
            return (false, "มีวันหยุดวันที่นี้อยู่แล้ว", null);

        entity.HolidayDate = dto.HolidayDate;
        entity.HolidayName = dto.HolidayName.Trim();
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return (true, "แก้ไขข้อมูลสำเร็จ", new PublicHolidayDto
        {
            Id = entity.Id,
            HolidayDate = entity.HolidayDate,
            HolidayName = entity.HolidayName,
            IsActive = entity.IsActive
        });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.PublicHolidays.FindAsync(id);

        if (entity == null)
            return false;

        _context.PublicHolidays.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }
}