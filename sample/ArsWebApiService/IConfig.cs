namespace ArsWebApiService
{
    public interface IConfig
    {
        int Age { get; }
        AnimalConfig AnimalConfig { get; }
    }

    public class Config : IConfig
    {
        public int Age { get; set; }

        public AnimalConfig AnimalConfig { get; set; }
    }

    public interface IAnimalConfig
    {
        string Name { get; }
    }

    public class AnimalConfig : IAnimalConfig
    {
        public string Name { get; set; }
    }

    public class ConfigOptions
    {
        public IEnumerable<RcsAreaInfo> RcsAreaInfos { get; set; }

        public string Test { get; set; }
    }


    public class RcsAreaInfo
    {
        /// <summary>
        /// 目标道口
        /// </summary>
        public int AllocateNo { get; set; }

        /// <summary>
        /// 载具类型
        /// </summary>
        public int MaterialType { get; set; }

        /// <summary>
        /// 目标编号
        /// </summary>
        public string EndCode { get; set; }

        /// <summary>
        /// 目标类型
        /// 00 站点
        /// 04 区域
        /// </summary>
        public string EndCodeType { get; set; }

        /// <summary>
        /// 任务模板
        /// </summary>
        public string TaskType { get; set; }
    }

}
