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

namespace INetSales.Objects.Dtos
{
    public class StatusEmissaoPedido
    {
        public bool HasNFeEmitida { get; set; }

        public string CodigoStatusEmissao { get; set; }

        public string DescricaoStatusEmissao { get; set; }
    }
}