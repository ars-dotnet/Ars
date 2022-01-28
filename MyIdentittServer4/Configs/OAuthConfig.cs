namespace MyIdentittServer4.Configs
{
    public class OAuthConfig
    {
        /// <summary>
        /// token有效时间 过期秒数
        /// </summary>
        public static int ExpireIn = 3600;

        /// <summary>
        /// 跨域地址
        /// </summary>

        public static string[] CorUrls = new[] { "http://*:7207", "http://*:5207" };

        /// <summary>
        /// Api资源名称
        /// </summary>

        public static string ApiName = "user_api";

        /// <summary>
        /// 客户端唯一ID
        /// </summary>

        public static string ClientId = "user_clientid";

        /// <summary>
        /// 密钥
        /// </summary>

        public static string Secret = "user_secret";
    }
}
