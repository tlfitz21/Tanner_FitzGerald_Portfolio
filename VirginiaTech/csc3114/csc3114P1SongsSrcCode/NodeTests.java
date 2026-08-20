import student.TestCase;

public class NodeTests extends TestCase
{

    private Node node;
    private Node nextNode;
    private Node prevNode;
    private Node nextNode2;
    private Node prevNode2;

    public void setUp()
    {
    }


    public void testNode()
    {
        nextNode = new Node(6);
        prevNode = new Node(4);
        node = new Node(5, nextNode, prevNode);
        assertEquals(node.getData(), 5);
        assertEquals(node.getNext().getData(), 6);
        assertEquals(node.getPrev().getData(), 4);
        node.setData(9);
        nextNode2 = new Node(10);
        prevNode2 = new Node(8);
        node.setNext(nextNode2);
        node.setPrev(prevNode2);
        assertEquals(node.getData(), 9);
        assertEquals(node.getNext().getData(), 10);
        assertEquals(node.getPrev().getData(), 8);
    }
}
