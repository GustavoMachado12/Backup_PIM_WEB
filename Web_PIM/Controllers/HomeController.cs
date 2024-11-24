using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Web_PIM.Acao;
using Web_PIM.Acoes;
using Web_PIM.Models;

namespace Web_PIM.Controllers
{
    public class HomeController : Controller
    {
        acaoLogin acLogin = new acaoLogin();
        acaoCliente acCliente = new acaoCliente();
        acaoProduto acProduto = new acaoProduto();
        

        public ActionResult Index()
        {
            var cliente = new mCliente();
            var login = new mLogin();

            var viewModel = new mClienteLogin
            {
                Login = login,
                Cliente = cliente
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(mClienteLogin viewModel)
        {
            var cmLogin = viewModel.Login;
            cmLogin = acLogin.TestaLogin(cmLogin);

            if (cmLogin.nvlAcesso == "Administrador")
            {
                Session["Administrador"] = cmLogin;
                return RedirectToAction("Index", "Administrador");
            }
            else if (cmLogin.nvlAcesso == "Funcionario")
            {
                Session["Funcionario"] = cmLogin;
                return RedirectToAction("Index", "Administrador");
            }
            else if (cmLogin.nvlAcesso == "Cliente")
            {
                var cliente = acLogin.GetClienteByLogin(cmLogin);
                Session["Cliente"] = cliente;

                var updatedViewModel = new mClienteLogin
                {
                    Login = cmLogin,
                    Cliente = cliente
                };

                return View(updatedViewModel);
            }

            var emptyViewModel = new mClienteLogin
            {
                Login = cmLogin,
                Cliente = null
            };

            return View(emptyViewModel);
        }


        public ActionResult Cadastra()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Cadastra(mCliente cmCliente)
        {
            if (cmCliente.senha == cmCliente.confirmaSenha)
            {
                cmCliente.telefone = cmCliente.telefone.Replace("(", "").Replace(")", "").Trim();
                cmCliente.documento = cmCliente.documento.ToLower().Replace(".", "").Replace("-", "").Replace("/", "").Trim();
                if (cmCliente.documento.Length == 11)
                {
                    acCliente.cadastraClienteF(cmCliente);
                    acCliente.cadastraLogin(cmCliente);
                    return RedirectToAction("Index", "Home");
                }
                else if (cmCliente.documento.Length == 14)
                {
                    acCliente.cadastraClienteJ(cmCliente);
                    acCliente.cadastraLogin(cmCliente);
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Logout()
        {
            Session["Cliente"] = null;
            Session["Funcionario"] = null;
            Session["Administrador"] = null;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult _ListaProduto()
        {

            return PartialView(acProduto.PegaTodosProdutos());
        }

        public ActionResult _ListaCarrinho()
        {
            // Recupera o carrinho da sessão ou cria um novo
            var carrinho = Session["Carrinho"] as mCompra;

            if (carrinho == null)
            {
                // Se a sessão não contém o carrinho, inicializa um novo
                carrinho = new mCompra
                {
                    ItensPedido = new List<mItensPedidos>()
                };
                Debug.WriteLine("Sessão de carrinho está vazia. Novo carrinho criado.");
            }
            else
            {
                Debug.WriteLine($"Carrinho recuperado da sessão. Total de itens: {carrinho.ItensPedido.Count}");
            }

            // Verifica se a lista de itens está corretamente inicializada
            if (carrinho.ItensPedido == null || !carrinho.ItensPedido.Any())
            {
                Debug.WriteLine("O carrinho não possui itens.");
            }
            else
            {
                Debug.WriteLine("Itens no carrinho:");
                foreach (var item in carrinho.ItensPedido)
                {
                    Debug.WriteLine($"Produto: {item.nmProduto}, Quantidade: {item.qtdProduto}, Preço: {item.vlrProduto}");
                }
            }
            return PartialView("_ListaCarrinho", carrinho); // Substitua "_ListaCarrinho" pelo nome correto da View
        }

        public ActionResult AdicionarCarrinho(int id)
        {
            // Recupera ou cria um novo carrinho na sessão
            var carrinho = Session["Carrinho"] as mCompra ?? new mCompra { ItensPedido = new List<mItensPedidos>() };

            // Busca o produto no banco de dados
            var produto = acProduto.consultaProdutoPorId(id);
            if (produto == null)
            {
                // Retorna algum erro ou redireciona caso o produto não exista
                return RedirectToAction("Index", "Home");
            }

            // Verifica se o produto já está no carrinho
            var itemExistente = carrinho.ItensPedido.FirstOrDefault(p => p.codProduto == id.ToString());
            if (itemExistente != null)
            {
                // Atualiza a quantidade e o preço do item existente
                itemExistente.qtdProduto += 1;
            }
            else
            {
                // Adiciona o novo item ao carrinho
                var novoItem = new mItensPedidos
                {
                    ItemPedidoID = Guid.NewGuid(),
                    codProduto = id.ToString(),
                    nmProduto = produto.nomeProduto,
                    imgProduto = produto.fotoProduto, // Certifique-se de que `fotoProduto` existe no modelo
                    vlrProduto = Convert.ToDecimal(produto.valor),
                    qtdProduto = 1
                };
                carrinho.ItensPedido.Add(novoItem);
            }

            // Atualiza o valor total do carrinho
            carrinho.vlCompra = carrinho.ItensPedido.Sum(item => item.vlrProduto * item.qtdProduto);

            // Salva o carrinho na sessão
            Session["Carrinho"] = carrinho;

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public ActionResult RemoverDoCarrinho(int id)
        {
            var carrinho = Session["Carrinho"] as mCompra;

            if (carrinho != null)
            {
                var item = carrinho.ItensPedido.FirstOrDefault(i => i.codProduto == id.ToString());
                if (item != null)
                {
                    carrinho.ItensPedido.Remove(item);
                    carrinho.vlCompra = carrinho.ItensPedido.Sum(i => Convert.ToDecimal(i.vlrProduto) * i.qtdProduto);
                }

                Session["Carrinho"] = carrinho;
            }

            return Json(new { success = true });
        }

    }
}