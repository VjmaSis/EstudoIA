using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using EstudoIA.Application.Services; // Ajustado
using EstudoIA.Domain.Entities;      // Ajustado
using Radzen;
using Radzen.Blazor;

// O namespace aqui deve corresponder ao local do arquivo dentro do projeto Blazor principal.
// Se o projeto raiz EstudoIA tem o namespace EstudoIA, e as páginas estão em Pages/Pessoas,
// então EstudoIA.Pages.Pessoas é uma convenção comum.
namespace EstudoIA.Pages.Pessoas
{
    public partial class ListarPessoasBase : ComponentBase
    {
        [Inject]
        protected PessoaService PessoaService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        protected IEnumerable<Pessoa> pessoas;
        protected RadzenDataGrid<Pessoa> grid;

        protected override async Task OnInitializedAsync()
        {
            await LoadPessoas();
        }

        protected async Task LoadPessoas()
        {
            try
            {
                pessoas = PessoaService.GetAllPessoas().ToList();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Erro ao carregar pessoas", Detail = ex.Message, Duration = 4000 });
                pessoas = new List<Pessoa>();
            }
            await InvokeAsync(StateHasChanged);
        }

        protected void NavigateToCadastrarPessoa()
        {
            NavigationManager.NavigateTo("/cadastrar-pessoa");
        }

        protected void NavigateToEditarPessoa(Guid pessoaId)
        {
            NavigationManager.NavigateTo($"/editar-pessoa/{pessoaId}");
        }

        protected async Task ConfirmDeletePessoa(Guid pessoaId, string nomePessoa)
        {
            var result = await DialogService.Confirm($"Tem certeza que deseja excluir {nomePessoa}?", "Confirmar Exclusão", new ConfirmOptions() { OkButtonText = "Sim", CancelButtonText = "Não" });
            if (result == true)
            {
                try
                {
                    PessoaService.DeletePessoa(pessoaId);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Sucesso", Detail = $"{nomePessoa} excluído(a) com sucesso.", Duration = 4000 });
                    await LoadPessoas();
                    if (grid != null)
                    {
                        await grid.Reload();
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Erro ao excluir", Detail = ex.Message, Duration = 4000 });
                }
            }
        }
    }
}
