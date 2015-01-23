using System;
using System.Globalization;

namespace BillyCMD.Data
{
    public static class FormatExtentions
    {
        static readonly CultureInfo En;
        static readonly CultureInfo Ua;

        static readonly NumberFormatInfo EnUsd;
        static readonly NumberFormatInfo UaUsd;

        static FormatExtentions()
        {
            En = CultureInfo.GetCultureInfo("en-gb");
            Ua = CultureInfo.GetCultureInfo("uk-ua");

            EnUsd = (NumberFormatInfo)En.NumberFormat.Clone();
            UaUsd = (NumberFormatInfo)Ua.NumberFormat.Clone();

            EnUsd.CurrencySymbol = "USD ";
            UaUsd.CurrencySymbol = " дол. США";
        }

        public static string ToLongDateForUa(this DateTime date)
        {
            return date.ToString("D", Ua);
        }

        public static string ToLongDateForEn(this DateTime date)
        {
            return date.ToString("D", En);
        }

        public static string ToShortDateForUa(this DateTime date)
        {
            return date.ToString("d", Ua);
        }

        public static string ToShortDateForEn(this DateTime date)
        {
            return date.ToString("d", En);
        }

        public static string ToShortDatesForUa(this Period period)
        {
            return period.From.ToShortDateForUa() + "-" + period.To.ToShortDateForUa();
        }

        public static string ToShortDatesForEn(this Period period)
        {
            return period.From.ToShortDateForEn() + "-" + period.To.ToShortDateForEn();
        }

        public static string AsUSDForUa(this decimal amount)
        {
            return amount.ToString("C", UaUsd);
        }

        public static string AsUSDForEn(this decimal amount)
        {
            return amount.ToString("C", EnUsd);
        }
    }
}