using Dashboard.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Dashboard.Common
{
    public static class AppConfiguration
    {
        static AppConfiguration()
        {
            InstanceData = JsonConvert.DeserializeObject<InstanceDataModel>(InstanceDataString);
        }
        public const string FILECREATEDINSTANCEWISE = "filecreatedinstancewise";
        public const string FILECREATEDDEPARTMENTWISE = "filecreateddepartmentwise";
        public const string FILECREATEDSECTIONWISE = "filecreatedsectionwise";
        public const string FILEPENDINGINSTANCEWISE = "filependinginstancewise";
        public const string FILEPENDINGDEPARTMENTWISE = "filependingdepartmentwise";
        public const string FILEPENDINGSECTIONWISE = "filependingsectionwise";
        public const string FILECLOSEDINSTANCEWISE = "fileclosedinstancewise";
        public const string FILESCLOSEDDEPARTMENTWISE = "filescloseddepartmentwise";
        public const string FILECLOSEDSECTIONWISE = "fileclosedsectionwise";
        public const string RECEIPTCREATEDINSTANCEWISE = "receiptcreatedinstancewise";
        public const string RECEIPTCREATEDDEPARTMENTWISE = "receiptcreateddepartmentwise";
        public const string RECEIPTCREATEDSECTIONWISE = "receiptcreatedsectionwise";
        public const string FILECREATEDUSERWISE = "filecreateduserwise";
        public const string InstanceDataString = "{\"Data\":[{\"S No\":\"1\",\"eOffice Instances URL\":\"sr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.14\",\"Instance Name\":\"Southern Railways\"},{\"S No\":\"2\",\"eOffice Instances URL\":\"au.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.24\",\"Instance Name\":\"All Units\"},{\"S No\":\"3\",\"eOffice Instances URL\":\"scr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.6\",\"Instance Name\":\"South Central Railways\"},{\"S No\":\"4\",\"eOffice Instances URL\":\"wr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.37\",\"Instance Name\":\"Western Railways\"},{\"S No\":\"5\",\"eOffice Instances URL\":\"nwr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.42\",\"Instance Name\":\"North Western Railways\"},{\"S No\":\"6\",\"eOffice Instances URL\":\"wcr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.66\",\"Instance Name\":\"West Central Railways\"},{\"S No\":\"7\",\"eOffice Instances URL\":\"swr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.61\",\"Instance Name\":\"South Western Railways\"},{\"S No\":\"8\",\"eOffice Instances URL\":\"ser.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.71\",\"Instance Name\":\"South Eastern Railways\"},{\"S No\":\"9\",\"eOffice Instances URL\":\"cr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.76\",\"Instance Name\":\"Central Railways\"},{\"S No\":\"10\",\"eOffice Instances URL\":\"ner.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.81\",\"Instance Name\":\"North Eastern Railways\"},{\"S No\":\"11\",\"eOffice Instances URL\":\"nfr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.86\",\"Instance Name\":\"North Frontier Railways\"},{\"S No\":\"12\",\"eOffice Instances URL\":\"er.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.91\",\"Instance Name\":\"Eastern Railways\"},{\"S No\":\"13\",\"eOffice Instances URL\":\"secr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.104\",\"Instance Name\":\"South East Central Railways\"},{\"S No\":\"14\",\"eOffice Instances URL\":\"ecor.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.109\",\"Instance Name\":\"East Coast Railways\"},{\"S No\":\"15\",\"eOffice Instances URL\":\"nr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.53\",\"Instance Name\":\"Northern Railways\"},{\"S No\":\"16\",\"eOffice Instances URL\":\"mrk.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.248.114\",\"Instance Name\":\"Metro Kolkata\"},{\"S No\":\"17\",\"eOffice Instances URL\":\"ecr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.224.5\",\"Instance Name\":\"East Central Railways\"},{\"S No\":\"18\",\"eOffice Instances URL\":\"ncr.eoffice.railnet.gov.in\",\"eOffice Instances IP Address\":\"10.0.224.2\",\"Instance Name\":\"North Central Railways\"}]}";
        public static InstanceDataModel InstanceData;
    }
}