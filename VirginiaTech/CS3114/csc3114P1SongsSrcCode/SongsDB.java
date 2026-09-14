import java.io.IOException;

/**
 * The database implementation for this project.
 * We have two hash tables and a memory manager.
 *
 * @author Tanner FitzGerald
 * @version <Put something here>
 */
public class SongsDB implements Songs {

    // establishing the data structures
    /**
     * the artist name hash table
     */
    public Hash artHash;

    /**
     * the son title hash table
     */
    public Hash songHash;

    /**
     * my memory manager
     */
    public MemManager manager;

    // record the initial sizes so the clear function knows where to go to
    /**
     * initial hash table size
     */
    public int initHashSize;

    /**
     * initial memory manager size
     */
    public int initMemSize;

    // current size of each structure
    /**
     * current hash table size
     */
    public int hashSize;

    /**
     * current memory manager size
     */
    public int memSize;

    /**
     * Create a new SongsDB object.
     * But don't set anything -- that gets done by "create"
     */

    // our actual database
    public SongsDB() {
    }


    /**
     * Create a brave new World.
     *
     * @param inHash
     *            Initial size for hash tables
     * @param inMemMan
     *            Initial size for the memory manager
     * @return Error messages if appropriate
     */
    public String create(int inHash, int inMemMan) {

        // checking for proper input
        if (inHash < 1)
            return "Initial hash table size must be positive";
        if (inMemMan < 1)
            return "Initial memory manager size must be positive";
        if (Math.log(inMemMan) / Math.log(2) % 1 != 0)
            return "Initial memory manager size must be a power of 2";

        // setting the data structures and variables
        manager = new MemManager(inMemMan);
        artHash = new Hash(inHash, manager);
        songHash = new Hash(inHash, manager);
        initHashSize = inHash;
        initMemSize = inMemMan;
        hashSize = inHash;
        memSize = inMemMan;

        return "";
    }


    /**
     * Re-initialize the database
     * 
     * @return true on successful clear of database
     */
    public boolean clear() {

        // won't clear if it wasn't initialized
        if (manager == null) {
            return false;
        }

        // reinitialize the data structures
        manager = new MemManager(initMemSize);
        artHash = new Hash(initHashSize, manager);
        songHash = new Hash(initHashSize, manager);

        return true;
    }


    // ----------------------------------------------------------
    /**
     * Insert to the hash table
     *
     * @param artistString
     *            Artist string to insert
     * @param songString
     *            Song string to insert
     * @return Error message if appropriate
     * @throws IOException
     */
    public String insert(String artistString, String songString)
        throws IOException {

        StringBuilder sb = new StringBuilder();

        // can't insert if it wasn't initialized
        if (manager == null)
            return "Database not initialized";
        // check for proper input
        if (artistString == null || artistString.equals("")
            || songString == null || songString.equals(""))
            return "Input strings cannot be null or empty";

        if (artHash.search(artistString) == -1) {
            byte[] artistBytes = artistString.getBytes();
            int artSize = artistBytes.length;
            int initSize = manager.memPool.length;
            int artInd = manager.findFree(artistBytes, artSize);
            System.out.println(artistString + " was inserted at " + artInd
                + " and is " + artSize + " bytes long.");
            String artPoolExpand = "";
            if (initSize < manager.memPool.length) {
                artPoolExpand = "Memory pool expanded to be "
                    + manager.memPool.length + " bytes\n";
            }
            String artAdd = artistString + " is added to the Artist database\n";

            sb.append(artPoolExpand);
            if (artHash.insert(artistString, songString, artInd, artSize))
                sb.append("artist hash table size doubled\n");
            sb.append(artAdd);
        }
        else {
            sb.append(artistString
                + " duplicates a record already in the Artist database\n");
        }

        if (songHash.search(songString) == -1) {
            byte[] songBytes = songString.getBytes();
            int songSize = songBytes.length;
            int initSize = manager.memPool.length;
            int songInd = manager.findFree(songBytes, songSize);
            System.out.println(songString + " was inserted at " + songInd
                + " and is " + songSize + " bytes long.");
            String songPoolExpand = "";
            if (initSize < manager.memPool.length) {
                songPoolExpand = "Memory pool expanded to be "
                    + manager.memPool.length + " bytes\n";
            }
            String songAdd = songString + " is added to the Song database\n";
            sb.append(songPoolExpand);
            if (songHash.insert(songString, artistString, songInd, songSize))
                sb.append("song hash table size doubled\n");
            sb.append(songAdd);
        }
        else {
            sb.append(songString
                + " duplicates a record already in the Song database\n");
        }

        return sb.toString();
    }

