using Microsoft.VisualBasic;
using System;
using System.Data.SqlClient;

namespace Web_PIM.Acoes
{
    public class conexao
    {

        SqlConnection cn =  new SqlConnection("Server=localhost;Database=db_FazendaPIM;Trusted_Connection=True;");
        public static string msg;

        // Método para abrir e retornar a conexão
        public SqlConnection OpenConnection()
        {
            try
            {
                cn.Open();
                Console.WriteLine("Conexão com o banco de dados estabelecida com sucesso.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Erro ao conectar-se ao banco de dados: " + ex.Message);
            }
            return cn;
        }

        // Método para fechar a conexão
        public SqlConnection CloseConnection()
        {
            try
            {
                cn.Close();
            }
            catch (SqlException e) {
                msg = "Erro ao desconectar" + e.Message;
            }

            return cn;
        }

    }
}
