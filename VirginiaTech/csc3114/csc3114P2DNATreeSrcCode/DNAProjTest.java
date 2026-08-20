import student.TestCase;
import student.testingsupport.annotations.ScoringWeight;

/**
 * @author CS3114/5040 staff
 * @version Spring 2026
 */
public class DNAProjTest extends TestCase {
    private DNA it;

    /**
     * Sets up the tests that follow. In general, used for initialization
     */
    public void setUp() {
        it = new DNADB();
    }


    /**
     * Test output formatting
     */
    public void testSampleInput() {
        assertFuzzyEquals("Sequence |ACGT| inserted", it.insert("ACGT"));
        assertFuzzyEquals("Sequence |ACGT| already exists", it.insert("ACGT"));
        assertFuzzyEquals("Sequence |ACGT| removed", it.remove("ACGT"));
        assertFuzzyEquals("Sequence |AAAA| inserted", it.insert("AAAA"));
        assertFuzzyEquals("Sequence |AA| inserted", it.insert("AA"));
        assertFuzzyEquals("Sequence |ACG| does not exist", it.remove("ACG"));
        assertFuzzyEquals("tree dump:\r\n" + "I\r\n" + "  I\r\n" + "    I\r\n"
            + "      AAAA\r\n" + "      E\r\n" + "      E\r\n" + "      E\r\n"
            + "      AA\r\n" + "    E\r\n" + "    E\r\n" + "    E\r\n"
            + "    E\r\n" + "  E\r\n" + "  E\r\n" + "  E\r\n" + "  E", it
                .print());
        assertFuzzyEquals("tree dump with lengths:\r\n" + "I\r\n" + "  I\r\n"
            + "    I\r\n" + "      AAAA 4\r\n" + "      E\r\n" + "      E\r\n"
            + "      E\r\n" + "      AA 2\r\n" + "    E\r\n" + "    E\r\n"
            + "    E\r\n" + "    E\r\n" + "  E\r\n" + "  E\r\n" + "  E\r\n"
            + "  E", it.printLengths());
        System.out.println(it.printStats());
        assertFuzzyEquals("tree dump with stats:\r\n" + "I\r\n" + "  I\r\n"
            + "    I\r\n" + "      AAAA A:100.00 C:0.00 G:0.00 T:0.00\r\n"
            + "      E\r\n" + "      E\r\n" + "      E\r\n"
            + "      AA A:100.00 C:0.00 G:0.00 T:0.00\r\n" + "    E\r\n"
            + "    E\r\n" + "    E\r\n" + "    E\r\n" + "  E\r\n" + "  E\r\n"
            + "  E\r\n" + "  E", it.printStats());
        assertFuzzyEquals("AAAA\r\n" + "# of nodes visited: 4", it.search(
            "AAAA$"));
        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 4", it
            .search("AAA$"));
        assertFuzzyEquals("AAAA\r\n" + "AA\r\n" + "# of nodes visited: 8", it
            .search("AA"));
        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 3", it
            .search("ACGT$"));
    }


    /**
     * 
     * a new search test
     * 
     */
    public void testNewSearch() {

        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 1", it
            .search("A$"));
        System.out.println(it.search("A$"));
        System.out.println(" ");

        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 1", it
            .search(""));
        System.out.println(it.search(""));
        System.out.println(" ");

        it.insert("A");
        it.insert("AT");
        it.insert("AA");
        it.insert("AAAT");
        it.insert("AAG");
        it.insert("TG");
        it.insert("TA");
        it.insert("T");
        it.insert("GA");

        assertFuzzyEquals("A\r\n" + "# of nodes visited: 3", it.search("A$"));
        System.out.println(it.search("A$"));
        System.out.println(" ");

        assertFuzzyEquals("TA\r\n" + "# of nodes visited: 3", it.search("TA$"));
        System.out.println(it.search("TA$"));
        System.out.println(" ");

        assertFuzzyEquals("AAG\r\n" + "# of nodes visited: 4", it.search(
            "AAG$"));
        System.out.println(it.search("AAG$"));
        System.out.println(" ");

        assertFuzzyEquals("AAAT\r\n" + "AAG\r\n" + "AA\r\n" + "AT\r\n" + "A\r\n"
            + "# of nodes visited: 12", it.search("A"));
        System.out.println(it.search("A"));
        System.out.println(" ");

        assertFuzzyEquals("AAAT\r\n" + "AAG\r\n" + "AA\r\n"
            + "# of nodes visited: 8", it.search("AA"));
        System.out.println(it.search("AA"));
        System.out.println(" ");

        assertFuzzyEquals("AAAT\r\n" + "AAG\r\n" + "AA\r\n" + "AT\r\n" + "A\r\n"
            + "GA\r\n" + "TA\r\n" + "TG\r\n" + "T\r\n"
            + "# of nodes visited: 21", it.search(""));
        System.out.println(it.search(""));
        System.out.println(" ");

        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 2", it
            .search("$"));
        System.out.println(it.search("$"));
        System.out.println(" ");

        System.out.println(it.print());

        System.out.println("NEW TESTING");
        System.out.println(" ");

        it.remove("AAG");

        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 4", it
            .search("AAG$"));
        System.out.println(it.search("AAG$"));
        System.out.println(" ");

        assertFuzzyEquals("AAAT\r\n" + "AA\r\n" + "AT\r\n" + "A\r\n"
            + "# of nodes visited: 12", it.search("A"));
        System.out.println(it.search("A"));
        System.out.println(" ");

        assertFuzzyEquals("AAAT\r\n" + "AA\r\n" + "# of nodes visited: 8", it
            .search("AA"));
        System.out.println(it.search("AA"));
        System.out.println(" ");

        assertFuzzyEquals("AAAT\r\n" + "AA\r\n" + "AT\r\n" + "A\r\n" + "GA\r\n"
            + "TA\r\n" + "TG\r\n" + "T\r\n" + "# of nodes visited: 21", it
                .search(""));
        System.out.println(it.search(""));
        System.out.println(" ");

        System.out.println(it.print());

        System.out.println("NEW TESTING");
        System.out.println(" ");

        it.remove("A");
        it.remove("AT");
        it.remove("AA");
        it.remove("AAAT");
        it.remove("TG");
        it.remove("TA");
        it.remove("T");
        it.remove("GA");

        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 1", it
            .search(""));
        System.out.println(it.search(""));
        System.out.println(" ");

        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 1", it
            .search("A$"));
        System.out.println(it.search("A$"));
        System.out.println(" ");

        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 1", it
            .search("A"));
        System.out.println(it.search("A"));
        System.out.println(" ");

        System.out.println(it.print());
        System.out.println(" ");

        System.out.println("NEW TESTING");
        System.out.println(" ");

        assertFuzzyEquals("Bad Input Sequence  ", it.search(" "));
        System.out.println(it.search(" "));
        System.out.println(" ");

        assertFuzzyEquals("Bad input: Sequence may not be null", it.search(
            null));
        System.out.println(it.search(null));
        System.out.println(" ");

        assertFuzzyEquals("Bad Input Sequence  XYZ", it.search("XYZ"));
        System.out.println(it.search("XYZ"));
        System.out.println(" ");

        assertFuzzyEquals("Bad Input Sequence  A$A", it.search("A$A"));
        System.out.println(it.search("A$A"));
        System.out.println(" ");

        assertFuzzyEquals("Bad Input Sequence  C$$", it.search("C$$"));
        System.out.println(it.search("C$$"));
        System.out.println(" ");

        assertFuzzyEquals("Bad Input Sequence  C A", it.search("C A"));
        System.out.println(it.search("C A"));
        System.out.println(" ");

        System.out.println("NEW TESTING");
        System.out.println(" ");

        it.insert("A");

        assertFuzzyEquals("A\r\n" + "# of nodes visited: 1", it.search("A"));
        assertFuzzyEquals("A\r\n" + "# of nodes visited: 1", it.search("A$"));

        it.insert("T");

        assertFuzzyEquals("A\r\n" + "# of nodes visited: 2", it.search("A$"));
        System.out.println(it.search("A$"));
        System.out.println(" ");

        assertFuzzyEquals("A\r\n" + "# of nodes visited: 2", it.search("A"));
        System.out.println(it.search("A"));
        System.out.println(" ");

        it.remove("A");
        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 1", it
            .search("$"));
        assertFuzzyEquals("T\r\n" + "# of nodes visited: 1", it.search("T"));
        assertFuzzyEquals("T\r\n" + "# of nodes visited: 1", it.search("T$"));

        it = new DNADB();
        it.insert("ACCCCC");
        assertFuzzyEquals("ACCCCC\r\n" + "# of nodes visited: 1", it.search(
            "A"));
        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 1", it
            .search("A$"));
        assertFuzzyEquals("ACCCCC\r\n" + "# of nodes visited: 1", it.search(
            "ACCCCC$"));
        assertFuzzyEquals("ACCCCC\r\n" + "# of nodes visited: 1", it.search(
            "ACCCCC"));
    }


    /**
     * Tests the simple version of test
     */
    public void testSimpleSearch() {
        assertFuzzyEquals("No sequence found\r\n" + "# of nodes visited: 1", it
            .search("$"));
        it.insert("A");
        assertFuzzyEquals("A\r\n" + "# of nodes visited: 1", it.search("A$"));
        it.insert("C");
        assertFuzzyEquals("C\r\n" + "# of nodes visited: 2", it.search("C$"));
        it.insert("G");
        assertFuzzyEquals("G\r\n" + "# of nodes visited: 2", it.search("G$"));
        it.insert("T");
        assertFuzzyEquals("T\r\n" + "# of nodes visited: 2", it.search("T$"));
        it = new DNADB();
        it.insert("A");
        it.insert("C");
        it.insert("AC");
        it.insert("AGA");
        it.insert("ACGT");
        it.insert("TGCA");
        assertFuzzyEquals("A\r\n" + "# of nodes visited: 3", it.search("A$"));
        assertFuzzyEquals("C\r\n" + "# of nodes visited: 2", it.search("C$"));
        assertFuzzyEquals("AC\r\n" + "# of nodes visited: 4", it.search("AC$"));
        assertFuzzyEquals("AGA\r\n" + "# of nodes visited: 3", it.search(
            "AGA$"));
        assertFuzzyEquals("ACGT\r\n" + "# of nodes visited: 4", it.search(
            "ACGT$"));
        assertFuzzyEquals("TGCA\r\n" + "# of nodes visited: 2", it.search(
            "TGCA$"));

        it = new DNADB();
        it.insert("A");
        it.insert("AA");
        assertFuzzyEquals("A\r\n" + "# of nodes visited: 3", it.search("A$"));
        assertFuzzyEquals("AA\r\n" + "# of nodes visited: 3", it.search("AA$"));
        assertFuzzyEquals("AA\r\nA\r\n" + "# of nodes visited: 7", it.search(
            "A"));
        it.insert("AAA");
        assertFuzzyEquals("AAA\r\nAA\r\nA\r\n" + "# of nodes visited: 12", it
            .search("A"));

        it = new DNADB();
        it.insert("T");
        assertFuzzyEquals("T\r\n" + "# of nodes visited: 1", it.search("T$"));
        it.insert("TTT");
        assertFuzzyEquals("TTT\r\n" + "# of nodes visited: 3", it.search(
            "TTT$"));
        it.insert("TTG");
        assertFuzzyEquals("TTG\r\n" + "# of nodes visited: 4", it.search(
            "TTG$"));
        it.insert("TTA");
        assertFuzzyEquals("TTA\r\n" + "# of nodes visited: 4", it.search(
            "TTA$"));
        it.insert("TTC");
        assertFuzzyEquals("TTC\r\n" + "# of nodes visited: 4", it.search(
            "TTC$"));
        it.insert("TT");
        assertFuzzyEquals("TT\r\n" + "# of nodes visited: 4", it.search("TT$"));
        assertFuzzyEquals("T\r\n" + "# of nodes visited: 3", it.search("T$"));
        assertFuzzyEquals("TTT\r\n" + "# of nodes visited: 4", it.search(
            "TTT$"));
    }


    /**
     * Tests the recursive version of search
     */
    public void testSearchAll() {
        it.insert("A");
        it.insert("AT");
        it.insert("AA");
        it.insert("AAAT");
        it.insert("AAG");
        it.insert("TG");
        it.insert("TA");
        it.insert("T");
        it.insert("GA");

        assertFuzzyEquals("AA\r\n" + "# of nodes visited: 4", it.search("AA$"));
        assertFuzzyEquals("AT\r\n" + "# of nodes visited: 3", it.search("AT$"));
        assertFuzzyEquals("AAAT\r\n" + "AAG\r\n" + "AA\r\n"
            + "# of nodes visited: 8", it.search("AA"));
        assertFuzzyEquals("TA\r\n" + "TG\r\n" + "T\r\n"
            + "# of nodes visited: 7", it.search("T"));
        assertFuzzyEquals("AAAT\r\n" + "AAG\r\n" + "AA\r\n" + "AT\r\n" + "A\r\n"
            + "GA\r\n" + "TA\r\n" + "TG\r\n" + "T\r\n"
            + "# of nodes visited: 21", it.search(""));

    }


    /**
     * Example tests for bad input error formatting
     */
    public void testBadInput() {
        assertFuzzyEquals("testBadInput",
            "Bad input: Sequence may not be null\r\n", it.insert(null));
        assertFuzzyEquals("testBadInput",
            "Bad input: Sequence may not be empty\r\n", it.insert(""));
        assertFuzzyEquals("testBadInput", "Bad Input Sequence |AXA|\r\n", it
            .insert("AXA"));
        assertFuzzyEquals("testBadInput", "Bad Input Sequence |A A|\r\n", it
            .insert("A A"));
        assertFuzzyEquals("testBadInput", "Bad Input Sequence |A |\r\n", it
            .insert("A "));
        assertFuzzyEquals("testBadInput", "Bad Input Sequence |A$|\r\n", it
            .insert("A$"));
        assertFuzzyEquals("testBadInput", "Bad input sequence |A$A|\r\n", it
            .search("A$A"));
        assertFuzzyEquals("testBadInput",
            "Bad input: Sequence may not be null\r\n", it.search(null));
        assertFuzzyEquals("testBadInput", "No sequence found\r\n"
            + "# of nodes visited: 1", it.search(""));
        assertFuzzyEquals("testBadInput", "Bad input sequence |A$A|\r\n", it
            .remove("A$A"));
        assertFuzzyEquals("testBadInput",
            "Bad input: Sequence may not be null\r\n", it.remove(null));
        assertFuzzyEquals("testBadInput",
            "Bad input: Sequence may not be empty", it.remove(""));
        assertFuzzyEquals("testBadInput", "Bad input sequence |A$A$|\r\n", it
            .search("A$A$"));
    }


    /**
     * Tests the remove function
     */
    public void testRemove() {
        it.insert("A");
        it.insert("C");
        assertFuzzyEquals("Sequence |A| removed\r\n", it.remove("A"));
        assertFuzzyEquals("Sequence |C| removed\r\n", it.remove("C"));

        it.insert("A");
        it.insert("AA");
        it.insert("C");
        assertFuzzyEquals("Sequence |A| removed\r\n", it.remove("A"));
        assertFuzzyEquals("tree dump:\r\n" + "I\r\n" + "AA\r\n" + "  C\r\n"
            + "  E\r\n" + "  E\r\n" + "  E\r\n", it.print());

        it.insert("G");
        it.insert("T");
        it.insert("A");
        assertFuzzyEquals("Sequence |G| removed\r\n", it.remove("G"));
        assertFuzzyEquals("Sequence |T| removed\r\n", it.remove("T"));
        assertFuzzyEquals("Sequence |A| removed\r\n", it.remove("A"));
        assertFuzzyEquals("Sequence |AA| removed\r\n", it.remove("AA"));
        assertFuzzyEquals("Sequence |C| removed\r\n", it.remove("C"));

        it.insert("A");
        it.insert("C");
        it.insert("G");
        assertFuzzyEquals("Sequence |A| removed\r\n", it.remove("A"));
        assertFuzzyEquals("Sequence |C| removed\r\n", it.remove("C"));
        assertFuzzyEquals("Sequence |G| removed\r\n", it.remove("G"));

        it.insert("A");
        it.insert("G");
        it.insert("T");
        assertFuzzyEquals("Sequence |A| removed\r\n", it.remove("A"));
        assertFuzzyEquals("Sequence |G| removed\r\n", it.remove("G"));
        assertFuzzyEquals("Sequence |T| removed\r\n", it.remove("T"));

        it.insert("A");
        it.insert("AA");
        it.insert("AAA");
        assertFuzzyEquals("Sequence |AAA| removed\r\n", it.remove("AAA"));
        assertFuzzyEquals("Sequence |AA| removed\r\n", it.remove("AA"));
        assertFuzzyEquals("Sequence |A| removed\r\n", it.remove("A"));

        it = new DNADB();
        it.insert("A");
        it.insert("C");
        it.insert("G");
        it.insert("T");
        it.insert("AA");
        it.insert("AC");
        it.insert("AG");
        it.insert("AT");
        it.insert("CA");
        it.insert("CC");
        it.insert("CG");
        it.insert("CT");
        it.insert("GA");
        it.insert("GC");
        it.insert("GG");
        it.insert("GT");
        it.insert("TA");
        it.insert("TC");
        it.insert("TG");
        it.insert("TT");
        assertFuzzyEquals("Sequence |AA| removed\r\n", it.remove("AA"));
        it.insert("AA");
        assertFuzzyEquals("Sequence |AC| removed\r\n", it.remove("AC"));
        it.insert("AC");
        assertFuzzyEquals("Sequence |AG| removed\r\n", it.remove("AG"));
        it.insert("AG");
        assertFuzzyEquals("Sequence |AT| removed\r\n", it.remove("AT"));
        it.insert("AT");

        assertFuzzyEquals("Sequence |CA| removed\r\n", it.remove("CA"));
        it.insert("CA");
        assertFuzzyEquals("Sequence |CC| removed\r\n", it.remove("CC"));
        it.insert("CC");
        assertFuzzyEquals("Sequence |CG| removed\r\n", it.remove("CG"));
        it.insert("CG");
        assertFuzzyEquals("Sequence |CT| removed\r\n", it.remove("CT"));
        it.insert("CT");

        assertFuzzyEquals("Sequence |GA| removed\r\n", it.remove("GA"));
        it.insert("GA");
        assertFuzzyEquals("Sequence |GC| removed\r\n", it.remove("GC"));
        it.insert("GC");
        assertFuzzyEquals("Sequence |GG| removed\r\n", it.remove("GG"));
        it.insert("GG");
        assertFuzzyEquals("Sequence |GT| removed\r\n", it.remove("GT"));
        it.insert("GT");

        assertFuzzyEquals("Sequence |TA| removed\r\n", it.remove("TA"));
        it.insert("TA");
        assertFuzzyEquals("Sequence |TC| removed\r\n", it.remove("TC"));
        it.insert("TC");
        assertFuzzyEquals("Sequence |TG| removed\r\n", it.remove("TG"));
        it.insert("TG");
        assertFuzzyEquals("Sequence |TT| removed\r\n", it.remove("TT"));
        it.insert("TT");

        assertFuzzyEquals("Sequence |A| removed\r\n", it.remove("A"));
        it.insert("A");
        assertFuzzyEquals("Sequence |C| removed\r\n", it.remove("C"));
        it.insert("C");
        assertFuzzyEquals("Sequence |G| removed\r\n", it.remove("G"));
        it.insert("G");
        assertFuzzyEquals("Sequence |T| removed\r\n", it.remove("T"));
        it.insert("T");

        it = new DNADB();
        it.insert("A");
        it.insert("C");
        it.insert("G");
        assertFuzzyEquals("Sequence |G| removed\r\n", it.remove("G"));

        it = new DNADB();
        it.insert("A");
        assertFuzzyEquals("Sequence |A| removed\r\n", it.remove("A"));
        it.insert("C");
        assertFuzzyEquals("Sequence |C| removed\r\n", it.remove("C"));
        it.insert("G");
        assertFuzzyEquals("Sequence |G| removed\r\n", it.remove("G"));
        it.insert("T");
        assertFuzzyEquals("Sequence |T| removed\r\n", it.remove("T"));

        it = new DNADB();
        it.insert("T");
        it.insert("TT");
        it.insert("TA");
        assertFuzzyEquals("Sequence |T| removed\r\n", it.remove("T"));

        it = new DNADB();
        it.insert("T");
        it.insert("TT");
        it.insert("TC");
        assertFuzzyEquals("Sequence |T| removed\r\n", it.remove("T"));

        it = new DNADB();
        it.insert("T");
        it.insert("TT");
        it.insert("TG");
        assertFuzzyEquals("Sequence |T| removed\r\n", it.remove("T"));

        it = new DNADB();
        it.insert("A");
        it.insert("AA");
        it.insert("AAA");
        assertFuzzyEquals("Sequence |A| removed\r\n", it.remove("A"));
        assertFuzzyEquals("Sequence |AA| removed\r\n", it.remove("AA"));
        assertFuzzyEquals("Sequence |AAA| removed\r\n", it.remove("AAA"));

        it.insert("C");
        it.insert("CC");
        it.insert("CCC");
        assertFuzzyEquals("Sequence |CC| removed\r\n", it.remove("CC"));
        assertFuzzyEquals("Sequence |CCC| removed\r\n", it.remove("CCC"));
        assertFuzzyEquals("Sequence |C| removed\r\n", it.remove("C"));

        it.insert("G");
        it.insert("GG");
        it.insert("GGG");
        assertFuzzyEquals("Sequence |GG| removed\r\n", it.remove("GG"));
        assertFuzzyEquals("Sequence |GGG| removed\r\n", it.remove("GGG"));
        assertFuzzyEquals("Sequence |G| removed\r\n", it.remove("G"));

        it.insert("T");
        it.insert("TT");
        it.insert("TTT");
        assertFuzzyEquals("Sequence |TT| removed\r\n", it.remove("TT"));
        assertFuzzyEquals("Sequence |TTT| removed\r\n", it.remove("TTT"));
        assertFuzzyEquals("Sequence |T| removed\r\n", it.remove("T"));

        it.insert("A");
        it.insert("AA");
        it.insert("AAA");
        it.insert("AAC");
        assertFuzzyEquals("tree dump:\r\n" + "I\r\n" + "  I\r\n" + "    I\r\n"
            + "      AAA\r\n" + "      AAC\r\n" + "      E\r\n" + "      E\r\n"
            + "      AA\r\n" + "    E\r\n" + "    E\r\n" + "    E\r\n"
            + "    A\r\n" + "  E\r\n" + "  E\r\n" + "  E\r\n" + "  E\r\n", it
                .print());
        assertFuzzyEquals("AAA\r\n" + "AAC\r\n" + "AA\r\n" + "A\r\n"
            + "# of nodes visited: 16", it.search(""));
        assertFuzzyEquals("Sequence |AA| removed\r\n", it.remove("AA"));
        assertFuzzyEquals("tree dump:\r\n" + "I\r\n" + "  I\r\n" + "    I\r\n"
            + "      AAA\r\n" + "      AAC\r\n" + "      E\r\n" + "      E\r\n"
            + "      E\r\n" + "    E\r\n" + "    E\r\n" + "    E\r\n"
            + "    A\r\n" + "  E\r\n" + "  E\r\n" + "  E\r\n" + "  E\r\n", it
                .print());
        assertFuzzyEquals("AAA\r\n" + "AAC\r\n" + "A\r\n"
            + "# of nodes visited: 16", it.search(""));
        assertFuzzyEquals("Sequence |AAA| removed\r\n", it.remove("AAA"));
        assertFuzzyEquals("tree dump:\r\n" + "I\r\n" + "  I\r\n" + "    AAC\r\n"
            + "    E\r\n" + "    E\r\n" + "    E\r\n" + "    A\r\n" + "  E\r\n"
            + "  E\r\n" + "  E\r\n" + "  E\r\n", it.print());
        assertFuzzyEquals("AAC\r\n" + "A\r\n" + "# of nodes visited: 11", it
            .search(""));
        assertFuzzyEquals("Sequence |A| removed\r\n", it.remove("A"));
        assertFuzzyEquals("tree dump:\r\n" + "AAC\r\n", it.print());
        assertFuzzyEquals("AAC\r\n" + "# of nodes visited: 1", it.search(""));
        assertFuzzyEquals("Sequence |AAC| removed\r\n", it.remove("AAC"));

        it.insert("C");
        it.insert("CC");
        it.insert("CCC");
        it.insert("CCA");
        assertFuzzyEquals("Sequence |CC| removed\r\n", it.remove("CC"));
        assertFuzzyEquals("Sequence |CCC| removed\r\n", it.remove("CCC"));
        assertFuzzyEquals("Sequence |C| removed\r\n", it.remove("C"));
        assertFuzzyEquals("Sequence |CCA| removed\r\n", it.remove("CCA"));

        it.insert("G");
        it.insert("GG");
        it.insert("GGG");
        it.insert("GGA");
        assertFuzzyEquals("Sequence |GG| removed\r\n", it.remove("GG"));
        assertFuzzyEquals("Sequence |GGG| removed\r\n", it.remove("GGG"));
        assertFuzzyEquals("Sequence |G| removed\r\n", it.remove("G"));
        assertFuzzyEquals("Sequence |GGA| removed\r\n", it.remove("GGA"));

        it.insert("T");
        it.insert("TT");
        it.insert("TTT");
        it.insert("TTA");
        assertFuzzyEquals("Sequence |TT| removed\r\n", it.remove("TT"));
        assertFuzzyEquals("Sequence |TTT| removed\r\n", it.remove("TTT"));
        assertFuzzyEquals("Sequence |T| removed\r\n", it.remove("T"));
        assertFuzzyEquals("Sequence |TTA| removed\r\n", it.remove("TTA"));

        it = new DNADB();
        it.insert("C");
        it.insert("AA");
        it.insert("AG");
        assertFuzzyEquals("tree dump:\r\n" + "I\r\n" + "  I\r\n" + "    AA\r\n"
            + "    E\r\n" + "    AG\r\n" + "    E\r\n" + "    E\r\n" + "  C\r\n"
            + "  E\r\n" + "  E\r\n" + "  E\r\n", it.print());
        assertFuzzyEquals("Sequence |AG| removed\r\n", it.remove("AG"));
        assertFuzzyEquals("tree dump:\r\n" + "I\r\n" + "  AA\r\n" + "  C\r\n"
            + "  E\r\n" + "  E\r\n" + "  E\r\n", it.print());
    }


    /**
     * Tests the print function
     */
    public void testPrint() {
        it.insert("CG");
        it.insert("GC");
        it.insert("TC");

        assertFuzzyEquals("tree dump with stats:\r\n" + "I\r\n" + "  E\r\n"
            + "  CG A:0.00 C:50.00 G:50.00 T:0.00\r\n"
            + "  GC A:0.00 C:50.00 G:50.00 T:0.00\r\n"
            + "  TC A:0.00 C:50.00 G:0.00 T:50.00\r\n" + "  E", it
                .printStats());

    }


    /**
     * Tests the insert function
     */
    public void testBasicInput() {

        assertFuzzyEquals("Sequence |A| inserted", it.insert("A"));

        assertFuzzyEquals("Sequence |C| inserted", it.insert("C"));

        assertFuzzyEquals("Sequence |G| inserted", it.insert("G"));

        assertFuzzyEquals("Sequence |T| inserted", it.insert("T"));

        assertFuzzyEquals("Sequence |AA| inserted", it.insert("AA"));

        assertFuzzyEquals("Sequence |AC| inserted", it.insert("AC"));

        assertFuzzyEquals("Sequence |AG| inserted", it.insert("AG"));

        assertFuzzyEquals("Sequence |AT| inserted", it.insert("AT"));

        assertFuzzyEquals("Sequence |CA| inserted", it.insert("CA"));

        assertFuzzyEquals("Sequence |CC| inserted", it.insert("CC"));

        assertFuzzyEquals("Sequence |CG| inserted", it.insert("CG"));

        assertFuzzyEquals("Sequence |CT| inserted", it.insert("CT"));

        assertFuzzyEquals("Sequence |GA| inserted", it.insert("GA"));

        assertFuzzyEquals("Sequence |GC| inserted", it.insert("GC"));

        assertFuzzyEquals("Sequence |GG| inserted", it.insert("GG"));

        assertFuzzyEquals("Sequence |GT| inserted", it.insert("GT"));

        assertFuzzyEquals("Sequence |TA| inserted", it.insert("TA"));

        assertFuzzyEquals("Sequence |TC| inserted", it.insert("TC"));

        assertFuzzyEquals("Sequence |TG| inserted", it.insert("TG"));

        assertFuzzyEquals("Sequence |TT| inserted", it.insert("TT"));

        assertFuzzyEquals("Sequence |AAA| inserted", it.insert("AAA"));

        assertFuzzyEquals("Sequence |AAC| inserted", it.insert("AAC"));

        assertFuzzyEquals("Sequence |AAG| inserted", it.insert("AAG"));

        assertFuzzyEquals("Sequence |AAT| inserted", it.insert("AAT"));

        assertFuzzyEquals("Sequence |ACA| inserted", it.insert("ACA"));

        assertFuzzyEquals("Sequence |ACC| inserted", it.insert("ACC"));

        assertFuzzyEquals("Sequence |ACG| inserted", it.insert("ACG"));

        assertFuzzyEquals("Sequence |ACT| inserted", it.insert("ACT"));

        assertFuzzyEquals("Sequence |AGA| inserted", it.insert("AGA"));

        assertFuzzyEquals("Sequence |AGC| inserted", it.insert("AGC"));

        assertFuzzyEquals("Sequence |AGG| inserted", it.insert("AGG"));

        assertFuzzyEquals("Sequence |AGT| inserted", it.insert("AGT"));

        assertFuzzyEquals("Sequence |ATA| inserted", it.insert("ATA"));

        assertFuzzyEquals("Sequence |ATC| inserted", it.insert("ATC"));

        assertFuzzyEquals("Sequence |ATG| inserted", it.insert("ATG"));

        assertFuzzyEquals("Sequence |ATT| inserted", it.insert("ATT"));

        assertFuzzyEquals("Sequence |CAA| inserted", it.insert("CAA"));

        assertFuzzyEquals("Sequence |CAC| inserted", it.insert("CAC"));

        assertFuzzyEquals("Sequence |CAG| inserted", it.insert("CAG"));

        assertFuzzyEquals("Sequence |CAT| inserted", it.insert("CAT"));

        assertFuzzyEquals("Sequence |CCA| inserted", it.insert("CCA"));

        assertFuzzyEquals("Sequence |CCC| inserted", it.insert("CCC"));

        assertFuzzyEquals("Sequence |CCG| inserted", it.insert("CCG"));

        assertFuzzyEquals("Sequence |CCT| inserted", it.insert("CCT"));

        assertFuzzyEquals("Sequence |CGA| inserted", it.insert("CGA"));

        assertFuzzyEquals("Sequence |CGC| inserted", it.insert("CGC"));

        assertFuzzyEquals("Sequence |CGG| inserted", it.insert("CGG"));

        assertFuzzyEquals("Sequence |CGT| inserted", it.insert("CGT"));

        assertFuzzyEquals("Sequence |CTA| inserted", it.insert("CTA"));

        assertFuzzyEquals("Sequence |CTC| inserted", it.insert("CTC"));

        assertFuzzyEquals("Sequence |CTG| inserted", it.insert("CTG"));

        assertFuzzyEquals("Sequence |CTT| inserted", it.insert("CTT"));

        assertFuzzyEquals("Sequence |GAA| inserted", it.insert("GAA"));

        assertFuzzyEquals("Sequence |GAC| inserted", it.insert("GAC"));

        assertFuzzyEquals("Sequence |GAG| inserted", it.insert("GAG"));

        assertFuzzyEquals("Sequence |GAT| inserted", it.insert("GAT"));

        assertFuzzyEquals("Sequence |GCA| inserted", it.insert("GCA"));

        assertFuzzyEquals("Sequence |GCC| inserted", it.insert("GCC"));

        assertFuzzyEquals("Sequence |GCG| inserted", it.insert("GCG"));

        assertFuzzyEquals("Sequence |GCT| inserted", it.insert("GCT"));

        assertFuzzyEquals("Sequence |GGA| inserted", it.insert("GGA"));

        assertFuzzyEquals("Sequence |GGC| inserted", it.insert("GGC"));

        assertFuzzyEquals("Sequence |GGG| inserted", it.insert("GGG"));

        assertFuzzyEquals("Sequence |GGT| inserted", it.insert("GGT"));

        assertFuzzyEquals("Sequence |GTA| inserted", it.insert("GTA"));

        assertFuzzyEquals("Sequence |GTC| inserted", it.insert("GTC"));

        assertFuzzyEquals("Sequence |GTG| inserted", it.insert("GTG"));

        assertFuzzyEquals("Sequence |GTT| inserted", it.insert("GTT"));

        assertFuzzyEquals("Sequence |TAA| inserted", it.insert("TAA"));

        assertFuzzyEquals("Sequence |TAC| inserted", it.insert("TAC"));

        assertFuzzyEquals("Sequence |TAG| inserted", it.insert("TAG"));

        assertFuzzyEquals("Sequence |TAT| inserted", it.insert("TAT"));

        assertFuzzyEquals("Sequence |TCA| inserted", it.insert("TCA"));

        assertFuzzyEquals("Sequence |TCC| inserted", it.insert("TCC"));

        assertFuzzyEquals("Sequence |TCG| inserted", it.insert("TCG"));

        assertFuzzyEquals("Sequence |TCT| inserted", it.insert("TCT"));

        assertFuzzyEquals("Sequence |TGA| inserted", it.insert("TGA"));

        assertFuzzyEquals("Sequence |TGC| inserted", it.insert("TGC"));

        assertFuzzyEquals("Sequence |TGG| inserted", it.insert("TGG"));

        assertFuzzyEquals("Sequence |TGT| inserted", it.insert("TGT"));

        assertFuzzyEquals("Sequence |TTA| inserted", it.insert("TTA"));

        assertFuzzyEquals("Sequence |TTC| inserted", it.insert("TTC"));

        assertFuzzyEquals("Sequence |TTG| inserted", it.insert("TTG"));

        assertFuzzyEquals("Sequence |TTT| inserted", it.insert("TTT"));

        assertFuzzyEquals("Sequence |TTT| already exists", it.insert("TTT"));
        assertFuzzyEquals("Sequence |AGT| already exists", it.insert("AGT"));
        assertFuzzyEquals("Sequence |A| already exists", it.insert("A"));
        assertFuzzyEquals("Sequence |TA| already exists", it.insert("TA"));
    }

// assertFuzzyEquals(
// "Sequence |AC| inserted",
// it.insert("AC"));
// assertFuzzyEquals(
// "Sequence |AG| inserted",
// it.insert("AG"));
// assertFuzzyEquals(
// "Sequence |TA| inserted",
// it.insert("TA"));
// assertFuzzyEquals(
// "Sequence |A| inserted",
// it.insert("A"));
// assertFuzzyEquals(
// "Sequence |CCA| inserted",
// it.insert("CCA"));
// assertFuzzyEquals(
// "Sequence |GAA| inserted",
// it.insert("GAA"));
// assertFuzzyEquals(
// "Sequence |ACGTGCA| inserted",
// it.insert("ACGTGCA"));
}
