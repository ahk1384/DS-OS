using DS_OS.DataStructer;

namespace DS_OS_Test;

[TestFixture]
public class IntegrationTests
{
    [Test]
    public void FileSystemIntegration_CompleteWorkflow()
    {
        // Arrange
        var fileTree = new FileTree();

        // Act & Assert - Create directory structure
        Assert.That(fileTree.CreateDirectory("/", "projects"), Is.True);
        Assert.That(fileTree.CreateDirectory("/projects", "app1"), Is.True);
        Assert.That(fileTree.CreateDirectory("/projects", "app2"), Is.True);

        // Create files
        Assert.That(fileTree.CreateFile("/projects/app1", "main.cs", 2048), Is.True);
        Assert.That(fileTree.CreateFile("/projects/app1", "config.json", 512), Is.True);
        Assert.That(fileTree.CreateFile("/projects/app2", "index.html", 1024), Is.True);

        // Verify structure
        Assert.That(fileTree.GetDirectories("/projects").Count(), Is.EqualTo(2));
        Assert.That(fileTree.GetFiles("/projects/app1").Count(), Is.EqualTo(2));

        // Test navigation
        var mainFile = fileTree.FindNode("/projects/app1/main.cs");
        Assert.That(mainFile, Is.Not.Null);
        Assert.That(mainFile!.IsFile, Is.True);

        // Test size calculation
        var app1Size = fileTree.GetDirectorySize("/projects/app1");
        Assert.That(app1Size, Is.EqualTo(2560));

        // Test move operation
        Assert.That(fileTree.Move("/projects/app1/config.json", "/projects/app2/config.json"), Is.True);
        Assert.That(fileTree.Exists("/projects/app1/config.json"), Is.False);
        Assert.That(fileTree.Exists("/projects/app2/config.json"), Is.True);

        // Test copy operation
        Assert.That(fileTree.Copy("/projects/app2/index.html", "/projects/app1/index.html"), Is.True);
        Assert.That(fileTree.Exists("/projects/app2/index.html"), Is.True);
        Assert.That(fileTree.Exists("/projects/app1/index.html"), Is.True);

        // Test deletion
        Assert.That(fileTree.Delete("/projects/app1/index.html"), Is.True);
        Assert.That(fileTree.Exists("/projects/app1/index.html"), Is.False);
    }

    [Test]
    public void ProcessScheduling_CompleteWorkflow()
    {
        // Arrange
        var queue = new ReadyPriorityQueue();
        var rootPcb = Pcb.Create(0, null!, 0, 100);

        // Create processes with different priorities
        var p1 = Pcb.Create(1, rootPcb, 5, 100);
        var p2 = Pcb.Create(2, rootPcb, 10, 150);
        var p3 = Pcb.Create(3, rootPcb, 3, 200);
        var p4 = Pcb.Create(4, rootPcb, 8, 120);

        // Act - Enqueue processes
        queue.Enqueue(p1);
        queue.Enqueue(p2);
        queue.Enqueue(p3);
        queue.Enqueue(p4);

        // Assert - Dequeue in priority order (highest first in max heap)
        Assert.That(queue.Count, Is.EqualTo(4));
        
        var first = queue.Dequeue();
        Assert.That(first.Priority, Is.EqualTo(10)); // Highest priority
        
        var second = queue.Dequeue();
        Assert.That(second.Priority, Is.EqualTo(8));
        
        var third = queue.Dequeue();
        Assert.That(third.Priority, Is.EqualTo(5));
        
        var fourth = queue.Dequeue();
        Assert.That(fourth.Priority, Is.EqualTo(3)); // Lowest priority

        Assert.That(queue.IsEmpty, Is.True);
    }

    [Test]
    public void ProcessWithFileAccess_Integration()
    {
        // Arrange
        var fileTree = new FileTree();
        var queue = new ReadyPriorityQueue();
        var rootPcb = Pcb.Create(0, null!, 0, 100);

        // Create file system
        fileTree.CreateDirectory("/", "data");
        fileTree.CreateFile("/data", "input.txt", 1024);

        // Create process that needs file
        var process = Pcb.Create(1, rootPcb, 5, 100, "/data/input.txt");

        // Act
        Assert.That(process.NeedsFile, Is.True);
        Assert.That(process.FilePath, Is.EqualTo("/data/input.txt"));
        Assert.That(fileTree.Exists(process.FilePath), Is.True);

        // Enqueue process
        queue.Enqueue(process);
        var retrieved = queue.Dequeue();

        // Assert
        Assert.That(retrieved, Is.EqualTo(process));
        Assert.That(retrieved.NeedsFile, Is.True);
    }

