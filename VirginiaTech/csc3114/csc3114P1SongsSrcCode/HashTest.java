import student.TestCase;

public class HashTest extends TestCase {
    
    private Hash hash;
    private MemManager manager;
    
    
    public void setUp() {
        manager = new MemManager(256);
        hash = new Hash(10, manager);
        hash.insert("jaded", "spiritbox", 0, 5);
        hash.insert("Long Live The King", "I prevail", 8, 18);
        hash.insert("Here for war", "Dragged under", 40, 12);
        hash.insert("Little wonder", "Architects", 56, 13);
        hash.insert("Agony", "heartbent", 72, 5);
    }
    
    public void testInsertAndResize()
    {
        
        assertEquals(10, hash.maxSize);
        assertEquals(5, hash.size);
//        System.out.println(hash.hFunc("jaded") % 10);
//        System.out.println(hash.hFunc("Long Live The King") % 10);
//        System.out.println(hash.hFunc("Here for war") % 10);
//        System.out.println(hash.hFunc("Little wonder") % 10);
//        System.out.println(hash.hFunc("Agony") % 10);
        assertEquals(0, hash.table[4].getMemIndex());
        assertEquals(8, hash.table[8].getMemIndex());
        assertEquals(40, hash.table[5].getMemIndex());
        assertEquals(56, hash.table[9].getMemIndex());
        assertEquals(72, hash.table[0].getMemIndex());
        
//        System.out.println(hash.hFunc("jaded") % 20);
//        System.out.println(hash.hFunc("Long Live The King") % 20);
//        System.out.println(hash.hFunc("Here for war") % 20);
//        System.out.println(hash.hFunc("Little wonder") % 20);
//        System.out.println(hash.hFunc("Agony") % 20);
//        System.out.println(hash.hFunc("Oracle") % 20);
        hash.insert("Oracle", "House & Home", 80, 6);
        assertEquals(20, hash.maxSize);
        assertEquals(6, hash.size);
        assertEquals(0, hash.table[14].getMemIndex());
        assertEquals(8, hash.table[8].getMemIndex());
        assertEquals(40, hash.table[4].getMemIndex());
        assertEquals(56, hash.table[9].getMemIndex());
        System.out.println(hash.table[9].getMemIndex());
        assertEquals(72, hash.table[10].getMemIndex());
        assertEquals(80, hash.table[3].getMemIndex());
    }
    
    public void testSearch() {
        
//        System.out.println(hash.hFunc("Two Faced") % 20);
//        System.out.println(hash.hFunc("Sinner") % 20);
//        System.out.println(hash.hFunc("Pale Moonlight") % 20);
//        System.out.println(hash.hFunc("Unraveling") % 20);
        hash.insert("Oracle", "House & Home", 80, 6);
        hash.insert("Two Faced", "Linkin park", 88, 9);
        hash.insert("Sinner", "Of Virtue", 104, 6);
        hash.insert("Pale Moonlight", "Dayseeker", 112, 14);
        hash.insert("Unraveling", "Muse", 128, 10);
        assertEquals(12, hash.search("Sinner"));
        assertEquals(12, hash.search(1852758968));
        assertEquals(-1, hash.search("tsunami sea"));
        hash.remove("Little wonder");
        assertEquals(12, hash.search("Sinner"));
    }
    
    public void testRemove() {
        
        assertEquals(null, hash.remove("tsunami sea"));
//        System.out.println(hash.table[4]);
        assertEquals(18, hash.remove(hash.hFunc("Long Live The King")).getByteSize());

    }
}
















