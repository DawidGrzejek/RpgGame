using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Core.Interface
{
    public interface IAnimal
    {
        /// <summary>
        /// Gets the width of the animal.
        /// </summary>
        /// <returns></returns>
        double GetWidth();

        /// <summary>
        /// Makes a sound specific to the animal.
        /// </summary>
        void MakeSound();

        /// <summary>
        /// Moves the animal in a specific way.
        /// </summary>
        void Move();
    }
}
