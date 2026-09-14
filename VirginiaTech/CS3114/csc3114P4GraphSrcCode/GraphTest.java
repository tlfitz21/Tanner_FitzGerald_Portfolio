import student.TestCase;

public class GraphTest extends TestCase {

  public void testInsert() throws Exception {

    Graph it = new Graph(10);

    assertEquals(0, it.insert());
    assertEquals(1, it.insert());
    assertEquals(2, it.insert());
    assertEquals(3, it.insert());
    assertEquals(4, it.insert());
    assertEquals(5, it.insert());
    assertEquals(6, it.insert());
    assertEquals(7, it.insert());
    assertEquals(8, it.insert());
    assertEquals(9, it.insert());
    assertEquals(10, it.insert());
  }

  public void testRemove() throws Exception {

    Graph it = new Graph(10);

    it.insert();
    it.insert();
    it.insert();
    it.insert();
    it.insert();
    it.insert();
    it.insert();
    it.insert();
    it.insert();
    it.insert();

    assertFalse(it.remove(20));
    assertFalse(it.remove(11));
    assertTrue(it.remove(5));
    assertFalse(it.remove(5));

    it.addEdge(0, 1);
    it.addEdge(0, 2);
    it.addEdge(3, 4);

    assertTrue(it.remove(0));

  }
}
