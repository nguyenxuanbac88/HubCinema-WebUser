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

            builder.Services.AddScoped<IVnPayService, VnPayService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();


            builder.Services.AddControllersWithViews();
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
