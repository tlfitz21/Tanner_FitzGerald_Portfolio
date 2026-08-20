/**
 * Implement a hash table.
 * Data: Strings
 * Hash function: sfold
 * Collision Resolution: Quadratic probing
 *
 * @author Can Pekkan
 * @version 1.0
 */

public class HashTable {
    private HashEntry[] table;
    private boolean[] tomb;
    private int count;
    private int initalSize;

    /**
     * Create a new Hash object.
     *
     * @param init
     *            Initial size for table
     */
    public HashTable(int init) {
        this.initalSize = init;
        clear();
    }


    /**
     * clears the hash table and frees all used handles
     */
    public void clear() {
        table = new HashEntry[initalSize];
        tomb = new boolean[initalSize];
        count = 0;
    }


    /**
     * inserts a new string into the hash table
     * 
     * @param key
     *            the string used as a key
     * @param data
     *            the index to be inserted
     * @return true for successful insert, false for failed to insert.
     */
    public boolean insert(String key, int data) {
        ensureHalfEmpty();
        if (find(key) != -1) {
            return false;
        }
        HashEntry e = new HashEntry(key, data);
        int startHash = hash(key);
        int tryNo = 0;
        int index;
        do {
            index = (startHash + (tryNo) * (tryNo++)) % table.length;
        }
        while (table[index] != null);

        count++;
        table[index] = e;
        tomb[index] = false;

        return true;
    }


    /**
     * removes a element from the hash table
     * 
     * @param key
     *            the string to be removed from the hash table.
     * @return true for successful remove, false for failure to remove.
     */
    public int remove(String key) {
        int startHash = hash(key);
        int tryNo = 0;
        int index;

        boolean loop = true;

        while (loop) {
            index = (startHash + (tryNo) * (tryNo++)) % table.length;
            if (tryNo >= table.length) {
                loop = false;
            }
            else if (table[index] == null) {
                if (!tomb[index]) {
                    loop = false;
                }
            }
            else if (key.equals(table[index].getKey())) {
                int returnIndex = table[index].getNodeIndex(); // modified it to
                                                               // return the
                                                               // graph index
                table[index] = null;
                count--;
                tomb[index] = true;
                return returnIndex;
            }
        }

        return -1;
    }


    /**
     * checks if the hash table is currently storing the given string
     * 
     * @param key
     *            string to be checked if it is being stored.
     * @return true if the hash table is storing the string, false if otherwise.
     */
    public int find(String key) {
        int startHash = hash(key);
        int tryNo = 0;
        int index;

        boolean loop = true;

        while (loop) {
            index = (startHash + (tryNo) * (tryNo++)) % table.length;
            if (table[index] == null) {
                if (!tomb[index]) {
                    loop = false;
                }
            }
            else if (key.equals(table[index].getKey())) {
                return table[index].getNodeIndex();
            }
        }

        return -1;
    }


    /**
     * gets the number of elements currently in the hash table
     * 
     * @return the number of elements in the hash table
     */
    public int getCount() {
        return count;
    }


    /**
     * get the size of the hash table being used.
     * 
     * @return the size of the hash table being used.
     */
    public int getCapacity() {
        return table.length;
    }


    /**
     * this function makes sure that half of the hash table is empty
     */
    private void ensureHalfEmpty() {
        // if (table.length > count * 2) {
        if (count < table.length / 2) {
            // still has space left
            return;
        }

        HashEntry[] oldTable = table;
        if (table.length <= 1) {
            table = new HashEntry[4];
            tomb = new boolean[4];
        }
        else {
            table = new HashEntry[table.length * 2];
            tomb = new boolean[oldTable.length * 2];
        }
        count = 0;

        for (HashEntry h : oldTable) {
            if (h != null) {
                // reinserting the handles

                int startHash = hash(h.getKey());
                int tryNo = 0;
                int index;
                do {
                    index = (startHash + (tryNo) * (tryNo++)) % table.length;
                }
                while (table[index] != null);

                count++;
                table[index] = h;
                tomb[index] = false;
            }
        }
    }


    /**
     * Compute the hash function. Uses the "sfold" method from the OpenDSA
     * module on hash functions
     *
     * @param s
     *            The string that we are hashing
     * @return The home slot for that string
     */
    private int hash(String s) {
        long sum = 0;
        long mult = 1;
        for (int i = 0; i < s.length(); i++) {
            mult = (i % 4 == 0) ? 1 : mult * 256;
            sum += s.charAt(i) * mult;
        }
        return (int)(Math.abs(sum) % table.length);
    }


    /**
     * what can i say turns the hash table into a string
     */
    @Override
    public String toString() {
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < table.length; i++) {
            if (table[i] != null) {
                s.append(i).append(": ").append(table[i].getKey()).append("\n");
            }
            else if (tomb[i]) {
                s.append(i).append(": ").append("TOMBSTONE").append("\n");
            }
        }
        return s.toString();
    }

}
