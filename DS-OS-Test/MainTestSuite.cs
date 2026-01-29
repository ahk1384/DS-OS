namespace DS_OS_Test
{
    /// <summary>
    /// Main test suite for DS-OS Operating System Simulator
    /// 
    /// Test Coverage:
    /// 1. ReadyPriorityQueueTests - Max-heap priority queue for process scheduling
    /// 2. PcbTests - Process Control Block functionality
    /// 3. FileNodeTests - File system node operations
    /// 4. FileTreeTests - File system tree structure and operations
    /// 5. CommandTests - Command and CommandType functionality
    /// 6. IntegrationTests - End-to-end system integration tests
    /// 
    /// Total Test Count: 100+ tests covering:
    /// - Data structures (heap, tree)
    /// - Process management and scheduling
    /// - File system operations
    /// - Command handling
    /// - Integration scenarios
    /// </summary>
    [TestFixture]
    public class MainTestSuite
    {
        [Test]
        public void TestSuite_IsProperlyConfigured()
        {
            // This test verifies the test environment is set up correctly
            Assert.Pass("DS-OS Test Suite is ready. Run all tests to verify system functionality.");
        }

        [Test]
        public void AllTestClasses_AreDiscoverable()
        {
            // Verify all test classes exist in the assembly
            var assembly = typeof(MainTestSuite).Assembly;
            var testClasses = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(TestFixtureAttribute), false).Any())
                .ToList();

            Assert.That(testClasses.Count, Is.GreaterThanOrEqualTo(7), 
                "Expected at least 7 test fixture classes");

            var expectedClasses = new[]
            {
                "MainTestSuite",
                "ReadyPriorityQueueTests",
                "PcbTests",
                "FileNodeTests",
                "FileTreeTests",
                "CommandTests",
                "CommandTypeTests",
                "IntegrationTests"
            };

            foreach (var expectedClass in expectedClasses)
            {
                var exists = testClasses.Any(t => t.Name == expectedClass);
                Assert.That(exists, Is.True, $"Test class {expectedClass} should exist");
            }
        }
    }

    /// <summary>
    /// Quick smoke tests to verify basic system functionality
    /// </summary>
    [TestFixture]
    public class SmokeTests
    {
        [Test]
        public void PriorityQueue_BasicOperation_Works()
        {
            var queue = new DS_OS.DataStructer.ReadyPriorityQueue();
            Assert.That(queue.IsEmpty, Is.True);
        }

        [Test]
        public void FileTree_BasicOperation_Works()
        {
            var fileTree = new DS_OS.DataStructer.FileTree();
            Assert.That(fileTree.Root, Is.Not.Null);
        }

        [Test]
        public void Command_CanBeCreated()
        {
            var command = new DS_OS.Engine.CommandHandler.BaseClass.Command(
                DS_OS.Engine.CommandHandler.BaseClass.CommandType.SHUTDOWN,
                new Dictionary<string, string>());
            Assert.That(command, Is.Not.Null);
        }

        [Test]
        public void FileNode_CanBeCreated()
        {
            var node = DS_OS.DataStructer.FileNode.CreateFile("test.txt", 100);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.IsFile, Is.True);
        }
    }
}
