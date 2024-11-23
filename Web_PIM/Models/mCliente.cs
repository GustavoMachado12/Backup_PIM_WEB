using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web_PIM.Models
{
    public class mCliente
    {
        [DisplayName("Código")]
        public int id { get; set; }

        [DisplayName("Nome")]
        public string nome { get; set; }

        [DisplayName("E-mail")]
        public string email { get; set;}

        [DisplayName("Endereço")]
        public string endereco { get; set;}

        [DisplayName("Telefone")]
        public string telefone { get; set;}

        [DisplayName("Documento")]
        public string documento { get; set;}


        //DADOS LOGIN
        [DisplayName("Login")]
        public string login { get; set;}

        [DisplayName("Senha")]
        public string senha { get; set;}

        [DisplayName("Confirma Senha")]
        public string confirmaSenha { get; set;}


        //DADOS ENDERECO
        public string cep { get; set;}

        public string logradouro { get; set;}

        public int numLogradouro { get; set;}
        
        public string bairro { get; set;}

        public string cidade { get; set;}

        public string estado { get; set;}

        public string complemento { get; set;}


    }
}
