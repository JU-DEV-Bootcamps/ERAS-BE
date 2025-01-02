# ERAS-BE

# **Code Review Checklist**

- [ ] Have the requirements been met?
- [ ] Is the code easy to read?
- [ ] Do unit tests pass?
- [ ] Is the code formatted correctly?
- [ ] ***
- [ ] Does this code make correct use of asynchronous programming constructs, including proper use of await and Task.WhenAll including CancellationTokens?
- [ ] Does the code handle exceptions correctly
- [ ] Is the code subject to concurrency issues? Are shared objects properly protected?
- [ ] There are no complex long boolean expressions (i.e; x = isMatched ? shouldMatch ? doesMatch ? blahBlahBlah).
- [ ] There are no negatively named booleans (i.e; notMatchshould be isMatch and the logical negation operator (!) should be used.
- [ ] Are internal vs private vs public classes and methods used the right way?

# Onion Architecture

For this project Onion architecture is going to use this architecture due to the Modularity, Flexibility and Easy to Learn pros. And our Domain Entities Will be surveys, variables, componentes and students info. For more info about the architecture https://github.com/JU-DEV-Bootcamps/ERAS/wiki/Hexagonal-and-Onion-Architecture

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

# Run project

Run the project from the command line

```bash
 dotnet run --project src/Presentation
```

> [!NOTE]
> If you are using Visual Studio you can use the Run and Debug feature to run the project. Set as startup project the same path `src/Presentation`.


# Project ORM
For data access and data modeling Entity Framework Core will be used with the "Code First" approach
