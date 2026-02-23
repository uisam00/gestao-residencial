using GastosResidenciais.CrossCutting.IoC;

var builder = WebApplication.CreateBuilder(args);

// Registro das dependências do backend (DbContext + serviços de aplicação).
builder.Services.AddDependencyInjection(builder.Configuration);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

// Controllers (atribute routing)
app.MapControllers();

app.Run();