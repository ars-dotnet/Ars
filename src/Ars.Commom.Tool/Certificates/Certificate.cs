using Ars.Commom.Tool.Extension;
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
            string allpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates\\IS4.pfx");
            return new X509Certificate2(allpath, "aabb1212");
        }

        public static X509Certificate2 Get(string certificatePath, string certificatePassWord) 
        {
            if(certificatePath.IsNullOrEmpty())
                throw new ArgumentNullException("certificatePath");
            if (certificatePassWord.IsNullOrEmpty())
                throw new ArgumentNullException("certificatePassWord");

            string allpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, certificatePath);
            return new X509Certificate2(allpath, certificatePassWord);
        }
    }
}
