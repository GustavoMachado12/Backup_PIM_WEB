using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_PIM.Models
{
    public class mNivelAcesso
    {

        public int codNivelAcesso { get; set; }
        public string nmNivelAcesso { get; set; }

        public List<SelectListItem> NivelAcessos { get; set; }

    }
}