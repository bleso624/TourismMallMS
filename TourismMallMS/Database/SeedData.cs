using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismMallMS.Database
{
    public static class SeedData 
    {
        public static IApplicationBuilder UseDataInitializer(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetService<AppDbContext>();
                System.Console.WriteLine("开始执行迁移数据库...");

                dbcontext.Database.Migrate();
                System.Console.WriteLine("数据库迁移完成...");
            }
            return builder;
        }
    }
}
