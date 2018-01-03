using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using System.Xml;

namespace HydroServerTools.Utilities
{
    public class ServiceRegistrationHelper
    {
        public string GetWebServicesXml(int? codePage = null)   //xmlFileName never referenced...
        {
            Encoding encoding = Encoding.ASCII; //Default to ASCII encoding

            if (null != codePage)
            {
                //Other than 'ASCII' encodings...
                Dictionary<int, Encoding> encodings = new Dictionary<int, Encoding>()
                                                      {
                                                          {Encoding.UTF7.CodePage, Encoding.UTF7},							//UTF-7
														  {Encoding.UTF8.CodePage, Encoding.UTF8},							//UTF-8
														  {Encoding.BigEndianUnicode.CodePage, Encoding.BigEndianUnicode},	//UTF-16
														  {Encoding.UTF32.CodePage, Encoding.UTF32},						//UTF-32
													  };

                foreach (int cp in encodings.Keys)
                {
                    if (cp == codePage)
                    {
                        encoding = encodings[cp];
                        break;
                    }
                }
            }

            HttpWebResponse response = null;

            try
            {
                //get url
                var hisCentralUrl = ConfigurationManager.AppSettings["ServiceUrl1_1_Endpoint"];

                var url = hisCentralUrl + "/GetWaterOneFlowServiceInfo";

                StringBuilder sb = new StringBuilder();

                byte[] buf = new byte[8192];

                //do get request
                HttpWebRequest request = (HttpWebRequest)
                    WebRequest.Create(url);

                //Set cache policy for request - No cache, no store...
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

                //Set Request for automatic decompression
                //Source: http://stackoverflow.com/questions/2815721/net-is-it-possible-to-get-httpwebrequest-to-automatically-decompress-gzipd-re
                //NOTE: 06-Apr-2017 - Causes exception: 'The property cannot be set after writing has started'...
                //request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                response = (HttpWebResponse)
                    request.GetResponse();


                Stream resStream = response.GetResponseStream();

                string tempString = null;
                int count = 0;
                //read the data and print it
                do
                {
                    count = resStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                    {
                        tempString = encoding.GetString(buf, 0, count);

                        sb.Append(tempString);
                    }
                }
                while (count > 0);

                return sb.ToString();

            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        public IEnumerable<WebServiceNode> GetWebServices()
        {
            
            var xmlString = GetWebServicesXml(Encoding.UTF8.CodePage);
            var xmlReaderSettings = new XmlReaderSettings
            {
                CloseInput = true,
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            var result = new List<WebServiceNode>();
            StringReader stringReader = new StringReader(xmlString);

            //using (var reader = XmlReader.Create(WebServicesFilename, xmlReaderSettings))
            using (var reader = XmlReader.Create(stringReader, xmlReaderSettings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (XmlContext.AdvanceReaderPastEmptyElement(reader))
                        {
                            //Empty element - advance and continue...
                            continue;
                        }

                        if (reader.Name == "ServiceInfo")
                        {
                            string desciptionUrl = null;
                            string serviceUrl = null;
                            string title = null;
                            int serviceID = -1;
                            string serviceCode = null;
                            string organization = null;

                            int variables = -1, values = -1, sites = -1;
                            double xmin = double.NaN, xmax = double.NaN, ymin = double.NaN, ymax = double.NaN;

                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ServiceInfo")
                                {
                                    break;
                                }

                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    if (XmlContext.AdvanceReaderPastEmptyElement(reader))
                                    {
                                        //Empty element - advance and continue...
                                        continue;
                                    }

                                    switch (reader.Name)
                                    {
                                        case "Title":
                                            if (!reader.Read()) continue;
                                            title = reader.Value.Trim();
                                            break;
                                        case "ServiceID":
                                            if (!reader.Read()) continue;
                                            serviceID = Convert.ToInt32(reader.Value.Trim());
                                            break;
                                        case "ServiceDescriptionURL":
                                            if (!reader.Read()) continue;
                                            desciptionUrl = reader.Value.Trim();
                                            break;
                                        case "organization":
                                            if (!reader.Read()) continue;
                                            organization = reader.Value.Trim();
                                            break;
                                        case "servURL":
                                            if (!reader.Read()) continue;
                                            serviceUrl = reader.Value.Trim();
                                            break;
                                        case "valuecount":
                                            if (!reader.Read()) continue;
                                            values = Convert.ToInt32(reader.Value.Trim());
                                            break;
                                        case "variablecount":
                                            if (!reader.Read()) continue;
                                            variables = Convert.ToInt32(reader.Value.Trim());
                                            break;
                                        case "sitecount":
                                            if (!reader.Read()) continue;
                                            sites = Convert.ToInt32(reader.Value.Trim());
                                            break;
                                        case "NetworkName":
                                            if (!reader.Read()) continue;
                                            serviceCode = reader.Value.Trim();
                                            break;
                                        case "minx":
                                            if (!reader.Read()) continue;
                                            double.TryParse(reader.Value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture,
                                                            out xmin);
                                            break;
                                        case "maxx":
                                            if (!reader.Read()) continue;
                                            double.TryParse(reader.Value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture,
                                                            out xmax);
                                            break;
                                        case "miny":
                                            if (!reader.Read()) continue;
                                            double.TryParse(reader.Value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture,
                                                            out ymin);
                                            break;
                                        case "maxy":
                                            if (!reader.Read()) continue;
                                            double.TryParse(reader.Value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture,
                                                            out ymax);
                                            break;
                                    }
                                }
                            }

                            var boundingBox = (Box)null;
                            if (!double.IsNaN(xmin) && !double.IsNaN(xmax) && !double.IsNaN(ymin) && !double.IsNaN(ymax))
                                boundingBox = new Box(xmin, xmax, ymin, ymax);

                            var node = new WebServiceNode(title, serviceCode, serviceID, desciptionUrl, serviceUrl, boundingBox, organization, sites, variables, values);
                            result.Add(node);
                        }
                    }
                }
            }

            return result;
        }

    }

