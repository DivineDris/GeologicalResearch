using System;
using GeologicalResearch.Models;
using Microsoft.EntityFrameworkCore;

namespace GeologicalResearch.Data;

public class GRDataContext(DbContextOptions<GRDataContext> options) 
: DbContext(options)
{
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<Brigade> Brigades => Set<Brigade>();
    public DbSet<Status> Statuses => Set<Status>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Status>().HasData(
            new{Id = 1, StatusName = "Request opened",},
            new{Id = 2, StatusName = "Request in progress",},
            new{Id = 3, StatusName = "Request closed",}
        );
        modelBuilder.Entity<Brigade>().HasData(
            new{Id = 1, BrigadeName = "Brigade #1"},
            new{Id = 2, BrigadeName = "Brigade #2"},
            new{Id = 3, BrigadeName = "Brigade #3"}
        );
    }
}
