using Dashboard.Common;
using Dashboard.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Dashboard.Service
{
    public class DashboardService
    {
        public List<DashboardModel> GetDahsboardData(string filter)
        {
            List<DashboardModel> data = new List<DashboardModel>();
            Parallel.ForEach(AppConfiguration.InstanceData.Data, (instance) =>
            {
                data.AddRange(TransformModel(instance.EOfficeInstancesUrl, filter, instance.InstanceName));
            });

            //var totalRow = new DashboardModel();
            //totalRow.InstanceName = "Totals";
            //totalRow.ElectronicFileClosed = data.Where(x => x != null).Sum(x => x.ElectronicFileClosed);
            //totalRow.ElectronicFileCreated = data.Where(x => x != null).Sum(x => x.ElectronicFileCreated);
            //totalRow.ElectronicFilePending = data.Where(x => x != null).Sum(x => x.ElectronicFilePending);
            //totalRow.ElectronicReceiptCreated = data.Where(x => x != null).Sum(x => x.ElectronicReceiptCreated);
            //totalRow.SortOrder = 1;
            //data.Add(totalRow);

            return data;
        }

        public List<DashboardModel> GetInstanceData(string filter, string subdomain)
        {
            var data = TransformModel(subdomain, filter, string.Empty);

            return data;
        }

        public List<DashboardModel> TransformModel(string instanceUrl, string filter, string instanceName)
        {
            var result = new List<DashboardModel>();

            if (filter == "Instance")
            {
                var FILECREATEDINSTANCEWISE = GetApiData(instanceUrl, AppConfiguration.FILECREATEDINSTANCEWISE);
                var FILEPENDINGINSTANCEWISE = GetApiData(instanceUrl, AppConfiguration.FILEPENDINGINSTANCEWISE);
                var FILECLOSEDINSTANCEWISE = GetApiData(instanceUrl, AppConfiguration.FILECLOSEDINSTANCEWISE);
                var RECEIPTCREATEDINSTANCEWISE = GetApiData(instanceUrl, AppConfiguration.RECEIPTCREATEDINSTANCEWISE);
                result = GetAggregateData(FILECREATEDINSTANCEWISE, FILEPENDINGINSTANCEWISE, FILECLOSEDINSTANCEWISE, RECEIPTCREATEDINSTANCEWISE);
            }
            else if (filter == "Department")
            {
                var FILECREATEDDEPARTMENTWISE = GetApiData(instanceUrl, AppConfiguration.FILECREATEDDEPARTMENTWISE);
                var FILEPENDINGDEPARTMENTWISE = GetApiData(instanceUrl, AppConfiguration.FILEPENDINGDEPARTMENTWISE);
                var FILESCLOSEDDEPARTMENTWISE = GetApiData(instanceUrl, AppConfiguration.FILESCLOSEDDEPARTMENTWISE);
                var RECEIPTCREATEDDEPARTMENTWISE = GetApiData(instanceUrl, AppConfiguration.RECEIPTCREATEDDEPARTMENTWISE);
                result = GetAggregateData(FILECREATEDDEPARTMENTWISE, FILEPENDINGDEPARTMENTWISE, FILESCLOSEDDEPARTMENTWISE, RECEIPTCREATEDDEPARTMENTWISE);
            }
            else if (filter == "Section")
            {
                var FILECREATEDSECTIONWISE = GetApiData(instanceUrl, AppConfiguration.FILECREATEDSECTIONWISE);
                var FILEPENDINGSECTIONWISE = GetApiData(instanceUrl, AppConfiguration.FILEPENDINGSECTIONWISE);
                var FILECLOSEDSECTIONWISE = GetApiData(instanceUrl, AppConfiguration.FILECLOSEDSECTIONWISE);
                var RECEIPTCREATEDSECTIONWISE = GetApiData(instanceUrl, AppConfiguration.RECEIPTCREATEDSECTIONWISE);
                result = GetAggregateData(FILECREATEDSECTIONWISE, FILEPENDINGSECTIONWISE, FILECLOSEDSECTIONWISE, RECEIPTCREATEDSECTIONWISE);
            }

            result.ForEach((x) => 
            {
                x.SubDomain = instanceUrl;
                x.InstanceName = instanceName;
            });
            return result;
        }

        public List<DashboardModel> GetAggregateData(List<DashboardModel> fileCreatedData,
            List<DashboardModel> filePendingData,
            List<DashboardModel> fileClosedData,
            List<DashboardModel> recptCreatedData)
        {
            var maxCount = Math.Max(fileCreatedData.Count, Math.Max(filePendingData.Count, Math.Max(fileClosedData.Count, recptCreatedData.Count)));
            List<DashboardModel> result = new List<DashboardModel>();
            var allData = new List<DashboardModel>(fileCreatedData);
            allData.AddRange(filePendingData);
            allData.AddRange(fileClosedData);
            allData.AddRange(recptCreatedData);
            var departmentIds = allData.Select(x => x.Departmentid).Distinct().ToList();
            foreach (var departmentId in departmentIds)
            {
                var data = new DashboardModel
                {
                    Departmentid = departmentId,
                    DepartmentName = allData.Where(x => x.Departmentid == departmentId).FirstOrDefault()?.DepartmentName,
                    ElectronicFileCreated  = fileCreatedData.FirstOrDefault(x => x.Departmentid  == departmentId)?.ElectronicFileCreated ?? 0,
                    ElectronicFileClosed = fileClosedData.FirstOrDefault(x => x.Departmentid == departmentId)?.ElectronicFileClosed,
                    ElectronicFilePending = filePendingData.FirstOrDefault(x => x.Departmentid == departmentId)?.ElectronicFilePending,
                    PhysicalFileCreated = fileCreatedData.FirstOrDefault(x => x.Departmentid == departmentId)?.PhysicalFileCreated,
                    PhysicalFileClosed  = fileClosedData.FirstOrDefault(x => x.Departmentid == departmentId)?.PhysicalFileClosed,
                    PhysicalFilePending = filePendingData.FirstOrDefault(x => x.Departmentid == departmentId)?.PhysicalFilePending,
                    ElectronicReceiptCreated = recptCreatedData.FirstOrDefault(x => x.Departmentid == departmentId)?.ElectronicReceiptCreated,
                    PhysicalReceiptCreated = recptCreatedData.FirstOrDefault(x => x.Departmentid == departmentId)?.PhysicalReceiptCreated
                };
                result.Add(data);
            }

            return result;
        }

        public List<DashboardModel> GetApiData(string instanceUrl, string api)
        {
            try
            {
                RestClient client = new RestClient($"https://{instanceUrl}/eFileServices/rest/xmldataset/efile/");
                RestRequest request = new RestRequest(api, Method.POST);

                var response = client.Execute(request);
                if (response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(response.Content.ToString());

                    string json = JsonConvert.SerializeXmlNode(doc);
                    var finalData = new ApiDataModelMultiple();
                    try
                    {
                        finalData = JsonConvert.DeserializeObject<ApiDataModelMultiple>(json);
                    }
                    catch
                    {
                        var data = JsonConvert.DeserializeObject<ApiDataModelSingle>(json);
                        finalData = new ApiDataModelMultiple
                        {
                            WsResponse = new WsResponse1
                            {
                                Row = new List<Row> { data.WsResponse.Row }
                            }
                        };
                    }

                    List<DashboardModel> result = new List<DashboardModel>();
                    if (finalData.WsResponse != null)
                    {
                        foreach (var row in finalData.WsResponse.Row)
                        {
                            var dashboardModel = new DashboardModel();
                            dashboardModel.Departmentid = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name.ToLowerInvariant() == "departmentid").Text);
                            dashboardModel.DepartmentName = row.Column.FirstOrDefault(x => x.Name.ToLowerInvariant() == "departmentname").Text;
                            switch (api)
                            {
                                case AppConfiguration.FILECREATEDINSTANCEWISE:
                                case AppConfiguration.FILECREATEDDEPARTMENTWISE:
                                case AppConfiguration.FILECREATEDSECTIONWISE:
                                    dashboardModel.ElectronicFileCreated = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name == "ElectronicFileCreated").Text);
                                    dashboardModel.PhysicalFileCreated = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name == "PhysicalFileCreated").Text);
                                    break;
                                case AppConfiguration.FILEPENDINGINSTANCEWISE:
                                case AppConfiguration.FILEPENDINGDEPARTMENTWISE:
                                case AppConfiguration.FILEPENDINGSECTIONWISE:
                                    dashboardModel.ElectronicFilePending = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name == "ElectronicFile").Text);
                                    dashboardModel.PhysicalFilePending = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name == "PhysicalFile").Text);
                                    break;
                                case AppConfiguration.FILECLOSEDINSTANCEWISE:
                                    dashboardModel.ElectronicFileClosed = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name.Contains("ElectronicFile")).Text);
                                    dashboardModel.PhysicalFileClosed = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name.Contains("PhysicalFile")).Text);
                                    break;
                                case AppConfiguration.RECEIPTCREATEDINSTANCEWISE:
                                    dashboardModel.ElectronicReceiptCreated = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name == "ElectronicReceiptCreated").Text);
                                    dashboardModel.PhysicalReceiptCreated = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name == "PhysicalReceiptCreated").Text);
                                    break;
                                case AppConfiguration.RECEIPTCREATEDDEPARTMENTWISE:
                                case AppConfiguration.RECEIPTCREATEDSECTIONWISE:
                                    dashboardModel.ElectronicReceiptCreated = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name == "ElectronicReceipt").Text);
                                    dashboardModel.PhysicalReceiptCreated = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name == "PhysicalReceipt").Text);
                                    break;
                                default:
                                    break;
                            }
                            result.Add(dashboardModel);
                        }
                    }
                    return result;
                }
                return new List<DashboardModel>();
            }
            catch
            {
                return new List<DashboardModel>();
            }
        }
    }
}