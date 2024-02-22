using Microsoft.AspNetCore.Mvc.Infrastructure;
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

#region Service Container
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
                          policy =>
                          {
                              policy.WithOrigins(
                                    "https://edev.p2o-community.summonerswar.com",
                                    "https://qa-p2o-community.summonerswar.com",
                                    "https://p2o-community.summonerswar.com"
                                    )
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                          });
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //��Ű �� HttpContext���� Ȱ��ȭ
//services.AddSession(); //�������� Ȱ��ȭ
builder.Services.AddSession(options =>
{
    // Set a short timeout for easy testing.
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    // Make the session cookie essential
    options.Cookie.IsEssential = true;
});//�������� Ȱ��ȭ ==> ���ǻ��� ���� Ȱ��ȭ

//ĳ��Ȱ��ȭ
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();

////Html.Action Ȱ��ȭ
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//HTTP ��û �����
builder.Services.AddHttpClient();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromHours(12);
    options.Cookie.Name = ".ChroniclesWeb.Session";
    options.Cookie.IsEssential = true;
});

ConfigureServices(builder.Services);
void ConfigureServices(IServiceCollection services)
{

    services.AddControllers();
    services.AddMvc();
    services.AddHttpContextAccessor();
    //services.AddSession(options =>
    //{
    //    // Set a short timeout for easy testing.
    //    options.IdleTimeout = TimeSpan.FromSeconds(10);
    //    options.Cookie.HttpOnly = true;
    //    // Make the session cookie essential
    //    options.Cookie.IsEssential = true;
    //});
}
#endregion

#region App
var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseRequestLocalization(); //����

app.UseSession(); //���ǽ���

app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });


app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next.Invoke();
});

#region router
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");
});
#endregion

app.Run();
#endregion