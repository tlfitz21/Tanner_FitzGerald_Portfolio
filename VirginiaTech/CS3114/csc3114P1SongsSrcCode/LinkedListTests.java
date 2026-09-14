import student.TestCase;

public class LinkedListTests extends TestCase 
{

    private Node startNode;
    private LinkedList list;

    public void setUp() 
    {
        startNode = new Node(115);
        list = new LinkedList(startNode);
        list.append(130);
        list.append(145);
        list.append(160);
        list.append(175);
    }


    public void testAccessors()
    {
        assertSame(list.getCurr(), startNode);
        assertSame(list.getHead(), startNode);
        assertEquals(list.getTail().getData(), 175);

    }


    public void testTraversal() 
    {
        list.forward();
        assertEquals(list.getCurr().getData(), 130);
        list.forward();
        assertEquals(list.getCurr().getData(), 145);
        list.forward();
        assertEquals(list.getCurr().getData(), 160);
        list.forward();
        assertEquals(list.getCurr().getData(), 175);
        assertFalse(list.forward());
        list.back();
        assertEquals(list.getCurr().getData(), 160);
        list.back();
        assertEquals(list.getCurr().getData(), 145);
        list.back();
        assertEquals(list.getCurr().getData(), 130);
        list.back();
        assertEquals(list.getCurr().getData(), 115);
        assertFalse(list.back());
    }


    public void testEmpty()
    {
        assertFalse(list.isEmpty());
        assertTrue(list.clear());
        assertTrue(list.isEmpty());
        list.append(115);
        assertFalse(list.isEmpty());
        list.append(130);
        list.append(145);
        list.append(160);
        list.append(175);

    }


    public void testInsertion() 
    {
        list.forward();
        list.insertAfter(135);
        list.forward();
        list.forward();
        list.forward();
        list.insertAfter(165);
        list.forward();
        list.forward();
        list.insertAfter(180);
        assertFuzzyEquals(list.print(),
            "The list: 115 -> 130 -> 135 -> 145 -> 160 -> 165 -> 175 -> 180");
        list.append(195);
        list.prepend(100);
        assertFuzzyEquals(list.print(),
            "The list: 100 -> 115 -> 130 -> 135 -> 145 -> 160 -> 165 -> 175"
            + " -> 180 -> 195");
        list.clear();
        assertFuzzyEquals(list.print(), "Empty List");
        list.append(115);
        list.append(130);
        list.append(145);
        list.append(160);
        list.append(175);
    }
    
    public void testRemoval() 
    {
        assertEquals(list.remove().getData(), 115);
        assertEquals(list.getHead().getData(), 130);
        list.forward();
        assertEquals(list.remove().getData(), 145);
        list.forward();
        assertEquals(list.remove().getData(), 175);
        assertEquals(list.getTail().getData(), 160);
    }
    
    public void testRun()
    {
        startNode = new Node(0);
        list = new LinkedList(startNode);
        list.append(8);
        list.forward();
        list.remove();
    }

}
