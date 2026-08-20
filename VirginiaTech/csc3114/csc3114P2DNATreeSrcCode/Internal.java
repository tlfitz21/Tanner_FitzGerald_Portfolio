
public class Internal implements Node {

    /**
     * 5 Node pointers
     */
    private Node pointA;
    private Node pointC;
    private Node pointG;
    private Node pointT;
    private Node pointD;



    /**
     * Constructor if nothing is specified
     * All pointers are set to flyweight
     */
    public Internal() {
        pointA = DNADB.fw;
        pointC = DNADB.fw;
        pointG = DNADB.fw;
        pointT = DNADB.fw;
        pointD = DNADB.fw;
    }


    /**
     * Accessor method for all 5 pointers
     * 
     * @param point
     *            the character corresponding to
     *            the pointer you want returned
     * @return
     *         the pointer
     */
    public Node getPoint(char point) {

        Node rPoint;

        switch (point) {
            case 'A':
                rPoint = pointA;
                break;
            case 'C':
                rPoint = pointC;
                break;
            case 'G':
                rPoint = pointG;
                break;
            case 'T':
                rPoint = pointT;
                break;
            default:
                rPoint = pointD;
                break;
        }

        return rPoint;
    }


    /**
     * Mutator method for all 5 pointers
     * 
     * @param point
     *            the character corresponding to
     *            the pointer you want updated
     * @param newNode
     *            the node you want to change it to
     */
    public void setPoint(char point, Node newNode) {
        switch (point) {
            case 'A':
                pointA = newNode;
                break;
            case 'C':
                pointC = newNode;
                break;
            case 'G':
                pointG = newNode;
                break;
            case 'T':
                pointT = newNode;
                break;
            default:
                pointD = newNode;
                break;
        }
    }


    /**
     * 
     * Part of the recursive insert function. identifies the pointer
     * the sequence would go through and recursively calls insert at
     * that node. returns itself.
     * 
     * @param sequence
     *            the sequence to be inserted
     * @param depth
     *            the current depth of the node within the entire tree
     * @return
     *         returns itself as the updated root of what was called before
     */
    public Node insert(String sequence, int depth) {
        // the letter that we follow to sort the sequence
        char letter = sequence.charAt(depth);
        // recursively call insert at the node the respective pointer
        // is pointing to, and set the pointer to it in case it changes
        setPoint(letter, getPoint(letter).insert(sequence, depth + 1));

        return this;
    }


    /**
     * Prints the tree
     * 
     * @param type
     *            what kind of print we're doing
     * 
     * @return the next part of the string in the final returned string
     */
    public String print(char type) {
        StringBuilder sb = new StringBuilder();

        sb.append("I\n  ");
        sb.append(pointA.print(type) + "\n  ");
        sb.append(pointC.print(type) + "\n  ");
        sb.append(pointG.print(type) + "\n  ");
        sb.append(pointT.print(type) + "\n  ");
        sb.append(pointD.print(type));

        return sb.toString();
    }


    /**
     * Searches the tree for a node
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
        if (currDepth < sequence.length()) {
            switch (sequence.charAt(currDepth)) {
                case ('A'):
                    return pointA.search(sequence, currDepth + 1);
                case ('C'):
                    return pointC.search(sequence, currDepth + 1);
                case ('G'):
                    return pointG.search(sequence, currDepth + 1);
                case ('T'):
                    return pointT.search(sequence, currDepth + 1);
                default:
                    return pointD.search(sequence, 0);
            }
        }
        return this;
    }


    /**
     * Searches for all children of a prefix
     * 
     * @return the next part of the final returned string
     */
    public String searchAll() {
        StringBuilder sb = new StringBuilder();
        DNADB.visited++;

        sb.append(pointA.searchAll());
        sb.append(pointC.searchAll());
        sb.append(pointG.searchAll());
        sb.append(pointT.searchAll());
        sb.append(pointD.searchAll());

        return sb.toString();
    }


    /**
     * Removes a node from the tree
     * 
     * @param sequence
     *            the sequence of the node to remove
     * 
     * @param currDepth
     *            the current depth in the tree to keep track of which letter
     *            we're on
     * 
     * @return returns either itself, or one of its pointers to collapse the
     *         tree if needed
     */
    public Node remove(String sequence, int currDepth) {
        // Mimics search logic to find node to be removed
        char currChar = sequence.charAt(currDepth);
        currDepth++;
        // If we're at the last internal node, this will set the pointer to
        // either the same leaf node it already was or a flyweight if it was
        // removed
        switch (currChar) {
            case ('A'):
                pointA = pointA.remove(sequence, currDepth);
                break;
            case ('C'):
                pointC = pointC.remove(sequence, currDepth);
                break;
            case ('G'):
                pointG = pointG.remove(sequence, currDepth);
                break;
            case ('T'):
                pointT = pointT.remove(sequence, currDepth);
                break;
            default:
                pointD = pointD.remove(sequence, 0);

        }

        // Check how many children we have, if we have one leaf: collapse
        if (!hasOneLeafOnly()) {
            return this;
        }

        // Collapse
        if (pointA instanceof Leaf) {
            return pointA;
        }
        if (pointC instanceof Leaf) {
            return pointC;
        }
        if (pointG instanceof Leaf) {
            return pointG;
        }
        if (pointT instanceof Leaf) {
            return pointT;
        }
        return pointD;
    }


    /**
     * Helper method for remove to see if we need to collapse
     * 
     * @return whether or not we exactly one leaf and nothing else
     */
    private boolean hasOneLeafOnly() {
        int leafs = 0;
        int ints = 0;
        if (pointA instanceof Leaf) {
            leafs++;
        }
        if (pointA instanceof Internal) {
            ints++;
        }
        if (pointC instanceof Leaf) {
            leafs++;
        }
        if (pointC instanceof Internal) {
            ints++;
        }
        if (pointG instanceof Leaf) {
            leafs++;
        }
        if (pointG instanceof Internal) {
            ints++;
        }
        if (pointT instanceof Leaf) {
            leafs++;
        }
        if (pointT instanceof Internal) {
            ints++;
        }
        if (pointD instanceof Leaf) {
            leafs++;
        }
        if (ints > 0) {
            return false;
        }
        return leafs == 1;
    }
}
