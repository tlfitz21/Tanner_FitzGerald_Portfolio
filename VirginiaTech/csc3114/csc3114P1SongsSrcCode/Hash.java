/**
 * Implement a hash table.
 * Data: Strings
 * Hash function: sfold
 * Collision Resolution: Quadratic probing
 *
 * @author Tanner FitzGerald
 * @version <Put something here>
 */

public class Hash {

    // establish data structures

    /**
     * establish the hash table
     */
    public Handle[] table;

    /**
     * the memory manager we'll be using
     */
    MemManager manager;

    // record the sizes of the structures

    /**
     * the maximum size of the hash table
     */
    public int maxSize;

    /**
     * the current size of the hash table
     */
    public int size;

    /**
     * Create a new Hash object.
     *
     * @param init
     *            Initial size for table
     * @param m
     *            Memory manager used by this table to store objects
     */
    public Hash(int init, MemManager m) { 

        table = new Handle[init];
        maxSize = init;
        size = 0;
        manager = m;
    }


    /**
     * Compute the hash function. Uses the "sfold" method from the OpenDSA
     * module on hash functions
     *
     * @param s
     *            The string that we are hashing
     * @return The home slot for that string
     */
    public long hFunc(String s) {
        long sum = 0;
        long mult = 1;
        for (int i = 0; i < s.length(); i++) {
            mult = (i % 4 == 0) ? 1 : mult * 256;
            sum += s.charAt(i) * mult;
        }
        return (long)(Math.abs(sum));
    }


    /**
     * 
     * add a new hanlde to the hash table
     * 
     * @param selfStr
     *            the string of that entry
     * @param otherStr
     *            the string of its accompanying entry (if entering a song, this
     *            is the artist name, and vice versa)
     * @param memInd
     *            the index the byte array will be stored in the memory pool
     * @param byteSize
     *            the amount of bytes in the byte array
     * @return
     *         returns true if the table had to be resized
     */
    public boolean insert(
        String selfStr,
        String otherStr,
        int memInd,
        int byteSize) {

        // resize the hash if entries after insertion would exceed 50% of the
        // maxSize
        boolean hasResized = false;
        if (size >= maxSize / 2) {
            resize();
            hasResized = true;
        }

        // convert both strings to the keys
        long selfKey = hFunc(selfStr);
        long otherKey = hFunc(otherStr);

        // start is the position the key hashes to, and where it will be
        // inserted at if there's no collision
        int start = (int)(selfKey % maxSize);
        int pos = start;

        // until an empty slot or tombstone is found
        // increase pos according to the quadratic hash function
        int i = 0;
        while (table[pos] != null && !table[pos].getTomb()) {
            i++;
            pos = (start + i * i) % maxSize;
        }

        // if it's an empty slot, just add a new handle
        if (table[pos] == null)
            table[pos] = new Handle(memInd, byteSize, selfKey, otherKey);

        // otherwise, update the existing tombstone with the new information
        else {
            table[pos].setTomb(false);
            table[pos].setMemIndex(memInd);
            table[pos].setByteSize(byteSize);
            table[pos].setKey(selfKey);
            table[pos].setOtherKey(otherKey);
        }

        // increase the current size of the hash
        size++;
        if (hasResized)
            return true;
        else
            return false;
    }


    /**
     * resize the hash table by creating a new array that's twice as large and
     * rehashing all the elements into it. recreate the removed array, which can
     * be set to all false since everything is getting rehashed. create a new
     * recycling bin that is twice as large, and copy all of the old elements
     * into the new one
     */
    public void resize() {
        maxSize *= 2;
        Handle[] newArr = new Handle[maxSize];

        for (int i = 0; i < table.length; i++) {
            // only rehash elements that are full
            if (table[i] != null && !table[i].getTomb()) {
                // record the slot it would go in if there's no collision
                int start = (int)(table[i].getKey() % maxSize);
                int pos = start;

                // if there's a collision,
                if (newArr[pos] != null) {
                    // increase pos according to the quadratic hash function
                    int j = 1;
                    while (newArr[pos] != null) {
                        pos = (start + j * j) % maxSize;
                        j++;
                    }

                }
                // add the handle into pos
                newArr[pos] = table[i];
            }
        }
        table = newArr;

        // remake the recycling bin
    }


    /**
     * 
     * given an input string, find where in the hash it is
     * 
     * @param input
     *            the input string
     * @return
     *         the position it is in, or -1 if it was not found
     */
    public int search(String input) {

        // convert the string into a key
        long key = hFunc(input);

        // find the positon it's supposed to be found in if there was no
        // collision when it was inserted
        int start = (int)(key % maxSize);
        int pos = start;

        for (int i = 0; i < maxSize; i++) {

            if (table[pos] == null)
                return -1;
            // if it has been removed or if it is the wrong handle, iterate pos
            // according to the quadratic has function
            if (!table[pos].getTomb() && table[pos].getKey() == key) {
                return pos;
                
            }
            // otherwise, it's been found, and return the pos it was found at
            else
                pos = (start + i * i) % maxSize;
        }

        return -1;
    }


    /**
     * 
     * given an input key, find where in the hash table the handle is
     * 
     * @param key
     *            the input key
     * @return
     *         the position the handle was found in, or -1 if it wasn't found
     */
    public int search(long key) {

        // same function as the search(string) function, except it doesn't need
        // to compute the key
        int start = (int)(key % maxSize);
        int pos = start;

        for (int i = 0; i < maxSize; i++) {

            if (table[pos] == null)
                return -1;

            else if (!table[pos].getTomb() && table[pos]
                .getKey() != key) {
                pos = (start + i * i) % maxSize;

            }
            else
                return pos;
        }

        return -1;
    }


    /**
     * 
     * given an input string, remove and return he handle holding it
     * 
     * @param inString
     *            the input string
     * @return
     *         return the handle that was removed
     */
    public Handle remove(String inString) {
        // use search to find the position it's in
        int hashSlot = search(inString);
        // if it can't be found, return null
        if (hashSlot < 0)
            return null;

        else {
            // make it a tombstone
            table[hashSlot].setTomb(true);

            // return it
            size--;
            return table[hashSlot];
        }
    }


    /**
     * 
     * given an input key, remove and return he handle holding it
     * 
     * @param key
     *            the input key
     * @return
     *         the handle that was removed
     */
    public Handle remove(long key) {
        // use search to find the position it's in
        int hashSlot = search(key);
        // if it can't be found, return null
        if (hashSlot < 0)
            return null;

        else {
            // make it a tombstone
            table[hashSlot].setTomb(true);

            // return it
            size--;
            return table[hashSlot];
        }
    }

}
