using System;
using GeologicalResearch.Models;
using Microsoft.EntityFrameworkCore;

namespace GeologicalResearch.Data;

public class GRDataContext(DbContextOptions<GRDataContext> options) 
: DbContext(options) //контекст бд для ef
{
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<Brigade> Brigades => Set<Brigade>();
    public DbSet<Status> Statuses => Set<Status>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Status>().HasData(
            new{Id = 1, StatusName = "Заявка открыта",},
            new{Id = 2, StatusName = "Заявка в работе",},
            new{Id = 3, StatusName = "Заявка закрыта",}
        );
        modelBuilder.Entity<Brigade>().HasData(
            new{Id = 1, BrigadeName = "Бригада №1"},
            new{Id = 2, BrigadeName = "Бригада №2"},
            new{Id = 3, BrigadeName = "Бригада №3"}
        );
    }
}
