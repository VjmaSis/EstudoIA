using EstudoIA.Components;
using EstudoIA.Application.Services; // Novo: Para PessoaService
using EstudoIA.Domain.Interfaces;    // Novo: Para IPessoaRepository
using EstudoIA.Infrastructure.Repositories; // Novo: Para PessoaRepository (concreto)
using EstudoIA.Infrastructure.Adapters;   // Novo: Para PessoaRepositoryAdapter
using Radzen; // Novo: Para serviços Radzen como DialogService, NotificationService

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Adicionar serviços Radzen (se ainda não estiverem presentes)
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>(); // Exemplo de outros serviços Radzen
builder.Services.AddScoped<ContextMenuService>(); // Exemplo de outros serviços Radzen


// Nossos serviços da aplicação de Pessoa
// 1. Repositório em memória (Singleton pois não tem estado que dependa do escopo da requisição e queremos os mesmos dados para todos)
builder.Services.AddSingleton<PessoaRepository>(); // A implementação concreta

// 2. O Adapter que usa o PessoaRepository.
//    Ele é Scoped porque pode, no futuro, adaptar um repositório com escopo (ex: DbContext do EF Core).
//    Ele recebe a implementação concreta do PessoaRepository.
builder.Services.AddScoped<IPessoaRepository>(sp =>
    new PessoaRepositoryAdapter(sp.GetRequiredService<PessoaRepository>()));

// 3. O serviço da aplicação que depende de IPessoaRepository (que será o adapter).
//    Scoped porque pode realizar operações que são relevantes para uma sessão de usuário ou requisição.
builder.Services.AddScoped<PessoaService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// app.UseStaticFiles(); // Esta linha geralmente está aqui, se não estiver, pode ser necessário.
// Verifique se já existe no seu pipeline. Pelo seu arquivo original, não estava, mas é comum.
// Se suas páginas Radzen usarem CSS/JS estáticos do Radzen, isso é importante.
// No entanto, os componentes Radzen mais recentes costumam lidar com seus próprios recursos estáticos.
app.UseStaticFiles(); // Adicionando para garantir, caso os componentes Radzen precisem.

app.UseAntiforgery();

app.MapStaticAssets(); // Esta chamada deve vir depois de UseStaticFiles, se UseStaticFiles for usado explicitamente.

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(EstudoIA.Client._Imports).Assembly);

app.Run();
