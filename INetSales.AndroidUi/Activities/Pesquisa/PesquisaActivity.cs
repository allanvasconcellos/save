using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views.InputMethods;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.ViewController;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Views;

namespace INetSales.AndroidUi.Activities.Pesquisa
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.PesquisaCategory })]
    public class PesquisaActivity : BaseActivity, IPesquisaView
    {
        private CategoriaPesquisaFragment _produtoPesquisafragment;
        private PerguntasPesquisaFragment _perguntasPesquisaFragment;
        private PrecoPesquisaFragment _precoPesquisaFragment;
        private FragmentAdapter _mAdapter;
        private ViewPager _mPager;
        private TabPageIndicator _mIndicator;
        private PesquisaController _controller;

        protected override void OnBeginView(Bundle bundle)
        {
            ApplicationController.Initialize(this);
        }

        private void FinalizarPesquisaClick(object sender, EventArgs e)
        {
            _controller.FinalizarPesquisa(_perguntasPesquisaFragment.MapRespostas, _produtoPesquisafragment.MapRespostas, _precoPesquisaFragment.MapRespostas);
        }

        #region Implementation of IPesquisaView

        public void Initialize(PesquisaController controller)
        {
            _controller = controller;
            SetContentView(Resource.Layout.Pesquisa);

            var btnFinalizarPesquisa = FindViewById<TextView>(Resource.Id.btnFinalizarPesquisa);
            var btnLimparPesquisa = FindViewById<TextView>(Resource.Id.btnLimparPesquisa);
            btnFinalizarPesquisa.Click += FinalizarPesquisaClick;
            btnLimparPesquisa.Click += LimparPesquisaClick;

            #region Fragments Pesquisa
            CriarFragments();

            #endregion
        }

        private void CriarFragments()
        {
            _produtoPesquisafragment = new CategoriaPesquisaFragment();
            _perguntasPesquisaFragment = new PerguntasPesquisaFragment();
            _precoPesquisaFragment = new PrecoPesquisaFragment();

            string[] tabTitles = { "Perguntas", "Categorias", "Preços" };
            _mAdapter = new FragmentAdapter(SupportFragmentManager, tabTitles);
            _mAdapter.SetCount(tabTitles.Length);

            ////////////
            // Retorna os fragments
            _mAdapter.BindingGetItem = position =>
                                           {
                                               switch (position)
                                               {
                                                   case 0:
                                                       return _perguntasPesquisaFragment;
                                                   case 1:
                                                       return _produtoPesquisafragment;
                                                   case 2:
                                                       return _precoPesquisaFragment;
                                                   default:
                                                       throw new NotImplementedException();
                                               }
                                           };
            ///////
            _mPager = FindViewById<ViewPager>(Resource.Id.pagerPesquisa);
            _mPager.Adapter = _mAdapter;
            _mPager.OffscreenPageLimit = tabTitles.Length - 1;

            _mIndicator = FindViewById<TabPageIndicator>(Resource.Id.indicatorPesquisa);
            ////////
            // 
            _mIndicator.PageSelected += 
                (sender, p) =>
                    {
                        if (CurrentFocus != null)
                        {
                            var inputMethodManager = (InputMethodManager) GetSystemService(InputMethodService);
                            inputMethodManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);
                        }
                    };
            /////////
            _mIndicator.SetViewPager(_mPager);
        }

        private void LimparPesquisaClick(object sender, EventArgs e)
        {
            MakeQuestion("Desejar limpar as respostas?",
                () => // ok
                {
                    CriarFragments();
                    _mAdapter.NotifyDataSetChanged();
                }, () => { });
        }

        #endregion
    }
}