using System;
using System.Collections.Generic;
using EstudoIA.Domain.Entities; // Ajustado
using EstudoIA.Domain.Interfaces; // Ajustado

namespace EstudoIA.Application.Services
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
            pessoa.Update(nome, dataNascimento, email, cpf);
            _pessoaRepository.Update(pessoa);
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
