using System;
using System.Collections.Generic;
using PessoaApp.Domain;

namespace PessoaApp.Application
{
    public class PessoaService
    {
        private readonly IPessoaRepository _pessoaRepository;

        public PessoaService(IPessoaRepository pessoaRepository)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
        }

        public Pessoa CreatePessoa(string nome, DateTime dataNascimento, string email, string cpf)
        {
            // Validações adicionais de negócio podem ser inseridas aqui antes de criar a entidade
            // Por exemplo, verificar se o CPF é válido (formato e dígito verificador)
            // ou se o e-mail tem um formato válido.
            // Por enquanto, essas validações estão na entidade Pessoa, mas algumas
            // poderiam ser movidas ou duplicadas aqui se fizerem sentido como regras de aplicação.

            var pessoa = new Pessoa(nome, dataNascimento, email, cpf);
            _pessoaRepository.Add(pessoa);
            return pessoa;
        }

        public Pessoa GetPessoaById(Guid id)
        {
            return _pessoaRepository.GetById(id);
        }

        public IEnumerable<Pessoa> GetAllPessoas()
        {
            return _pessoaRepository.GetAll();
        }

        public void UpdatePessoa(Guid id, string nome, DateTime dataNascimento, string email, string cpf)
        {
            var pessoa = _pessoaRepository.GetById(id);
            if (pessoa == null)
            {
                throw new KeyNotFoundException($"Pessoa com ID {id} não encontrada para atualização.");
            }

            // Novamente, validações de negócio podem ocorrer aqui.
            // A entidade Pessoa já tem suas próprias validações ao chamar o método Update.
            pessoa.Update(nome, dataNascimento, email, cpf); // A entidade valida os campos
            _pessoaRepository.Update(pessoa); // O repositório pode ter lógicas de persistência/duplicidade
        }

        public void DeletePessoa(Guid id)
        {
            var pessoa = _pessoaRepository.GetById(id);
            if (pessoa == null)
            {
                throw new KeyNotFoundException($"Pessoa com ID {id} não encontrada para exclusão.");
            }
            _pessoaRepository.Delete(id);
        }
    }
}
