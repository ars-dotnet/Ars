using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionDemo
{
    class A { }

    class AA : A { }

    public class M 
    {
        IEnumerable<A> aa = new List<AA>();
    }

    interface IToo<T> 
    {
        void Get(T t);
    }

    class Too<T> : IToo<string>
    {
        public void Get(string t)
        {
            throw new NotImplementedException();
        }
    }

    interface IFoo 
    {
        void InterfaceMenthod();
    }

    interface IFoo<InterfaceT> : IFoo
    {
        void InterfaceMenthod(InterfaceT interfaceT);
    }

    class Foo<ClassT, ClassT1> : IFoo<ClassT>
    {
        public ClassT1 Field;

        public delegate void MyDelegate<DelegateT>(DelegateT delegateT);

        public void DelegateMenthod<DelegateT>(DelegateT delegateT, MyDelegate<DelegateT> myDelegate)
        {
            myDelegate(delegateT);
        }

        public static string operator +(Foo<ClassT, ClassT1> foo, string s)
        {
            return $"{s}:{foo.GetType().Name}";
        }


        public List<ClassT> Property { get; set; }
        public ClassT1 Property1 { get; set; }

        public ClassT this[int index] => Property[index];//没判断越界


        public Foo(List<ClassT> classT, ClassT1 classT1)
        {
            Property = classT;
            Property1 = classT1;
            Field = classT1;
            Console.WriteLine($"构造函数:parameter1 type:{Property.GetType().Name}，parameter2 type:{Property1.GetType().Name}");
        }

        //方法声明了多个新的类型参数
        public void Method<MenthodT, MenthodT1>(MenthodT menthodT, MenthodT1 menthodT1)
        {
            Console.WriteLine($"Method<MenthodT, MenthodT1>:{(menthodT.GetType().Name)}:{menthodT.ToString()}," +
             $"{menthodT1.GetType().Name}:{menthodT1.ToString()}");
        }

        public void Method(ClassT classT)
        {
            Console.WriteLine($"{nameof(Method)}:{classT.GetType().Name}:classT?.ToString()");
        }

        public void InterfaceMenthod(StringBuilder interfaceT)
        {
            Console.WriteLine(interfaceT.ToString());
        }

        public void InterfaceMenthod(ClassT interfaceT)
        {
            throw new NotImplementedException();
        }

        public void InterfaceMenthod()
        {
            
        }
    }
}
