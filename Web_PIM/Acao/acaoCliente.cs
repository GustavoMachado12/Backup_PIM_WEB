using Web_PIM.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;

namespace Web_PIM.Acoes
{
    public class acaoCliente
    {
        conexao con = new conexao();


        //CONSULTA CLIENTE FISICO 
        public List<mCliente> consultaClienteF()
        {
            List<mCliente> ClienteLista = new List<mCliente>();

            SqlCommand cmd = new SqlCommand("EXEC pSelectClienteF_Descripto", con.OpenConnection());
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sd.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                ClienteLista.Add(
                        new mCliente
                        {
                            id = Convert.ToInt32(dr["Codigo"]),
                            nome = Convert.ToString(dr["Nome"]),
                            email = Convert.ToString(dr["E-mail"]),
                            endereco = Convert.ToString(dr["Endereco"]),
                            telefone = Convert.ToString(dr["Telefone"]),
                            documento = Convert.ToString(dr["CPF"])
                        });
            }
            con.CloseConnection();

            return ClienteLista;
        }

        //CONSULTA CLIENTE JURIDICO
        public List<mCliente> consultaClienteJ()
        {
            List<mCliente> ClienteLista = new List<mCliente>();

            SqlCommand cmd = new SqlCommand("EXEC pSelectClienteJ_Descripto", con.OpenConnection());
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sd.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                ClienteLista.Add(
                        new mCliente
                        {
                            id = Convert.ToInt32(dr["Codigo"]),
                            nome = Convert.ToString(dr["Nome"]),
                            email = Convert.ToString(dr["E-mail"]),
                            endereco = Convert.ToString(dr["Endereco"]),
                            telefone = Convert.ToString(dr["Telefone"]),
                            documento = Convert.ToString(dr["CNPJ"])
                        });
            }
            con.CloseConnection();

