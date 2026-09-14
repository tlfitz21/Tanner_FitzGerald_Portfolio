import student.TestCase;
import java.io.IOException;

public class LinkedListTest extends TestCase {

    private LinkedList it = new LinkedList();

    public void testAppend() throws Exception {

        it.append(new Node(5));

        assertEquals("5", Integer.toString(it.getHead().getIndex()));

        it.append(new Node(10));
        it.append(new Node(15));
        it.append(new Node(20));
        it.append(new Node(25));

        assertEquals("25", Integer.toString(it.getTail().getIndex()));
    }


    public void testAccess() throws Exception {

        it.append(new Node(10));
        it.append(new Node(15));
        it.append(new Node(20));
        it.append(new Node(25));

        assertEquals("10", Integer.toString(it.getHead().getIndex()));
        assertEquals("25", Integer.toString(it.getTail().getIndex()));
    }


    public void testClear() throws Exception {

        it.append(new Node(10));
        it.append(new Node(15));
        it.append(new Node(20));
        it.append(new Node(25));

        it.clear();

        assertEquals(null, it.getHead());
        assertEquals(null, it.getTail());
    }


    public void testTraversal() throws Exception {

        it.append(new Node(10));
        it.append(new Node(15));
        it.append(new Node(20));
        it.append(new Node(25));

        assertEquals("10", Integer.toString(it.getCurr().getIndex()));

        assertTrue(it.forward());
        assertEquals("15", Integer.toString(it.getCurr().getIndex()));
        assertTrue(it.forward());
        assertTrue(it.forward());
        assertEquals("25", Integer.toString(it.getCurr().getIndex()));
        assertFalse(it.forward());

        it.currReset();
        assertEquals("10", Integer.toString(it.getCurr().getIndex()));

        it.clear();

        assertFalse(it.forward());

    }


    public void testFind() throws Exception {

        assertEquals(null, it.find(4));

        it.append(new Node(10));
        it.append(new Node(15));
        it.append(new Node(20));
        it.append(new Node(25));

        assertEquals(null, it.find(4));
        assertEquals(10, it.find(10).getIndex());
        assertEquals(20, it.find(20).getIndex());
        assertEquals(25, it.find(25).getIndex());
    }


    public void testRemove() throws Exception {

        assertFalse(it.remove(4));

        it.append(new Node(10));
        it.append(new Node(15));
        it.append(new Node(20));
        it.append(new Node(25));

        assertFalse(it.remove(4));

        assertTrue(it.remove(10));
        assertFuzzyEquals("15", Integer.toString(it.getHead().getIndex()));

        assertTrue(it.remove(25));
        assertFuzzyEquals("20", Integer.toString(it.getTail().getIndex()));

        it.append(new Node(30));
        assertTrue(it.remove(20));
        assertFuzzyEquals("15", Integer.toString(it.getHead().getIndex()));
        assertFuzzyEquals("30", Integer.toString(it.getHead().getNext()
            .getIndex()));

        assertTrue(it.remove(15));
        assertTrue(it.remove(30));
        assertEquals(null, it.getHead());
        assertEquals(null, it.getTail());

    }
}
