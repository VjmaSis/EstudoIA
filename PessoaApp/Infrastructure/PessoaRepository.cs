using System;
using System.Collections.Generic;
using System.Linq;
using PessoaApp.Domain;

namespace PessoaApp.Infrastructure
{
    public class PessoaRepository : IPessoaRepository
    {
        private readonly List<Pessoa> _pessoas = new List<Pessoa>();

        public void Add(Pessoa pessoa)
        {
            if (_pessoas.Any(p => p.Cpf == pessoa.Cpf))
            {
                throw new InvalidOperationException("Já existe uma pessoa cadastrada com este CPF.");
            }
            _pessoas.Add(pessoa);
        }

        public Pessoa GetById(Guid id)
        {
            return _pessoas.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Pessoa> GetAll()
        {
            return _pessoas.ToList();
        }

        public void Update(Pessoa pessoa)
        {
            var existingPessoa = GetById(pessoa.Id);
            if (existingPessoa != null)
            {
                // Verifica se o CPF está sendo alterado para um já existente em outro cadastro
                if (_pessoas.Any(p => p.Cpf == pessoa.Cpf && p.Id != pessoa.Id))
                {
                    throw new InvalidOperationException("Já existe outra pessoa cadastrada com este CPF.");
                }
                existingPessoa.Update(pessoa.Nome, pessoa.DataNascimento, pessoa.Email, pessoa.Cpf);
            }
            else
            {
                throw new KeyNotFoundException($"Pessoa com ID {pessoa.Id} não encontrada.");
            }
        }

        public void Delete(Guid id)
        {
            var pessoa = GetById(id);
            if (pessoa != null)
            {
                _pessoas.Remove(pessoa);
            }
            else
            {
                throw new KeyNotFoundException($"Pessoa com ID {id} não encontrada.");
            }
        }
    }
}
