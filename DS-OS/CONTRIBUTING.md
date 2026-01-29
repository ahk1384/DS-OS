# Contributing to DS-OS

First off, thank you for considering contributing to DS-OS! It's people like you that make DS-OS such a great tool for learning and understanding operating system concepts.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [How Can I Contribute?](#how-can-i-contribute)
  - [Reporting Bugs](#reporting-bugs)
  - [Suggesting Enhancements](#suggesting-enhancements)
  - [Pull Requests](#pull-requests)
- [Development Guidelines](#development-guidelines)
  - [Coding Standards](#coding-standards)
  - [Testing Requirements](#testing-requirements)
  - [Documentation](#documentation)
- [Project Structure](#project-structure)
- [Building and Testing](#building-and-testing)

## Code of Conduct

This project and everyone participating in it is governed by our commitment to fostering an open and welcoming environment. By participating, you are expected to uphold this standard.

### Our Standards

- Using welcoming and inclusive language
- Being respectful of differing viewpoints and experiences
- Gracefully accepting constructive criticism
- Focusing on what is best for the community
- Showing empathy towards other community members

## Getting Started

1. Fork the repository on GitHub
2. Clone your fork locally
   ```bash
   git clone https://github.com/YOUR-USERNAME/DS-OS.git
   cd DS-OS
   ```
3. Create a new branch for your contribution
   ```bash
   git checkout -b feature/your-feature-name
   ```
4. Make your changes
5. Test your changes thoroughly
6. Commit and push your changes
7. Create a Pull Request

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the existing issues to avoid duplicates. When you create a bug report, include as many details as possible:

**Bug Report Template:**

```markdown
**Describe the bug**
A clear and concise description of what the bug is.

**To Reproduce**
Steps to reproduce the behavior:
1. Run command '...'
2. Perform action '...'
3. See error

**Expected behavior**
A clear and concise description of what you expected to happen.

**Screenshots**
If applicable, add screenshots or log outputs.

**Environment:**
 - OS: [e.g. Windows 11, Ubuntu 22.04]
 - .NET Version: [e.g. 8.0.1]
 - Project Version/Commit: [e.g. commit hash or release version]

**Additional context**
Add any other context about the problem here.
```

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion:

**Enhancement Template:**

```markdown
**Is your feature request related to a problem?**
A clear and concise description of what the problem is.

**Describe the solution you'd like**
A clear and concise description of what you want to happen.

**Describe alternatives you've considered**
A clear and concise description of alternative solutions or features.

**Additional context**
Add any other context or screenshots about the feature request.
```

### Pull Requests

1. **Create an Issue First**: For significant changes, create an issue first to discuss the proposed changes
2. **Follow Coding Standards**: Ensure your code follows the project's coding standards
3. **Write Tests**: Add tests for new functionality
4. **Update Documentation**: Update README.md and inline documentation as needed
5. **Keep PRs Focused**: One feature/fix per pull request
6. **Write Clear Commit Messages**: Use descriptive commit messages

**Pull Request Template:**

```markdown
**Description**
Brief description of what this PR does.

**Related Issue**
Fixes #(issue number)

**Type of Change**
- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update

**How Has This Been Tested?**
Describe the tests you ran and how to reproduce them.

**Checklist:**
- [ ] My code follows the style guidelines of this project
- [ ] I have performed a self-review of my own code
- [ ] I have commented my code, particularly in hard-to-understand areas
- [ ] I have made corresponding changes to the documentation
- [ ] My changes generate no new warnings
- [ ] I have added tests that prove my fix is effective or that my feature works
- [ ] New and existing unit tests pass locally with my changes
```

## Development Guidelines

### Coding Standards

#### C# Style Guide

1. **Naming Conventions**
   ```csharp
   // PascalCase for public members, types, and namespaces
   public class ProcessManager { }
   public string ProcessName { get; set; }
   public void ExecuteProcess() { }
   
   // camelCase for private fields with underscore prefix
   private readonly ILogger _logger;
   private int _processCount;
   
   // camelCase for local variables and parameters
   public void CreateProcess(int processId, string name) { }
   ```

2. **Code Organization**
   - Keep classes focused and single-purpose
   - Use regions to organize large classes
   - Maximum method length: ~50 lines
   - Maximum class length: ~500 lines

3. **Comments**
   ```csharp
   /// <summary>
   /// Creates a new process with the specified parameters.
   /// </summary>
   /// <param name="pid">Process ID</param>
   /// <param name="parent">Parent process</param>
   /// <returns>True if process was created successfully</returns>
   public bool CreateProcess(int pid, Pcb parent) { }
   ```

4. **Error Handling**
   ```csharp
   // Use specific exceptions
   throw new ProcessNotFoundException($"Process {pid} not found");
   
   // Always validate input
   if (process == null)
       throw new ArgumentNullException(nameof(process));
   ```

5. **LINQ Usage**
   ```csharp
   // Prefer LINQ for collections
   var readyProcesses = processes
       .Where(p => p.State == State.Ready)
       .OrderByDescending(p => p.Priority)
       .ToList();
   ```

### Testing Requirements

1. **Test Coverage**
   - All new features must have unit tests
   - Aim for >80% code coverage
   - Include edge cases and error scenarios

2. **Test Naming**
   ```csharp
   [Test]
   public void MethodName_StateUnderTest_ExpectedBehavior()
   {
       // Arrange
       // Act
       // Assert
   }
   ```

3. **Test Structure**
   ```csharp
   [TestFixture]
   public class ProcessManagerTests
   {
       private IProcessManager _processManager;
       
       [SetUp]
       public void Setup()
       {
           _processManager = new ProcessManager(...);
       }
       
       [TearDown]
       public void TearDown()
       {
           // Cleanup
       }
       
       [Test]
       public void CreateProcess_ValidInput_ReturnsTrue()
       {
           // Arrange
           var process = Pcb.Create(1, null, 5, 100);
           
           // Act
           var result = _processManager.CreateProcess(process);
           
           // Assert
           Assert.That(result, Is.True);
       }
   }
   ```

### Documentation

1. **Code Documentation**
   - Use XML documentation comments for public APIs
   - Explain complex algorithms with inline comments
   - Document assumptions and limitations

2. **README Updates**
   - Update README.md for new features
   - Add usage examples
   - Update architecture diagrams if needed

3. **API Documentation**
   - Document all public interfaces
   - Include parameter descriptions
   - Provide return value documentation

## Project Structure

```
DS-OS/
??? DS-OS/                    # Main application
?   ??? DataBaseManager/      # State management
?   ??? DataStructer/         # Core data structures
?   ??? Engine/               # Execution components
?   ??? FileManager/          # File system
?   ??? Logger/               # Logging system
?   ??? Parser/               # Command parsing
?   ??? Exceptions/           # Custom exceptions
??? DS-OS-Test/               # Test project
??? Logs/                     # External logging
```

### Adding New Components

1. **Create Interface First**
   ```csharp
   public interface INewComponent
   {
       bool DoSomething(string parameter);
   }
   ```

2. **Implement Interface**
   ```csharp
   public class NewComponent : INewComponent
   {
       public bool DoSomething(string parameter)
       {
           // Implementation
       }
   }
   ```

3. **Add Tests**
   ```csharp
   [TestFixture]
   public class NewComponentTests
   {
       // Test implementation
   }
   ```

4. **Update Documentation**
   - Add to README.md
   - Update architecture section
   - Add usage examples

## Building and Testing

### Build the Project

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Build in Release mode
dotnet build -c Release
```

### Run Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~NewComponentTests"

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Run the Application

```bash
cd DS-OS
dotnet run
```

## Commit Message Guidelines

Use clear and descriptive commit messages:

```
type(scope): subject

body

footer
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

**Examples:**
```
feat(file-manager): add recursive file search functionality

Implement GetAllFilesRecursive method that traverses the entire
directory tree and returns all files with their full paths.

Closes #42

fix(process-executor): correct time quantum calculation

The time quantum was being calculated incorrectly, causing
processes to execute for longer than intended.

Fixes #38

docs(readme): update installation instructions

Add missing step for configuration file setup.
```

## Questions?

Feel free to:
- Open an issue for questions
- Reach out to the maintainers
- Check existing documentation

## Recognition

Contributors will be recognized in:
- README.md contributors section
- Release notes
- Project documentation

Thank you for contributing to DS-OS! ??
