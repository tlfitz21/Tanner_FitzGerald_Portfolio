//-------------------------------------------------------------------------

/**
 * Interface class for the DNA String Database Project
 *
 * @author CS3114/5040 Staff
 * @version Spring 2026
 *
 */
public interface DNA {

    // ----------------------------------------------------------
    /**
     * Insert a DNA string into the database
     * @param sequence The sequence to insert
     * @return The outcomes message string
     */
    public String insert(String sequence);


    // ----------------------------------------------------------
    /**
     * Remove a DNA string into the database
     * @param sequence The sequence to remove
     * @return The outcomes message string
     */
    public String remove(String sequence);


    // ----------------------------------------------------------
    /**
     * Print the tree
     * @return the print string
     */
    public String print();


    // ----------------------------------------------------------
    /**
     * Print the lengths
     * @return the print string
     */
    public String printLengths();


    // ----------------------------------------------------------
    /**
     * Print the stats
     * @return the print string
     */
    public String printStats();


    // ----------------------------------------------------------
    /**
     * Search for a given string
     * @param sequence The sequence to search for
     * @return the print string
     */
    public String search(String sequence);
}
