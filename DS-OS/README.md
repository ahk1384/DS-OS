# DS-OS: Distributed System Operating System Simulator

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](https://github.com/ahk1384/DS-OS)
[![Tests](https://img.shields.io/badge/tests-141%20passed-success.svg)](https://github.com/ahk1384/DS-OS)

A sophisticated operating system simulator built with .NET 8 that demonstrates core OS concepts including process scheduling, file system management, memory management, and inter-process communication.

## ?? Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Getting Started](#-getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
- [Usage](#-usage)
  - [Commands](#commands)
  - [Examples](#examples)
- [Project Structure](#-project-structure)
- [Testing](#-testing)
- [Core Components](#-core-components)
- [Design Patterns](#-design-patterns)
- [Contributing](#-contributing)
- [Documentation](#-documentation)
- [License](#-license)
- [Authors](#-authors)

## ? Features

### Process Management
- **Priority-based Scheduling**: Max-heap priority queue implementation for efficient process scheduling
- **Process Hierarchy**: Parent-child process relationships with process tree structure
- **Multi-state Processes**: Support for New, Ready, Running, Waiting, and Terminated states
- **Dynamic Priority Adjustment**: Runtime priority recalculation based on execution history
- **File-dependent Processes**: Processes can wait for file availability before execution

### File System
- **Hierarchical Structure**: Unix-like directory tree with root-based navigation
- **File Operations**: Create, delete, move, copy, and rename files and directories
- **Recursive Search**: Deep file system traversal and pattern matching
- **Path Validation**: Comprehensive validation for paths and filenames
- **Size Tracking**: Automatic directory and file size calculation
- **Metadata Management**: Creation time, modification time, and file properties

### Logging & Monitoring
- **Dual Logging System**: Separate loggers for process execution and file system operations
- **Named Pipe Communication**: Real-time log streaming to external monitoring tools
- **Cycle-based Logging**: Detailed execution cycle summaries with process states
- **File System Snapshots**: Complete file tree logging at shutdown

### Command Interface
- **Interactive REPL**: Read-Eval-Print Loop for real-time system interaction
- **Command Parser**: Flexible command syntax with parameter validation
- **Error Handling**: Comprehensive error messages and validation feedback
- **Help System**: Built-in command documentation and usage examples

## ?? Architecture

```
???????????????????????????????????????????????????????????????
?                     DS-OS Architecture                       ?
???????????????????????????????????????????????????????????????
?                                                               ?
?  ????????????????  ????????????????  ????????????????      ?
?  ?   Command    ?  ?   Process    ?  ?     File     ?      ?
?  ?   Handler    ?  ?   Executor   ?  ?   Manager    ?      ?
?  ????????????????  ????????????????  ????????????????      ?
?         ?                  ?                  ?              ?
?         ?                  ?                  ?              ?
?  ??????????????????????????????????????????????????????    ?
?  ?           Database Manager (Core State)            ?    ?
?  ?  ??????????????  ????????????????  ?????????????? ?    ?
?  ?  ?  Process   ?  ?    Ready     ?  ?  Waiting   ? ?    ?
?  ?  ?    Tree    ?  ?    Queue     ?  ?   Queue    ? ?    ?
?  ?  ??????????????  ????????????????  ?????????????? ?    ?
?  ???????????????????????????????????????????????????????    ?
?                          ?                                   ?
?                  ??????????????????                         ?
?                  ?  Logger System  ?                         ?
?                  ??????????????????                         ?
???????????????????????????????????????????????????????????????
```

### Key Components

#### 1. **Process Executor**
- Quantum-based execution cycles
- Time-out handling and context switching
- File dependency checking
- State transition management

#### 2. **Database Manager**
- Centralized process and queue management
- Thread-safe operations with semaphores
- Ready queue with configurable size limits
- Dual waiting queues (file and ready-limit)

#### 3. **File System**
- Tree-based directory structure
- Lock-based thread safety
- Recursive operations support
- Efficient path resolution

#### 4. **Process Manager**
- Process state updates
- File availability monitoring
- Queue management coordination
- Priority recalculation

## ?? Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A code editor (Visual Studio 2022, VS Code, or Rider recommended)
- Windows, macOS, or Linux operating system

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/ahk1384/DS-OS.git
   cd DS-OS
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

### Running the Application

```bash
cd DS-OS
dotnet run
```

You should see:
```
=== DS-OS Starting ===
Loading system settings...
Starting process executor...
=== DS-OS Ready ===
Operating System Simulator is now running.
Type commands to interact with the system.
DS-OS>
```

## ?? Usage

### Commands

#### File System Commands

```bash
# Create a directory
DIRECTORYCREATE(path:/,name:home)
DIRECTORYCREATE(path:/home,name:user1)

# Create a file
FILECREATE(path:/home,name:test.txt)
FILECREATE(path:/home,name:data.bin,size:1024)

# Delete a file or directory
FILEDELETE(path:/home/test.txt)
```

#### Process Commands

```bash
# Create a process without file dependency
PROCESSCREATE(pid:1,parent:0,priority:5,burst:100)

# Create a process with file dependency
PROCESSCREATE(pid:2,parent:0,priority:3,burst:50,filepath:/home/data.txt)

# Delete a process
PROCESSDELETE(pid:1)
```

#### System Commands

```bash
# Start the process executor
start

# Display help
help

# Shutdown the system
SHUTDOWN()
```

### Examples

#### Example 1: Basic File System Operations

```bash
DS-OS> DIRECTORYCREATE(path:/,name:projects)
Directory 'projects' created successfully in '/'
Full path: /projects

DS-OS> DIRECTORYCREATE(path:/projects,name:app1)
Directory 'app1' created successfully in '/projects'

DS-OS> FILECREATE(path:/projects/app1,name:main.cs,size:2048)
File 'main.cs' created successfully in '/projects/app1'

DS-OS> FILECREATE(path:/projects/app1,name:config.json,size:512)
File 'config.json' created successfully in '/projects/app1'
```

#### Example 2: Process Creation and Execution

```bash
DS-OS> start
Process executor started

DS-OS> PROCESSCREATE(pid:10,parent:0,priority:8,burst:50)
Process 10 created successfully

DS-OS> PROCESSCREATE(pid:11,parent:0,priority:5,burst:75,filepath:/projects/app1/main.cs)
Process 11 created successfully
```

#### Example 3: File-Dependent Process

```bash
DS-OS> PROCESSCREATE(pid:20,parent:0,priority:7,burst:100,filepath:/data/input.txt)
Process 20 created successfully

# Process will wait if file doesn't exist
DS-OS> DIRECTORYCREATE(path:/,name:data)
DS-OS> FILECREATE(path:/data,name:input.txt,size:1024)
# Process 20 will now become ready and execute
```

## ?? Project Structure

```
DS-OS/
??? DS-OS/                          # Main application project
?   ??? DataBaseManager/            # Process and queue management
?   ?   ??? DataBaseManager.cs
?   ?   ??? IDataBaseManager.cs
?   ??? DataStructer/               # Core data structures
?   ?   ??? FileTree/
?   ?   ?   ??? FileNode.cs        # File system node
?   ?   ?   ??? FileTree.cs        # File system tree
?   ?   ??? PCB.cs                 # Process Control Block
?   ?   ??? ProcessTree.cs         # Process hierarchy
?   ?   ??? ReadyPriorityQueue.cs  # Priority-based queue
?   ?   ??? WaitingQueue.cs        # Waiting processes
?   ?   ??? State.cs               # Process states
?   ?   ??? WaitReason.cs          # Wait reasons
?   ??? Engine/                     # Execution engine
?   ?   ??? CommandHandler/        # Command processing
?   ?   ?   ??? CommandHandler.cs
?   ?   ?   ??? ICommandHandler.cs
?   ?   ?   ??? BaseClass/
?   ?   ?       ??? Command.cs
?   ?   ?       ??? CommandType.cs
?   ?   ??? ProcessExecutor/       # Process execution
?   ?   ?   ??? ProcessExecutor.cs
?   ?   ?   ??? IProcessExecutor.cs
?   ?   ??? ProcessManager/        # Process state management
?   ?       ??? ProcessManager.cs
?   ?       ??? IProcessManager.cs
?   ??? FileManager/                # File system operations
?   ?   ??? FileManager.cs
?   ?   ??? IFileManager.cs
?   ??? Logger/                     # Logging system
?   ?   ??? ProcessLogger.cs       # Process execution logs
?   ?   ??? FileLogger.cs          # File system logs
?   ?   ??? ILogger.cs
?   ?   ??? LongType.cs            # Log message types
?   ??? Parser/                     # Command parsing
?   ?   ??? QueryParser.cs
?   ?   ??? IQueryParser.cs
?   ??? Exceptions/                 # Custom exceptions
?   ??? ConfigureSettings.cs       # Configuration loader
?   ??? Program.cs                  # Entry point
??? DS-OS-Test/                     # Test project
?   ??? FileManagerTests.cs        # File manager tests (46)
?   ??? LoggerTests.cs             # Logger tests (13)
?   ??? IntegrationTests.cs        # Integration tests
?   ??? FileTreeTests.cs           # File tree tests
?   ??? FileNodeTests.cs           # File node tests
?   ??? PcbTests.cs                # PCB tests
?   ??? ReadyPriorityQueueTests.cs # Queue tests
?   ??? CommandTests.cs            # Command tests
?   ??? TESTING_SUMMARY.md         # Test documentation
??? Logs/                           # External logging service
??? README.md                       # This file
```

## ?? Testing

The project includes a comprehensive test suite with **143 tests** covering all major components.

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~FileManagerTests"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### Test Coverage

| Component | Tests | Coverage |
|-----------|-------|----------|
| FileManager | 46 | 95% |
| Logger | 13 | 90% |
| FileTree | 25 | 100% |
| FileNode | 20 | 100% |
| PCB | 15 | 100% |
| Priority Queue | 12 | 100% |
| Integration | 12 | 85% |

### Test Results

```
Test summary: total: 143; failed: 0; succeeded: 141; skipped: 2
Success Rate: 100%
```

See [TESTING_SUMMARY.md](DS-OS-Test/TESTING_SUMMARY.md) for detailed test documentation.

## ?? Core Components

### Process Control Block (PCB)

```csharp
public class Pcb
{
    public int Pid { get; }
    public string Name { get; }
    public int Priority { get; set; }
    public int RemaningTime { get; set; }
    public State State { get; set; }
    public bool NeedsFile { get; set; }
    public string FilePath { get; set; }
    public WaitReason WaitReason { get; set; }
    public Pcb Parent { get; set; }
    public Dictionary<int, Pcb> Children { get; set; }
}
```

### File Node

```csharp
public class FileNode
{
    public string Name { get; set; }
    public FileNode? Parent { get; set; }
    public bool IsFile { get; set; }
    public bool IsDirectory => !IsFile;
    public Dictionary<string, FileNode> Children { get; set; }
    public string FullPath => GetFullPath();
    public DateTime CreatedTime { get; set; }
    public DateTime ModifiedTime { get; set; }
    public long Size { get; set; }
}
```

### Ready Priority Queue

```csharp
public class ReadyPriorityQueue
{
    public int Count { get; }
    public int MaxSize { get; set; }
    public bool IsEmpty { get; }
    
    public bool Enqueue(Pcb process);
    public Pcb Dequeue();
    public Pcb Peek();
    public bool IsFull();
}
```

## ?? Design Patterns

### 1. **Repository Pattern**
- `IDataBaseManager` abstracts data access
- Centralized state management
- Easy to test and mock

### 2. **Strategy Pattern**
- Command handlers implement different strategies
- Extensible command system
- Clean separation of concerns

### 3. **Factory Pattern**
- `Pcb.Create()` factory methods
- `FileNode.CreateFile()`, `FileNode.CreateDirectory()`
- Consistent object creation

### 4. **Observer Pattern**
- Logger system observes process execution
- File manager notifies process manager of file changes

### 5. **Singleton Pattern**
- Single instance of core managers
- Dependency injection for testability

### 6. **Template Method Pattern**
- Base command processing flow
- Customizable command execution steps

## ?? Contributing

Contributions are welcome! Please follow these guidelines:

### How to Contribute

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/amazing-feature
   ```
3. **Commit your changes**
   ```bash
   git commit -m 'Add some amazing feature'
   ```
4. **Push to the branch**
   ```bash
   git push origin feature/amazing-feature
   ```
5. **Open a Pull Request**

### Coding Standards

- Follow C# coding conventions
- Write unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR
- Use meaningful commit messages

### Areas for Contribution

- ?? Bug fixes
- ? New features
- ?? Documentation improvements
- ?? Additional test coverage
- ?? UI/UX enhancements
- ?? Internationalization

## ?? Documentation

### Configuration File (settings.json)

```json
{
  "readyLimit": 10,
  "quantumSize": 5,
  "executionPerCycle": 3,
  "Prf": 2
}
```

- **readyLimit**: Maximum processes in ready queue
- **quantumSize**: Time quantum for each process (in time units)
- **executionPerCycle**: Number of processes to execute per cycle
- **Prf**: Priority recalculation factor

### Process States

| State | Description |
|-------|-------------|
| `New` | Process just created |
| `Ready` | Process ready to execute |
| `Running` | Process currently executing |
| `Waiting` | Process waiting for resource |
| `Terminated` | Process completed |

### Wait Reasons

| Reason | Description |
|--------|-------------|
| `File` | Waiting for file to be created |
| `ReadyLimit` | Ready queue is full |
| `IO` | Waiting for I/O operation |

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ?? Authors

- **Amir Hossein Khazaei** - *Initial work* - [@ahk1384](https://github.com/ahk1384)

## ?? Acknowledgments

- Inspired by modern operating system concepts
- Built with .NET 8 and C# 12
- Thanks to all contributors and testers

## ?? Contact

- GitHub: [@ahk1384](https://github.com/ahk1384)
- Project Link: [https://github.com/ahk1384/DS-OS](https://github.com/ahk1384/DS-OS)

## ?? Future Enhancements

- [ ] Multi-threading support for parallel process execution
- [ ] Memory management simulation (paging, segmentation)
- [ ] Deadlock detection and prevention
- [ ] GUI interface for visualization
- [ ] Network simulation for distributed processes
- [ ] Performance metrics and statistics dashboard
- [ ] Docker containerization
- [ ] API endpoints for external control
- [ ] Disk scheduling algorithms
- [ ] Virtual memory management

---

<div align="center">

**? Star this repository if you find it helpful!**

Made with ?? by [Amir Hossein Khazaei](https://github.com/ahk1384)

</div>
