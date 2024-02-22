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
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //쿠키 및 HttpContext정보 활성화
//services.AddSession(); //세션정보 활성화
builder.Services.AddSession(options =>
{
    // Set a short timeout for easy testing.
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    // Make the session cookie essential
    options.Cookie.IsEssential = true;
});//세션정보 활성화 ==> 세션상태 유지 활성화

//캐시활성화
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();

////Html.Action 활성화
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//HTTP 요청 만들기
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

app.UseRequestLocalization(); //언어셋

app.UseSession(); //세션실행

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