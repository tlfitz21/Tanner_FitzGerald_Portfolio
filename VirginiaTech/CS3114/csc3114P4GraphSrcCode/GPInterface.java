import java.io.IOException;

/**
 * Interface class for the Graph Project
 *
 * @author CS3114/5040 Staff
 * @version Spring 2026
 *
 */
public interface GPInterface {

    // ----------------------------------------------------------
    /**
     * (Re)initialize the database
     * @param initHash Initial hash table size
     * @return Error messages if appropriate
     */
    public String create(int initHash);


    // ----------------------------------------------------------
    /**
     * (Re)initialize the database
     * @return true on successful clear of database
     */
    public boolean clear();


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
        throws IOException;


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
    public String remove(String type, String nameString) throws IOException;


    // ----------------------------------------------------------
    /**
     * Print out the hash table contents
     *
     * @param type
     *            Controls what object is being printed
     * @return The string that was printed
     * @throws IOException
     */
    public String print(String type) throws IOException;

    // ----------------------------------------------------------
    /**
     * Print out the graph information
     *
     * @return The string that was printed
     * @throws IOException
     */
    public String printgraph() throws IOException;
}
