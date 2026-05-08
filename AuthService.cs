using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaLojaGeek.Services
{
    public class AuthService
    {
        // Simulação de banco de dados de usuários
        private readonly List<Usuario> _usuarios = new List<Usuario>();
        public Usuario UsuarioAutenticado { get; private set; }

        public bool Login(string login, string senha)
        {
            // RN: Todo acesso ao sistema é feito por login e senha
            var usuario = _usuarios.FirstOrDefault(u => u.Login == login && u.Senha == senha);

            if (usuario != null)
            {
                UsuarioAutenticado = usuario;
                Console.WriteLine($"Bem-vindo, {usuario.Nome}! Nível: {usuario.Perfil}");
                return true;
            }

            Console.WriteLine("Login ou senha inválidos.");
            return false;
        }

        public void Logout()
        {
            UsuarioAutenticado = null;
        }

        // Método para verificar se o usuário atual tem permissão de Supervisor
        public bool IsSupervisor()
        {
            return UsuarioAutenticado?.Perfil == Perfil.Supervisor;
        }

        // Método para validar credenciais de supervisor "on-the-fly" (para exclusão de itens)
        public bool ValidarCredenciaisSupervisor(string login, string senha)
        {
            return _usuarios.Any(u => u.Login == login && u.Senha == senha && u.Perfil == Perfil.Supervisor);
        }
    }

    public enum Perfil { Atendente, Estoquista, Supervisor }

    public class Usuario
    {
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public Perfil Perfil { get; set; }
    }
}