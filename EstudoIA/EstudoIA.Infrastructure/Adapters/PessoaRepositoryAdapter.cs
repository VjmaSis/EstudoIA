using System;
using System.Collections.Generic;
using EstudoIA.Domain.Entities; // Ajustado
using EstudoIA.Domain.Interfaces; // Ajustado

namespace EstudoIA.Infrastructure.Adapters
{
    public class PessoaRepositoryAdapter : IPessoaRepository
    {
        private readonly IPessoaRepository _adapteeRepository;

        public PessoaRepositoryAdapter(IPessoaRepository adapteeRepository)
        {
            _adapteeRepository = adapteeRepository ?? throw new ArgumentNullException(nameof(adapteeRepository));
        }

        public void Add(Pessoa pessoa)
        {
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
