using System;
using System.Collections.Generic;

namespace PessoaApp.Domain
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
