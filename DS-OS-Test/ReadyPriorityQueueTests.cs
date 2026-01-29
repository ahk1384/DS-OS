using DS_OS.DataStructer;
using DS_OS.Exceptions;

namespace DS_OS_Test;

[TestFixture]
public class ReadyPriorityQueueTests
{
    private ReadyPriorityQueue _queue;
    private Pcb _rootPcb;

    [SetUp]
    public void Setup()
    {
        _queue = new ReadyPriorityQueue();
        // Create a root PCB to use as parent for test PCBs
        _rootPcb = Pcb.Create(0, null!, 0, 100);
    }

    [Test]
    public void Enqueue_SingleItem_CountIsOne()
    {
        // Arrange
        var pcb = Pcb.Create(1, _rootPcb, 5, 100);

        // Act
        _queue.Enqueue(pcb);

        // Assert
        Assert.That(_queue.Count, Is.EqualTo(1));
        Assert.That(_queue.IsEmpty, Is.False);
    }

    [Test]
    public void Enqueue_NullPcb_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _queue.Enqueue(null!));
    }

    [Test]
    public void Dequeue_EmptyQueue_ThrowsInvalidOperationException()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _queue.Dequeue());
    }

    [Test]
    public void Dequeue_SingleItem_ReturnsItemAndQueueIsEmpty()
    {
        // Arrange
        var pcb = Pcb.Create(1, _rootPcb, 5, 100);
        _queue.Enqueue(pcb);

        // Act
        var result = _queue.Dequeue();

        // Assert
        Assert.That(result, Is.EqualTo(pcb));
        Assert.That(_queue.Count, Is.EqualTo(0));
        Assert.That(_queue.IsEmpty, Is.True);
    }

    [Test]
    public void Dequeue_MaxHeap_ReturnsHighestPriorityFirst()
    {
        // Arrange - Max heap: higher priority values come first
        var pcb1 = Pcb.Create(1, _rootPcb, 3, 100);
        var pcb2 = Pcb.Create(2, _rootPcb, 7, 100);
        var pcb3 = Pcb.Create(3, _rootPcb, 5, 100);
        var pcb4 = Pcb.Create(4, _rootPcb, 9, 100);

        _queue.Enqueue(pcb1);
        _queue.Enqueue(pcb2);
        _queue.Enqueue(pcb3);
        _queue.Enqueue(pcb4);

        // Act & Assert - Should come out in descending priority order
        Assert.That(_queue.Dequeue().Priority, Is.EqualTo(9));
        Assert.That(_queue.Dequeue().Priority, Is.EqualTo(7));
        Assert.That(_queue.Dequeue().Priority, Is.EqualTo(5));
        Assert.That(_queue.Dequeue().Priority, Is.EqualTo(3));
    }

    [Test]
    public void Peek_EmptyQueue_ThrowsInvalidOperationException()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _queue.Peek());
    }

    [Test]
    public void Peek_NonEmptyQueue_ReturnsTopWithoutRemoving()
    {
        // Arrange
        var pcb1 = Pcb.Create(1, _rootPcb, 5, 100);
        var pcb2 = Pcb.Create(2, _rootPcb, 10, 100);
        _queue.Enqueue(pcb1);
        _queue.Enqueue(pcb2);

        // Act
        var result = _queue.Peek();

        // Assert
        Assert.That(result.Priority, Is.EqualTo(10));
        Assert.That(_queue.Count, Is.EqualTo(2)); // Count unchanged
    }

    [Test]
    public void Clear_NonEmptyQueue_EmptiesQueue()
    {
        // Arrange
        _queue.Enqueue(Pcb.Create(1, _rootPcb, 5, 100));
        _queue.Enqueue(Pcb.Create(2, _rootPcb, 3, 100));

        // Act
        _queue.Clear();

        // Assert
        Assert.That(_queue.Count, Is.EqualTo(0));
        Assert.That(_queue.IsEmpty, Is.True);
    }

    [Test]
    public void EnqueueDequeue_ManyItems_MaintainsHeapProperty()
    {
        // Arrange
        var priorities = new[] { 15, 3, 8, 20, 1, 12, 9, 4, 18, 7 };
        var expectedOrder = priorities.OrderByDescending(p => p).ToArray();

        foreach (var priority in priorities)
        {
            _queue.Enqueue(Pcb.Create(priority, _rootPcb, priority, 100));
        }

        // Act & Assert
        for (int i = 0; i < expectedOrder.Length; i++)
        {
            var pcb = _queue.Dequeue();
            Assert.That(pcb.Priority, Is.EqualTo(expectedOrder[i]),
                $"Expected priority {expectedOrder[i]} at position {i}, but got {pcb.Priority}");
        }
    }

    [Test]
    public void IsEmpty_NewQueue_ReturnsTrue()
    {
        // Assert
        Assert.That(_queue.IsEmpty, Is.True);
    }

    [Test]
    public void IsEmpty_AfterEnqueueDequeue_ReturnsTrue()
    {
        // Arrange
        var pcb = Pcb.Create(1, _rootPcb, 5, 100);
        _queue.Enqueue(pcb);

        // Act
        _queue.Dequeue();

        // Assert
        Assert.That(_queue.IsEmpty, Is.True);
    }

    [Test]
    public void EnqueueDequeue_AlternatingOperations_WorksCorrectly()
    {
        // Arrange & Act
        _queue.Enqueue(Pcb.Create(1, _rootPcb, 10, 100));
        _queue.Enqueue(Pcb.Create(2, _rootPcb, 5, 100));
        
        var first = _queue.Dequeue();
        
        _queue.Enqueue(Pcb.Create(3, _rootPcb, 15, 100));
        _queue.Enqueue(Pcb.Create(4, _rootPcb, 3, 100));
        
        var second = _queue.Dequeue();
        var third = _queue.Dequeue();

        // Assert
        Assert.That(first.Priority, Is.EqualTo(10));
        Assert.That(second.Priority, Is.EqualTo(15));
        Assert.That(third.Priority, Is.EqualTo(5));
        Assert.That(_queue.Count, Is.EqualTo(1));
    }
}
