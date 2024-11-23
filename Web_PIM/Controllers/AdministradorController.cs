using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web_PIM.Acao;
using Web_PIM.Acoes;
using Web_PIM.Models;

namespace Web_PIM.Controllers
{
    public class AdministradorController : Controller
    {
        conexao con = new conexao();
        acaoCliente acCliente = new acaoCliente();
        acaoFuncionario acFuncionario = new acaoFuncionario();
        acaoFornecedor acFornecedor = new acaoFornecedor();
        acaoProduto acProduto = new acaoProduto();

        
        public ActionResult Index()
        {
            if (Session["Administrador"] != null)
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
                
        }

        //------------------------------------------------------------------------------------------------------------------|CLIENTE|---------------------------------------

        public ActionResult ConsultaClienteF()
        {
            if (Session["Administrador"] != null)
            {
                ViewBag.Clientes = acCliente.consultaClienteF();
                return View(new mCliente());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ConsultaClienteJ()
        {
            if(Session["Administrador"] != null)
            {
                ViewBag.Clientes = acCliente.consultaClienteJ();
                return View(new mCliente());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public ActionResult CadastraCliente(mCliente cmCliente)
        {
            cmCliente.documento = cmCliente.documento.ToLower().Replace(".", "").Replace("-", "").Replace("/", "").Trim();
            cmCliente.endereco = acCliente.CombinarEndereco(cmCliente);

            if (cmCliente.documento.Length == 11)
            {
                acCliente.cadastraClienteF(cmCliente);
            }
            else if (cmCliente.documento.Length == 14)
            {
                acCliente.cadastraClienteJ(cmCliente);
            }
            else
            {
                ViewBag.Aviso("Documento inválido");
            }

            return RedirectToAction("ConsultaClienteF");
        }

        [HttpPost]
        public ActionResult EditaCliente(mCliente cmCliente)
        {
            if (cmCliente == null || cmCliente.id <= 0)
            {
                ViewBag.Aviso = "ID não identificado";
                return View("ConsultaClienteF");
            }

            cmCliente.documento = cmCliente.documento.ToLower().Replace(".", "").Replace("-", "").Replace("/", "").Trim();
            cmCliente.endereco = acCliente.CombinarEndereco(cmCliente);

            if (cmCliente.documento.Length == 11)
            {
                acCliente.atualizaClienteF(cmCliente);
            }
            else if (cmCliente.documento.Length == 14)
            {
                acCliente.atualizaClienteJ(cmCliente);
            }
            else
            {
                ViewBag.Aviso = "Documento inválido";
                return View("ConsultaClienteF");
            }

            return RedirectToAction("ConsultaClienteJ");
        }

        [HttpGet]
        public JsonResult GetClienteById(int id)
        {
            try
            {
                var cliente = acCliente.consultaClientePorId(id);
                acCliente.SepararEndereco(cliente);
                if (cliente != null)
                {
                    return Json(new
                    {
                        success = true,
                        nome = cliente.nome,
                        email = cliente.email,
                        telefone = cliente.telefone,
                        documento = cliente.documento,
                        cep = cliente.cep,
                        logradouro = cliente.logradouro,
                        numLogradouro = cliente.numLogradouro,
                        bairro = cliente.bairro,
                        cidade = cliente.cidade,
                        estado = cliente.estado,
                        complemento = cliente.complemento
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Cliente não encontrado." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult excluiCliente(mCliente cliente)
        {
            if (Session["Administrador"] != null)
            {
                int id = cliente.id;
                var docCliente = acCliente.consultaClientePorId(id);
                string numDoc = Convert.ToString(docCliente.documento.Trim());
                bool excluido = acCliente.deletaCliente(cliente);

                if (excluido)
                {
                    if (numDoc.Length == 11)
                    {
                        return RedirectToAction("ConsultaClienteF"); 
                    }
                    else
                    {
                        return RedirectToAction("ConsultaClienteJ");
                    }
                }
                else
                {
                    ViewBag.Aviso = "Erro ao excluir o cliente. Tente novamente.";
                    return RedirectToAction("ConsultaClienteF"); 
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session["Cliente"] = null;
            Session["Funcionario"] = null;
            Session["Administrador"] = null;

            return RedirectToAction("Index", "Home");
        }

        //------------------------------------------------------------------------------------------------------------------|FUNCIONARIO|----------------------------------

        public ActionResult ConsultaFuncionario()
        {
            if (Session["Administrador"] != null)
            {
                ViewBag.Clientes = acFuncionario.ConsultaFuncionario();

                var listaCargo = acFuncionario.ConsultaCargos();
                List<SelectListItem> cargos = new List<SelectListItem>();
                
                var listaNvlAcesso = acFuncionario.ConsultaNivelAcesso();
                List<SelectListItem> acessos = new List<SelectListItem>();

                foreach (var cargo in listaCargo)
                {
                    cargos.Add(new SelectListItem
                    {
                        Value = cargo.idCargo.ToString(),
                        Text = cargo.nmCargo.ToString()
                    });
                }
                ViewBag.Cargos = cargos;

                foreach (var acesso in listaNvlAcesso)
                {
                    acessos.Add(new SelectListItem
                    {
                        Value = acesso.codNivelAcesso.ToString(),
                        Text = acesso.nmNivelAcesso.ToString()
                    });
                }
                ViewBag.Acessos = acessos;

                return View(new mFuncionario());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult CadastraFuncionario(mFuncionario funcionario)
        {
            if (Session["Administrador"] != null)
            {
                acFuncionario.cadastraFuncionario(funcionario);
                return RedirectToAction("ConsultaFuncionario", "Administrador");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditaFuncionario(mFuncionario funcionario)
        {
            if (Session["Administrador"] != null)
            {
                acFuncionario.atualizaFuncionario(funcionario);
                return RedirectToAction("ConsultaFuncionario", "Administrador");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult excluiFuncionario(mFuncionario funcionario)
        {
            if (Session["Administrador"] != null)
            {
                acFuncionario.deletaFuncionario(funcionario);
                return RedirectToAction("ConsultaFuncionario", "Administrador");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public JsonResult GetFuncionarioById(int id)
        {
            try
            {
                var funcionario = acFuncionario.consultaFuncionarioPorId(id);
                if (funcionario != null)
                {
                    Console.WriteLine($"Funcionario encontrado: {funcionario.nmFuncionario}, Senha: {funcionario.senhaFuncionario}");

                    return Json(new
                    {
                        success = true,
                        nome = funcionario.nmFuncionario,
                        login = funcionario.loginFuncionario,
                        cargo = funcionario.idCargo,
                        tipoAcesso = funcionario.idNvlAcesso,
                        senhaFuncionario = funcionario.senhaFuncionario
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Funcionário não encontrado." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //------------------------------------------------------------------------------------------------------------------|FORNECEDORES|----------------------------------

        public ActionResult ConsultaFornecedor()
        {
            if (Session["Administrador"] != null || Session["Funcionario"] != null)
            {
                ViewBag.Fornecedores = acFornecedor.consultaFornecedor();
                return View(new mFornecedor());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CadastraFornecedor(mFornecedor fornecedor)
        {
            if (Session["Administrador"] != null || Session["Funcionario"] != null)
            {
                fornecedor.enderecoFornecedor = acFornecedor.CombinarEndereco(fornecedor);
                acFornecedor.cadastraFornecedor(fornecedor);
                return RedirectToAction("ConsultaFornecedor", "Administrador");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult atualizaFornecedor(mFornecedor fornecedor)
        {
            if (Session["Administrador"] != null || Session["Funcionario"] != null)
            {
                fornecedor.enderecoFornecedor = acFornecedor.CombinarEndereco(fornecedor);
                ViewBag.Fornecedores = acFornecedor.atualizaFornecedor(fornecedor);
                return RedirectToAction("ConsultaFornecedor", "Administrador");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult deletaFornecedor(int id)
        {
            if (Session["Administrador"] != null || Session["Funcionario"] != null)
            {
                var fornecedor = new mFornecedor { idFornecedor = id };

                bool sucesso = acFornecedor.deletaFornecedor(fornecedor);

                if (sucesso)
                {
                    ViewBag.Aviso = "Fornecedor excluído com sucesso.";
                }
                else
                {
                    ViewBag.Aviso = "Erro ao excluir o fornecedor.";
                }

                return RedirectToAction("ConsultaFornecedor", "Administrador");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public JsonResult GetFornecedorById(int id)
        {
            try
            {
                var fornecedor = acFornecedor.consultaFornecedorPorId(id);
                acFornecedor.SepararEndereco(fornecedor);
                if (fornecedor != null)
                {
                    Console.WriteLine($"Fornecedor encontrado: {fornecedor.nomeFornecedor}, CNPJ: {fornecedor.cnpjFornecedor}");

                    return Json(new
                    {
                        success = true,

                        nomeFornecedor = fornecedor.nomeFornecedor,
                        cnpjFornecedor = fornecedor.cnpjFornecedor,
                        emailFornecedor = fornecedor.emailFornecedor,
                        enderecoFornecedor = fornecedor.enderecoFornecedor,
                        telefoneFornecedor = fornecedor.telefoneFornecedor,
                        cep = fornecedor.cep,
                        logradouro = fornecedor.logradouro,
                        numLogradouro = fornecedor.numLogradouro,
                        bairro = fornecedor.bairro,
                        cidade = fornecedor.cidade,
                        estado = fornecedor.estado,
                        complemento = fornecedor.complemento
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Fornecedor não encontrado." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //------------------------------------------------------------------------------------------------------------------|PRODUTOS|----------------------------------

        public ActionResult ConsultaProduto()
        {
            if (Session["Administrador"] != null || Session["Funcionario"] != null)
            {
                ViewBag.Produtos = acProduto.consultaProduto();

                var listaCategorias = acProduto.ConsultaCategorias();
                List<SelectListItem> categorias = new List<SelectListItem>();

                foreach (var categoria in listaCategorias)
                {
                    categorias.Add(new SelectListItem
                    {
                        Value = categoria.idCategoria.ToString(),
                        Text = categoria.nmCategoria.ToString()
                    });
                }
                ViewBag.Categorias = categorias;

                return View(new mProduto());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult CadastraProduto(mProduto produto, HttpPostedFileBase fotoProduto)
        {

            if (fotoProduto != null && fotoProduto.ContentLength > 0)
            {
                string arquivo = Path.GetFileName(fotoProduto.FileName); 
                string caminho = "/Imagens/Produtos/" + arquivo; 
                string nomeArquivo = Path.Combine(Server.MapPath("/Imagens/Produtos/"), arquivo);
                fotoProduto.SaveAs(nomeArquivo);
                produto.fotoProduto = caminho;
                
                acProduto.CadastraProduto(produto); 
            }

            return RedirectToAction("ConsultaProduto", "Administrador");
        }

        //EDITA PRODUTO
        [HttpPost]
        public ActionResult EditaProduto(mProduto produto, HttpPostedFileBase fotoProduto, string fotoProdutoLink)
        {
            if (!string.IsNullOrEmpty(fotoProdutoLink))
            {
                produto.fotoProduto = fotoProdutoLink; 
            }
            else if (fotoProduto != null && fotoProduto.ContentLength > 0)
            {
                string arquivo = Path.GetFileName(fotoProduto.FileName);
                string caminho = "/Imagens/Produtos/" + arquivo;
                string nomeArquivo = Path.Combine(Server.MapPath("/Imagens/Produtos/"), arquivo);
                fotoProduto.SaveAs(nomeArquivo);
                produto.fotoProduto = caminho;
            }

            if (Session["Administrador"] != null || Session["Funcionario"] != null)
            {
                bool atualizado = acProduto.atualizaProduto(produto);

                if (atualizado)
                {
                    TempData["MensagemSucesso"] = "Produto atualizado com sucesso!";
                    return RedirectToAction("ConsultaProduto", "Administrador");
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao atualizar o produto. Tente novamente.";
                    return RedirectToAction("ConsultaProduto", "Administrador");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //DELETA PRODUTO
        public ActionResult ExcluiProduto(int id)
        {
            if (Session["Administrador"] != null || Session["Funcionario"] != null)
            {

                bool sucesso = acProduto.deletaProduto(id);

                if (sucesso)
                {
                    ViewBag.Aviso = "Produto excluído com sucesso.";
                }
                else
                {
                    ViewBag.Aviso = "Erro ao excluir o fornecedor.";
                }

                return RedirectToAction("ConsultaProduto", "Administrador");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpGet]
        public JsonResult GetProdutoById(int id)
        {
            var produto = acProduto.consultaProdutoPorId(id);
            if (produto != null)
            {
                return Json(new
                {
                    success = true,
                    idProduto = produto.idProduto,
                    nomeProduto = produto.nomeProduto,
                    valor = produto.valor,
                    idCategoria = produto.idCategoria, 
                    quantidade = produto.quantidade,
                    fotoProduto = produto.fotoProduto
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Produto não encontrado." }, JsonRequestBehavior.AllowGet);
        }


    }
}
