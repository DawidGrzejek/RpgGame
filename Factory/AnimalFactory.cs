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
        public static Animal CreateAnimal(string animalType)
        {
            switch (animalType.ToLower())
            {
                case "dog":
                    return new Dog();
                case "cat":
                    return new Cat();
                default:
                    throw new ArgumentException("Unknown animal type");
            }
        }
    }
}
