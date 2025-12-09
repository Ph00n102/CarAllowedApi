using System;
using Microsoft.EntityFrameworkCore;

namespace CarAllowedApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<JobImage> JobImages { get; set; }
    public DbSet<JobRequestCar> JobRequestCars { get; set; }
    public DbSet<JobStatus> JobStatuses { get; set; }
    public DbSet<ImageEmp> ImageEmps { get; set; }
    public DbSet<Garage> Garages { get; set; }
}