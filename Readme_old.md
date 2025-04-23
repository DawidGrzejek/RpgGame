# Object-Oriented Programming: The Three Major Pillars

This project demonstrates the implementation of the three major pillars of object-oriented programming (OOP) through a simple animal hierarchy and factory pattern.

## 1. Encapsulation

Encapsulation involves hiding the internal state and functionality of an object and only exposing what is necessary through a well-defined interface. This protects the integrity of the object and controls how its data is accessed or modified.

### Key Concepts of Encapsulation:

- **Data Hiding**: Keeps the internal workings of a class hidden from the outside world using access modifiers like `private` and `protected`.
- **Controlled Access**: Uses public methods (getters and setters) to access and modify private fields, allowing for validation logic.
- **Improved Maintainability**: Makes code easier to manage as changes to internal implementation don't affect external code.

### Example from our project:

The `Animal` abstract class encapsulates the implementation details, while exposing only the necessary methods through the `IAnimal` interface.

```csharp
public abstract class Animal : IAnimal
{
    // Encapsulated implementation details would go here
    public abstract void MakeSound();
    public abstract void Move();
}
```

## 2. Inheritance

Inheritance allows a class to inherit properties and methods from another class, enabling code reuse and establishing an "is-a" relationship between classes.

### Key Concepts of Inheritance:

- **Base and Derived Classes**: A derived class (child) inherits from a base class (parent).
- **Code Reuse**: Avoids duplicating code by inheriting shared functionality.
- **Method Overriding**: Allows derived classes to provide specific implementations of methods defined in the base class.

### Example from our project:

Both `Dog` and `Cat` classes inherit from the `Animal` abstract class:

```csharp
public class Dog : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("Woof!");
    }
    
    public override void Move()
    {
        Console.WriteLine("The dog runs.");
    }
}

public class Cat : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("Meow!");
    }
    
    public override void Move()
    {
        Console.WriteLine("The cat jumps.");
    }
}
```

## 3. Polymorphism

Polymorphism allows you to use a base class or interface to refer to objects of derived classes, letting you write code that works with objects of different classes in a uniform way.

### Key Concepts of Polymorphism:

- **Interface-Based**: Using interfaces to define common behavior across different classes.
- **Runtime Method Resolution**: The appropriate method implementation is determined at runtime.
- **Flexibility**: Code can work with different object types without knowing their specific class.

### Example from our project:

We can use polymorphism with the `IAnimal` interface:

```csharp
public class Program
{
    public static void Main()
    {
        IAnimal myDog = new Dog();
        IAnimal myCat = new Cat();
        
        // Both objects are treated as IAnimal but behave differently
        myDog.MakeSound(); // Outputs: Woof!
        myDog.Move();      // Outputs: The dog runs.
        
        myCat.MakeSound(); // Outputs: Meow!
        myCat.Move();      // Outputs: The cat jumps.
    }
}
```

## Factory Pattern Implementation

This project also demonstrates the Factory Pattern, which is a creational design pattern that provides an interface for creating objects without specifying their concrete classes.

```csharp
public class AnimalFactory
{
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
```

### Usage Example:

```csharp
// Creating animals using the factory
Animal dog = AnimalFactory.CreateAnimal("dog");
Animal cat = AnimalFactory.CreateAnimal("cat");

// Polymorphic behavior
dog.MakeSound(); // Outputs: Woof!
cat.MakeSound(); // Outputs: Meow!
```

This factory pattern implementation demonstrates how encapsulation, inheritance, and polymorphism work together in a practical application.