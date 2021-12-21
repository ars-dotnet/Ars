using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyArsenalWebApi.Controllers
{
    public interface IServices
    {
        int Get(int i);

        int Action(Func<int> action);

        int Action(object o);

        event EventHandler oncpmplte;
    }

    public class Services : IServices
    {
        public event EventHandler oncpmplte;

        public Func<int> @event;

        public int Action(object o) 
        {
            oncpmplte.Invoke(o,null);
            return 1;
        }

        public int Action(Func<int> action)
        {
            @event += action;
            return @event?.Invoke() ?? 0;
        }

        public int Get(int i)
        {
            List<int> a = new List<int>();
            ArrayList b = new ArrayList();
            LinkedList<int> c = new LinkedList<int>();
            Array array = new int[] { 1, 2 };
            var m = array.GetValue(0);

            HashSet<int> h = new HashSet<int>();
            h.Add(1);
            h.Add(1);

            Hashtable ha = new Hashtable();
            ha.Add(1, "21");
            ha.Add("12", "1212");

            return ++i;
        }
    }
}
