using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using EstudoIA.Application.Services;
// Removido: using EstudoIA.Domain.Entities; // PessoaModel está agora neste arquivo
// Removido: using Radzen; // Não mais necessário

namespace EstudoIA.Pages.Pessoas
{
    public class PessoaModel // Mantido o PessoaModel para validação com DataAnnotations
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Data de Nascimento é obrigatória")]
        [Range(typeof(DateTime), "1/1/1900", "1/1/2100", ErrorMessage = "Data de Nascimento inválida. Use dd/mm/aaaa e entre 01/01/1900 e 01/01/2100.")]
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "CPF é obrigatório")]
        // TODO: Adicionar uma validação customizada para CPF (formato e lógica)
        // Exemplo: [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "Formato de CPF inválido (###.###.###-##)")]
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

        // Removido: NotificationService, pois não usaremos mais o do Radzen.
        // As mensagens de erro/sucesso serão tratadas de forma mais simples.

        protected PessoaModel pessoaModel = new PessoaModel();
        protected bool IsEditMode => PessoaId.HasValue && PessoaId.Value != Guid.Empty;
        protected string PageTitle => IsEditMode ? "Editar Pessoa" : "Cadastrar Nova Pessoa";
        protected string SubmitButtonText => IsEditMode ? "Salvar Alterações" : "Cadastrar Pessoa";

        protected string errorMessage;
        protected string successMessage;

        protected override async Task OnParametersSetAsync()
        {
            errorMessage = null;
            successMessage = null;

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
                        errorMessage = $"Pessoa com ID {PessoaId.Value} não encontrada.";
                        // Opcional: redirecionar se não encontrado em modo de edição
                        // NavigateToListarPessoas();
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = $"Erro ao carregar pessoa: {ex.Message}";
                }
            }
            else
            {
                pessoaModel = new PessoaModel();
            }
            await base.OnParametersSetAsync();
        }

        protected async Task HandleValidSubmit()
        {
            errorMessage = null;
            successMessage = null;

            try
            {
                if (!pessoaModel.DataNascimento.HasValue)
                {
                    // Esta validação já deve ser coberta pelo [Required] e DataAnnotationsValidator
                    // mas uma dupla checagem não faz mal se houver dúvidas.
                    errorMessage = "Data de Nascimento é obrigatória.";
                    return;
                }

                if (IsEditMode)
                {
                    PessoaService.UpdatePessoa(pessoaModel.Id, pessoaModel.Nome, pessoaModel.DataNascimento.Value, pessoaModel.Email, pessoaModel.Cpf);
                    // successMessage = "Pessoa atualizada com sucesso!"; // Pode ser melhor redirecionar e mostrar na lista
                    NavigationManager.NavigateTo("/listar-pessoas", forceLoad: true); // forceLoad para garantir que a lista recarregue com a mensagem de outra página (se houver)
                }
                else
                {
                    PessoaService.CreatePessoa(pessoaModel.Nome, pessoaModel.DataNascimento.Value, pessoaModel.Email, pessoaModel.Cpf);
                    // successMessage = "Pessoa cadastrada com sucesso!";
                    NavigationManager.NavigateTo("/listar-pessoas", forceLoad: true);
                }
                // Após salvar, normalmente redirecionamos para a lista.
                // A mensagem de sucesso pode ser passada via query string ou um serviço de estado simples se necessário.
                // Por simplicidade, vamos apenas redirecionar. A lista pode ter sua própria mensagem ao carregar.
            }
            catch (InvalidOperationException ex)
            {
                errorMessage = ex.Message; // Ex: CPF duplicado
            }
            catch (ArgumentException ex)
            {
                 errorMessage = ex.Message; // Ex: Validação da entidade
            }
            catch (Exception ex)
            {
                errorMessage = $"Ocorreu um erro inesperado: {ex.Message}";
            }
            StateHasChanged(); // Atualiza a UI para mostrar mensagens de erro, se houver
        }

        protected void HandleInvalidSubmit()
        {
            errorMessage = "Por favor, corrija os erros de validação.";
            successMessage = null;
        }

        protected void NavigateToListarPessoas()
        {
            NavigationManager.NavigateTo("/listar-pessoas");
        }
    }
}
