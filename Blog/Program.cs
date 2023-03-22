using Blog.Data.Entities;
using Blog.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Blog.Services.EmailService;
using Blog.AutoMapperProfiles;
using Blog.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.IsEssential = true;
});

builder.Services.AddAutoMapper(
    typeof(UserProfile),
    typeof(PostProfile),
    typeof(CategoryProfile),
    typeof(CommentProfile)
    );

builder.Services.AddTransient<IEmailService, EmailService>(); // вот здесь

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration
        .GetConnectionString("DefaultConnection"));
});

builder.Services
    .AddIdentity<User, IdentityRole>((IdentityOptions options) =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 1;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
    })
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();
// Благодаря последнему вызову добавляется функциональность генерации токенов,
// которые отсылаются в письме (gmail) для подтверждения

builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy(
    //    MyPolicies.SuperAdminAccessOnly,
    //    policy =>
    //    {
    //        policy.RequireClaim(MyClaims.SuperAdmin);
    //        policy.RequireClaim(MyClaims.Admin);
    //        policy.RequireClaim(MyClaims.PostsWriter);
    //    });

    //options.AddPolicy(
    //    MyPolicies.AdminAndAboveAccess,
    //    policy =>
    //    {
    //        policy.RequireClaim(MyClaims.Admin);
    //        policy.RequireClaim(MyClaims.PostsWriter);
    //    });

    //options.AddPolicy(
    //    MyPolicies.PostWriterAndAboveAccess,
    //    policy => policy.RequireClaim(MyClaims.PostsWriter));


    options.AddPolicy(
        MyPolicies.PostsWriterAndAboveAccess,
        policy => policy.RequireAssertion(context =>
        {
            return context.User.HasClaim(
                claim => claim.Type is 
                    MyClaims.SuperAdmin or 
                    MyClaims.Admin or 
                    MyClaims.PostsWriter);
        }));

    options.AddPolicy(
        MyPolicies.AdminAndAboveAccess,
        policy => policy.RequireAssertion(context =>
        {
            return context.User.HasClaim(
                claim => claim.Type is 
                    MyClaims.SuperAdmin or
                    MyClaims.Admin);
        }));

    options.AddPolicy(
        MyPolicies.SuperAdminAccessOnly,
        policy => policy.RequireAssertion(context =>
        {
            return context.User.HasClaim(
                claim => claim.Type is MyClaims.SuperAdmin);
        }));
});

builder.Services.AddControllersWithViews();

builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    string viewsharedAuth = "Views/Shared/AuthPartialViews/";
    string viewsharedComments = "Views/Shared/CommentsPartialViews/";

    options.ViewLocationFormats.Add(viewsharedAuth + "{0}.cshtml");
    options.ViewLocationFormats.Add(viewsharedComments + "{0}.cshtml");
});

var app = builder.Build();

// Configure the HTTP request pipeline.

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    await SeedData.Initialize(
        services,
        app.Configuration
        );
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication(); // (!)
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
