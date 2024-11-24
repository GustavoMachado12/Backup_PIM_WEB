using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PIM.Models
{
    public class mCarrinho
    {
        public int idProduto { get; set; }

        public string nmProduto {  get; set; }

        public string imgProduto { get; set; }

        public string prcProduto { get; set; }

        public int qtdProduto { get; set; }

        public string vlTotal { get; set; }

    }
}