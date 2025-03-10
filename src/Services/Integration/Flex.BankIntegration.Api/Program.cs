using Flex.BankIntegration.Api.Services;
System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

string soapServiceUrl = "https://localhost:52514/HOSTService/HOSTService.svc";
builder.Services.AddSingleton(new SoapClientService(soapServiceUrl));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/call-soap", async (SoapClientService soapService) =>
{
    var response = await soapService.GetFlagSignatureAsync();
    return Results.Ok(response);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
