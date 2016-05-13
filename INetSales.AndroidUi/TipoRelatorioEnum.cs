using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace INetSales.AndroidUi
{
    public enum TipoRelatorioEnum : byte
    {
        Indefinido = 0,
        Analitico,
        Sintetico,
    }

    public enum RelatorioEnum : byte 
    {
        Indefinido = 0,
        QuantidadeValor,
        Financeiro,
    }
}