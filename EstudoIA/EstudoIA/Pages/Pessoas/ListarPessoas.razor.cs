using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop; // Necessário para o confirm de exclusão
using EstudoIA.Application.Services;
using EstudoIA.Domain.Entities;

namespace EstudoIA.Pages.Pessoas
{
    public partial class ListarPessoasBase : ComponentBase
    {
        [Inject]
        protected PessoaService PessoaService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; } // Injetar IJSRuntime

        protected IEnumerable<Pessoa> pessoas;
        protected string errorMessage;
        protected string successMessage;

        protected override async Task OnInitializedAsync()
        {
            await LoadPessoas();
        }

        protected async Task LoadPessoas()
        {
            errorMessage = null;
            // successMessage = null; // Limpar mensagem de sucesso ao recarregar pode ser opcional
            try
            {
                pessoas = PessoaService.GetAllPessoas().ToList();
            }
            catch (Exception ex)
            {
                errorMessage = $"Erro ao carregar pessoas: {ex.Message}";
                pessoas = new List<Pessoa>();
            }
            // StateHasChanged(); // Não é necessário chamar StateHasChanged aqui explicitamente se OnInitializedAsync é o chamador
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
            successMessage = null;
            errorMessage = null;

            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", $"Tem certeza que deseja excluir {nomePessoa}?");

            if (confirmed)
            {
                try
                {
                    PessoaService.DeletePessoa(pessoaId);
                    successMessage = $"{nomePessoa} excluído(a) com sucesso.";
                    await LoadPessoas();
                }
                catch (Exception ex)
                {
                    errorMessage = $"Erro ao excluir {nomePessoa}: {ex.Message}";
                }
            }
            StateHasChanged(); // Chamar StateHasChanged após operações assíncronas que alteram o estado e não são eventos de UI diretos
        }
    }
}
