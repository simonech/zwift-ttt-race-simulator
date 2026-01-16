# Coding guidelens for Zwift TTT race simulator

---
description: This file provides coding guidelines for agents contributing to the Zwift TTT race simulator project.
applyTo: **/*.cs
---

When contributing to the Zwift TTT race simulator project, please adhere to the following coding guidelines: 

1. **Naming Conventions**:
   - Use PascalCase for class names, method names, and properties.
   - Use camelCase for local variables and method parameters.
   - Use meaningful and descriptive names that convey the purpose of the variable, method, or class.
2. **Code Structure**:
   - Organize code into appropriate namespaces that reflect the project structure.
   - Group related methods and properties together within classes.
   - Keep methods focused on a single task; if a method exceeds 20 lines, consider refactoring it into smaller methods.
   - **One class per file**: Never put multiple classes in a single file. Each class must have its own file with a name matching the class name (e.g., `RiderData.cs` for the `RiderData` class, `RiderDataTests.cs` for the `RiderDataTests` test class).
   - Never use more than one namespace per file.
   - Never use regions to collapse code.
   - Use file scoped namespaces. Never use block scoped namespaces.
3. **Comments and Documentation**:
   - Use XML documentation comments for public methods and classes to describe their purpose, parameters, and return values.
   - Add inline comments to explain complex logic or decisions in the code.
   - Avoid redundant comments that do not add value or explain the code.
4. **Error Handling**:
   - Use exceptions for error handling and provide meaningful error messages.
   - Avoid using generic exceptions; create custom exception classes when necessary.
5. **Testing**:
   - Tests are writtien using xUnit framework.
   - Write unit tests for all public methods and critical logic paths.
   - Use descriptive names for test methods that indicate the scenario being tested.
   - Ensure tests are independent and can be run in any order.
6. **Performance**:
   - Optimize code for performance where necessary, especially in simulation logic.
   - Avoid premature optimization; focus on readability and maintainability first. 
7. **Version Control**:
   - Commit changes with clear and descriptive messages.
   - Follow branching strategies that facilitate collaboration and code review.


By following these guidelines, contributors can help maintain a high standard of code quality and ensure the project remains maintainable and extensible.