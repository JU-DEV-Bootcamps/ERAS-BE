# ERAS-BE

# **Code Review Checklist**
- [ ] Have the requirements been met?
- [ ] Is the code easy to read?
- [ ] Do unit tests pass?
- [ ] Is the code formatted correctly?
- [ ] ----------------------------------
- [ ] Does this code make correct use of asynchronous programming constructs, including proper use of await and Task.WhenAll including CancellationTokens?
- [ ] Does the code handle exceptions correctly
- [ ] Is the code subject to concurrency issues? Are shared objects properly protected?
- [ ] There are no complex long boolean expressions (i.e; x = isMatched ? shouldMatch ? doesMatch ? blahBlahBlah).
- [ ] There are no negatively named booleans (i.e; notMatchshould be isMatch and the logical negation operator (!) should be used.
- [ ] Are internal vs private vs public classes and methods used the right way?


# Onion Architecture

![image](https://github.com/user-attachments/assets/14e243c2-9001-4264-be08-df63e398a661)


It is an architecture variant of Clean Architecture that focuses on organizing code into concentric layers, where the core of the system is at the center and the outer layers represent different levels of abstraction and implementation details.

One of the key differences between Onion Architecture and Clean Architecture is how they approach the dependency management problem.

In Onion Architecture, dependencies flow from the outer layers toward the innermost core layer. This means the core layer is entirely decoupled from the outside world and can be tested independently of other components.

Clean Architecture places a particular emphasis on using interfaces to decouple components, allowing components to be easily swapped out or replaced.

Onion Architecture explicitly separates technical concerns from business logic by placing them in the outer layers of the application.

![image](https://github.com/user-attachments/assets/02716456-9eba-4ae0-a050-2ef3906ad95a)

### Onion example folder structure

```
src/
|-- Domain/
|   |-- Entities/
|   |-- Repositories/
|   |-- Services/
|-- Application/
|   |-- UseCases/ 
|   |-- Dtos/
|   |-- Mappers/
|-- Infrastructure/
|   |-- Persistence/
|   |-- Services/
|   |-- External/
|-- Presentation/
|   |-- Controllers/
|   |-- Views/
|   |-- Routes/
```

For this project we're going to use this architecture due to the Modularity, Flexibility and Easy to Learn pros.
