using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Web_PIM.Models
{
    public class mProduto
    {

        [DisplayName("Código")]
        public string idProduto { get; set; }

        [DisplayName("Produto")]
        public string nomeProduto { get; set; }

        [DisplayName("Valor")]
        public string valor {  get; set; }

        [DisplayName("Categoria")]
        public string categoria { get; set; }

        [DisplayName("Quantidade")]
        public int quantidade { get; set; } 

        public string status { get; set;}

        [DisplayName("Foto")]
        public string fotoProduto { get; set; }


        public int idCategoria { get; set; }
    }
}