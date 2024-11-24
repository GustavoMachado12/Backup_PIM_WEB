using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PIM.Models
{
    public class mItensPedidos
    {
        public Guid ItemPedidoID { get; set; }
        public string codProduto { get; set; }
        public string nmProduto { get; set; }
        public decimal vlrProduto { get; set; } 
        public int qtdProduto { get; set; }
        public string imgProduto { get; set; } 
    }

}