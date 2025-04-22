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
        private double _width;

        protected Animal(string name, double width)
        {
            SetName(name);
            SetWidth(width);
        }

        public string Name
        {
            get { return _name; }
            set { SetName(value); }
        }

        public double Width
        {
            get { return _width; }
            set { SetWidth(value); }
        }

        public void SetName(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name), "Name cannot be null");
            if (name.Length < 2)
                throw new ArgumentException("Name must be at least 2 characters long", nameof(name));

            _name = char.ToUpper(name[0]) + name.Substring(1);
        }

        public void SetWidth(double width)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width cannot be negative nor zero");

            _width = width;
        }

        /// <summary>
        /// The behavior of the animal when it moves.
        /// </summary>
        public IMoveBehavior MoveBehavior { get; set; }

        /// <summary>
        /// The width of the animal.
        /// </summary>
        public void PerformMove(double gapWidth)
        {
            if (MoveBehavior == null)
                throw new InvalidOperationException("Move behavior is not set");

            MoveBehavior.MoveThroughGap(GetWidth(), gapWidth);
        }

        /// <inheritdoc/> 
        public double GetWidth()
        {
            return Width;
        }

        /// <inheritdoc/>
        public abstract void MakeSound();

        /// <inheritdoc/>
        public abstract void Move();
    }
}
