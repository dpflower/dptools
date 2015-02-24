using System;

namespace ConsoleApplication
{
    class A
    {

        public A()
        {

            PrintFields();

        }

        public virtual void PrintFields() { }

    }

    class B : A
    {

        int x = 1;

        int y;

        public B()
        {

            y = -1;

        }

        public override void PrintFields()
        {

            Console.WriteLine("x={0},y={1}", x, y);

        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            A a = new B();
            Console.ReadLine();
            return;
            //int b = 100;
            //int a = 40;
            //int c = GCD(a, b);
            //b = b / c;
            //a = a / c;

            //Console.WriteLine("b:{0}", b);
            //Console.WriteLine("a:{0}", a);
            //Console.WriteLine("c:{0}", c);
            //Console.WriteLine("++++++++++++++++++++++++++++++++++++++");
            //int count = 21;
            //for (int i = 0; i < count; i++)
            //{
            //    if (count > 20 && (i % b) < a)
            //    {
            //        continue;
            //    }
            //    Console.WriteLine("i:{0}", i);
            //}

            Console.ReadKey();
        }

        public static int GCD(int a, int b)
        {
            int rel = 0;
            int max = a < b ? a : b;
            for (int i = max; i > 0; i--)
                if (a % i == 0 && b % i == 0)
                {
                    rel = i;
                    break;
                }
            return rel;
        }


    }


    //internal class A
    //{
    //    public static int X;

    //    static A()
    //    {
    //        X = B.Y + 1;
    //    }
    //}

    //internal class B
    //{
    //    public static int Y = A.X + 1;

    //    static B()
    //    {
    //    }

    //    private static void Main()
    //    {
    //        Console.WriteLine("X={0},Y={1}", A.X, B.Y);
    //    }
    //}
}