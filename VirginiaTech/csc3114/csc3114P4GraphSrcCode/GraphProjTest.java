import java.io.IOException;
import student.TestCase;
import student.TestableRandom;

/**
 * Graph project tests
 *
 * @author CS3114/5040 Staff
 * @version Spring 2026
 */
public class GraphProjTest extends TestCase {
    private GPInterface it;

    /**
     * Set up the tests that follow.
     */
    public void setUp() { // Nothing needed yet

    }


    // ----------------------------------------------------------
    /**
     * Test various bad inputs
     *
     * @throws Exception
     */
    public void testBadInput() throws Exception {
        it = new GraphDB();
        assertFalse(it.clear()); // Not been initialized yet
        assertFuzzyEquals("Initial hash table size must be positive", it.create(
            -1));

        assertFuzzyEquals("Database not initialized", it.insert("a", "b"));
        assertFuzzyEquals("Database not initialized", it.remove("song", "a"));
        assertFuzzyEquals("Database not initialized", it.print("artist"));

        assertNull(it.create(32));
        assertFuzzyEquals("Database already exists", it.create(32));
        assertFuzzyEquals("Bad print parameter", it.print("dum"));
        assertFuzzyEquals("Bad type value |Dum| on remove", it.remove("Dum",
            "Dum"));

        assertFuzzyEquals("Input strings cannot be null or empty", it.print(
            ""));
        assertFuzzyEquals("Input strings cannot be null or empty", it.print(
            null));

        assertFuzzyEquals("Input strings cannot be null or empty", it.insert("",
            "b"));
        assertFuzzyEquals("Input strings cannot be null or empty", it.insert(
            null, "b"));
        assertFuzzyEquals("Input strings cannot be null or empty", it.insert(
            "a", ""));
        assertFuzzyEquals("Input strings cannot be null or empty", it.insert(
            "a", null));

        assertFuzzyEquals("Input strings cannot be null or empty", it.remove(
            "song", ""));
        assertFuzzyEquals("Input strings cannot be null or empty", it.remove(
            "song", null));
        assertFuzzyEquals("Input strings cannot be null or empty", it.remove("",
            "a"));
        assertFuzzyEquals("Input strings cannot be null or empty", it.remove(
            null, "a"));
    }


    // ----------------------------------------------------------
    /**
     * Test various uses of empty or missing data
     *
     * @throws Exception
     */
    public void testEmpty() throws Exception {
        it = new GraphDB();
        it.create(10);

        assertFuzzyEquals("total artists: 0", it.print("artist"));
        assertFuzzyEquals("total songs: 0", it.print("song"));
        it.insert("Hello World", "Hello World2");
        assertFuzzyEquals("|Dum| does not exist in the Artist database", it
            .remove("artist", "Dum"));
        assertFuzzyEquals("|Dum| does not exist in the song database", it
            .remove("song", "Dum"));
    }


