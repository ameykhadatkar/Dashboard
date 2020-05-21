﻿using Dashboard.Common;
using Dashboard.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Dashboard.Service
{
    public class DashboardService
    {
        public List<DashboardModel> GetDahsboardData(string filter)
        {
            var data = TransformModel("sr", filter);
            return data;
        }


        public List<DashboardModel> TransformModel(string subdomain, string filter)
        {
            var result = new List<DashboardModel>();

            if (filter == "Instance")
            {
                var FILECREATEDINSTANCEWISE = GetApiData(subdomain, AppConfiguration.FILECREATEDINSTANCEWISE);
                var FILEPENDINGINSTANCEWISE = GetApiData(subdomain, AppConfiguration.FILEPENDINGINSTANCEWISE);
                var FILECLOSEDINSTANCEWISE = GetApiData(subdomain, AppConfiguration.FILECLOSEDINSTANCEWISE);
                var RECEIPTCREATEDINSTANCEWISE = GetApiData(subdomain, AppConfiguration.RECEIPTCREATEDINSTANCEWISE);
                result = GetAggregateData(FILECREATEDINSTANCEWISE, FILEPENDINGINSTANCEWISE, FILECLOSEDINSTANCEWISE, RECEIPTCREATEDINSTANCEWISE);
            }
            else if (filter == "Department")
            {
                var FILECREATEDDEPARTMENTWISE = GetApiData(subdomain, AppConfiguration.FILECREATEDDEPARTMENTWISE);
                var FILEPENDINGDEPARTMENTWISE = GetApiData(subdomain, AppConfiguration.FILEPENDINGDEPARTMENTWISE);
                var FILESCLOSEDDEPARTMENTWISE = GetApiData(subdomain, AppConfiguration.FILESCLOSEDDEPARTMENTWISE);
                var RECEIPTCREATEDDEPARTMENTWISE = GetApiData(subdomain, AppConfiguration.RECEIPTCREATEDDEPARTMENTWISE);
                result = GetAggregateData(FILECREATEDDEPARTMENTWISE, FILEPENDINGDEPARTMENTWISE, FILESCLOSEDDEPARTMENTWISE, RECEIPTCREATEDDEPARTMENTWISE);
            }
            else if (filter == "Section")
            {
                var FILECREATEDSECTIONWISE = GetApiData(subdomain, AppConfiguration.FILECREATEDSECTIONWISE);
                var FILEPENDINGSECTIONWISE = GetApiData(subdomain, AppConfiguration.FILEPENDINGSECTIONWISE);
                var FILECLOSEDSECTIONWISE = GetApiData(subdomain, AppConfiguration.FILECLOSEDSECTIONWISE);
                var RECEIPTCREATEDSECTIONWISE = GetApiData(subdomain, AppConfiguration.RECEIPTCREATEDSECTIONWISE);
                result = GetAggregateData(FILECREATEDSECTIONWISE, FILEPENDINGSECTIONWISE, FILECLOSEDSECTIONWISE, RECEIPTCREATEDSECTIONWISE);
            }

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

        public List<DashboardModel> GetApiData(string subdomain, string api)
        {
            try
            {
                RestClient client = new RestClient($"https://{subdomain}.eoffice.railnet.gov.in/eFileServices/rest/xmldataset/efile/");
                RestRequest request = new RestRequest(api, Method.POST);

                var response = client.Execute(request);
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
                                dashboardModel.ElectronicFileClosed = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name == "ElectronicFile").Text);
                                dashboardModel.PhysicalFileClosed = Convert.ToInt32(row.Column.FirstOrDefault(x => x.Name == "PhysicalFile").Text);
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
            catch
            {
                return null;
            }
        }
    }
}