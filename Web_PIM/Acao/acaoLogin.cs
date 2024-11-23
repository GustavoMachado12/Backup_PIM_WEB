using Web_PIM.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Web_PIM.Acoes
{
    public class acaoLogin
    {
        conexao con = new conexao();


        public mLogin TestaLogin(mLogin cmLogin)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("EXEC pVerificaDadosLogin @login, @senha", con.OpenConnection()))
                {
                    if (string.IsNullOrEmpty(cmLogin.login) || string.IsNullOrEmpty(cmLogin.senha))
                    {
                        throw new ArgumentException("Login e senha não podem ser nulos ou vazios.");
                    }
                    cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = cmLogin.login;
                    cmd.Parameters.Add("@senha", SqlDbType.VarChar).Value = cmLogin.senha;

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            while (r.Read())
                            {
                                cmLogin.name = Convert.ToString(r["Nome"]);
                                cmLogin.nvlAcesso = Convert.ToString(r["Nivel"]);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Erro SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
            finally
            {
                con.CloseConnection();
            }

            return cmLogin;
        }

        public mCliente GetClienteByLogin(mLogin login)
        {
            mCliente cliente = new mCliente();
            SqlCommand cmd = new SqlCommand("EXEC pVerificaDadosLogin @pLogin, @pSenha", con.OpenConnection());
            cmd.Parameters.AddWithValue("@pLogin", login.login);
            cmd.Parameters.AddWithValue("@pSenha", login.senha);

            SqlDataReader r = cmd.ExecuteReader();
            if (r.HasRows)
            {
                while (r.Read())
                {
                    cliente.id = Convert.ToInt32(r["Codigo"]);
                    cliente.nome = Convert.ToString(r["Nome"]);
                }
            }

            con.CloseConnection();
            return cliente;
        }

    }
}
