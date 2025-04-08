using DesignPatterns.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Core.Models
{
    public abstract class Animal : IAnimal
    {
        public abstract void MakeSound();
        public abstract void Move();
    }
}
