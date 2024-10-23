using GeoJsonRandom.Core.Services;
using GeoJsonRandom.Services;

namespace GeoJsonRandom
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // json序列化時不自動將字首小寫
            builder.Services.AddControllersWithViews().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
            builder.Services.AddSingleton<IGeoJsonPrefixTreeBuilder>(sp => new GeoJsonPrefixTreeBuilder(builder.Configuration["GeojsonPath"] ?? string.Empty));
            builder.Services.AddScoped<IGeoJsonRandomPointGenerator, GeoJsonRandomPointGenerator>();
            builder.Services.AddScoped<IGeoDataService, GeoDataService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