    [Test]
    public void ProcessHierarchy_ParentChildRelationship()
    {
        // Arrange
        var rootPcb = Pcb.Create(0, null!, 0, 100);
        var parent = Pcb.Create(1, rootPcb, 5, 100);
        var child1 = Pcb.Create(2, parent, 3, 50);
        var child2 = Pcb.Create(3, parent, 4, 75);

        // Act
        parent.Children.Add(child1.Pid, child1);
        parent.Children.Add(child2.Pid, child2);

        // Assert
        Assert.That(parent.Children.Count, Is.EqualTo(2));
        Assert.That(child1.Parent, Is.EqualTo(parent));
        Assert.That(child2.Parent, Is.EqualTo(parent));
        Assert.That(parent.Children.ContainsKey(child1.Pid), Is.True);
        Assert.That(parent.Children.ContainsKey(child2.Pid), Is.True);
    }

    [Test]
    public void ProcessStateTransitions_CompleteLifecycle()
    {
        // Arrange
        var rootPcb = Pcb.Create(0, null!, 0, 100);
        var process = Pcb.Create(1, rootPcb, 5, 100);

        // Act & Assert - Simulate process lifecycle
        Assert.That(process.State, Is.EqualTo(State.New));

        process.State = State.Ready;
        Assert.That(process.State, Is.EqualTo(State.Ready));

        process.State = State.Running;
        process.StartTime = 10;
        Assert.That(process.State, Is.EqualTo(State.Running));
        Assert.That(process.StartTime, Is.EqualTo(10));

        process.RemaningTime -= 20;
        Assert.That(process.RemaningTime, Is.EqualTo(80));

        process.State = State.Waiting;
        process.WaitReason = WaitReason.IO;
        Assert.That(process.State, Is.EqualTo(State.Waiting));
        Assert.That(process.WaitReason, Is.EqualTo(WaitReason.IO));

        process.State = State.Ready;
        Assert.That(process.State, Is.EqualTo(State.Ready));
    }

    [Test]
    public void MultipleQueues_DifferentPriorities()
    {
        // Arrange
        var highPriorityQueue = new ReadyPriorityQueue();
        var lowPriorityQueue = new ReadyPriorityQueue();
        var rootPcb = Pcb.Create(0, null!, 0, 100);

        // Create high priority processes
        highPriorityQueue.Enqueue(Pcb.Create(1, rootPcb, 10, 50));
        highPriorityQueue.Enqueue(Pcb.Create(2, rootPcb, 9, 50));

        // Create low priority processes
        lowPriorityQueue.Enqueue(Pcb.Create(3, rootPcb, 2, 100));
        lowPriorityQueue.Enqueue(Pcb.Create(4, rootPcb, 1, 100));

        // Act & Assert
        Assert.That(highPriorityQueue.Peek().Priority, Is.EqualTo(10));
        Assert.That(lowPriorityQueue.Peek().Priority, Is.EqualTo(2));

        // Process high priority first
        var hp1 = highPriorityQueue.Dequeue();
        Assert.That(hp1.Priority, Is.EqualTo(10));

        // Still have processes in both queues
        Assert.That(highPriorityQueue.Count, Is.EqualTo(1));
        Assert.That(lowPriorityQueue.Count, Is.EqualTo(2));
    }

    [Test]
    public void FileTree_LargeDirectoryStructure()
    {
        // Arrange
        var fileTree = new FileTree();

        // Create a complex directory structure
        fileTree.CreateDirectory("/", "home");
        fileTree.CreateDirectory("/home", "user1");
        fileTree.CreateDirectory("/home", "user2");
        fileTree.CreateDirectory("/home/user1", "documents");
        fileTree.CreateDirectory("/home/user1", "downloads");
        fileTree.CreateDirectory("/home/user2", "documents");

        // Create files in various locations
        fileTree.CreateFile("/home/user1/documents", "file1.txt", 100);
        fileTree.CreateFile("/home/user1/documents", "file2.txt", 200);
        fileTree.CreateFile("/home/user1/downloads", "file3.zip", 5000);
        fileTree.CreateFile("/home/user2/documents", "file4.pdf", 3000);

        // Act & Assert
        var user1Size = fileTree.GetDirectorySize("/home/user1");
        var user2Size = fileTree.GetDirectorySize("/home/user2");
        var totalSize = fileTree.GetDirectorySize("/home");

        Assert.That(user1Size, Is.EqualTo(5300));
        Assert.That(user2Size, Is.EqualTo(3000));
        Assert.That(totalSize, Is.EqualTo(8300));

        // Test navigation
        Assert.That(fileTree.Exists("/home/user1/documents/file1.txt"), Is.True);
        Assert.That(fileTree.GetFiles("/home/user1/documents").Count(), Is.EqualTo(2));
        Assert.That(fileTree.GetDirectories("/home").Count(), Is.EqualTo(2));
    }

