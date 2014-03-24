using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace jQuery.DataTables.Mvc
{
    public static class ControllerExtensions
    {
        public static JsonResult DataTablesJson<T>(this Controller controller, IEnumerable<T> items,
            int totalRecords,
            int totalDisplayRecords,
            int sEcho)
        {
            var result = new JsonResult();
            result.Data = new JQueryDataTablesResponse<T>(items, totalRecords, totalDisplayRecords, sEcho);
            return result;
        }
    }
}
