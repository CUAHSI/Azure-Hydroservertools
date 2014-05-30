using CsvHelper;
using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace HydroServerTools.Controllers
{
    public class ExportController : Controller
    {
        //
        // GET: /Export/
        public FileStreamResult Download(int identifier, string viewName)
        {
            //add suffix for download depending on tab
            string downloadSuffix = string.Empty;
            switch (identifier)
            {
                case 0:
                    downloadSuffix = "new";
                    break;
                case 1:
                    downloadSuffix = "rejected";
                    break;
                case 2:
                    downloadSuffix = "update";
                    break;
                case 3:
                    downloadSuffix = "duplicate";
                    break;
            }

            var filename = viewName + "_" + downloadSuffix + ".csv";

            if (viewName == "sites")
            {

                var listOfRecords = (List<SiteModel>)BusinessObjectsUtils.GetRecordsFromCache<SiteModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "variables")
            {
                var listOfRecords = (List<VariablesModel>)BusinessObjectsUtils.GetRecordsFromCache<VariablesModel>(MvcApplication.InstanceGuid, identifier, "default");
                var result = WriteCsvToMemory(listOfRecords);

                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "offsettypes")
            {

                var listOfRecords = (List<OffsetTypesModel>)BusinessObjectsUtils.GetRecordsFromCache<OffsetTypesModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "sources")
            {

                var listOfRecords = (List<SourcesModel>)BusinessObjectsUtils.GetRecordsFromCache<SourcesModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "methods")
            {
                var listOfRecords = (List<MethodModel>)BusinessObjectsUtils.GetRecordsFromCache<MethodModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "labmethods")
            {
                var listOfRecords = (List<LabMethodModel>)BusinessObjectsUtils.GetRecordsFromCache<LabMethodModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "samples")
            {
                var listOfRecords = (List<SampleModel>)BusinessObjectsUtils.GetRecordsFromCache<SampleModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "qualifiers")
            {
                var listOfRecords = (List<QualifiersModel>)BusinessObjectsUtils.GetRecordsFromCache<QualifiersModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "qualitycontrollevels")
            {
                var listOfRecords = (List<QualityControlLevelModel>)BusinessObjectsUtils.GetRecordsFromCache<QualityControlLevelModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "datavalues")
            {
                var listOfRecords = (List<DataValuesModel>)BusinessObjectsUtils.GetRecordsFromCache<DataValuesModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "groupdescriptions")
            {
                var listOfRecords = (List<GroupDescriptionModel>)BusinessObjectsUtils.GetRecordsFromCache<GroupDescriptionModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "groups")
            {
                var listOfRecords = (List<GroupsModel>)BusinessObjectsUtils.GetRecordsFromCache<GroupsModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "derivedfrom")
            {
                var listOfRecords = (List<DerivedFromModel>)BusinessObjectsUtils.GetRecordsFromCache<DerivedFromModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }
            if (viewName == "categories")
            {
                var listOfRecords = (List<CategoriesModel>)BusinessObjectsUtils.GetRecordsFromCache<CategoriesModel>(MvcApplication.InstanceGuid, identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = filename };
            }

            return null;
        }

        public ActionResult DownloadTemplate(string id)
        {
                     
                try
                {
                    //var fs = System.IO.File.Open(Server.MapPath("~/Templates/ODMTemplates_" + id));
                    var dir = "~/Templates/";
                    var filename = "ODMTemplate_" + id + ".csv";
                    var filePath = Server.MapPath(dir + filename);
                    if (System.IO.File.Exists(filePath))
                        return base.File(filePath, "text/csv", filename);
                    else
                        return Content("Couldn't find file");
                }
                catch
                {
                   return Content("Couldn't find file");
                }        
               
        }
        public ActionResult DownloadFile(string id)
        {

            try
            {
                //var fs = System.IO.File.Open(Server.MapPath("~/Templates/ODMTemplates_" + id));
                var dir = "~/Templates/";
                var filename = id + ".xlsx";
                var filePath = Server.MapPath(dir + filename);
                if (System.IO.File.Exists(filePath))
                    return base.File(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                else
                    return Content("Couldn't find file");
            }
            catch
            {
                return Content("Couldn't find file");
            }

        }
        public byte[] WriteCsvToMemory<T>(List<T> records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.GetEncoding("iso-8859-1")))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                //Get the properties for type T for the headers
                PropertyInfo[] propInfos = typeof(T).GetProperties();
                //var sb = new StringBuilder();

                //var propInfo =
                //    (from property in typeof(T).GetProperties()
                //     let attributes = property
                //         .GetCustomAttributes(typeof(DisplayAttribute), false)
                //         .OfType<DisplayAttribute>()
                //     where attributes.Any(a => a.Name == "LatLongDatumSRSName")
                //     select property).FirstOrDefault();

                //for (int i = 0; i <= propInfos.Length - 1; i++)
                //{
                //    sb.Append(propInfos[i].Name);
                //    //csvWriter.WriteHeader(typeof(T));
                //    if (i < propInfos.Length - 1)
                //    {
                //        //sb.Append(",");
                //        var displayAttributes = propInfos.GetType().GetCustomAttributes(typeof(DisplayAttribute), true);
                //        if (displayAttributes != null && displayAttributes.Length == 1)
                //        {
                //            var d = ((DisplayAttribute)displayAttributes[0]).Name;
                //        }
                //        //var d = !Attribute.IsDefined(propInfos, typeof(DataAnnotations.DisplayAttribute));
                //        csvWriter.WriteField(propInfos[i].Name);
                //    }

                //}

                foreach (var prop in propInfos)
                {
                    var attribs = prop.GetCustomAttributes();
                    if (attribs.OfType<DisplayAttribute>().Count() == 0)
                    {
                        csvWriter.WriteField(prop.Name);
                    }
                }
                csvWriter.NextRecord();


                //Loop through the collection, then the properties and add the values
                for (int i = 0; i <= records.Count - 1; i++)
                {

                    T item = records[i];
                    //csvWriter..WriteField("SiteCode");
                    for (int j = 0; j <= propInfos.Length - 1; j++)
                    {

                        var attribs = propInfos[j].GetCustomAttributes();
                        if (attribs.OfType<DisplayAttribute>().Count() == 0)
                        {
                            object o = item.GetType().GetProperty(propInfos[j].Name).GetValue(item, null);

                            if (o != null)
                            {
                                string value = o.ToString();

                                csvWriter.WriteField(value);
                            }
                        }
                    }
                    csvWriter.NextRecord();
                }


                //foreach (var item in records)
                //{
                //    foreach (var prop in props)
                //    {
                //        if (prop.DisplayName !="NotVisible")
                //        {
                //            csvWriter.WriteField((prop.Name));
                //        }
                //    }
                //}            
                //csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}