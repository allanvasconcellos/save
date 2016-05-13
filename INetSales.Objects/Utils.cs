using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace INetSales.Objects
{
    public static class Utils
    {
        /// <summary>
        /// Retorna o nome do metodo chamador.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetCallMethodFullname(int level)
        {
            var st = new StackTrace(true);
            var frame = st.GetFrame(level);
            return frame.GetMethod().DeclaringType.Name + "." + frame.GetMethod().Name;
        }

        /// <summary>
        /// Retorna o numero da linha do metodo chamador.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetCallMethodLine(int level)
        {
            var st = new StackTrace(true);
            var frame = st.GetFrame(level);
            return frame.GetFileLineNumber().ToString();
        }

        public static string GetRandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
            {
                return builder.ToString().ToLower();
            }
            return builder.ToString();
        }

        public static string HashString(string str)
        {
            return str;
        }

        public static bool ValidarCpf(string cpf)
        {
            var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto;
            return cpf.EndsWith(digito);
        }

        public static bool ValidarCnpj(string cnpj)
        {
            var multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            int resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            string digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (var i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto;
            return cnpj.EndsWith(digito);
        }

        public static bool ValidarEmail(string email)
        {
            var rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
            return rg.IsMatch(email);
        }

        public static string ToDescription(this Enum e)
        {
            FieldInfo fi = e.GetType().GetField(e.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : e.ToString();
        }

		public static void InvokeOnSelect<TArgs>(EventHandler<TArgs> handler, object sender, TArgs e)
			where TArgs : EventArgs
		{
			if (handler != null) handler(sender, e);
		}

		public static Stream GetStringStream(string value)
		{
			var buffer = Encoding.UTF8.GetBytes( value );
			return new MemoryStream (buffer);
		}
    }
}