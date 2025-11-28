using Azure.Identity;
using HCI.AIAssistant.API.Managers;
using HCI.AIAssistant.API.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration[$"AppConfigurations{ConfigurationPath.KeyDelimiter}KeyVaultName"];
var secretsPrefix = builder.Configuration[$"AppConfigurations{ConfigurationPath.KeyDelimiter}SecretsPrefix"];

if(string.IsNullOrWhiteSpace(keyVaultName))
{
    throw new ArgumentNullException("KeyVaultName", "KeyVaultName is missing");
}

if(string.IsNullOrWhiteSpace(secretsPrefix))
{
    throw new ArgumentNullException("SecretsPrefix", "SecretsPrefix is missing");
}

var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

builder.Configuration.AddAzureKeyVault(
    keyVaultUri,
    new DefaultAzureCredential(),
    new CustomSecretManager(secretsPrefix)
);

// Add services to the container.

builder.Services.Configure<SecretsService>(builder.Configuration.GetSection("Secrets"));
builder.Services.Configure<AppConfigurationsService>(builder.Configuration.GetSection("AppConfigurations"));

builder.Services.AddSingleton<ISecretsService>(
    provider => provider.GetRequiredService<IOptions<SecretsService>>().Value
);
builder.Services.AddSingleton<IAppConfigurationService>(
    provider => provider.GetRequiredService<IOptions<AppConfigurationsService>>().Value
);
builder.Services.AddSingleton<IParametricFunctions, ParametricFunctions>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

if(app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

Console.WriteLine(app.Services.GetService<ISecretsService>()?.AIAssistantSecrets?.EndPoint);
Console.WriteLine(app.Services.GetService<ISecretsService>()?.AIAssistantSecrets?.Key);
Console.WriteLine(app.Services.GetService<ISecretsService>()?.AIAssistantSecrets?.Id);
Console.WriteLine(app.Services.GetService<ISecretsService>()?.IoTHubSecrets?.ConnectionString);
Console.WriteLine(app.Services.GetService<IAppConfigurationService>()?.KeyVaultName);
Console.WriteLine(app.Services.GetService<IAppConfigurationService>()?.SecretsPrefix);
Console.WriteLine(app.Services.GetService<IAppConfigurationService>()?.IoTDeviceName);

app.Run();
