using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;
public class Startup
{
    public IConfiguration Configuration { get; }
    private readonly IWebHostEnvironment env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        this.Configuration = configuration;
        this.env = env;
    }

    // This method gets called by the runtime. Use this method to add serices to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        if(env.IsProduction()){
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(
                Configuration.GetConnectionString("PlatformsDB")
            ));
        }
        else{
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
        }   
        services.AddScoped<IPlatformRepo, PlatformRepo>();
        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

        services.AddControllers();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
        });

        Console.WriteLine($"-->Command Service endpoint: {Configuration["CommandService"]}");
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if(env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService"));
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => 
        {
            endpoints.MapControllers();
        });

        PrepDb.PrepPopulation(app, env.IsProduction());
    }
}