    // check that there's not already and artist matching the input
// if (artHash.search(artistString) > -1 && songHash.search(
// songString) > -1)
// return "No duplicates";
// /*
// * ^does not account for the extremely rare edge case of having a song A
// * and artist A in the tables,
// * and also having song B by artist B in the tables. then trying to
// * insert song B(same title) by
// * artist A in the table. we should be able to insert it, but this logic
// * finds song B and artist A
// * and thinks it should not be inserted.
// */
//
// // create byte arrays and determine their sizes
// byte[] songBytes = songString.getBytes();
// byte[] artistBytes = artistString.getBytes();
// int songSize = songBytes.length;
// int artSize = artistBytes.length;
//
// // first, findFree: pass byte array and size, get memPool position
// int initSize = manager.memPool.length;
// int artInd = manager.findFree(artistBytes, artSize);
// System.out.println(artistString + " was inserted at " + artInd
// + " and is " + artSize + " bytes long.");
// String artPoolExpand = "";
// if (initSize < manager.memPool.length) {
// artPoolExpand = "Memory pool expanded to be "
// + manager.memPool.length + " bytes\n";
// }
// String artAdd = artistString + " is added to the Artist database\n";
//
// initSize = manager.memPool.length;
// int songInd = manager.findFree(songBytes, songSize);
// System.out.println(songString + " was inserted at " + songInd
// + " and is " + songSize + " bytes long.");
// String songPoolExpand = "";
// if (initSize < manager.memPool.length) {
// songPoolExpand = "Memory pool expanded to be "
// + manager.memPool.length + " bytes\n";
// }
// String songAdd = songString + " is added to the Song database\n";
//
// // Insert the data into the hash tables
// sb.append(artPoolExpand);
// if (artHash.insert(artistString, songString, artInd, artSize))
// sb.append("artist hash table size doubled\n");
// sb.append(artAdd);
//
// sb.append(songPoolExpand);
// if (songHash.insert(songString, artistString, songInd, songSize))
// sb.append("song hash table size doubled\n");
// sb.append(songAdd);
//
// return sb.toString();


    // ----------------------------------------------------------
    /**
     * Remove from the hash table
     *
     * @param type
     *            The table to be removed
     * @param nameString
     *            The string to be removed from the table
     * @return Error message if appropriate
     * @throws IOException
     */
    public String remove(String type, String nameString) throws IOException {

        // can't remove if database wasn't initialized
        if (manager == null)
            return "Database not initialized";

        // interpret the "type" input
        boolean isSong;
        // check for proper input
        if (type == null || type.equals("") || nameString == null || nameString
            .equals(""))
            return "Input strings cannot be null or empty";
        else if (type.equals("song"))
            isSong = true;
        else if (type.equals("artist"))
            isSong = false;
        else
            return "Bad type value " + type + " on remove";

        // for songs:
        if (isSong) {
            // remove both entries from their hash tables
            Handle songHandle = songHash.remove(nameString);
            if (songHandle == null)
                return nameString + " does not exist in the song database";
            // Handle artHandle = artHash.remove(songHandle.getOtherKey());
            // hash.remove returns null if hash.search returned -1, meaning the
            // entry wasn't found

            // remove the entries from the memory pool, then update the freelist
            manager.remove(songHandle.getMemIndex(), songHandle.getByteSize());
            // manager.remove(artHandle.getMemIndex(), artHandle.getByteSize());
            System.out.println(nameString + " was removed from " + songHandle
                .getMemIndex() + ". It was " + songHandle.getByteSize()
                + " bytes long");
            return nameString + " is removed from the song database";
        }
        // for artists, do the same
        else {
            Handle artHandle = artHash.remove(nameString);
            if (artHandle == null)
                return nameString + " does not exist in the artist database";
            // Handle songHandle = songHash.remove(artHandle.getOtherKey());

            manager.remove(artHandle.getMemIndex(), artHandle.getByteSize());
            // manager.remove(songHandle.getMemIndex(),
            // songHandle.getByteSize());

            System.out.println(nameString + " was removed from " + artHandle
                .getMemIndex() + ". It was " + artHandle.getByteSize()
                + " bytes long");
            return nameString + " is removed from the artist database";
        }

    }


