using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleArray
{
    class Program
    {
        static void Main(string[] args)
        {
            int width = 6;
            int columns = 2;
            int blocks = 2;

            int[] source = Enumerable.Range(0, width * columns).ToArray();

            for (int i = blocks - 1; i > -1; i--)
            {
                for (int j = width; j < width * columns; j+=blocks)
                {
                    int copyElement = source[j - width +i];
                    source[j+1-1*i] = copyElement;
                }
            }

            int m = 1;
            foreach(int element in source)
            {
                if (m != 1 & m % blocks == 0)
                {
                    if (m == width)
                    {
                        Console.WriteLine(element);
                    }
                    else
                    {
                        Console.Write($"{element} | ");
                    }

                }
                else
                {
                    Console.Write(element);
                }
                m += 1;
            }
            Console.ReadLine();
        }
    }
}