    // ----------------------------------------------------------
    /**
     * Define output formats
     *
     * @throws Exception
     */
    public void testSampleInput() throws Exception {
        it = new GraphDB();
        it.create(10);

        assertFuzzyEquals(
            "|When Summer's Through| does not exist in the Song database", it
                .remove("song", "When Summer's Through"));
        assertFuzzyEquals("total songs: 0\r\n", it.print("song"));
        assertFuzzyEquals("total artists: 0\r\n", it.print("artist"));
        assertFuzzyEquals("There are 0 connected components\r\n"
            + "The largest connected component has 0 elements\r\n"
            + "The diameter of the largest component is 0\r\n", it
                .printgraph());
        assertFuzzyEquals(
            "|Blind Lemon Jefferson| is added to the Artist database\r\n"
                + "|Long Lonesome Blues| is added to the Song database\r\n", it
                    .insert("Blind Lemon Jefferson", "Long Lonesome Blues"));
        assertFuzzyEquals(
            "|Blind Lemon Jefferson<SEP>Long Lonesome Blues| duplicates a record already in the database\r\n",
            it.insert("Blind Lemon Jefferson", "Long Lonesome Blues"));
        assertFuzzyEquals(
            "|Long   Lonesome Blues| is added to the Song database\r\n", it
                .insert("Blind Lemon Jefferson", "Long   Lonesome Blues"));
        assertFuzzyEquals(
            "|long Lonesome Blues| is added to the Song database\r\n", it
                .insert("Blind Lemon Jefferson", "long Lonesome Blues"));
        assertFuzzyEquals("|Ma Rainey| is added to the Artist database\r\n"
            + "|Ma Rainey's Black Bottom| is added to the Song database\r\n", it
                .insert("Ma Rainey", "Ma Rainey's Black Bottom"));
        assertFuzzyEquals("", it.insert("Ma Rainey", "Long Lonesome Blues"));
        assertFuzzyEquals(
            "|Mississippi Boweavil Blues| is added to the Song database\r\n", it
                .insert("Ma Rainey", "Mississippi Boweavil Blues"));
        assertFuzzyEquals("song hash table size doubled\r\n"
            + "|Fixin' To Die Blues| is added to the Song database\r\n", it
                .insert("Ma Rainey", "Fixin' To Die Blues"));
        assertFuzzyEquals("0: |Blind Lemon Jefferson|\r\n"
            + "7: |Ma Rainey|\r\n" + "total artists: 2\r\n", it.print(
                "artist"));
        assertFuzzyEquals("1: |Fixin' To Die Blues|\r\n"
            + "2: |Mississippi Boweavil Blues|\r\n"
            + "7: |long Lonesome Blues|\r\n" + "15: |Long Lonesome Blues|\r\n"
            + "16: |Ma Rainey's Black Bottom|\r\n"
            + "19: |Long   Lonesome Blues|\r\n" + "total songs: 6\r\n", it
                .print("song"));
        assertFuzzyEquals("There are 1 connected components\r\n"
            + "The largest connected component has 8 elements\r\n"
            + "The diameter of the largest component is 4\r\n", it
                .printgraph());
        assertFuzzyEquals("|Sleepy| does not exist in the Song database\r\n", it
            .remove("song", "Sleepy"));
        assertFuzzyEquals(
            "|ma rainey| does not exist in the Artist database\r\n", it.remove(
                "artist", "ma rainey"));
        assertFuzzyEquals("|Ma Rainey| is removed from the Artist database\r\n",
            it.remove("artist", "Ma Rainey"));
        assertFuzzyEquals("0: |Blind Lemon Jefferson|\r\n" + "7: TOMBSTONE\r\n"
            + "total artists: 1\r\n", it.print("artist"));
        assertFuzzyEquals(
            "|Barenaked Ladies| is added to the Artist database\r\n"
                + "|Sarah Yellin| is added to the Song database\r\n", it.insert(
                    "Barenaked Ladies", "Sarah Yellin"));
        assertFuzzyEquals(
            "|Gerard Lenorman| is added to the Artist database\r\n"
                + "Graph size doubled to 20\r\n"
                + "|Oversleeping| is added to the Song database", it.insert(
                    "Gerard Lenorman", "Oversleeping"));
        assertTrue(it.clear());
    }


    public void testPrintGraphBasic() throws IOException {
        it = new GraphDB();
        it.create(10);

        it.insert("Spiritbox", "Jaded");
        it.insert("Spiritbox", "Void");
        it.insert("Three Days Grace", "World so Cold");
        it.insert("12 Stones", "World so Cold");

        assertFuzzyEquals("There are 2 connected components\n"
            + "The largest connected component has 3 elements\n"
            + "The diameter of the largest component is 2\n", it.printgraph());
    }


