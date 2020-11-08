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
            int height = 25;
            int blocks = 2;

            int[] source = Enumerable.Range(0, width * height).ToArray();


            for (int i = 0; i < height; i++)
            {
                for (int j = 0 ; j < width/blocks; j++)

                {

                    for (int k = j+0; k < blocks + j; k++)
                    {
                        int index = j + k + i * width;
                        Console.Write(source[index]);
                    }

                    Console.Write(" | ");
                }
                Console.Write(Environment.NewLine);
            }
            Console.ReadLine();
        }
    }
}
