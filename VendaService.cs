using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaLojaGeek.Services
{
    public class VendaService
    {
        private readonly AuthService _authService;
        private Venda _vendaAtual;

        public VendaService(AuthService authService)
        {
            _authService = authService;
        }

        public void IniciarNovaVenda(Cliente cliente)
        {
            _vendaAtual = new Venda
            {
                CodigoVenda = new Random().Next(1000, 9999),
                DataVenda = DateTime.Now,
                Cliente = cliente,
                StatusVenda = "Em Aberto",
                Produtos = new List<Produto>()
            };
        }

        // RN: Atendente pode consultar preços, mas exclusão exige supervisor
        public void RemoverProduto(Produto produto, string loginSup = null, string senhaSup = null)
        {
            if (_authService.UsuarioAutenticado.Perfil == Perfil.Supervisor || 
                _authService.ValidarCredenciaisSupervisor(loginSup, senhaSup))
            {
                _vendaAtual.Produtos.Remove(produto);
                Console.WriteLine("Produto removido com sucesso.");
            }
            else
            {
                throw new UnauthorizedAccessException("Apenas o supervisor pode excluir produtos da venda.");
            }
        }

        // RN: A venda pode ser cancelada APENAS pelo supervisor
        public void CancelarVenda(string loginSup, string senhaSup)
        {
            if (_authService.ValidarCredenciaisSupervisor(loginSup, senhaSup))
            {
                _vendaAtual.StatusVenda = "Cancelada";
                
                // RN: No momento do cancelamento, enviar para o sistema financeiro
                EnviarParaSistemaFinanceiro(_vendaAtual.CodigoVenda);
                
                Console.WriteLine($"Venda {_vendaAtual.CodigoVenda} cancelada e enviada ao financeiro.");
            }
            else
            {
                throw new UnauthorizedAccessException("Cancelamento negado. Credenciais de supervisor inválidas.");
            }
        }

        private void EnviarParaSistemaFinanceiro(int codigoVenda)
        {
            // Lógica de integração (API ou Banco de Dados)
            Console.WriteLine($"LOG: Notificando cancelamento da venda {codigoVenda} ao módulo financeiro...");
        }

        public void FinalizarVenda(string formaPagamento)
        {
            _vendaAtual.FormaPagamento = formaPagamento;
            _vendaAtual.StatusPagamento = "Pago";
            _vendaAtual.StatusVenda = "Concluída";
            _vendaAtual.ValorTotal = _vendaAtual.Produtos.Sum(p => p.Valor);
            
            Console.WriteLine("Venda finalizada com sucesso!");
        }
    }

    public class Venda
    {
        public int CodigoVenda { get; set; }
        public DateTime DataVenda { get; set; }
        public Cliente Cliente { get; set; }
        public List<Produto> Produtos { get; set; }
        public decimal ValorTotal { get; set; }
        public string FormaPagamento { get; set; } // Dinheiro/Cartão
        public string StatusPagamento { get; set; }
        public string StatusVenda { get; set; }
    }
}