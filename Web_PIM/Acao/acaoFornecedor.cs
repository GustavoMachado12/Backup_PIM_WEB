using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using Web_PIM.Acoes;
using Web_PIM.Models;

namespace Web_PIM.Acao
{
    public class acaoFornecedor
    {
        conexao con = new conexao();

        public List<mFornecedor> consultaFornecedor()
        {
            List<mFornecedor> fornecedorLista = new List<mFornecedor>();

            SqlCommand cmd = new SqlCommand("EXEC pSelectFornecedor_Descripto", con.OpenConnection());
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sd.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                fornecedorLista.Add(
                    new mFornecedor
                    {
                        idFornecedor = Convert.ToInt32(dr["Codigo"]),
                        nomeFornecedor = Convert.ToString(dr["Nome"]),
                        cnpjFornecedor = Convert.ToString(dr["CNPJ"]),
                        emailFornecedor = Convert.ToString(dr["Email"]),
                        enderecoFornecedor = Convert.ToString(dr["Endereco"]),
                        telefoneFornecedor = Convert.ToString(dr["Telefone"])
                    });
            }

            con.CloseConnection();

            return fornecedorLista;
        }


        //PROCURA POR ID
        public mFornecedor consultaFornecedorPorId(int id)
        {
            mFornecedor fornecedor = null;

            SqlCommand cmd = new SqlCommand("EXEC pSelectFornecedorPorID @CodFornecedor", con.OpenConnection());
            cmd.Parameters.AddWithValue("@CodFornecedor", id);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                fornecedor = new mFornecedor
                {
                    idFornecedor = Convert.ToInt32(reader["Codigo"]),
                    nomeFornecedor = Convert.ToString(reader["Nome"]),
                    cnpjFornecedor = Convert.ToString(reader["CNPJ"]),
                    emailFornecedor = Convert.ToString(reader["Email"]),
                    enderecoFornecedor = Convert.ToString(reader["Endereco"]),
                    telefoneFornecedor = Convert.ToString(reader["Telefone"])
                };
            }

            con.CloseConnection();

