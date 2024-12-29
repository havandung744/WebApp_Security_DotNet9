var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication().AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    options.AddPolicy("HRManagerOnly", policy =>
        policy.RequireAssertion(context =>
                  context.User.HasClaim("Department", "HR") ||
                  context.User.HasClaim("Manager", "true")));
});
builder.Services.AddHttpClient("OurWebAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7005/");
});
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromSeconds(20);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseSession();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