    [Test]
    public void FileManager_GetAllFilesRecursive_Integration()
    {
        // Arrange
        var fileManager = new DS_OS.FileManager.FileManager();
        
        // Create complex directory structure
        fileManager.CreateDirectory("/", "projects");
        fileManager.CreateDirectory("/projects", "app1");
        fileManager.CreateDirectory("/projects", "app2");
        fileManager.CreateDirectory("/projects/app1", "src");
        fileManager.CreateDirectory("/projects/app1", "tests");
        
        // Create files in various locations
        fileManager.CreateFile("/projects", "readme.md", 500);
        fileManager.CreateFile("/projects/app1", "main.cs", 2048);
        fileManager.CreateFile("/projects/app1/src", "helper.cs", 1024);
        fileManager.CreateFile("/projects/app1/tests", "test.cs", 512);
        fileManager.CreateFile("/projects/app2", "index.html", 768);

        // Act
        var allFiles = fileManager.GetAllFilesRecursive("/").ToList();
        var app1Files = fileManager.GetAllFilesRecursive("/projects/app1").ToList();

        // Assert
        Assert.That(allFiles.Count, Is.EqualTo(5));
        Assert.That(app1Files.Count, Is.EqualTo(3));
        
        // Verify full paths are correct
        Assert.That(allFiles.Select(f => f.FullPath), Contains.Item("/projects/readme.md"));
        Assert.That(allFiles.Select(f => f.FullPath), Contains.Item("/projects/app1/main.cs"));
        Assert.That(allFiles.Select(f => f.FullPath), Contains.Item("/projects/app1/src/helper.cs"));
        Assert.That(allFiles.Select(f => f.FullPath), Contains.Item("/projects/app1/tests/test.cs"));
        Assert.That(allFiles.Select(f => f.FullPath), Contains.Item("/projects/app2/index.html"));
    }

    [Test]
    public void FileManager_SearchAndRecursiveRetrieval_Integration()
    {
        // Arrange
        var fileManager = new DS_OS.FileManager.FileManager();
        
        fileManager.CreateDirectory("/", "docs");
        fileManager.CreateDirectory("/docs", "2023");
        fileManager.CreateDirectory("/docs", "2024");
        
        fileManager.CreateFile("/docs/2023", "report.pdf", 1000);
        fileManager.CreateFile("/docs/2024", "report.pdf", 1500);
        fileManager.CreateFile("/docs/2024", "summary.txt", 200);

        // Act - Get all files recursively
        var allFiles = fileManager.GetAllFilesRecursive("/docs").ToList();
        
        // Act - Search for specific pattern
        var reports = fileManager.Search("report", "/docs").ToList();

        // Assert
        Assert.That(allFiles.Count, Is.EqualTo(3));
        Assert.That(reports.Count, Is.EqualTo(2));
        Assert.That(reports.All(r => r.Name.Contains("report")), Is.True);
    }

    [Test]
    public void StressTest_ManyProcesses()
    {
        // Arrange
        var queue = new ReadyPriorityQueue();
        var rootPcb = Pcb.Create(0, null!, 0, 100);
        var random = new Random(42); // Fixed seed for reproducibility

        // Create many processes
        var processes = new List<Pcb>();
        for (int i = 1; i <= 100; i++)
        {
            var priority = random.Next(1, 20);
            var pcb = Pcb.Create(i, rootPcb, priority, 100);
            processes.Add(pcb);
            queue.Enqueue(pcb);
        }

        // Act - Dequeue all and verify order
        var previousPriority = int.MaxValue;
        while (!queue.IsEmpty)
        {
            var pcb = queue.Dequeue();
            Assert.That(pcb.Priority, Is.LessThanOrEqualTo(previousPriority),
                "Priority queue should maintain max-heap property");
            previousPriority = pcb.Priority;
        }

        // Assert
        Assert.That(queue.IsEmpty, Is.True);
    }

    [Test]
    public void FileLogging_CompleteWorkflow_Integration()
    {
        // Arrange
        var fileManager = new DS_OS.FileManager.FileManager();
        var fileLogger = new DS_OS.Logger.FileLogger();
        
        // Create test log path
        var testLogPath = Path.Combine(Path.GetTempPath(), $"test_integration_{Guid.NewGuid()}.txt");
        var pathField = typeof(DS_OS.Logger.FileLogger).GetField("path", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pathField?.SetValue(fileLogger, testLogPath);

        try
        {
            // Create file structure
            fileManager.CreateDirectory("/", "data");
            fileManager.CreateDirectory("/data", "logs");
            fileManager.CreateFile("/data", "config.xml", 256);
            fileManager.CreateFile("/data/logs", "app.log", 1024);
            fileManager.CreateFile("/data/logs", "error.log", 512);

            // Act - Get all files and log them
            var allFiles = fileManager.GetAllFilesRecursive("/");
            fileLogger.Log(allFiles);

            // Assert
            Assert.That(File.Exists(testLogPath), Is.True);
            var logContent = File.ReadAllText(testLogPath);
            
            Assert.That(logContent, Does.Contain("/data/config.xml"));
            Assert.That(logContent, Does.Contain("/data/logs/app.log"));
            Assert.That(logContent, Does.Contain("/data/logs/error.log"));
        }
        finally
        {
            // Cleanup
            if (File.Exists(testLogPath))
            {
                File.Delete(testLogPath);
            }
        }
    }
}