            return fornecedor;
        }

        //CADASTRA FORNECEDOR
        public void cadastraFornecedor(mFornecedor fornecedor)
        {
            SqlCommand cmd = new SqlCommand("pCadastraFornecedor", con.OpenConnection());
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.Add("@nmForn", SqlDbType.VarChar).Value = fornecedor.nomeFornecedor;
                cmd.Parameters.Add("@numCNPJ", SqlDbType.Char).Value = fornecedor.cnpjFornecedor;
                cmd.Parameters.Add("@emailForn", SqlDbType.VarChar).Value = fornecedor.emailFornecedor;
                cmd.Parameters.Add("@endeForn", SqlDbType.VarChar).Value = fornecedor.enderecoFornecedor;
                cmd.Parameters.Add("@teleForn", SqlDbType.Char).Value = fornecedor.telefoneFornecedor;

                int linhasAfetadas = cmd.ExecuteNonQuery();

                if (linhasAfetadas > 0)
                {
                    Console.WriteLine("Fornecedor cadastrado com sucesso.");
                }
                else
                {
                    Console.WriteLine("Nenhuma alteração foi realizada.");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"Erro ao cadastrar fornecedor: {e.Message}");
            }
            finally
            {
                con.CloseConnection();
            }

        }

        //ATUALIZA FORNECEDOR
        public mFornecedor atualizaFornecedor(mFornecedor fornecedor)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "pAlteraFornecedor", con.OpenConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                // Adiciona os parâmetros
                cmd.Parameters.Add("@CodForn", SqlDbType.Int).Value = fornecedor.idFornecedor;
                cmd.Parameters.Add("@NomeForn", SqlDbType.VarChar).Value = fornecedor.nomeFornecedor;
                cmd.Parameters.Add("@CNPJ", SqlDbType.VarChar).Value = fornecedor.cnpjFornecedor;
                cmd.Parameters.Add("@EmailForn", SqlDbType.VarChar).Value = fornecedor.emailFornecedor;
                cmd.Parameters.Add("@EnderecoForn", SqlDbType.VarChar).Value = fornecedor.enderecoFornecedor;
                cmd.Parameters.Add("@TelefoneForn", SqlDbType.VarChar).Value = fornecedor.telefoneFornecedor;

                // Executa a query e verifica o resultado
                int linhasAfetadas = cmd.ExecuteNonQuery();

                if (linhasAfetadas > 0)
                {
                    Console.WriteLine("Fornecedor atualizado com sucesso.");
                }
                else
                {
                    Console.WriteLine("Nenhuma alteração foi realizada.");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Erro ao atualizar fornecedor: " + e.Message);
            }
            finally
            {
                con.CloseConnection();
            }

            return fornecedor;
        }

        //DELETA FORNECEDOR
        public bool deletaFornecedor(mFornecedor fornecedor)
        {
            bool sucesso = false;

            try
            {
                Console.WriteLine($"Excluindo fornecedor com ID: {fornecedor.idFornecedor}");

                using (SqlCommand cmd = new SqlCommand("pExcluiFornecedor @CodForn", con.OpenConnection()))
                {
                    cmd.Parameters.AddWithValue("@CodForn", fornecedor.idFornecedor);

                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    if (linhasAfetadas > 0)
                    {
                        sucesso = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir fornecedor: {ex.Message}");
            }
            finally
            {
                con.CloseConnection();
            }

            return sucesso;
        }

        //SEPARA ENDERECO
        public void SepararEndereco(mFornecedor fornecedor)
        {
            if (string.IsNullOrEmpty(fornecedor.enderecoFornecedor))
                throw new ArgumentException("O endereço completo não pode ser vazio ou nulo.");

            var partes = fornecedor.enderecoFornecedor.Split(',');

            if (partes.Length < 6)
                throw new ArgumentException("Formato de endereço inválido. Certifique-se de incluir CEP, Logradouro, Número, Bairro, Cidade e Estado.");

            fornecedor.cep = partes[0].Trim();
            fornecedor.logradouro = partes[1].Trim();

            var numeroLogradouroTexto = partes[2].Trim();
            if (!int.TryParse(numeroLogradouroTexto, out int numeroLogradouro))
            {
                fornecedor.numLogradouro = 0;
            }
            else
            {
                fornecedor.numLogradouro = numeroLogradouro;
            }

            fornecedor.bairro = partes[3].Trim();
            fornecedor.cidade = partes[4].Trim();
            fornecedor.estado = partes[5].Trim();

            if (partes.Length > 6)
            {
                fornecedor.complemento = partes[6].Trim();
            }

            if (fornecedor.logradouro.Contains(" "))
            {
                var logradouroPartes = fornecedor.logradouro.Split(' ');
                if (int.TryParse(logradouroPartes.Last(), out int numero))
                {
                    fornecedor.numLogradouro = numero;
                    fornecedor.logradouro = string.Join(" ", logradouroPartes.Take(logradouroPartes.Length - 1));
                }
            }
        }

        //COMBINA ENDERECO
        public string CombinarEndereco(mFornecedor fornecedor)
        {
            string cep = string.IsNullOrWhiteSpace(fornecedor.cep) ? "Não Informado" : fornecedor.cep;
            string logradouro = string.IsNullOrWhiteSpace(fornecedor.logradouro) ? "Não Informado" : fornecedor.logradouro;
            string numero = fornecedor.numLogradouro == 0 ? "Não Informado" : fornecedor.numLogradouro.ToString();
            string bairro = string.IsNullOrWhiteSpace(fornecedor.bairro) ? "Não Informado" : fornecedor.bairro;
            string cidade = string.IsNullOrWhiteSpace(fornecedor.cidade) ? "Não Informado" : fornecedor.cidade;
            string estado = string.IsNullOrWhiteSpace(fornecedor.estado) ? "Não Informado" : fornecedor.estado;
            string complemento = string.IsNullOrWhiteSpace(fornecedor.complemento) ? "Não Informado" : fornecedor.complemento;

            var endereco = $"{cep}, {logradouro}, {numero}, {bairro}, {cidade}, {estado}";

            endereco += complemento != "Não Informado" ? $", {complemento}" : "";

            return endereco;
        }

    }
}