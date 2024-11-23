using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web_PIM.Models
{
    public class mFuncionario
    {
        [DisplayName("Codigo")]
        public int id { get; set; }

        [DisplayName("Nome")]
        public string nmFuncionario { get; set; }

        [DisplayName("E-mail")]
        public string loginFuncionario { get; set; }

        [DisplayName("Senha")]
        public string senhaFuncionario { get; set; }

        [DisplayName("Confirma Senha")]
        public string confSenhaFuncionario { get; set; }

        [DisplayName("Acesso")]
        public string nvlAcesso { get; set; }

        [DisplayName("Cargo")]
        public string cargoFuncionario { get; set; }

        [DisplayName("Status")]
        public string statusFuncionario { get; set; }

        public int idCargo { get; set; }

        public int idNvlAcesso { get; set; }

    }
}
