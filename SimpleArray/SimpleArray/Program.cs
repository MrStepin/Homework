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
            int height = 2;
            int blocks = 2;

            int[] source = Enumerable.Range(0, width * height).ToArray();


            for (int i = 0; i < height; i++)
            {
                for (int j = width/blocks -1; j > -1; j--)

                {
                    int index = j * blocks + i * width;

                    for (int k = 0; k < blocks; k++)
                    {
                        Console.Write(source[index + k]);
                    }

                    Console.Write(" | ");
                }
                Console.Write(Environment.NewLine);
            }
            Console.ReadLine();
        }
    }
}
