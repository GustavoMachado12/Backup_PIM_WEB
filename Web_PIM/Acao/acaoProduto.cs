using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using Web_PIM.Models;
using Web_PIM.Acoes;

namespace Web_PIM.Acao
{
    public class acaoProduto
    {
        conexao con = new conexao();

        public bool CadastraProduto(mProduto produto)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("pCadastraProduto", con.OpenConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@nmProduto", produto.nomeProduto);
                cmd.Parameters.AddWithValue("@vl_Produto", produto.valor);
                cmd.Parameters.AddWithValue("@cd_Categoria", produto.categoria);
                cmd.Parameters.AddWithValue("@qt_Produto", produto.quantidade);
                cmd.Parameters.AddWithValue("@ft_Produto", produto.fotoProduto);

                int rowsAffected = cmd.ExecuteNonQuery();

                con.CloseConnection();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao cadastrar produto: {ex.Message}");

                con.CloseConnection();
                return false;
            }
            finally { con.CloseConnection(); }
        }

        public List<mProduto> PegaTodosProdutos()
        {
            List<mProduto> ProdutoLista = new List<mProduto>();

            SqlCommand cmd = new SqlCommand("EXEC pSelectProduto", con.OpenConnection());
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sd.Fill(dt);
            con.CloseConnection();

            foreach (DataRow dr in dt.Rows)
            {
                ProdutoLista.Add(
                        new mProduto
                        {
                            idProduto = Convert.ToString(dr["Codigo"]),
                            nomeProduto = Convert.ToString(dr["Nome"]),
                            categoria = Convert.ToString(dr["Categoria"]),
                            valor = Convert.ToString(dr["Valor"]),
                            quantidade = Convert.ToInt32(dr["Quantidade"]),
                            fotoProduto = Convert.ToString(dr["Foto"])
                        });
            }
            return ProdutoLista;
        }

        //CONSULTA PRODUTO
        public List<mProduto> consultaProduto()
        {
            List<mProduto> ProdutoLista = new List<mProduto>();

            SqlCommand cmd = new SqlCommand("EXEC pSelectProduto", con.OpenConnection());
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sd.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                ProdutoLista.Add(
                    new mProduto
                    {
                        idProduto = Convert.ToString(dr["Codigo"]),
                        nomeProduto = Convert.ToString(dr["Nome"]),
                        categoria = Convert.ToString(dr["Categoria"]),
                        valor = Convert.ToString(dr["Valor"]),
                        quantidade = Convert.ToInt32(dr["Quantidade"]),
                        fotoProduto = Convert.ToString(dr["Foto"])
                    });
            }
            con.CloseConnection();

            return ProdutoLista;
        }

        //DELETA PRODUTO
        public bool deletaProduto(int idProduto)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("EXEC pDeletaProduto @CodProduto", con.OpenConnection());
                cmd.Parameters.AddWithValue("@CodProduto", idProduto);
                cmd.ExecuteNonQuery();
                con.CloseConnection();
                return true;
            }
            catch (Exception ex)
            {
                // Log ou tratamento de erro
                con.CloseConnection();
                return false;
            }
        }

        //ATUALIZA PRODUTO
        public bool atualizaProduto(mProduto produto)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("EXEC pAlteraProduto @CodProd, @NomeProd, @Categoria, @Quantidade, @Valor, @Ft_Prod", con.OpenConnection());
                cmd.Parameters.AddWithValue("@CodProd", produto.idProduto);
                cmd.Parameters.AddWithValue("@NomeProd", produto.nomeProduto);
                cmd.Parameters.AddWithValue("@Categoria", produto.idCategoria);
                cmd.Parameters.AddWithValue("@Quantidade", produto.quantidade);
                cmd.Parameters.AddWithValue("@Valor", produto.valor);
                cmd.Parameters.AddWithValue("@Ft_Prod", produto.fotoProduto);
                cmd.ExecuteNonQuery();
                con.CloseConnection();
                return true;
            }
            catch (Exception ex)
            {
                con.CloseConnection();
                return false;
            }
            finally { con.CloseConnection(); }
        }

        //CONSULTA POR ID 
        public mProduto consultaProdutoPorId(int idProduto)
        {
            mProduto produto = null;

            try
            {
                SqlCommand cmd = new SqlCommand("EXEC pSelectProdutoPorID @CodProduto", con.OpenConnection());
                cmd.Parameters.AddWithValue("@CodProduto", idProduto);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    produto = new mProduto
                    {
                        idProduto = reader["Codigo"].ToString(),
                        nomeProduto = reader["Nome"].ToString(),
                        categoria = reader["Categoria"].ToString(),
                        valor = reader["Valor"].ToString(),
                        quantidade = Convert.ToInt32(reader["Quantidade"]),
                        fotoProduto = reader["Foto"].ToString()
                    };
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                con.CloseConnection();
            }
            pegaIdCategoria(produto);

            return produto;
        }

        public List<mCategoria> ConsultaCategorias()
        {
            List<mCategoria> categorias = new List<mCategoria>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM tb_Categoria", con.OpenConnection());
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                categorias.Add(new mCategoria
                {
                    idCategoria = (int)reader["cd_Categoria"],
                    nmCategoria = reader["nm_Categoria"].ToString()
                });
            }

            con.CloseConnection();
            return categorias;
        }

        public mProduto pegaIdCategoria(mProduto produto)
        {
            if (string.IsNullOrEmpty(produto.categoria))
            {
                throw new ArgumentException("Categoria não pode ser nula ou vazia.");
            }

            using (SqlCommand cmd = new SqlCommand("SELECT cd_Categoria FROM tb_Categoria WHERE nm_Categoria = @nmCategoria", con.OpenConnection()))
            {
                cmd.Parameters.AddWithValue("@nmCategoria", produto.categoria);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            produto.idCategoria = Convert.ToInt32(dr["cd_Categoria"]);
                        }
                    }
                    else
                    {
                        throw new Exception($"Categoria {produto.categoria} não encontrada.");
                    }
                }
            }

            return produto;
        }


    }
}