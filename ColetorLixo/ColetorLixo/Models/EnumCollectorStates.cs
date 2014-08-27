using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ColetorLixo.Models
{
    public enum EnumCollectorStates
    {
        LIMPAR,
        CARREGAR,
        ESVAZIAR,
        VAZIO,
        PARADO
    }
}
