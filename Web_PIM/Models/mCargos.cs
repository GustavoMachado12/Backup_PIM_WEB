using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_PIM.Models
{
    public class mCargos
    {

        public int idCargo { get; set; }

        public string nmCargo { get; set; }

        public List<SelectListItem> Cargos { get; set; }

    }
}