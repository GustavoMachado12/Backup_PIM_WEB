using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using Web_PIM.Acoes;
using Web_PIM.Models;

namespace Web_PIM.Acao
{
    public class acaoFuncionario
    {
        conexao con = new conexao();

        //CADASTRA
        public void cadastraFuncionario(mFuncionario funcionario)
        {
            SqlCommand cmd = new SqlCommand("pCadastraFuncionario", con.OpenConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Nm_Func", SqlDbType.VarChar).Value = funcionario.nmFuncionario;
            cmd.Parameters.Add("@Email_Func", SqlDbType.VarChar).Value = funcionario.loginFuncionario;
            cmd.Parameters.Add("@Cd_Cargo", SqlDbType.Int).Value = funcionario.idCargo;
            cmd.Parameters.Add("@Cd_TipoUsu", SqlDbType.Int).Value = funcionario.idNvlAcesso;
            cmd.Parameters.Add("@Login", SqlDbType.VarChar).Value = funcionario.loginFuncionario;
            cmd.Parameters.Add("@Senha", SqlDbType.VarChar).Value = funcionario.senhaFuncionario;

            int linhasAfetadas = cmd.ExecuteNonQuery();

            if (linhasAfetadas > 0)
            {
                Console.WriteLine("Funcionario cadastrado com sucesso.");
            }
            else
            {
                Console.WriteLine("Nenhuma alteração foi realizada.");
            }

            con.CloseConnection();
        }

        //CONSULTA FUNCIONARIOS
        public List<mFuncionario> ConsultaFuncionario()
        {
            List<mFuncionario> FuncionarioLista = new List<mFuncionario>();

            SqlCommand cmd = new SqlCommand("EXEC pSelectFuncionario", con.OpenConnection());
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sd.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                FuncionarioLista.Add(
                        new mFuncionario
                        {
                            id = Convert.ToInt32(dr["Codigo"]),
                            nmFuncionario = Convert.ToString(dr["Nome"]),
                            loginFuncionario = Convert.ToString(dr["Login"]),
                            cargoFuncionario = Convert.ToString(dr["Cargo"]),
                            nvlAcesso = Convert.ToString(dr["Tipo"]),
                            statusFuncionario = Convert.ToString(dr["Status"])
                        });
            }
            con.CloseConnection();

            return FuncionarioLista;
        }

        //DELETA FUNCIONARIO
        public bool deletaFuncionario(mFuncionario funcionario)
        {
            bool sucesso = false;
            try
            {
                using (SqlCommand cmd = new SqlCommand("pExcluiFuncionario @CodFuncionario", con.OpenConnection()))
                {
                    cmd.Parameters.AddWithValue("@CodFuncionario", funcionario.id);

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

        //ATUALIZA CLIENTE FISICO
        public mFuncionario atualizaFuncionario(mFuncionario funcionario)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("pAlterarFuncionario", con.OpenConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Cd_Func", SqlDbType.Int).Value = funcionario.id;
                cmd.Parameters.Add("@Nm_Func", SqlDbType.VarChar).Value = funcionario.nmFuncionario;
                cmd.Parameters.Add("@Email_Func", SqlDbType.VarChar).Value = funcionario.loginFuncionario;
                cmd.Parameters.Add("@Cd_Cargo", SqlDbType.Int).Value = funcionario.idCargo;
                cmd.Parameters.Add("@Cd_TipoUsu", SqlDbType.Int).Value = funcionario.idNvlAcesso;
                cmd.Parameters.Add("@Login", SqlDbType.VarChar).Value = funcionario.loginFuncionario;
                cmd.Parameters.Add("@Senha", SqlDbType.VarChar).Value = funcionario.senhaFuncionario;

                int linhasAfetadas = cmd.ExecuteNonQuery();

                if (linhasAfetadas > 0)
                {
                    Console.WriteLine("Funcionário atualizado com sucesso.");
                }
                else
                {
                    Console.WriteLine("Nenhuma alteração foi realizada.");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Erro ao atualizar o funcionário: " + e.Message);
            }
            finally
            {
                con.CloseConnection();
            }

            return funcionario;
        }


        //CONSULTA PELO ID
        public mFuncionario consultaFuncionarioPorId(int id)
        {
            mFuncionario funcionario = null;

            SqlCommand cmd = new SqlCommand("EXEC pSelectFuncionarioPorID @CodFunc", con.OpenConnection());
            cmd.Parameters.AddWithValue("@CodFunc", id);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    funcionario = new mFuncionario
                    {
                        id = Convert.ToInt32(dr["Codigo"]),
                        nmFuncionario = dr["Nome"].ToString(),
                        loginFuncionario = dr["Login"].ToString(),
                        nvlAcesso = dr["Tipo"].ToString(),
                        cargoFuncionario = dr["Cargo"].ToString(),
                        statusFuncionario = dr["Status"].ToString(),
                        senhaFuncionario = dr["Senha"] != DBNull.Value ? dr["Senha"].ToString() : null
                    };
                }
            }

            con.CloseConnection();

            return funcionario;
        }

        public mFuncionario pegaIdNivelAcesso(mFuncionario funcionario)
        {
            try
            {
                SqlCommand cmd_Tipo = new SqlCommand("SELECT cd_TipoUsu FROM tb_TipoUsuario WHERE nm_TipoUsu = @nvlAcesso", con.OpenConnection());
                cmd_Tipo.Parameters.AddWithValue("@nvlAcesso", funcionario.nvlAcesso);
                SqlDataReader dr = cmd_Tipo.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        funcionario.idNvlAcesso = Convert.ToInt32(dr["cd_TipoUsu"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao pegar ID do nível de acesso: {ex.Message}");
            }
            finally
            {
                con.CloseConnection();
            }

            return funcionario;
        }


        public mFuncionario pegaIdCargo(mFuncionario funcionario)
        {
            SqlCommand cmd = new SqlCommand("SELECT cd_Cargo FROM tb_Cargo WHERE nm_Cargo = @nmCargo", con.OpenConnection());
            cmd.Parameters.AddWithValue("@nmCargo", funcionario.cargoFuncionario);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    funcionario.idCargo = Convert.ToInt32(dr["cd_Cargo"]); 
                }
            }

            con.CloseConnection();
            return funcionario;
        }

        public List<mCargos> ConsultaCargos()
        {
            List<mCargos> cargos = new List<mCargos>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM tb_Cargo", con.OpenConnection());
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                cargos.Add(new mCargos
                {
                    idCargo = (int)reader["cd_Cargo"],
                    nmCargo = reader["nm_Cargo"].ToString()
                });
            }

            con.CloseConnection();
            return cargos;
        }

        public List<mNivelAcesso> ConsultaNivelAcesso()
        {
            List<mNivelAcesso> cargos = new List<mNivelAcesso>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM tb_TipoUsuario", con.OpenConnection());
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                cargos.Add(new mNivelAcesso
                {
                    codNivelAcesso = (int)reader["cd_TipoUsu"],
                    nmNivelAcesso = reader["nm_TipoUsu"].ToString()
                });
            }

            con.CloseConnection();
            return cargos;
        }
    }
}