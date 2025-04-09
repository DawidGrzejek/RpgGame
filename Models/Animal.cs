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
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetName(value); }
        }

        public void SetName(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name), "Name cannot be null");
            if (name.Length < 2)
                throw new ArgumentException("Name must be at least 2 characters long", nameof(name));

            _name = char.ToUpper(name[0]) + name.Substring(1);
        }

        public abstract void MakeSound();
        public abstract void Move();
    }
}
