#Explaining three major pillars which object-oriented programming relies

#Interfaces
An interface is a contract that defines a set of methods and properties that a class must implement. Interfaces do not contain any implementation; they only specify what methods and properties a class should have.
Here's a simple example of an interface: public interface IAnimal { void MakeSound(); void Move(); }

Implementing an Interface A class that implements an interface must provide the implementation for all the methods and properties defined in the interface. public class Dog : IAnimal { public void MakeSound() { Console.WriteLine("Woof!"); } public void Move() { Console.WriteLine("The dog runs."); } } public class Cat : IAnimal { public void MakeSound() { Console.WriteLine("Meow!"); } public void Move() { Console.WriteLine("The cat jumps."); } } Polymorphism Polymorphism allows you to use a base class or interface to refer to objects of derived classes. This means you can write code that works with objects of different classes in a uniform way. Here's how you can use polymorphism with the IAnimal interface: public class Program { public static void Main() { IAnimal myDog = new Dog(); IAnimal myCat = new Cat(); myDog.MakeSound(); // Outputs: Woof! myDog.Move(); // Outputs: The dog runs. myCat.MakeSound(); // Outputs: Meow! myCat.Move(); // Outputs: The cat jumps. } } In this example, myDog and myCat are both of type IAnimal, but they refer to instances of Dog and Cat, respectively. This allows you to call MakeSound and Move on both objects, even though the actual implementation of these methods is different for each class.

Encapsulation is one of the core principles of object-oriented programming. It involves hiding the internal state and functionality of an object and only exposing what is necessary through a well-defined interface. The goal is to protect the integrity of the object and control how its data is accessed or modified.

Key Ideas Behind Encapsulation:
Data Hiding: Encapsulation keeps the internal workings of a class hidden from the outside world. This is typically achieved using access modifiers like private and protected. Only the necessary methods and properties are exposed via public access.

Controlled Access: Public methods (sometimes called "getter" and "setter" methods) are used to access and modify private fields. This allows validation or other logic to be added when data is being read or modified.

Improved Maintainability: Encapsulation makes code easier to manage, as changes to the internal implementation don't affect other parts of the program that rely on the object's interface.