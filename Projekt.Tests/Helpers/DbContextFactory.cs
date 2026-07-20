using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using System;

namespace Projekt.Tests.Helpers
{
    public static class DbContextFactory
    {
        public static AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new AppDbContext(options);
        }
    }
}
