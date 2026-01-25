namespace DS_OS.DataStructer;

public class ReadyPriorityQueue
{
    private readonly List<Pcb> _heap = new();

    public int Count => _heap.Count;

    public bool IsEmpty => _heap.Count == 0;

    public ReadyPriorityQueue()
    {
        _heap = new List<Pcb>();
    }
    public void Clear()
    {
        _heap.Clear();
    }
    public Pcb Peek()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Priority queue is empty");

        return _heap[0];
    }

    public Pcb Dequeue()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Priority queue is empty");

        Pcb root = _heap[0];
        int lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);

        if (_heap.Count > 0)
            HeapifyDown(0);

        return root;
    }

    public bool Enqueue(Pcb pcb)
    {
        if (pcb == null)
            throw new ArgumentNullException(nameof(pcb));
        pcb.EntryTime = DateTime.Now;
        _heap.Add(pcb);
        HeapifyUp(_heap.Count - 1);
        return true;
    }


    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (_heap[index].Priority <= _heap[parentIndex].Priority)
            {
                if (_heap[index].Priority == _heap[parentIndex].Priority)
                {
                    if (_heap[index].EntryTime < _heap[parentIndex].EntryTime)
                        Swap(index, parentIndex);
                }
                else
                    break;
            }
            else
            {
                Swap(index, parentIndex);
            }
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        while (true)
        {
            int largest = index;
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;

            if (leftChild < _heap.Count)
            {
                if (_heap[leftChild].Priority > _heap[largest].Priority)
                    largest = leftChild;
                else if (_heap[leftChild].Priority == _heap[largest].Priority)
                {
                    if (_heap[leftChild].EntryTime < _heap[largest].EntryTime)
                        largest = leftChild;
                }
            }

            if (rightChild < _heap.Count)
            {
                if (_heap[rightChild].Priority > _heap[largest].Priority)
                    largest = rightChild;
                else if (_heap[rightChild].Priority == _heap[largest].Priority)
                {
                    if (_heap[rightChild].EntryTime < _heap[largest].EntryTime)
                        largest = rightChild;
                }
            }

            if (largest == index)
                break;

            Swap(index, largest);
            index = largest;
        }
    }

    private void Swap(int i, int j)
        {
            (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
        }
}
