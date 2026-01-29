# Quick Start Guide

Get DS-OS up and running in less than 5 minutes!

## ? Quick Install

```bash
# Clone the repository
git clone https://github.com/ahk1384/DS-OS.git
cd DS-OS

# Build and run
dotnet build
cd DS-OS
dotnet run
```

## ?? Your First Session

### 1. Start the System

```bash
DS-OS> start
Process executor started
```

### 2. Create Your First Directory

```bash
DS-OS> DIRECTORYCREATE(path:/,name:workspace)
Directory 'workspace' created successfully in '/'
Full path: /workspace
```

### 3. Create a File

```bash
DS-OS> FILECREATE(path:/workspace,name:hello.txt,size:256)
File 'hello.txt' created successfully in '/workspace'
```

### 4. Create Your First Process

```bash
DS-OS> PROCESSCREATE(pid:100,parent:0,priority:5,burst:50)
Process 100 created successfully
```

### 5. Watch It Execute!

The process will automatically execute based on the configured quantum size. Watch the console for execution logs.

## ?? Example Workflow

Here's a complete example demonstrating file-dependent processes:

```bash
# Start the executor
DS-OS> start

# Create project directory structure
DS-OS> DIRECTORYCREATE(path:/,name:project)
DS-OS> DIRECTORYCREATE(path:/project,name:data)
DS-OS> DIRECTORYCREATE(path:/project,name:output)

# Create data file
DS-OS> FILECREATE(path:/project/data,name:input.dat,size:1024)

# Create a process that depends on the data file
DS-OS> PROCESSCREATE(pid:1,parent:0,priority:8,burst:100,filepath:/project/data/input.dat)
Process 1 created successfully
# Process immediately becomes ready because file exists!

# Create another process without file dependency
DS-OS> PROCESSCREATE(pid:2,parent:0,priority:5,burst:75)
Process 2 created successfully

# Both processes will execute based on priority
# Process 1 (priority 8) will execute before Process 2 (priority 5)
```

## ?? Configuration

Edit `settings.json` to customize behavior:

```json
{
  "readyLimit": 10,
  "quantumSize": 5,
  "executionPerCycle": 3,
  "Prf": 2
}
```

- **readyLimit**: Maximum processes in ready queue (default: 10)
- **quantumSize**: Time units per quantum (default: 5)
- **executionPerCycle**: Processes executed per cycle (default: 3)
- **Prf**: Priority recalculation factor (default: 2)

## ?? Key Commands Reference

### File System

| Command | Description | Example |
|---------|-------------|---------|
| `DIRECTORYCREATE` | Create a directory | `DIRECTORYCREATE(path:/,name:docs)` |
| `FILECREATE` | Create a file | `FILECREATE(path:/docs,name:file.txt,size:512)` |
| `FILEDELETE` | Delete file/directory | `FILEDELETE(path:/docs/file.txt)` |

### Process Management

| Command | Description | Example |
|---------|-------------|---------|
| `PROCESSCREATE` | Create process | `PROCESSCREATE(pid:1,parent:0,priority:5,burst:100)` |
| `PROCESSCREATE` (with file) | Create file-dependent process | `PROCESSCREATE(pid:2,parent:0,priority:3,burst:50,filepath:/data/input.txt)` |
| `PROCESSDELETE` | Delete process | `PROCESSDELETE(pid:1)` |

### System

| Command | Description |
|---------|-------------|
| `start` | Start process executor |
| `help` | Show help information |
| `SHUTDOWN()` | Graceful system shutdown |
| `exit` / `quit` | Exit immediately |

## ?? Pro Tips

1. **Always start the executor** with `start` command before creating processes
2. **File paths are case-sensitive** - `/Home` is different from `/home`
3. **Check logs** - Process execution logs are written to `Long.txt`
4. **File system snapshot** - All files are logged to `FileLog.txt` on shutdown
5. **Priority matters** - Higher priority processes execute first (max-heap)

## ?? Learning Path

### Beginner
1. Create directories and files
2. Create simple processes without file dependencies
3. Observe process execution order

### Intermediate
1. Create file-dependent processes
2. Experiment with priority values
3. Create process hierarchies (parent-child)
4. Modify configuration settings

### Advanced
1. Test ready queue limits
2. Observe waiting queue behavior
3. Analyze log files for execution patterns
4. Experiment with different quantum sizes

## ?? Troubleshooting

### Process Not Executing?
- Make sure you ran `start` command
- Check if the ready queue is full (increase `readyLimit`)
- Verify file exists if process has file dependency

### File Creation Failed?
- Ensure parent directory exists
- Check path syntax (must start with `/`)
- Verify directory name is valid (no special characters)

### "Process X does not exist"?
- Process may have already completed execution
- Check if PID is correct
- Parent process must exist before creating child

## ?? Next Steps

- Read the [full README](README.md) for comprehensive documentation
- Check out [TESTING_SUMMARY.md](DS-OS-Test/TESTING_SUMMARY.md) to understand testing
- Review [CONTRIBUTING.md](CONTRIBUTING.md) if you want to contribute
- Explore the source code in `DS-OS/` directory

## ?? Need Help?

- Open an issue on [GitHub](https://github.com/ahk1384/DS-OS/issues)
- Check existing documentation
- Review example workflows above

---

**Happy simulating!** ??

For detailed documentation, see [README.md](README.md)