    public void testRemoveBasic() throws Exception {
        it = new GraphDB();
        it.create(10);

        it.insert("Spiritbox", "Jaded");
        it.insert("Spiritbox", "Void");
        it.insert("Three Days Grace", "World so Cold");
        it.insert("12 Stones", "World so Cold");

        assertFuzzyEquals("|Sleepy| does not exist in the Song database\r\n", it
            .remove("song", "Sleepy"));
        assertFuzzyEquals(
            "|Fall out Boy| does not exist in the Artist database\r\n", it
                .remove("artist", "Fall out Boy"));
        assertFuzzyEquals("|12 Stones| is removed from the Artist database\r\n",
            it.remove("artist", "12 Stones"));
        assertFuzzyEquals("2: |TOMBSTONE|\r\n" + "4: |Spiritbox|\r\n"
            + "6: |Three Days Grace|\r\n" + "total artists: 2\r\n", it.print(
                "artist"));
        assertFuzzyEquals("|Jaded| is removed from the Song database\r\n", it
            .remove("song", "Jaded"));
        assertFuzzyEquals("1: |World So Cold|\r\n" + "2: |Tombstone|\r\n"
            + "3: |Void|\r\n" + "total songs: 2\r\n", it.print("song"));
        assertFuzzyEquals("there are 2 connected components\r\n"
            + "the largest connected component has 2 elements\r\n"
            + "the diameter of the largest component is 1\r\n", it
                .printgraph());
    }


    public void testInsertBasic() throws Exception {
        it = new GraphDB();
        it.create(10);

        assertFuzzyEquals("|Spiritbox| is added to the Artist database\r\n"
            + "|Jaded| is added to the Song database\r\n", it.insert(
                "Spiritbox", "Jaded"));
        assertFuzzyEquals("|Void| is added to the Song database\r\n", it.insert(
            "Spiritbox", "Void"));
        assertFuzzyEquals(
            "|Three Days Grace| is added to the Artist database\r\n"
                + "|World so Cold| is added to the Song database\r\n", it
                    .insert("Three Days Grace", "World so Cold"));
        assertFuzzyEquals("|12 Stones| is added to the Artist database\r\n", it
            .insert("12 Stones", "World so Cold"));
        assertFuzzyEquals("1: |World so Cold|\r\n" + "2: |Jaded|\r\n"
            + "3: |Void|\r\n" + "total songs: 3\r\n", it.print("song"));
        assertFuzzyEquals("2: |12 Stones|\r\n" + "4: |Spiritbox|\r\n"
            + "6: |Three Days Grace|\r\n" + "total artists: 3\r\n", it.print(
                "artist"));
    }


    public void testResize() throws Exception {
        it = new GraphDB();
        it.create(3);

        it.insert("Spiritbox", "Jaded");
        it.insert("Spiritbox", "Void");
        it.insert("Three Days Grace", "World so Cold");
        it.insert("12 Stones", "World so Cold");
        it.insert("Fall Out Boy", "Jet Pack Blues");

        assertFuzzyEquals("3: |Jet pack blues|\r\n" + "6: |void|\r\n"
            + "10: |jaded|\r\n" + "11: |world so cold|\r\n"
            + "total songs: 4\r\n", it.print("song"));
        assertFuzzyEquals("4: |spiritbox|\r\n" + "8: |three days grace|\r\n"
            + "9: |12 stones|\r\n" + "10: |Fall Out Boy|\r\n"
            + "total artists: 4\r\n", it.print("artist"));
    }


