using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Blogging.Controllers
{
    public class GeneralFns
    {
        /// <summary>
        /// <b>param</b> <c>number</c><br></br>
        /// <b>return</b> <br></br>
        /// <c>
        /// 123        ->  123<br></br>
        /// 1234       ->  1,234<br></br>
        /// 12345      ->  12.35k<br></br>
        /// 123456     ->  123.4k<br></br>
        /// 1234567    ->  1.23M<br></br>
        /// 12345678   ->  12.35M<br></br>
        /// 123456789  ->  123.5M<br></br>
        /// </c>
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string FormatNumber(long num)
        {
            if (num >= 100000000)
            {
                return (num / 1000000D).ToString("0.#M");
            }
            if (num >= 1000000)
            {
                return (num / 1000000D).ToString("0.##M");
            }
            if (num >= 100000)
            {
                return (num / 1000D).ToString("0.#k");
            }
            if (num >= 10000)
            {
                return (num / 1000D).ToString("0.##k");
            }

            return num.ToString("#,0");
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static string TimeAgo(DateTime dt)
        {
            if (dt > DateTime.Now)
                return "about sometime from now";
            TimeSpan span = DateTime.Now - dt;

            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("about {0} {1} ago", years, years == 1 ? "year" : "years");
            }

            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("about {0} {1} ago", months, months == 1 ? "month" : "months");
            }

            if (span.Days > 0)
                return String.Format("about {0} {1} ago", span.Days, span.Days == 1 ? "day" : "days");

            if (span.Hours > 0)
                return String.Format("about {0} {1} ago", span.Hours, span.Hours == 1 ? "hour" : "hours");

            if (span.Minutes > 0)
                return String.Format("about {0} {1} ago", span.Minutes, span.Minutes == 1 ? "minute" : "minutes");

            if (span.Seconds > 5)
                return String.Format("about {0} seconds ago", span.Seconds);

            if (span.Seconds <= 5)
                return "just now";

            return string.Empty;
        }
    }
}