    [Serializable]
    public class WebServiceNode
    {
        private WebServiceNode()
        {
        }

        public WebServiceNode(string title, string serviceCode, int serviceID, string descriptionUrl, string serviceUrl,
            Box boundingBox, string organization, long sites, long variables, long values)
        {
            ServiceID = serviceID;
            ServiceCode = serviceCode;
            Title = title;
            DescriptionUrl = descriptionUrl;
            ServiceUrl = serviceUrl;
            ServiceBoundingBox = boundingBox;
            Checked = true;
            Organization = organization;
            Sites = sites;
            Variables = variables;
            Values = values;
        }
        public Box ServiceBoundingBox { get; private set; }
        public int ServiceID { get; private set; }
        public string ServiceCode { get; private set; }
        public string Title { get; private set; }
        public string DescriptionUrl { get; private set; }
        public string ServiceUrl { get; private set; }
        public bool Checked { get; set; }
        public string Organization { get; private set; }
        public long Sites { get; private set; }
        public long Variables { get; private set; }
        public long Values { get; private set; }
    }
       

    [Serializable]
    public class Box
    {
        /// <summary>
        /// Bounding Box for HydroDesktop Search
        /// </summary>
        /// <param name="xMin">minimum x longitude</param>
        /// <param name="xMax">maximum x longitude</param>
        /// <param name="yMin">minimum y latitude</param>
        /// <param name="yMax">maximum y latitude</param>
        public Box(double xMin, double xMax, double yMin, double yMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
        }
        /// <summary>
        /// Minimum X (latitude)
        /// </summary>
        public double XMin { get; private set; }
        /// <summary>
        /// Maximum X (latitude)
        /// </summary>
        public double XMax { get; private set; }
        /// <summary>
        /// Minimum Y (longitude)
        /// </summary>
        public double YMin { get; private set; }
        /// <summary>
        /// Maximum Y (longitude)
        /// </summary>
        public double YMax { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("Point1 (Lng/Lat): {0:N6} {1:N6} " + Environment.NewLine +
                                 "Point2 (Lng/Lat): {2:N6} {3:N6} ", XMin, YMin, YMax, YMax);
        }
    }

    public static class XmlContext
    {
        //Check for empty element - advance input reader to next non-empty content, if indicated...
        public static bool AdvanceReaderPastEmptyElement(XmlReader reader)
        {
            //Check reader... 
            if ((null != reader) &&
                (XmlNodeType.Element == reader.NodeType) && reader.IsEmptyElement)
            {
                //Success - current element is a self-closing, non-content node without an 'EndElement' (e.g., <methodLink />)- 
                //	advance to next content node (which can be an 'EndElement', among others) - return true...
                reader.MoveToContent();
                return true;
            }

            //Reader check NOT successful - DO NOT advance reader - return false
            return false;
        }

    }
}