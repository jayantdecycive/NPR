using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares_entity_manager.model;

namespace cfares_entity_manager
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Welcome to entity manager. Enter a command.");
            do{
                string input = Console.ReadLine();
                EntityManager.Command(input);
            }while(AskBool("Continue?"));
            

        }

        static bool AskBool(string txt) {
            Console.WriteLine(txt+" [y/n]");
            string i = Console.ReadLine();
            return i.ToLower().StartsWith("y") || i.ToLower().StartsWith("t");
        }
    }
}
