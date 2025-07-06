using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using EstudoIA.Application.Services; // Ajustado
using EstudoIA.Domain.Entities;      // Ajustado
using Radzen;

// O namespace aqui deve corresponder ao local do arquivo dentro do projeto Blazor principal.
namespace EstudoIA.Pages.Pessoas
{
    // Modelo auxiliar para o formulário, incluindo DataAnnotations para validação do Radzen
    public class PessoaModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Data de Nascimento é obrigatória")]
        [Range(typeof(DateTime), "1/1/1900", "1/1/2100", ErrorMessage = "Data de Nascimento inválida")]
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "CPF é obrigatório")]
        public string Cpf { get; set; }

        public PessoaModel()
        {
            DataNascimento = null;
        }
    }

    public partial class SalvarPessoaBase : ComponentBase
    {
        [Parameter]
        public Guid? PessoaId { get; set; }

        [Inject]
        protected PessoaService PessoaService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        protected PessoaModel pessoaModel = new PessoaModel();
        protected bool IsEditMode => PessoaId.HasValue && PessoaId.Value != Guid.Empty;
        protected string ErrorMessage { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (IsEditMode)
            {
                try
                {
                    var pessoaDomain = PessoaService.GetPessoaById(PessoaId.Value);
                    if (pessoaDomain != null)
                    {
                        pessoaModel = new PessoaModel
                        {
                            Id = pessoaDomain.Id,
                            Nome = pessoaDomain.Nome,
                            DataNascimento = pessoaDomain.DataNascimento,
                            Email = pessoaDomain.Email,
                            Cpf = pessoaDomain.Cpf
                        };
                    }
                    else
                    {
                        NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Pessoa com ID {PessoaId.Value} não encontrada.");
                        NavigateToListarPessoas();
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Erro ao carregar pessoa", ex.Message, 5000);
                    NavigateToListarPessoas();
                }
            }
            else
            {
                pessoaModel = new PessoaModel();
            }
            await base.OnParametersSetAsync(); // Adicionado para garantir ciclo de vida
        }

        protected async Task OnSubmit()
        {
            ErrorMessage = null;
            try
            {
                if (!pessoaModel.DataNascimento.HasValue)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Erro de Validação", "Data de Nascimento é obrigatória.", 5000);
                    return;
                }

                if (IsEditMode)
                {
                    PessoaService.UpdatePessoa(pessoaModel.Id, pessoaModel.Nome, pessoaModel.DataNascimento.Value, pessoaModel.Email, pessoaModel.Cpf);
                    NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Pessoa atualizada com sucesso!", 3000);
                }
                else
                {
                    PessoaService.CreatePessoa(pessoaModel.Nome, pessoaModel.DataNascimento.Value, pessoaModel.Email, pessoaModel.Cpf);
                    NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Pessoa cadastrada com sucesso!", 3000);
                }
                NavigateToListarPessoas();
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                NotificationService.Notify(NotificationSeverity.Error, "Erro de Negócio", ex.Message, 5000);
            }
            catch (ArgumentException ex)
            {
                 ErrorMessage = ex.Message;
                 NotificationService.Notify(NotificationSeverity.Error, "Erro de Validação", ex.Message, 5000);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ocorreu um erro inesperado: {ex.Message}";
                NotificationService.Notify(NotificationSeverity.Error, "Erro Inesperado", ErrorMessage, 5000);
            }
            await InvokeAsync(StateHasChanged);
        }

        protected void OnInvalidSubmit()
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Atenção", "Por favor, corrija os erros de validação.", 4000);
        }

        protected void NavigateToListarPessoas()
        {
            NavigationManager.NavigateTo("/listar-pessoas");
        }
    }
}
