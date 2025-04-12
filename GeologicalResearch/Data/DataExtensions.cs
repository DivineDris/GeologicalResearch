using System;
using Microsoft.EntityFrameworkCore;

namespace GeologicalResearch.Data;

public static class DataExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app) //Расширение для автоматического применения миграций во время запуска приложения
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GRDataContext>();
        await dbContext.Database.MigrateAsync();
    }
}
