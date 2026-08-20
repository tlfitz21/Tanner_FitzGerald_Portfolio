
public class Flyweight implements Node {

    /**
     * 
     * part of the recursive insert algorithm. this acts as the base case
     * 
     * @param inSeq
     *            the sequence to be inserted
     * @param depth
     *            the current depth of the node within the entire tree
     * @return
     *         returns the new leaf, representing the new "root" that is at the
     *         position
     *         that this node was in when it was called
     */
    public Node insert(String inSeq, int depth) {

        Leaf newLeaf = new Leaf(inSeq);
        return newLeaf;

    }


    /**
     * part of the recursive insert algorithm. one of the base cases
     * 
     * @param type
     *            the type of print function
     * @return "E" for empty
     */
    public String print(char type) {
        return "E";
    }


    /**
     * Base case of the simple search function
     * 
     * @param sequence
     *            the sequence to search for
     * 
     * @param currDepth
     *            the current depth in the tree to keep track of which letter
     *            we're on
     * 
     * @return returns the node found
     */
    public Node search(String sequence, int currDepth) {
        DNADB.visited = DNADB.visited + 1;
        return this;
    }


    /**
     * Base case for the complex search function
     * 
     * @return
     *         returns an empty string
     */
    public String searchAll() {
        DNADB.visited++;
        return "";
    }


    /**
     * Base case for remove function
     * 
     * @param sequence
     *            the sequence to remove
     * 
     * @param currDepth
     *            the current depth in the tree to keep track of which letter
     *            we're on
     * 
     * @return returns the node removed
     */
    public Node remove(String sequence, int currDepth) {
        return this;
    }
}
