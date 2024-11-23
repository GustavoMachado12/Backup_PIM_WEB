using System;
using System.Collections.Generic;
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
    }
}