    public void testRemoveComplex() throws Exception {

        it = new GraphDB();
        it.create(10);

        it.insert("Spiritbox", "Jaded");
        it.insert("Spiritbox", "Void");
        it.insert("Three Days Grace", "World so Cold");
        it.insert("12 Stones", "World so Cold");
        it.insert("Fall Out Boy", "Jet Pack Blues");
        it.insert("Spiritbox", "Tsunami Sea");
        it.insert("Spiritbox", "Black Rainbow");
        it.insert("Three Days Grace", "Riot");
        it.insert("Fall Out Boy", "Phoenix");
        it.insert("Fall Out Boy", "Champion");
        it.insert("Against the Current", "Lullaby");
        it.insert("3 Doors Down", "When I'm gone");
        it.insert("Shinedown", "Enemies");
        it.insert("House and Home", "Oracle");
        it.insert("House and Home", "Cellophane");
        it.insert("Fall Out Boy", "Cellophane");

        assertFuzzyEquals("0 tsunami sea\r\n" + "3 oracle\r\n" + "5 lullaby\r\n"
            + "6 phoenix\r\n" + "12 when im gone\r\n" + "15 jet pack blues\r\n"
            + "21 black rainbow\r\n" + "22 jaded\r\n" + "23 void\r\n"
            + "24 cellophane\r\n" + "30 enemies\r\n" + "31 world so cold\r\n"
            + "34 riot\r\n" + "35 champion\r\n" + "total songs 14\r\n", it
                .print("song"));
        assertFuzzyEquals("2 fall out boy\r\n" + "3 3 doors down\r\n"
            + "4 spiritbox\r\n" + "10 house and home\r\n" + "12 12 stones\r\n"
            + "15 against the current\r\n" + "16 three days grace\r\n"
            + "18 shinedown\r\n" + "total artists: 8\r\n", it.print("artist"));

        assertFuzzyEquals("|12 Stones| is removed from the Artist database\r\n",
            it.remove("artist", "12 Stones"));

        assertFuzzyEquals("0 tsunami sea\r\n" + "3 oracle\r\n" + "5 lullaby\r\n"
            + "6 phoenix\r\n" + "12 when im gone\r\n" + "15 jet pack blues\r\n"
            + "21 black rainbow\r\n" + "22 jaded\r\n" + "23 void\r\n"
            + "24 cellophane\r\n" + "30 enemies\r\n" + "31 world so cold\r\n"
            + "34 riot\r\n" + "35 champion\r\n" + "total songs 14\r\n", it
                .print("song"));
        assertFuzzyEquals("2 fall out boy\r\n" + "3 3 doors down\r\n"
            + "4 spiritbox\r\n" + "10 house and home\r\n" + "12 tombstone\r\n"
            + "15 against the current\r\n" + "16 three days grace\r\n"
            + "18 shinedown\r\n" + "total artists: 7\r\n", it.print("artist"));

    }


    public void testAdvInsert() throws Exception {

        it = new GraphDB();
        it.create(100);

        assertFuzzyEquals("artist1 is added to the artist database\n"
            + "song1 is added to the song database", it.insert("Artist1",
                "Song1"));
        assertFuzzyEquals("song2 is added to the song database", it.insert(
            "Artist1", "Song2"));
        assertFuzzyEquals("song3 is added to the song database", it.insert(
            "Artist1", "Song3"));
        assertFuzzyEquals("song4 is added to the song database", it.insert(
            "Artist1", "Song4"));
        assertFuzzyEquals("song5 is added to the song database", it.insert(
            "Artist1", "Song5"));
        assertFuzzyEquals("song6 is added to the song database", it.insert(
            "Artist1", "Song6"));
        assertFuzzyEquals("song7 is added to the song database", it.insert(
            "Artist1", "Song7"));
        assertFuzzyEquals("song8 is added to the song database", it.insert(
            "Artist1", "Song8"));
        assertFuzzyEquals("song9 is added to the song database", it.insert(
            "Artist1", "Song9"));
        assertFuzzyEquals("song10 is added to the song database", it.insert(
            "Artist1", "Song10"));

        assertFuzzyEquals("artist2 is added to the artist database\n"
            + "song11 is added to the song database", it.insert("Artist2",
                "Song11"));
        assertFuzzyEquals("song12 is added to the song database", it.insert(
            "Artist2", "Song12"));
        assertFuzzyEquals("song13 is added to the song database", it.insert(
            "Artist2", "Song13"));
        assertFuzzyEquals("song14 is added to the song database", it.insert(
            "Artist2", "Song14"));
        assertFuzzyEquals("song15 is added to the song database", it.insert(
            "Artist2", "Song15"));
        assertFuzzyEquals("song16 is added to the song database", it.insert(
            "Artist2", "Song16"));
        assertFuzzyEquals("song17 is added to the song database", it.insert(
            "Artist2", "Song17"));
        assertFuzzyEquals("song18 is added to the song database", it.insert(
            "Artist2", "Song18"));
        assertFuzzyEquals("song19 is added to the song database", it.insert(
            "Artist2", "Song19"));
        assertFuzzyEquals("song20 is added to the song database", it.insert(
            "Artist2", "Song20"));

        assertFuzzyEquals("artist3 is added to the artist database\n"
            + "song21 is added to the song database", it.insert("Artist3",
                "Song21"));
        assertFuzzyEquals("artist4 is added to the artist database", it.insert(
            "Artist4", "Song21"));
        assertFuzzyEquals("artist5 is added to the artist database", it.insert(
            "Artist5", "Song21"));
        assertFuzzyEquals("artist6 is added to the artist database", it.insert(
            "Artist6", "Song21"));
        assertFuzzyEquals("artist7 is added to the artist database", it.insert(
            "Artist7", "Song21"));
        assertFuzzyEquals("artist8 is added to the artist database", it.insert(
            "Artist8", "Song21"));
        assertFuzzyEquals("artist9 is added to the artist database", it.insert(
            "Artist9", "Song21"));
        assertFuzzyEquals("artist10 is added to the artist database", it.insert(
            "Artist10", "Song21"));
        assertFuzzyEquals("artist11 is added to the artist database", it.insert(
            "Artist11", "Song21"));
        assertFuzzyEquals("artist12 is added to the artist database", it.insert(
            "Artist12", "Song21"));

        assertFuzzyEquals("artist13 is added to the artist database\n"
            + "song22 is added to the song database", it.insert("Artist13",
                "Song22"));
        assertFuzzyEquals("artist14 is added to the artist database", it.insert(
            "Artist14", "Song22"));
        assertFuzzyEquals("artist15 is added to the artist database", it.insert(
            "Artist15", "Song22"));
        assertFuzzyEquals("artist16 is added to the artist database", it.insert(
            "Artist16", "Song22"));
        assertFuzzyEquals("artist17 is added to the artist database", it.insert(
            "Artist17", "Song22"));
        assertFuzzyEquals("artist18 is added to the artist database", it.insert(
            "Artist18", "Song22"));
        assertFuzzyEquals("artist19 is added to the artist database", it.insert(
            "Artist19", "Song22"));
        assertFuzzyEquals("artist20 is added to the artist database", it.insert(
            "Artist20", "Song22"));
        assertFuzzyEquals("artist21 is added to the artist database", it.insert(
            "Artist21", "Song22"));
        assertFuzzyEquals("artist22 is added to the artist database", it.insert(
            "Artist22", "Song22"));

        assertFuzzyEquals("there are 4 connected components\n"
            + "the largest connected component has 11 elements\n"
            + "the diameter of the largest component is 2", it.printgraph());
    }


