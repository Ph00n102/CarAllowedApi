using System.Globalization;
using CarAllowedApi.Data;
using CarAllowedApi.Dto;
using CarAllowedApi.DTOs;
using CarAllowedApi.Models1;
using Microsoft.EntityFrameworkCore;
using static CarAllowedApi.Services.JobRequestCarService;

namespace CarAllowedApi.Services;

public interface IDivisionsService
{
    Task<DivisionDto[]> GetDivisionAsync(string name);
}

public class DivisionsService : IDivisionsService
{
    private readonly DivisionsDbContext _context;

    public DivisionsService(DivisionsDbContext context)
    {
        _context = context;
    }
    
    public async Task<DivisionDto[]> GetDivisionAsync(string name = null)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var query = _context.Divisions
            .AsNoTracking();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(j => EF.Functions.Like(j.ChiefName, $"%{name}%"));
        }

        return await query
            .Select(g => new DivisionDto
            {
                Id = g.Id,
                DivisionName = g.DivisionName,
                ChiefName = g.ChiefName,
                Cid = g.Cid,
                LoginNameHosxp = g.LoginNameHosxp,
                LoginNameCustom = g.LoginNameCustom
            })
            .ToArrayAsync();
    }
}
    