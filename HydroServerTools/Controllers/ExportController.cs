using CsvHelper;
using HydroServerTools.Helper;
using HydroServerTools.Models;
using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (viewName == "sites")
            {

                var listOfRecords = (List<SiteModel>)Utils.GetRecordsFromCache<SiteModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "variables")
            {
                var listOfRecords = (List<VariablesModel>)Utils.GetRecordsFromCache<VariablesModel>(identifier, "default");
                var result = WriteCsvToMemory(listOfRecords);

                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "offsettypes")
            {

                var listOfRecords = (List<OffsetTypesModel>)Utils.GetRecordsFromCache<OffsetTypesModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "sources")
            {

                var listOfRecords = (List<SourcesModel>)Utils.GetRecordsFromCache<SourcesModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "methods")
            {
                var listOfRecords = (List<MethodModel>)Utils.GetRecordsFromCache<MethodModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "labmethods")
            {
                var listOfRecords = (List<LabMethodModel>)Utils.GetRecordsFromCache<LabMethodModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "samples")
            {
                var listOfRecords = (List<SampleModel>)Utils.GetRecordsFromCache<SampleModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "qualifiers")
            {
                var listOfRecords = (List<QualifiersModel>)Utils.GetRecordsFromCache<QualifiersModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "qualitycontrollevels")
            {
                var listOfRecords = (List<QualityControlLevelModel>)Utils.GetRecordsFromCache<QualityControlLevelModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "datavalues")
            {
                var listOfRecords = (List<DataValuesModel>)Utils.GetRecordsFromCache<DataValuesModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "groupdescriptions")
            {
                var listOfRecords = (List<GroupDescriptionModel>)Utils.GetRecordsFromCache<GroupDescriptionModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "groups")
            {
                var listOfRecords = (List<GroupsModel>)Utils.GetRecordsFromCache<GroupsModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "derivedfrom")
            {
                var listOfRecords = (List<DerivedFromModel>)Utils.GetRecordsFromCache<DerivedFromModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
            }
            if (viewName == "categories")
            {
                var listOfRecords = (List<CategoriesModel>)Utils.GetRecordsFromCache<CategoriesModel>(identifier, "default");

                var result = WriteCsvToMemory(listOfRecords);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "Sites.csv" };
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

        public byte[] WriteCsvToMemory<T>(List<T> records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.GetEncoding("iso-8859-1")))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}