    public void testStress() throws Exception {
        it = new GraphDB();
        it.create(10);

        assertFuzzyEquals("1 is added to the artist database\n"
            + "s is added to the song database", it.insert("1", "s"));
        assertFuzzyEquals("2 is added to the artist database", it.insert("2",
            "s"));
        assertFuzzyEquals("s2 is added to the song database", it.insert("2",
            "s2"));
        assertFuzzyEquals("3 is added to the artist database", it.insert("3",
            "s"));
        assertFuzzyEquals("s3 is added to the song database", it.insert("3",
            "s3"));

        assertFuzzyEquals("4 is added to the artist database", it.insert("4",
            "s"));
        assertFuzzyEquals("s4 is added to the song database", it.insert("4",
            "s4"));

        assertFuzzyEquals("5 is added to the artist database", it.insert("5",
            "s"));
        assertFuzzyEquals("s5 is added to the song database", it.insert("5",
            "s5"));

        assertFuzzyEquals("graph size doubled to 20\n"
            + "artist hash table size doubled\n"
            + "6 is added to the artist database", it.insert("6", "s"));
        assertFuzzyEquals("7 is added to the artist database", it.insert("7",
            "s"));
        assertFuzzyEquals("", it.insert("7", "s2"));

        assertFuzzyEquals("there are 1 connected components\n"
            + "the largest connected component has 12 elements\n"
            + "the diameter of the largest component is 4", it.printgraph());

        assertFuzzyEquals("s is removed from the song database", it.remove(
            "song", "s"));

        assertFuzzyEquals("there are 6 connected components\n"
            + "the largest connected component has 3 elements\n"
            + "the diameter of the largest component is 2", it.printgraph());
        assertFuzzyEquals("a5 is added to the song database", it.insert("6",
            "a5"));
        assertFuzzyEquals("song hash table size doubled\n"
            + "a1 is added to the song database", it.insert("1", "a1"));
        assertFuzzyEquals("a3 is added to the song database", it.insert("3",
            "a3"));

        assertFuzzyEquals("there are 6 connected components\n"
            + "the largest connected component has 3 elements\n"
            + "the diameter of the largest component is 2", it.printgraph());

    }


