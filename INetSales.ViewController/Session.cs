using System;
using INetSales.Objects.Dtos;

namespace INetSales.ViewController
{
    public static class Session
    {
        public static UsuarioDto UsuarioLogado { get; private set; }

        public static UsuarioDto UsuarioAdm { get; private set; }

        public static DateTime HoraLogin { get; private set; }

        public static DateTime HoraLoginAdm { get; private set; }

        public static bool HasLogin { get; private set; }

        public static bool HasLoginAdm { get; private set; }

        public static void RegisterLogin(UsuarioDto usuario, DateTime hora)
        {
            UsuarioLogado = usuario;
            HoraLogin = hora;
            HasLogin = true;
        }

        public static void RegisterLoginAdm(UsuarioDto usuario, DateTime hora)
        {
            UsuarioAdm = usuario;
            HoraLoginAdm = hora;
            HasLoginAdm = true;
        }

        public static void UpdateHoraLoginAdm(DateTime hora)
        {
            HoraLoginAdm = hora;
        }

        public static void UnregisterLoginAdm(UsuarioDto usuario, DateTime hora)
        {
            UsuarioAdm = null;
            HoraLoginAdm = DateTime.MinValue;
            HasLoginAdm = false;
        }
    }
}