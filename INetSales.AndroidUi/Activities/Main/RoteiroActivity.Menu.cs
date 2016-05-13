using Android.Content;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects.Dtos;
using Java.IO;

namespace INetSales.AndroidUi.Activities.Main
{
    partial class RoteiroActivity : IMenuActivity
    {
        private TipoMenuEnum _tipoMenu;

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);

            if (_tipoMenu == TipoMenuEnum.Sincronizar)
            {
                menu.Add(Menu.None, 0, 0, "Enviar");
                menu.Add(Menu.None, 1, 1, "Sincronizar");
            }
            else if (_tipoMenu == TipoMenuEnum.Cliente)
            {
                menu.Add(Menu.None, 0, 0, "Novo");
                menu.Add(Menu.None, 1, 1, "Listar");
            }
            else if(_tipoMenu == TipoMenuEnum.RoteiroCliente)
            {
                menu.Add(Menu.None, 0, 0, "Visualizar cliente");
                menu.Add(Menu.None, 1, 1, "Realizar pesquisa");
            }
    }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            if(_tipoMenu == TipoMenuEnum.Cliente)
            {
                if (item.ItemId == 0) // Novo cliente
                {
                    this.LaunchActivity(ActivityFlags.ClienteDetalheCategory);
                    return true;
                }
                this.LaunchActivity(ActivityFlags.ClienteFilterable);
                return true;
            }
            if(_tipoMenu == TipoMenuEnum.RoteiroCliente)
            {
                var info = ((AdapterView.AdapterContextMenuInfo)item.MenuInfo);
                var listView = FindViewById<ListView>(Resource.Id.lvRoteiro);
                var clienteSelecionado = ((DtoAdapter<ClienteDto>)listView.Adapter)[info.Position];
                if (item.ItemId == 0) // Cliente detalhe
                {
                    _controller.SelectClienteDetail(clienteSelecionado);
                    return true;
                }
                _controller.SelectClientePesquisa(clienteSelecionado);
                return true;
            }
            return false;
        }

        public void ExecuteInstall(string packagePath)
        {
            var intent = new Intent(Intent.ActionView);
            var file = new File(packagePath);
            intent.SetDataAndType(Android.Net.Uri.FromFile(file), "application/vnd.android.package-archive");
            StartActivity(intent);
        }

        #region Implementation of IMenuActivity

        public void SetTipoMenu(TipoMenuEnum tipo)
        {
            _tipoMenu = tipo;
        }

        #endregion
    }
}