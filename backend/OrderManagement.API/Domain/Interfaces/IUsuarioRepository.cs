using OrderManagement.API.Domain.Entities;

namespace OrderManagement.API.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario> CreateAsync(Usuario usuario);
}
