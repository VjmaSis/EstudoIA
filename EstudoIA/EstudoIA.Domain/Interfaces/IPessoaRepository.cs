using System;
using System.Collections.Generic;
using EstudoIA.Domain.Entities; // Ajustado para o novo namespace da entidade

namespace EstudoIA.Domain.Interfaces
{
    public interface IPessoaRepository
    {
        void Add(Pessoa pessoa);
        Pessoa GetById(Guid id);
        IEnumerable<Pessoa> GetAll();
        void Update(Pessoa pessoa);
        void Delete(Guid id);
    }
}
