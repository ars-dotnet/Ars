namespace ArsMvcApp
{
    public interface IUserAppService
    {
        IEnumerable<string> GetList();
    }

    public abstract class UserBase : IUserAppService
    {
        public virtual IEnumerable<string> GetList()
        {
            yield return "aa";
        }
    }

    public class User : UserBase 
    {
        public override IEnumerable<string> GetList()
        {
            yield return "bb";
        }
    }

    public class Configs 
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
}
