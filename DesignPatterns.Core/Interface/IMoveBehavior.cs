using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Core.Interface
{
    public interface IMoveBehavior
    {
        void MoveThroughGap(double animalWidth, double gapWidth);
    }
}
