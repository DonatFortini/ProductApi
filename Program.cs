using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-API-Version"),
                new QueryStringApiVersionReader("api-version")
            );
        });
        builder.Services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });
        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen(c =>
        {
            var provider = builder.Services.BuildServiceProvider()
                .GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                c.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = "Your API Name",
                    Version = description.ApiVersion.ToString(),
                    Description = $"API Version {description.ApiVersion}"
                });
            }
        });

        var app = builder.Build();
        app.UseHttpsRedirection();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant()
                );
            }

            options.RoutePrefix = string.Empty;
        });

        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}