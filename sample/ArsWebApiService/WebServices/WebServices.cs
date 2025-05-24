using ArsWebApiService.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel;
using System.Xml;
using Topro.CombRetLine.HttpApi.Contract.MesXmlModel;

namespace ArsWebApiService.WebServices
{
    public class WebServices : IWebServices
    {
        public StudentModel EchoWithGet(string s)
        {
            return new StudentModel { Id = 1, No = "123" };
        }

        public string EchoWithPost(StudentModel s)
        {
            return "a";
        }

        public string Publish(string xml)
        {
            return "b";
        }

        public CallMesXmlOutput TestCallMes(string input)
        {
            return new CallMesXmlOutput
            {
                ErrorCode = "1"
            };
        }

        public CallMesXmlOutput TestCallMesXml(string xml) 
        {
            return new CallMesXmlOutput
            {
                ErrorCode = "0",
            };
        }
    }
}
