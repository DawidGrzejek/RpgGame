using DesignPatterns.Core.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Core.Models
{
    public class Cat : Animal
    {
        private Cat(string name, double width) : base(name, width)
        {
            MoveBehavior = new GoThroughGapBehavior();
        }

        public static Cat Create(string name, double width)
        {
            return new Cat(name, width);
        }

        public override void MakeSound()
        {
            Console.WriteLine("Meow!");
        }

        public override void Move()
        {
            Console.WriteLine("The cat jumps.");
        }
    }
}
