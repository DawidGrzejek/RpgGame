using DesignPatterns.Core.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Core.Models
{
    public class Dog : Animal
    {
        private Dog(string name, double width) : base(name, width)
        {
            MoveBehavior = new GoAroundGapBehavior();
        }

        public static Dog Create(string name, double width)
        {
            return new Dog(name, width);
        }

        public override void MakeSound()
        {
            Console.WriteLine("Woof!");
        }
        public override void Move()
        {
            Console.WriteLine("The dog runs.");
        }
    }
}
