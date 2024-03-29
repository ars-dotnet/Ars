﻿using Ars.Commom.Tool.Extension;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Ars.Commom.Tool.Certificates
{
    public class Certificate
    {
        public static X509Certificate2 Get()
        {
            string allpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates//ars.pfx");
            return new X509Certificate2(allpath, "aabb1212");
        }

        public static X509Certificate2 Get(string certificatePath, string certificatePassWord, ILogger? logger = null) 
        {
            if(certificatePath.IsNullOrEmpty())
                throw new ArgumentNullException("certificatePath");
            if (certificatePassWord.IsNullOrEmpty())
                throw new ArgumentNullException("certificatePassWord");

            string allpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, certificatePath);
            logger?.LogInformation("certificatePath:{0}", allpath);
            return new X509Certificate2(allpath, certificatePassWord);
        }
    }
}
