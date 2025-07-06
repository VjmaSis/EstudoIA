namespace PessoaApp.Domain
{
    public class Pessoa
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public string Email { get; private set; }
        public string Cpf { get; private set; }

        public Pessoa(string nome, DateTime dataNascimento, string email, string cpf)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            DataNascimento = dataNascimento;
            Email = email;
            Cpf = cpf;
            Validate();
        }

        public void Update(string nome, DateTime dataNascimento, string email, string cpf)
        {
            Nome = nome;
            DataNascimento = dataNascimento;
            Email = email;
            Cpf = cpf;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new ArgumentException("Nome não pode ser vazio.", nameof(Nome));
            if (DataNascimento == default)
                throw new ArgumentException("Data de Nascimento não pode ser vazia.", nameof(DataNascimento));
            if (string.IsNullOrWhiteSpace(Email)) // TODO: Adicionar validação de formato de e-mail
                throw new ArgumentException("Email não pode ser vazio.", nameof(Email));
            if (string.IsNullOrWhiteSpace(Cpf)) // TODO: Adicionar validação de formato de CPF
                throw new ArgumentException("CPF não pode ser vazio.", nameof(Cpf));
        }
    }
}
