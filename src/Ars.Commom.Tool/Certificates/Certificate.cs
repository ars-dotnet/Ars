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
        //private const string CertName = "AceLink.Influencer.IdentityServer4.pfx";secret[SVJZ4hfjA1]
        //private const string CertName = "AceLink.Influencer.Ids4.Certificates.IS4.pfx";

        public static X509Certificate2 Get()
        {
            //Assembly assembly = typeof(Certificate).Assembly;
            //using (Stream manifestResourceStream = assembly.GetManifestResourceStream(
            //    string.Concat(new AssemblyName(assembly.FullName!).Name, ".Certificates.IS4.pfx"))!)
            //    return new X509Certificate2(ReadStream(manifestResourceStream), "aabb1212");

            string allpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates\\IS4.pfx");
            return new X509Certificate2(allpath, "aabb1212");
        }

        public static X509Certificate2 Get([NotNull]string path, [NotNull]string password) 
        {
            string allpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            return new X509Certificate2(allpath, password);
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16384];
            using (MemoryStream memoryStream = new MemoryStream())
            {
                int count;
                while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
                    memoryStream.Write(buffer, 0, count);
                return memoryStream.ToArray();
            }
        }
    }
}
