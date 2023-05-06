using eCommerce.Models;

namespace eCommerce.API.Repositories
{
    public interface IUsuarioRepository
    {
        List<Usuario> Get(); // Irá ler todos usuários
        Usuario Get(int id); // Ler apenas um usuário
        void Add(Usuario usuario);
        void Update(Usuario usuario);
        void Delete(int id);
    }
}
