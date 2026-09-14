import java.util.Random;
import student.TestCase;

/**
 * a class to test the HashTableTable table
 *
 * @author Can Pekkan
 * @version 1.0
 */
public class HashTableTest extends TestCase {
    private HashTable h0;
    private HashTable h1;
    private HashTable h2;

    /**
     * get a few HashTable tables ready to be tested
     */
    @Override
    protected void setUp() throws Exception {
        h0 = new HashTable(0);
        h1 = new HashTable(4);
        h2 = new HashTable(11);
        h1.insert("one item", 3);
    }


    /**
     * tests the clear method
     * 
     * @throws Exception
     *             why dose this throw a exception.
     */
    public void testClear() throws Exception {
        assertEquals(3, h1.find("one item"));
        h1.clear();
        assertEquals(-1, h1.find("one item"));
    }


    /**
     * this function test the insert method and the contains method, I made it
     * like that because each insert call first calls the contains method
     * 
     * @throws Exception
     *             why dose this throw a exception.
     */
    public void testInsertAndFind() throws Exception {
        assertTrue(h0.insert("the first string is a 0 size HashTable", 3));
        assertFuzzyEquals("0: one item", h1.toString());
        Random r = new Random(10);
        for (int i = 0; i < 5; i++) {
            h1.insert(Integer.toString(i) + ": " + Integer.toString(r
                .nextInt()), 5);
        }
        assertEquals("2: 1: 1913984760\n" + "7: 3: 1773446580\n"
            + "8: one item\n" + "9: 4: 254270492\n" + "14: 2: 1107254586\n"
            + "15: 0: -1157793070\n", h1.toString());
        h1.remove("4: 254270492");
        assertEquals("2: 1: 1913984760\n" + "7: 3: 1773446580\n"
            + "8: one item\n" + "9: TOMBSTONE\n" + "14: 2: 1107254586\n"
            + "15: 0: -1157793070\n", h1.toString());
        assertEquals(-1, h1.find("4: 254270492"));
        assertEquals(5, h1.find("2: 1107254586"));
        assertEquals(5, h1.find("1: 1913984760"));
        assertEquals(3, h1.find("one item"));
        assertEquals(5, h1.find("3: 1773446580"));
        assertEquals(-1, h1.find("afjsdj"));
    }


    /**
     * this function test the remove method of the HashTable table
     * 
     * @throws Exception
     *             why dose this throw a exception.
     */
    public void testRemove() throws Exception {
        h1.insert("something nice", 1);
        h1.insert("something cool", 2);
        h1.insert("something", 3);
        h1.insert("something like a string", 4);
        h1.insert("something nice again", 5);

        assertFuzzyEquals("1: something nice\n" + "2: something like a string\n"
            + "8: one item\n" + "9: something nice again\n"
            + "13: something cool\n" + "14: something\n", h1.toString());

        assertEquals(2, h1.remove("something cool"));
        assertEquals(4, h1.remove("something like a string"));
        assertEquals(5, h1.remove("something nice again"));
        assertEquals(-1, h1.remove("something nice again"));
        assertEquals(-1, h1.remove("something like a string"));
        assertEquals("1: something nice\n" + "2: TOMBSTONE\n" + "8: one item\n"
            + "9: TOMBSTONE\n" + "13: TOMBSTONE\n" + "14: something\n", h1
                .toString());
        assertEquals(-1, h2.remove("0"));
    }
}
