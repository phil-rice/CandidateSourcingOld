
using CandidateSourcing.CAS;
using CandidateSourcing.common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

var path = "/cas/{namespace}";

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CAS");

Console.WriteLine(connectionString);
// Add required services
builder.Services.AddDbContext<CasDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton<IShaCodex, ShaCodex>();

var app = builder.Build();

app.MapPost(path, async (HttpContext context, CasDbContext db, IShaCodex shaCodec) =>
{
if (!context.Request.Headers.TryGetValue("MimeType", out var mimeType) || string.IsNullOrEmpty(mimeType))
{
    context.Response.StatusCode = 400;
    await context.Response.WriteAsync("Missing MimeType header");
    return;
}

byte[] content = await RequestHelper.BodyToByteArray(context.Request);

string computedSha = shaCodec.ComputeSha(content);

var existingContent = await db.ContentItems.FindAsync(computedSha);
if (existingContent != null)
{
    if (!shaCodec.Validate(existingContent.Data, computedSha))
    {
        context.Response.StatusCode = 409;
        await context.Response.WriteAsync("SHA collision detected");
        return;
    }
}
else
{
    var contentItem = new ContentItem(
                SHA: computedSha,
                Namespace: context.Request.RouteValues["namespace"].ToString(),
                MimeType: mimeType,
                Data: content
            );

    db.ContentItems.Add(contentItem);
    await db.SaveChangesAsync();
}
context.Response.StatusCode = 201;
await context.Response.WriteAsync($"/api/{context.Request.RouteValues["namespace"]}/content/{computedSha}");
});

app.MapGet(path+"/content/{sha}", async (HttpContext context, CasDbContext db) =>
{
    var contentItem = await db.ContentItems.FindAsync(context.Request.RouteValues["sha"].ToString());
    if (contentItem == null)
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Content not found");
        return;
    }

    context.Response.Headers.Add("MimeType", contentItem.MimeType);
    await context.Response.Body.WriteAsync(contentItem.Data, 0, contentItem.Data.Length);
});


app.MapGet(path + "/samples", async (HttpContext context, CasDbContext db) =>
{
    var data = from r in db.ContentItems select r;
    return data.Take(10);
});

app.Run();