    // ----------------------------------------------------------
    /**
     * Print out the hash table contents
     *
     * @param type
     *            Controls what object is being printed
     * @return The string that was printed
     * @throws IOException
     */
    public String print(String type) throws IOException {

        // can't print if it wasn't initialized
        if (manager == null)
            return "Database not initialized";

        // interpret the "type" parameter
        boolean isSong;
        if (type == null || type.equals(""))
            return "Input strings cannot be null or empty";

        else if (type.equals("blocks")) {

            boolean isEmpty = true;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < manager.freeList.length; i++) {

                if (manager.freeList[i].getHead() == null)
                    continue;

                manager.freeList[i].toStart();
                isEmpty = false;
                sb.append(String.valueOf((int)Math.pow(2, i)) + ": ");

                while (manager.freeList[i].getCurr() != null) {
                    sb.append(String.valueOf(manager.freeList[i].getCurr()
                        .getData()) + " ");
                    if (manager.freeList[i].getCurr().getNext() == null)
                        break;
                    manager.freeList[i].forward();
                }

                sb.append("\n");
            }
            if (isEmpty)
                return "No free blocks are available.";
            else
                return sb.toString();
        }

        else if (type.equals("song"))
            isSong = true;
        else if (type.equals("artist"))
            isSong = false;
        else
            return "Bad print parameter";

        // check for trying to print an empty hash
        if (isSong && songHash.size == 0)
            return "total songs: 0";
        else if (!isSong && artHash.size == 0)
            return "total artists: 0";

        // create a stringBuilder so we can return/print the results as one
        // string
        StringBuilder returnString = new StringBuilder();

        // for songs:
        if (isSong) {
            // only do it an amount of times equal to the current size of the
            // hash
// int songTombs = 0;
            for (int i = 0; i < songHash.maxSize; i++) {
                // skip over empty hash entries
// if (songHash.table[i] == null)
// if (songHash.removed[i] == false)
// continue;
// else {
// returnString.append(String.valueOf(i)
// + ": tombstone\n");
// songTombs++;
// continue;
// }
// create the empty byte array
                if (songHash.table[i] == null) 
                    continue;
                else if (songHash.table[i].getTomb() == true) {
                    returnString.append(String.valueOf(i) + ": tombstone\n");
                    continue;
                }
                Handle handle = songHash.table[i];
                byte[] bytes = new byte[handle.getByteSize()];
                // copy the memoryPoll at the handle's index data into bytes,
                // for the byte size
                System.arraycopy(manager.memPool, handle.getMemIndex(), bytes,
                    0, handle.getByteSize());
                // convert it to a string and append that to the returnString
                String outStr = new String(bytes);
                returnString.append(String.valueOf(i) + ": " + outStr + "\n");
            }
            returnString.append("total songs " + (songHash.size));
        }

        // for artists: do the same
        else {
// int artTombs = 0;
            for (int i = 0; i < artHash.maxSize; i++) {
//
// if (artHash.table[i] == null)
// if (artHash.removed[i] == false)
// continue;
// else {
// returnString.append(String.valueOf(i)
// + ": tombstone\n");
// artTombs++;
// continue;
// }
                if (artHash.table[i] == null)
                    continue;
                else if (artHash.table[i].getTomb() == true) {
                    returnString.append(String.valueOf(i) + ": tombstone\n");
                    continue;
                }
                Handle handle = artHash.table[i];
                byte[] bytes = new byte[handle.getByteSize()];
                System.arraycopy(manager.memPool, handle.getMemIndex(), bytes,
                    0, handle.getByteSize());
                String outStr = new String(bytes);
                returnString.append(String.valueOf(i) + ": " + outStr + "\n");
            }
            returnString.append("total artists " + (artHash.size));
        }
        // build the string and return it
        return returnString.toString();
    }
}
