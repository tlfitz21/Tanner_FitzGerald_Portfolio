
public interface Node {

    /**
     * 
     * Recursive insert function. calls itself recursively to find/create the
     * correct
     * place to insert the sequence. It will return once the flyweight calls
     * this and
     * updates it's "position" with a new leaf node
     * 
     * @param sequence
     *            the sequence to be inserted
     * @param depth
     *            the current depth of the node within the entire tree
     * @return
     *         eventually returns the new or updated root node of the tree
     *         -the flyweight method returns the new leaf node
     *         -the leaf method returns the new internal node
     *         -the internal method returns itself
     */
    public Node insert(String sequence, int depth);


    /**
     * 
     * Recursive print function. calls itself recursively to print the contents
     * or type of each node. There are three print methods, and DNADB.print will
     * pass a different character to this type parameter to tell it which one
     * to do
     * 
     * @param type
     *            the type of print method we want to use.
     *            -r: regular print
     *            -l: lengths print
     *            -s: stats print
     * @return
     *         the string to be print
     */
    public String print(char type);


    /**
     * Searches for a single sequence
     * 
     * @param sequence
     *            the sequence to search for
     * @param currDepth
     *            how far down the tree we are, so we can keep original sequence
     *            and
     *            just substring it to figure out which node to call next
     * @return
     *         result of search with number of nodes searched
     */
    public Node search(String sequence, int currDepth);


    /**
     * Recursively searches for all nodes that start with given sequence
     * 
     * @param sequence
     *            the starting sequence to search for
     * @return
     *         string representation of nodes found
     */
    public String searchAll();


    /**
     * Recursively searches for the node to remove, then returns that node for
     * DNADB
     * to handle actually removing
     * 
     * @param sequence
     *            the sequence to remove
     * @param currDepth
     *            keeping track of how far into the sequence we've searched
     * @return
     *         reference to the node to remove
     */
    public Node remove(String sequence, int currDepth);
}
