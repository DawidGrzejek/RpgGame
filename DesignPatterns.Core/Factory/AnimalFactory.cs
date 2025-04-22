using DesignPatterns.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Core.Factory
{
    /// <summary>
    /// AnimalFactory is a factory class that creates instances of Animal subclasses.
    /// </summary>
    public class AnimalFactory
    {
        /// <summary>
        /// Creates an instance of an Animal subclass based on the provided animal type.
        /// </summary>
        /// <param name="animalType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Animal CreateAnimal(string animalType, string name, double width)
        {
            switch (animalType.ToLower())
            {
                case "dog":
                    return Dog.Create(name, width);
                case "cat":
                    return Cat.Create(name, width);
                default:
                    throw new ArgumentException("Unknown animal type");
            }
        }
    }
}