    public void testStressRandom() throws Exception {
        it = new GraphDB();
        it.create(10);
        TestableRandom r = new TestableRandom(192);
        for (int i = 0; i < 100; i++) {
            it.insert(Integer.toString(i), Integer.toString(r.nextInt()));
        }

        r.setSeed(192);
        for (int i = 0, z = r.nextInt() % 20; i < 100; i++, z--) {
            if (z <= 0) {
                it.remove("artist", Integer.toString(i));
                it.remove("song", Integer.toString(r.nextInt()));
                z = r.nextInt() % 20;
            }
        }

        assertFuzzyEquals("1 88\n" + "2 93\n" + "9 98\n" + "48 tombstone\n"
            + "49 17\n" + "50 1\n" + "51 2\n" + "52 3\n" + "53 4\n" + "54 5\n"
            + "55 6\n" + "56 7\n" + "57 8\n" + "58 9\n" + "59 22\n" + "60 32\n"
            + "61 42\n" + "62 52\n" + "63 62\n" + "64 72\n" + "65 12\n"
            + "66 27\n" + "67 37\n" + "68 47\n" + "69 tombstone\n" + "70 67\n"
            + "71 tombstone\n" + "72 82\n" + "73 92\n" + "81 87\n" + "82 97\n"
            + "113 tombstone\n" + "114 16\n" + "115 21\n" + "116 31\n"
            + "117 41\n" + "118 26\n" + "119 tombstone\n" + "120 46\n"
            + "121 51\n" + "122 61\n" + "123 71\n" + "124 81\n" + "125 91\n"
            + "126 56\n" + "127 66\n" + "128 tombstone\n" + "129 tombstone\n"
            + "130 96\n" + "177 10\n" + "178 15\n" + "179 20\n" + "180 30\n"
            + "181 40\n" + "182 tombstone\n" + "183 tombstone\n" + "184 45\n"
            + "185 50\n" + "186 60\n" + "187 70\n" + "188 80\n" + "189 90\n"
            + "190 55\n" + "191 65\n" + "192 75\n" + "193 85\n" + "194 95\n"
            + "241 14\n" + "242 19\n" + "243 24\n" + "244 34\n" + "245 44\n"
            + "246 29\n" + "247 39\n" + "248 49\n" + "249 54\n"
            + "250 tombstone\n" + "251 tombstone\n" + "252 84\n" + "253 94\n"
            + "254 tombstone\n" + "255 69\n" + "256 79\n" + "257 89\n"
            + "258 99\n" + "305 13\n" + "306 23\n" + "307 33\n" + "308 38\n"
            + "309 18\n" + "310 28\n" + "311 78\n" + "312 43\n" + "313 53\n"
            + "314 63\n" + "315 73\n" + "316 83\n" + "317 tombstone\n"
            + "318 tombstone\n" + "319 68\n" + "total artists 86\n", it.print(
                "artist"));
        assertFuzzyEquals("16 1610187345\n" + "17 1279073904\n"
            + "18 tombstone\n" + "20 1895106253\n" + "21 338160561\n"
            + "22 1033096446\n" + "23 1302454883\n" + "24 1236745565\n"
            + "25 1149746012\n" + "26 1104163775\n" + "27 1673727531\n"
            + "28 tombstone\n" + "29 1855803634\n" + "30 1158312573\n"
            + "31 701665285\n" + "32 499033679\n" + "33 912808647\n"
            + "34 tombstone\n" + "35 1237832677\n" + "38 1509731152\n"
            + "39 1989228880\n" + "41 532207212\n" + "46 2138401925\n"
            + "55 778670350\n" + "79 888141016\n" + "80 1672654185\n"
            + "81 2034143009\n" + "82 972023245\n" + "83 342266242\n"
            + "84 980656403\n" + "86 tombstone\n" + "87 1706332492\n"
            + "88 1262069994\n" + "90 380321971\n" + "91 1505588455\n"
            + "92 1247871831\n" + "93 594348134\n" + "94 2133837444\n"
            + "95 1944978850\n" + "96 2080974150\n" + "97 2061367382\n"
            + "99 tombstone\n" + "101 410283489\n" + "102 2107525234\n"
            + "104 932898346\n" + "108 825649740\n" + "111 532431358\n"
            + "141 412036008\n" + "144 tombstone\n" + "145 1547000909\n"
            + "148 293605114\n" + "153 2080493534\n" + "154 1643742218\n"
            + "155 1695362895\n" + "156 215861934\n" + "157 tombstone\n"
            + "158 317996548\n" + "159 984879993\n" + "160 589098522\n"
            + "161 401502139\n" + "162 tombstone\n" + "166 893093705\n"
            + "170 3063791\n" + "207 tombstone\n" + "208 587058723\n"
            + "210 tombstone\n" + "211 1130860647\n" + "212 tombstone\n"
            + "213 711739517\n" + "214 257635531\n" + "215 669122395\n"
            + "216 1383037777\n" + "217 790616371\n" + "218 154694480\n"
            + "219 tombstone\n" + "220 373650464\n" + "221 582212987\n"
            + "222 748327311\n" + "225 1738821780\n" + "226 1937433543\n"
            + "227 1222298476\n" + "228 10963767\n" + "229 966943938\n"
            + "230 1717593972\n" + "234 1589167083\n" + "237 595640384\n"
            + "270 2080968147\n" + "277 tombstone\n" + "278 1781070733\n"
            + "279 175756023\n" + "281 589909374\n" + "282 1917814691\n"
            + "283 566756050\n" + "284 987003202\n" + "286 1049726363\n"
            + "287 1620701870\n" + "290 tombstone\n" + "291 1882997390\n"
            + "293 1126196290\n" + "298 585782361\n" + "total songs 86", it
                .print("song"));
        assertFuzzyEquals("there are 98 connected components\n"
            + "the largest connected component has 2 elements\n"
            + "the diameter of the largest component is 1", it.printgraph());
    }


