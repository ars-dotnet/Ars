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

        public static X509Certificate2 Get(string path, string password) 
        {
            if(path.IsNullOrEmpty())
                throw new ArgumentNullException("path");
            if (password.IsNullOrEmpty())
                throw new ArgumentNullException("password");

            string allpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            return new X509Certificate2(allpath, password);
        }
    }
}
