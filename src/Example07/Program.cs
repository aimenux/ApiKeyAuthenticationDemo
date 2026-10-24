using Example07;
using Example07.Infrastructure;
using Example07.Presentation;

var app = WebApplication.CreateBuilder(args)
    .WithHttpsCheck()
    .AddPresentation()
    .AddInfrastructure()
    .Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHsts();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app
    .MapControllers()
    .RequireAuthorization();

app.Run();