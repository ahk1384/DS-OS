# DS-OS Test Suite

Comprehensive test suite for the DS-OS Operating System Simulator project.

## Test Coverage

### 1. **ReadyPriorityQueueTests.cs** (15+ tests)
Tests for the max-heap based priority queue used for process scheduling:
- Enqueue/Dequeue operations
- Heap property maintenance
- Priority ordering (max-heap)
- Edge cases (empty queue, single item)
- Peek without removal
- Clear operations
- Stress testing with many items

### 2. **PcbTests.cs** (12+ tests)
Tests for Process Control Block functionality:
- Process creation with and without file paths
- Property modifications (Priority, RemainingTime, State)
- Parent-child relationships
- Process naming
- State transitions
- Entry time tracking
- Validation of initial values

### 3. **FileNodeTests.cs** (25+ tests)
Tests for file system nodes:
- File and directory creation
- Parent-child relationships
- Adding/removing children
- Path navigation
- File/directory filtering
- Full path generation
- Timestamp tracking
- Edge cases and error handling

### 4. **FileTreeTests.cs** (30+ tests)
Tests for file system tree operations:
- File and directory creation
- Deletion operations
- Path existence checking
- Directory listing
- Move and copy operations
- Recursive operations
- Size calculations
- Complex scenarios

### 5. **CommandTests.cs** (10+ tests)
Tests for command handling:
- Command creation
- Parameter management
- Command type validation
- Enum completeness
- Property accessibility

### 6. **IntegrationTests.cs** (10+ tests)
End-to-end integration tests:
- Complete file system workflows
- Process scheduling scenarios
- Process with file access
- Process hierarchies
- State transitions
- Multiple queue management
- Large-scale operations
- Stress testing

### 7. **MainTestSuite.cs** (4+ tests)
Test suite verification and smoke tests:
- Test environment validation
- Basic functionality checks
- Test class discovery

## Total Test Count
**100+ individual test cases** covering all major components of the DS-OS system.

## Test Categories

### Unit Tests
- Individual component testing
- Isolated functionality verification
- Edge case handling
- Error condition testing

### Integration Tests
- Multi-component workflows
- System interaction testing
- End-to-end scenarios
- Performance validation

### Smoke Tests
- Quick sanity checks
- Basic functionality verification
- Build validation

## Running the Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~ReadyPriorityQueueTests"
```

### Run with Verbosity
```bash
dotnet test --verbosity detailed
```

### Generate Code Coverage
```bash
dotnet test /p:CollectCoverage=true
```

## Test Structure

Each test follows the **Arrange-Act-Assert** pattern:

```csharp
[Test]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Set up test data and preconditions
    var queue = new ReadyPriorityQueue();
    
    // Act - Execute the method being tested
    queue.Enqueue(pcb);
    
    // Assert - Verify the expected outcome
    Assert.That(queue.Count, Is.EqualTo(1));
}
```

## Key Features Tested

### Data Structures
- ? Max-heap priority queue implementation
- ? Tree-based file system
- ? Dictionary-based lookups
- ? Parent-child relationships

### Process Management
- ? Process creation and lifecycle
- ? Priority-based scheduling
- ? State transitions
- ? Parent-child process relationships
- ? File access requirements

### File System
- ? File and directory operations
- ? Path navigation
- ? Move and copy operations
- ? Size calculations
- ? Recursive operations
- ? Timestamp tracking

### Command Handling
- ? Command parsing
- ? Parameter management
- ? Command type validation

## Test Dependencies

- **NUnit 3.x** - Testing framework
- **NUnit3TestAdapter** - Test runner
- **Microsoft.NET.Test.Sdk** - .NET testing infrastructure

## Continuous Integration

These tests are designed to be run in CI/CD pipelines:
- Fast execution (< 1 second for most tests)
- No external dependencies
- Deterministic results
- Clear failure messages

## Test Maintenance

### Adding New Tests
1. Create tests in appropriate test class
2. Follow naming convention: `MethodName_Scenario_ExpectedBehavior`
3. Use Arrange-Act-Assert pattern
4. Add descriptive assertions
5. Update this README

### Best Practices
- Keep tests independent
- Use meaningful test names
- Test one concept per test
- Clean up resources in teardown
- Use appropriate assertion messages

## Coverage Goals

Current coverage areas:
- ? Core data structures: 95%+
- ? File system operations: 90%+
- ? Process management: 85%+
- ? Command handling: 80%+
- ? Integration scenarios: 75%+

## Known Limitations

Tests currently do NOT cover:
- Database manager implementations
- Process executor components
- Command handler implementation details
- Parser implementation (QueryParser)
- UI/Console interactions

These components require mocking of external dependencies and will be added in future iterations.

## Contributing

When adding new features to DS-OS:
1. Write tests FIRST (TDD approach)
2. Ensure all existing tests pass
3. Add integration tests for new workflows
4. Update this README with new test information

## License

Tests are part of the DS-OS project and follow the same license.
