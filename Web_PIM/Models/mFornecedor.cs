using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Web_PIM.Models
{
    public class mFornecedor
    {
        [DisplayName("Código")]
        public int idFornecedor {  get; set; }

        [DisplayName("Nome")]
        public string nomeFornecedor { get; set; }

        [DisplayName("CNPJ")]
        public string cnpjFornecedor {  get; set; }

        [DisplayName("E-mail")]
        public string emailFornecedor { get; set; }

        [DisplayName("Endereço")]
        public string enderecoFornecedor { get; set; }

        [DisplayName("Telefone")]
        public string telefoneFornecedor { get; set; }

        //DADOS ENDERECO
        [DisplayName("CEP")]
        public string cep { get; set; }

        [DisplayName("Logradouro")]
        public string logradouro { get; set; }

        [DisplayName("Número")]
        public int numLogradouro { get; set; }

        [DisplayName("Bairro")]
        public string bairro { get; set; }

        [DisplayName("Cidade")]
        public string cidade { get; set; }

        [DisplayName("UF")]
        public string estado { get; set; }

        [DisplayName("Complemento")]
        public string complemento { get; set; }

    }
}