    public void testStress2() throws Exception {

        it = new GraphDB();
        it.create(13);

        assertFuzzyEquals("artist1 is added to the artist database\n"
            + "song1 is added to the song database", it.insert("Artist1",
                "Song1"));
        assertFuzzyEquals("song2 is added to the song database", it.insert(
            "Artist1", "Song2"));
        assertFuzzyEquals("song3 is added to the song database", it.insert(
            "Artist1", "Song3"));
        assertFuzzyEquals("song4 is added to the song database", it.insert(
            "Artist1", "Song4"));
        assertFuzzyEquals("song5 is added to the song database", it.insert(
            "Artist1", "Song5"));
        assertFuzzyEquals("song6 is added to the song database", it.insert(
            "Artist1", "Song6"));
        assertFuzzyEquals("song hash table size doubled\r\n"
            + "song7 is added to the song database", it.insert("Artist1",
                "Song7"));
        assertFuzzyEquals("song8 is added to the song database", it.insert(
            "Artist1", "Song8"));
        assertFuzzyEquals("song9 is added to the song database", it.insert(
            "Artist1", "Song9"));
        assertFuzzyEquals("song10 is added to the song database", it.insert(
            "Artist1", "Song10"));
        assertFuzzyEquals("song11 is added to the song database", it.insert(
            "Artist1", "Song11"));
        assertFuzzyEquals("song12 is added to the song database", it.insert(
            "Artist1", "Song12"));
        assertFuzzyEquals("graph size doubled to 26\r\n"
            + "song13 is added to the song database", it.insert("Artist1",
                "Song13"));
        assertFuzzyEquals("song hash table size doubled\r\n"
            + "song14 is added to the song database", it.insert("Artist1",
                "Song14"));
        assertFuzzyEquals("song15 is added to the song database", it.insert(
            "Artist1", "Song15"));
        assertFuzzyEquals("song16 is added to the song database", it.insert(
            "Artist1", "Song16"));
        assertFuzzyEquals("song17 is added to the song database", it.insert(
            "Artist1", "Song17"));
        assertFuzzyEquals("song18 is added to the song database", it.insert(
            "Artist1", "Song18"));
        assertFuzzyEquals("song19 is added to the song database", it.insert(
            "Artist1", "Song19"));
        assertFuzzyEquals("song20 is added to the song database", it.insert(
            "Artist1", "Song20"));
        assertFuzzyEquals("song21 is added to the song database", it.insert(
            "Artist1", "Song21"));
        assertFuzzyEquals("song22 is added to the song database", it.insert(
            "Artist1", "Song22"));
        assertFuzzyEquals("song23 is added to the song database", it.insert(
            "Artist1", "Song23"));
        assertFuzzyEquals("song24 is added to the song database", it.insert(
            "Artist1", "Song24"));
        assertFuzzyEquals("song25 is added to the song database", it.insert(
            "Artist1", "Song25"));
        assertFuzzyEquals("graph size doubled to 52\r\n"
            + "song26 is added to the song database", it.insert("Artist1",
                "Song26"));
        assertFuzzyEquals("song hash table size doubled\r\n"
            + "song27 is added to the song database", it.insert("Artist1",
                "Song27"));
        assertFuzzyEquals("Song25 is removed from the song database", it.remove(
            "song", "Song25"));
        assertFuzzyEquals("song28 is added to the song database", it.insert(
            "Artist1", "Song28"));
        assertFuzzyEquals("song29 is added to the song database", it.insert(
            "Artist1", "Song29"));
        assertFuzzyEquals("song30 is added to the song database", it.insert(
            "Artist1", "Song30"));
        assertFuzzyEquals("song31 is added to the song database", it.insert(
            "Artist1", "Song31"));
        assertFuzzyEquals("song32 is added to the song database", it.insert(
            "Artist1", "Song32"));
        assertFuzzyEquals("song33 is added to the song database", it.insert(
            "Artist1", "Song33"));
        assertFuzzyEquals("song34 is added to the song database", it.insert(
            "Artist1", "Song34"));
        assertFuzzyEquals("song35 is added to the song database", it.insert(
            "Artist1", "Song35"));
        assertFuzzyEquals("song36 is added to the song database", it.insert(
            "Artist1", "Song36"));
        assertFuzzyEquals("song37 is added to the song database", it.insert(
            "Artist1", "Song37"));
        assertFuzzyEquals("Artist1 is removed from the artist database", it
            .remove("artist", "Artist1"));
        assertFuzzyEquals("artist1 is added to the artist database\r\n"
            + "song38 is added to the song database", it.insert("Artist1",
                "Song38"));
        assertFuzzyEquals("song39 is added to the song database", it.insert(
            "Artist1", "Song39"));
        assertFuzzyEquals("song40 is added to the song database", it.insert(
            "Artist1", "Song40"));
        assertFuzzyEquals("song41 is added to the song database", it.insert(
            "Artist1", "Song41"));
        assertFuzzyEquals("song42 is added to the song database", it.insert(
            "Artist1", "Song42"));
        assertFuzzyEquals("song43 is added to the song database", it.insert(
            "Artist1", "Song43"));
        assertFuzzyEquals("song44 is added to the song database", it.insert(
            "Artist1", "Song44"));
        assertFuzzyEquals("song45 is added to the song database", it.insert(
            "Artist1", "Song45"));
        assertFuzzyEquals("song46 is added to the song database", it.insert(
            "Artist1", "Song46"));
        assertFuzzyEquals("song47 is added to the song database", it.insert(
            "Artist1", "Song47"));
        assertFuzzyEquals("song48 is added to the song database", it.insert(
            "Artist1", "Song48"));
        assertFuzzyEquals("song49 is added to the song database", it.insert(
            "Artist1", "Song49"));
        assertFuzzyEquals("song50 is added to the song database", it.insert(
            "Artist1", "Song50"));

        assertFuzzyEquals("there are 37 connected components\r\n"
            + "the largest connected component has 14 elements\r\n"
            + "the diameter of the largest component is 2", it.printgraph());

    }
}
