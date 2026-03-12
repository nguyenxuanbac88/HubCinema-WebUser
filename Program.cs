using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using MovieTicketWebsite.middlewares;
using MovieTicketWebsite.Services.Transaction;
using MovieTicketWebsite.Services.VNPay;

namespace MovieTicketWebsite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            // 1. Add localization service
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddScoped<IVnPayService, VnPayService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();


            builder.Services.AddControllersWithViews().AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder)
    .AddDataAnnotationsLocalization();
            // 3. Add support for cookie-based culture provider
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en", "vi" };
                options.SetDefaultCulture("vi")
                       .AddSupportedCultures(supportedCultures)
                       .AddSupportedUICultures(supportedCultures);

                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
            });
            builder.Services.AddHttpClient();
            builder.Services.AddSession();

            builder.Services.AddHttpContextAccessor(); // ✅ Thêm dòng này


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // 👉 Thêm dòng này
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // 4. Enable localization middleware
            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseMiddleware<TokenValidationMiddleware>();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
