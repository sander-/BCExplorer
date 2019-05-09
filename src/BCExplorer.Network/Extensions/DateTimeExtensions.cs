using System;
using System.Collections.Generic;
using System.Text;

namespace BCExplorer.Network.Extensions
{
    public static class DateTimeExtensions
    {
        private static DateTime _baseDateTime = new DateTime(1970, 1, 1);

        public static DateTime FromUnixDateTime(this uint unixDateTime)
        {
            return _baseDateTime.AddSeconds(unixDateTime);
        }
    }
}
