using ArsWebApiService.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

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
    }
}
