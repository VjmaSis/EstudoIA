using System;
using System.Collections.Generic;
using PessoaApp.Domain;

namespace PessoaApp.Infrastructure.Adapters
{
    // Este Adapter implementa a interface IPessoaRepository que o serviço da aplicação espera.
    // Ele "adapta" uma implementação concreta de repositório (inicialmente, o PessoaRepository em memória)
    // para essa interface. No futuro, o _adapteeRepository poderia ser uma instância de um
    // repositório que acessa um banco de dados, e o PessoaService não precisaria saber disso.
    public class PessoaRepositoryAdapter : IPessoaRepository
    {
        private readonly IPessoaRepository _adapteeRepository; // Depende da abstração para flexibilidade

        // O construtor recebe uma implementação de IPessoaRepository.
        // Na configuração da injeção de dependência, passaremos o PessoaRepository (em memória)
        // ou, futuramente, um PessoaDbRepository.
        public PessoaRepositoryAdapter(IPessoaRepository adapteeRepository)
        {
            _adapteeRepository = adapteeRepository ?? throw new ArgumentNullException(nameof(adapteeRepository));
        }

        public void Add(Pessoa pessoa)
        {
            // A lógica de adaptação poderia entrar aqui se as interfaces fossem diferentes.
            // Por exemplo, se _adapteeRepository.CreatePessoa(pessoa) fosse o método real.
            _adapteeRepository.Add(pessoa);
        }

        public Pessoa GetById(Guid id)
        {
            return _adapteeRepository.GetById(id);
        }

        public IEnumerable<Pessoa> GetAll()
        {
            return _adapteeRepository.GetAll();
        }

        public void Update(Pessoa pessoa)
        {
            _adapteeRepository.Update(pessoa);
        }

        public void Delete(Guid id)
        {
            _adapteeRepository.Delete(id);
        }
    }
}
