using DS_OS.DataStructer;
using DS_OS.Exceptions;

namespace DS_OS_Test;

[TestFixture]
public class PcbTests
{
    private Pcb _rootPcb;

    [SetUp]
    public void Setup()
    {
        // Create a root PCB that can be used as parent
        _rootPcb = Pcb.Create(0, null!, 0, 100);
    }

    [Test]
    public void Create_WithoutFilePath_CreatesValidPcb()
    {
        // Act
        var pcb = Pcb.Create(1, _rootPcb, 5, 100);

        // Assert
        Assert.That(pcb.Pid, Is.EqualTo(1));
        Assert.That(pcb.Priority, Is.EqualTo(5));
        Assert.That(pcb.RemaningTime, Is.EqualTo(100));
        Assert.That(pcb.Parent, Is.EqualTo(_rootPcb));
        Assert.That(pcb.NeedsFile, Is.False);
        Assert.That(pcb.FilePath, Is.Null);
        Assert.That(pcb.State, Is.EqualTo(State.New));
        Assert.That(pcb.Name, Is.EqualTo("Process_1"));
    }

    [Test]
    public void Create_WithFilePath_CreatesValidPcbWithFile()
    {
        // Act
        var pcb = Pcb.Create(2, _rootPcb, 3, 200, "/test/file.txt");

        // Assert
        Assert.That(pcb.Pid, Is.EqualTo(2));
        Assert.That(pcb.Priority, Is.EqualTo(3));
        Assert.That(pcb.RemaningTime, Is.EqualTo(200));
        Assert.That(pcb.Parent, Is.EqualTo(_rootPcb));
        Assert.That(pcb.NeedsFile, Is.True);
        Assert.That(pcb.FilePath, Is.EqualTo("/test/file.txt"));
        Assert.That(pcb.State, Is.EqualTo(State.New));
    }

    [Test]
    public void Priority_CanBeModified()
    {
        // Arrange
        var pcb = Pcb.Create(1, _rootPcb, 5, 100);

        // Act
        pcb.Priority = 10;

        // Assert
        Assert.That(pcb.Priority, Is.EqualTo(10));
    }

    [Test]
    public void RemainingTime_CanBeModified()
    {
        // Arrange
        var pcb = Pcb.Create(1, _rootPcb, 5, 100);

        // Act
        pcb.RemaningTime = 50;

        // Assert
        Assert.That(pcb.RemaningTime, Is.EqualTo(50));
    }

    [Test]
    public void State_CanBeChanged()
    {
        // Arrange
        var pcb = Pcb.Create(1, _rootPcb, 5, 100);

        // Act
        pcb.State = State.Ready;

        // Assert
        Assert.That(pcb.State, Is.EqualTo(State.Ready));
    }

    [Test]
    public void Children_InitializedAsEmptyDictionary()
    {
        // Arrange
        var pcb = Pcb.Create(1, _rootPcb, 5, 100);

        // Assert
        Assert.That(pcb.Children, Is.Not.Null);
        Assert.That(pcb.Children.Count, Is.EqualTo(0));
    }

    [Test]
    public void StartTime_InitializedToMinusOne()
    {
        // Arrange
        var pcb = Pcb.Create(1, _rootPcb, 5, 100);

        // Assert
        Assert.That(pcb.StartTime, Is.EqualTo(-1));
    }

    [Test]
    public void Name_GeneratedFromPid()
    {
        // Act
        var pcb1 = Pcb.Create(1, _rootPcb, 5, 100);
        var pcb2 = Pcb.Create(42, _rootPcb, 5, 100);

        // Assert
        Assert.That(pcb1.Name, Is.EqualTo("Process_1"));
        Assert.That(pcb2.Name, Is.EqualTo("Process_42"));
    }

    [Test]
    public void WaitReason_CanBeSet()
    {
        // Arrange
        var pcb = Pcb.Create(1, _rootPcb, 5, 100);

        // Act
        pcb.WaitReason = WaitReason.IO;

        // Assert
        Assert.That(pcb.WaitReason, Is.EqualTo(WaitReason.IO));
    }

    [Test]
    public void Parent_CanBeChanged()
    {
        // Arrange
        var pcb = Pcb.Create(1, _rootPcb, 5, 100);
        var newParent = Pcb.Create(2, _rootPcb, 3, 100);

        // Act
        pcb.Parent = newParent;

        // Assert
        Assert.That(pcb.Parent, Is.EqualTo(newParent));
    }
}
