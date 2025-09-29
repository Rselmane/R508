using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IProductRepository, ProductManager>();
        builder.Services.AddScoped<IDataRepository<Brand>, BrandManager>();
        builder.Services.AddScoped<IDataRepository<TypeProduct>, TypeProductManager>();
        builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("SeriesDbContextRemote")));
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<App.Mapper.ProductMapper>();
            cfg.AddProfile<App.Mapper.ProductDetailMapper>();
            cfg.AddProfile<App.Mapper.ProductMappingProfile>();
            cfg.AddProfile<App.Mapper.BrandMapper>();
            cfg.AddProfile<App.Mapper.TypeProductMapper>();
            // Add any other individual mapper profiles here
        });


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }

}