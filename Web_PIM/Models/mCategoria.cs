using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_PIM.Models
{
    public class mCategoria
    {

        public int idCategoria { get; set; }
        public string nmCategoria { get; set; }

        public List<SelectListItem> Categorias { get; set; }
    }
}