            return ClienteLista;
        }



        //DELETA CLIENTE FISICO
        public bool deletaCliente(mCliente cliente)
        {
            bool sucesso = false;
            try
            {
                // Abrindo a conexão
                using (SqlCommand cmd = new SqlCommand("pExcluiCliente @CodCliente", con.OpenConnection()))
                {
                    cmd.Parameters.AddWithValue("@CodCliente", cliente.id);

                    int i = cmd.ExecuteNonQuery();

                    if (i >= 1)
                    {
                        sucesso = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir cliente: {ex.Message}");
            }
            finally
            {
                con.CloseConnection();
            }

            return sucesso;
        }

        //CADASTRA CLIENTE FISICO
        public void cadastraClienteF(mCliente cmCliente)
        {
            SqlCommand cmd = new SqlCommand("pCadastraClienteF", con.OpenConnection());
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.Add("@NomeCli", SqlDbType.VarChar).Value = cmCliente.nome;
                cmd.Parameters.Add("@EmailCli", SqlDbType.VarChar).Value = cmCliente.email;
                cmd.Parameters.Add("@EnderecoCli", SqlDbType.VarChar).Value = cmCliente.endereco;
                cmd.Parameters.Add("@TelefoneCli", SqlDbType.VarChar).Value = cmCliente.telefone;
                cmd.Parameters.Add("@CPF_Cli", SqlDbType.VarChar).Value = cmCliente.documento;

                int linhasAfetadas = cmd.ExecuteNonQuery();

                if (linhasAfetadas > 0)
                {
                    Console.WriteLine("Cliente cadastrado com sucesso.");
                }
                else
                {
                    Console.WriteLine("Nenhuma alteração foi realizada.");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.CloseConnection();
            }

            pegaUltimoCadastro(cmCliente);
        }


        //CADASTRA CLIENTE JURIDICO
        public void cadastraClienteJ(mCliente cmCliente)
        {
            SqlCommand cmd = new SqlCommand("pCadastraClienteJ", con.OpenConnection());
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.Add("@NomeCli", SqlDbType.VarChar).Value = cmCliente.nome;
                cmd.Parameters.Add("@EmailCli", SqlDbType.VarChar).Value = cmCliente.email;
                cmd.Parameters.Add("@EnderecoCli", SqlDbType.VarChar).Value = "Não Informado";
                cmd.Parameters.Add("@TelefoneCli", SqlDbType.VarChar).Value = cmCliente.telefone;
                cmd.Parameters.Add("@CNPJ_Cli", SqlDbType.VarChar).Value = cmCliente.documento;

                int linhasAfetadas = cmd.ExecuteNonQuery();

                if (linhasAfetadas > 0)
                {
                    Console.WriteLine("Cliente cadastrado com sucesso.");
                }
                else
                {
                    Console.WriteLine("Nenhuma alteração foi realizada.");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.CloseConnection();
            }

            pegaUltimoCadastro(cmCliente);
        }


        //CADASTRA LOGIN 
        public void cadastraLogin(mCliente cmCliente)
        {
            SqlCommand cmd = new SqlCommand("pCadastraLoginCliente", con.OpenConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@pCdCliente", SqlDbType.Int).Value = cmCliente.id;
            cmd.Parameters.Add("@pLogin", SqlDbType.VarChar).Value = cmCliente.email;
            cmd.Parameters.Add("@pSenha", SqlDbType.VarChar).Value = cmCliente.senha;

            int linhasAfetadas = cmd.ExecuteNonQuery();

            if (linhasAfetadas > 0)
            {
                Console.WriteLine("Cliente cadastrado com sucesso.");
            }
            else
            {
                Console.WriteLine("Nenhuma alteração foi realizada.");
            }

            con.CloseConnection();
        }

        //PEGA ULTIMO CADASTRO DO LOGIN
        public void pegaUltimoCadastro(mCliente cmCliente)
        {
            SqlCommand cmd = new SqlCommand(
               "SELECT * FROM vVisualizaCliente ORDER BY Codigo DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY", con.OpenConnection());
            Console.WriteLine(cmd);

            SqlDataReader r;
            r = cmd.ExecuteReader();

            if (r.HasRows)
            {
                while (r.Read())
                {
                    cmCliente.id = Convert.ToInt32(r["Codigo"]);
                }
            }

            con.CloseConnection();
        }

        //CONSULTA PELO ID
        public mCliente consultaClientePorId(int id)
        {
            mCliente cliente = null;

            SqlCommand cmd = new SqlCommand("EXEC pSelectCliente_PorID @CodCliente", con.OpenConnection());
            cmd.Parameters.AddWithValue("@CodCliente", id);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    cliente = new mCliente
                    {
                        id = Convert.ToInt32(dr["Codigo"]),
                        nome = dr["Nome"].ToString(),
                        email = dr["E-mail"].ToString(),
                        telefone = dr["Telefone"].ToString(),
                        documento = dr["Documento"].ToString(),
                        endereco = dr["Endereco"].ToString(),
                    };
                }
            }

            con.CloseConnection();

            return cliente;
        }

        //ATUALIZA CLIENTE FISICO
        public mCliente atualizaClienteF(mCliente cliente)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "pAlteraClienteF", con.OpenConnection());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CodCli", SqlDbType.Int).Value = cliente.id;
                cmd.Parameters.Add("@NomeCli", SqlDbType.VarChar).Value = cliente.nome;
                cmd.Parameters.Add("@EmailCli", SqlDbType.VarChar).Value = cliente.email;
                cmd.Parameters.Add("@EnderecoCli", SqlDbType.VarChar).Value = cliente.endereco;
                cmd.Parameters.Add("@TelefoneCli", SqlDbType.VarChar).Value = cliente.telefone;
                cmd.Parameters.Add("@CPF_Cli", SqlDbType.VarChar).Value = cliente.documento;

                int linhasAfetadas = cmd.ExecuteNonQuery();

                if (linhasAfetadas > 0)
                {
                    Console.WriteLine("Cliente cadastrado com sucesso.");
                }
                else
                {
                    Console.WriteLine("Nenhuma alteração foi realizada.");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.CloseConnection();
            }

            return cliente;
        }

        //ATUALIZA CLIENTE JURIDICO
        public mCliente atualizaClienteJ(mCliente cliente)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "pAlteraClienteJ", con.OpenConnection());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CodCli", SqlDbType.Int).Value = cliente.id;
                cmd.Parameters.Add("@NomeCli", SqlDbType.VarChar).Value = cliente.nome;
                cmd.Parameters.Add("@EmailCli", SqlDbType.VarChar).Value = cliente.email;
                cmd.Parameters.Add("@EnderecoCli", SqlDbType.VarChar).Value = cliente.endereco;
                cmd.Parameters.Add("@TelefoneCli", SqlDbType.VarChar).Value = cliente.telefone;
                cmd.Parameters.Add("@CNPJ_Cli", SqlDbType.VarChar).Value = cliente.documento;

                int linhasAfetadas = cmd.ExecuteNonQuery();

                if (linhasAfetadas > 0)
                {
                    Console.WriteLine("Cliente cadastrado com sucesso.");
                }
                else
                {
                    Console.WriteLine("Nenhuma alteração foi realizada.");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.CloseConnection();
            }

            return cliente;
        }

        //SEPARA ENDERECO
        public void SepararEndereco(mCliente cliente)
        {
            if (string.IsNullOrEmpty(cliente.endereco))
                throw new ArgumentException("O endereço completo não pode ser vazio ou nulo.");

            var partes = cliente.endereco.Split(',');

            if (partes.Length < 6)
                throw new ArgumentException("Formato de endereço inválido. Certifique-se de incluir CEP, Logradouro, Número, Bairro, Cidade e Estado.");

            cliente.cep = partes[0].Trim();
            cliente.logradouro = partes[1].Trim();

            var numeroLogradouroTexto = partes[2].Trim();
            if (!int.TryParse(numeroLogradouroTexto, out int numeroLogradouro))
            {
                cliente.numLogradouro = 0; 
            }
            else
            {
                cliente.numLogradouro = numeroLogradouro;
            }

            cliente.bairro = partes[3].Trim();
            cliente.cidade = partes[4].Trim();
            cliente.estado = partes[5].Trim();

            if (partes.Length > 6)
            {
                cliente.complemento = partes[6].Trim();
            }

            if (cliente.logradouro.Contains(" "))
            {
                var logradouroPartes = cliente.logradouro.Split(' ');
                if (int.TryParse(logradouroPartes.Last(), out int numero))
                {
                    cliente.numLogradouro = numero;
                    cliente.logradouro = string.Join(" ", logradouroPartes.Take(logradouroPartes.Length - 1));
                }
            }
        }

        //COMBINA ENDERECO
        public string CombinarEndereco(mCliente cliente)
        {
            string cep = string.IsNullOrWhiteSpace(cliente.cep) ? "Não Informado" : cliente.cep;
            string logradouro = string.IsNullOrWhiteSpace(cliente.logradouro) ? "Não Informado" : cliente.logradouro;
            string numero = cliente.numLogradouro == 0 ? "Não Informado" : cliente.numLogradouro.ToString();
            string bairro = string.IsNullOrWhiteSpace(cliente.bairro) ? "Não Informado" : cliente.bairro;
            string cidade = string.IsNullOrWhiteSpace(cliente.cidade) ? "Não Informado" : cliente.cidade;
            string estado = string.IsNullOrWhiteSpace(cliente.estado) ? "Não Informado" : cliente.estado;
            string complemento = string.IsNullOrWhiteSpace(cliente.complemento) ? "Não Informado" : cliente.complemento;

            var endereco = $"{cep}, {logradouro}, {numero}, {bairro}, {cidade}, {estado}";

            endereco += complemento != "Não Informado" ? $", {complemento}" : "";

            return endereco;
        }

    }
}
