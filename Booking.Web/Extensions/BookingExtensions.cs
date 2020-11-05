using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Web.Extensions
{
    public static class BookingExtensions
    {
        public static bool IsAjax(this HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
