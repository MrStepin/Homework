using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> testlist = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            List<int> revertlist = new List<int>() { };

            List<int> revertedByBlockslist = new List<int>() {0,0,0,0,0,0,0,0,0};

            int CountOfElements = testlist.Count;

            for (int i = CountOfElements-1; i > -1; i--)
            {
                revertlist.Add(testlist.ElementAt(i));               
            }

            int l = 0;
            for (int k = 0; k < CountOfElements; k+=3)
            {
                
                for (int j = 2; j > -1; j--)
                {
                    revertedByBlockslist.RemoveAt(k+j);
                    revertedByBlockslist.Insert(k+j, revertlist.ElementAt(l));
                    l += 1;
                }
                
            }

            foreach (int element in revertedByBlockslist)
            {
                testlist.Add(element);
            }

            int m = 1; 
            foreach (int element in testlist)
            {
                if (m != 1 & m % 3 == 0)
                {
                    if ( m == 9)
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
