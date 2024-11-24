using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PIM.Models
{
    public class mCompra
    {
        public string codCompra { get; set; }

        public int codCliente {  get; set; }

        public int codProduto { get; set; }

        public int qtdProdutoPedido { get; set; }

        public decimal vlCompra { get; set; }

        public string tpPagamento { get; set;}

        public List<mItensPedidos> ItensPedido { get; set; } = new List<mItensPedidos>();

        public List<mProduto> Produto = new List<mProduto> ();
    }
}