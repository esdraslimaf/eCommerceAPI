using eCommerce.API.Database;
using eCommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly eCommerceContext _db;
        public UsuarioRepository(eCommerceContext db)
        {
            _db = db;
        }
        public List<Usuario> Get()
        {
            return _db.Usuarios.Include(a => a.Contato).OrderBy(a => a.Id).ToList(); //Operação primeiro é feita no banco de dados e depois convertida para lista C#
        }

        public Usuario Get(int id)
        {
            return _db.Usuarios.Include(a => a.Contato).Include(a => a.EnderecosEntrega).Include(a => a.Departamentos).FirstOrDefault(a => a.Id == id)!;
        }


        //CASO ESPECIAL: N pra N, as vezes não queremos criar um novo objeto na tabela intermediária, e sim associar o usuário a um departamento existente.
        public void Add(Usuario usuario)
        {
            // O objeto JSON que chega ao controlador pode ou não incluir os departamentos.
            CriarVinculoDoUsuarioComDepartamento(usuario);
            _db.Usuarios.Add(usuario);
            _db.SaveChanges();
        }
        
        /* Update é mais complicado, tendo em vista que os vínculos podem ser ordem para criar novos vínculos(Adicionar - Insert),
         * deletar um vínculo existente(Delete) ou mesmo alterar um vínculo existente(Update).
         * A tabela intermediária pode sofrer todas as operações ditas acima: Insert, Delete ou Update. Então analisar essa tabela é bem complicada*/
        // Podemos optar por deletar todos os vínculos do Usuário(Todos os dados da tabela intermediária) e recriamos os vínculos(Insert).
        /* Lembrando que no nosso caso que estamos utilizando não temos uma classe que represente essa tabela intermediária, essa tabela intermediária foi criada pelo
         * próprio EF de forma automática. Se tivéssemos uma classe intermediária poderiamos navegar nessa
         * classe intermediária e deletar todos os registros que batem com o id do usuário.
         * Como não temos uma classe que representa essa tabela intermediária, teremos que pegar o usuário do banco de dados incluido os departamentos que ele possui
         * depois deletar da lista associadas a ele os departamentos que não precisamos.*/
        public void Update(Usuario usuario)
        {
            ExcluirVinculoUsuarioComDepartamento(usuario); //Esse método faz alterações em um objeto vindo da tabela usuário do db(Objeto rastreado pelo EF)
            CriarVinculoDoUsuarioComDepartamento(usuario); //Esse método faz alterações em um objeto usuário enviado no JSON(Objeto não rastreado pelo EF)
            _db.Usuarios.Update(usuario); // Como são dois objetos diferentes, o EF vai reclamar na hora de dar o update. 
            _db.SaveChanges();
        }
     
        //Exclusão é bem mais tranquila já que os relacionamentos são do tipo cascata(Se deleto o pai, o filho é deletado junto).
        public void Delete(int id)
        {
            _db.Usuarios.Remove(Get(id));          
            _db.SaveChanges();
        }

        private void ExcluirVinculoUsuarioComDepartamento(Usuario usuario)
        {
            var userDoBanco = _db.Usuarios.Include(a => a.Departamentos).FirstOrDefault(a => a.Id == usuario.Id);
            foreach (Departamento dept in userDoBanco!.Departamentos!)
            {
                userDoBanco.Departamentos.Remove(dept);
            }
            _db.SaveChanges();
            _db.ChangeTracker.Clear();
        }
        private void CriarVinculoDoUsuarioComDepartamento(Usuario usuario)
        {
            if (usuario.Departamentos != null) // Se houver departamentos no objeto JSON inserido
            {
                var departamentosController = usuario.Departamentos;
                usuario.Departamentos = new List<Departamento>();

                // Verifique se cada departamento no objeto JSON existe ou não no banco de dados e adiciona referências a eles à lista de departamentos do usuário.
                foreach (var dept in departamentosController)
                {
                    if (dept.Id > 0) //Então existe no banco de dados
                    {
                        //Referência ao registro existente no banco de dados
                        usuario.Departamentos.Add(_db.Departamentos.Find(dept.Id)!); //Adiciona uma referência ao departamento existente ao usuário.
                        //acima estaremos associando aquele usuário a um departamento existente
                    }
                    else //<0 ou = 0 porque não iremos inserir ID no Body em JSON
                    {
                        //Um novo departamento será criado posteriormente e associado aquele usuário.
                        usuario.Departamentos.Add(dept);
                    }
                }
            }
        }

    }
}
