using HydroserverToolsBusinessObjects.Models;
using ODM_1_1_1EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroServerToolsRepository
{
    class DatatablesHelper
    {
    //    public static List<SiteModel> FilterSitesTable(ODM_1_1_1EFModel.ODM_1_1_1Entities context, string searchString, int pageSize)
    //    {
    //        var result = new List<SiteModel>();
            
    //        var filteredItems = context.Sites.
    //                        Where(c =>
    //                                   (c.SiteCode != null && c.SiteCode.ToLower().Contains(searchString))
    //                                || (c.SiteName != null && c.SiteName.ToLower().Contains(searchString))
    //                                || (c.Latitude != null && c.Latitude.ToString().Contains(searchString))
    //                                || (c.Longitude != null && c.Longitude.ToString().Contains(searchString))
    //                               // || (c.LatLongDatumSRSName != null && c.LatLongDatumSRSName.ToLower().Contains(searchString))
    //                                || (c.Elevation_m != null && c.Elevation_m.ToString().Contains(searchString))
    //                                || (c.LocalX != null && c.LocalX.ToString().Contains(searchString))
    //                                || (c.LocalY != null && c.LocalY.ToString().Contains(searchString))
    //                                       //|| c.LocalProjectionID.Contains(searchString)                                   
    //                                || (c.PosAccuracy_m != null && c.PosAccuracy_m.ToString().Contains(searchString))
    //                                || (c.State != null && c.State.ToLower().Contains(searchString))
    //                                || (c.County != null && c.County.ToLower().Contains(searchString))
    //                                || (c.Comments != null && c.Comments.ToLower().Contains(searchString))
    //                                || (c.SiteType != null && c.SiteType.ToLower().Contains(searchString))
    //                                )
    //                                .Take(pageSize)
    //                                .ToList();
    //                    ;
    //        return result;

    //        //if (filteredItems != null)
    //        //{
    //        //    searchRecordCount = filteredItems.Count();
    //        //    items = filteredItems.Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();
    //        //}
    //    }

    //    public static List<SiteModel> SortSitesTable(ODM_1_1_1EFModel.ODM_1_1_1Entities context, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, int startIndex, int pageSize)
    //    {
    //        List<Site> sortedItems = null;
            
    //        foreach (var sortedColumn in sortedColumns)
    //                {
    //                    switch (sortedColumn.PropertyName.ToLower())
    //                    {
    //                        case "0":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "1":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.SiteName).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.SiteName).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "2":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.Latitude).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.Latitude).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "3":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.Longitude).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.Longitude).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "4":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.SpatialReference.SRSName).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.SpatialReference.SRSName).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "5":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.Elevation_m).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.Elevation_m).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "6":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.VerticalDatum).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.VerticalDatum).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "7":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.LocalX).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.LocalX).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "8":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.LocalY).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.LocalY).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "9":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.LocalProjectionID).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.LocalProjectionID).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "10":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.PosAccuracy_m).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.PosAccuracy_m).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "11":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "12":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.County).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.County).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "13":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.Comments).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.Comments).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                        case "14":
    //                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
    //                            { sortedItems = context.Sites.OrderBy(a => a.SiteType).Skip(startIndex).Take(pageSize).ToList(); }
    //                            else
    //                            { sortedItems = context.Sites.OrderByDescending(a => a.SiteType).Skip(startIndex).Take(pageSize).ToList(); }
    //                            break;
    //                    }
    //                }
    //        if (sortedItems == null) sortedItems = context.Sites.OrderByDescending(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList();

    //        return  sortedItems;
    //}
    }
}
