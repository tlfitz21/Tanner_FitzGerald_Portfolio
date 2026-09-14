import student.TestCase;

public class ManagerTest extends TestCase {

    /**
     * the memory manager
     */
    public MemManager manager;

    public void setUp() {
        manager = new MemManager(256);
    }


    public void testFindFree() {
        manager.findFree("jaded".getBytes(), 5);
        assertEquals(8, manager.freeList[3].getHead().getData());
        byte[] bytes = new byte[5];
        System.arraycopy(manager.memPool, 0, bytes, 0, 5);
        String myString = new String(bytes);
        assertEquals("jaded", myString);

        bytes = new byte[6];
        manager.findFree("Oracle".getBytes(), 6);
        System.arraycopy(manager.memPool, 8, bytes, 0, 6);
        myString = new String(bytes);
        assertEquals("Oracle", myString);

        bytes = new byte[38];
        manager.findFree("My Songs Know What You Did In The Dark".getBytes(),
            38);
        System.arraycopy(manager.memPool, 64, bytes, 0, 38);
        myString = new String(bytes);
        assertEquals("My Songs Know What You Did In The Dark", myString);

        bytes = new byte[9];
        manager.findFree("Butterfly".getBytes(), 9);
        System.arraycopy(manager.memPool, 16, bytes, 0, 9);
        myString = new String(bytes);
        assertEquals("Butterfly", myString);

        bytes = new byte[5];
        manager.findFree("Agony".getBytes(), 5);
        System.arraycopy(manager.memPool, 32, bytes, 0, 5);
        myString = new String(bytes);
        assertEquals("Agony", myString);

        // will need to resize
        bytes = new byte[22];
        manager.findFree("Sugar, We're Goin Down".getBytes(), 22);
        System.arraycopy(manager.memPool, 128, bytes, 0, 22);
        myString = new String(bytes);
        assertEquals("Sugar, We're Goin Down", myString);

        bytes = new byte[91];
        manager.findFree(
            "I need something that's bigger than 64 characters so I'm just throwing this giant string in"
                .getBytes(), 91);
    }
// for (int j = 0; j < manager.freeList.length; j++) {
// if (manager.freeList[j].getHead() == null)
// System.out.println("List at " + j + " is null");
// else {
// System.out.print("List at " + j + " is: ");
// System.out.print(manager.freeList[j].getCurr().getData()
// + " -> ");
// manager.freeList[j].forward();
// while (manager.freeList[j].getCurr().getNext() != null) {
// System.out.print(manager.freeList[j].getCurr().getData()
// + " -> ");
// manager.freeList[j].forward();
// }
// System.out.println("");
// }
// }

//        System.arraycopy(manager.memPool, 256, bytes, 0, 91);
//        myString = new String(bytes);
//        assertEquals(
//            "I need something that's bigger than 64 characters so I'm just throwing this giant string in",
//            myString);
//
//    }
//

    public void testRemove() {
        manager.findFree("jaded".getBytes(), 5);
        manager.findFree("Oracle".getBytes(), 6);
        manager.findFree("My Songs Know What You Did In The Dark".getBytes(),
            38);
        manager.findFree("Butterfly".getBytes(), 9);
        manager.findFree("Agony".getBytes(), 5);
        manager.findFree("Sugar, We're Goin Down".getBytes(), 22);
        manager.findFree(
            "I need something that's bigger than 64 characters so I'm just throwing this giant string in"
                .getBytes(), 91);

        manager.remove(32, 5);
//        for (int j = 0; j < manager.freeList.length; j++) {
//            if (manager.freeList[j].getHead() == null)
//                System.out.println("List at " + j + " is null");
//            else {
//                manager.freeList[j].toStart();
//                System.out.print("List at " + j + " is: ");
//                System.out.print(manager.freeList[j].getCurr().getData()
//                    + " -> ");
//                manager.freeList[j].forward();
//                while (manager.freeList[j].getCurr().getNext() != null) {
//                    System.out.print(manager.freeList[j].getCurr().getData()
//                        + " -> ");
//                    manager.freeList[j].forward();
//                }
//                System.out.println("");
//            }
//        }
        assertEquals(null, manager.freeList[3].getHead());
        assertEquals(null, manager.freeList[4].getHead());
        assertEquals(32, manager.freeList[5].getHead().getData());
        manager.freeList[5].forward();
        assertEquals(160, manager.freeList[5].getCurr().getData());
        
        manager.remove(0, 5);
        manager.remove(8, 6);
        assertEquals(null, manager.freeList[3].getHead());
        assertEquals(0, manager.freeList[4].getHead().getData());
    }

}
