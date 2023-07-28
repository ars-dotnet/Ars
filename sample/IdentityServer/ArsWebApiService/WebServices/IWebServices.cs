using ArsWebApiService.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;

namespace ArsWebApiService.WebServices
{
    [ServiceContract]
    public interface IWebServices
    {
        [OperationContract]
        StudentModel EchoWithGet(string s);

        /// <summary>
        /// 传入对象要报错，全是get请求
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [OperationContract]
        string EchoWithPost(StudentModel s);

        [OperationContract]
        string Publish(string xml);
    }
}
