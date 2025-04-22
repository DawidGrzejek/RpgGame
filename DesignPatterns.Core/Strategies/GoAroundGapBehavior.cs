using DesignPatterns.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Core.Strategies
{
    public class GoAroundGapBehavior : IMoveBehavior
    {
        public void MoveThroughGap(double animalWidth, double gapWidth)
        {
            if (animalWidth > gapWidth)
            {
                Console.WriteLine("The animal goes around the gap.");
            }
            else
            {
                Console.WriteLine("The animal can go through the gap.");
            }
        }
    }
}
