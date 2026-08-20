/**
 * This is a subclass of Node. It contains data for he DNA sequence, but no
 * pointers
 * 
 * @author Tanner FitzGerald / Collin Greene
 * @version Spring 2026
 */
public class Leaf implements Node {

    /**
     * The data for this leaf node
     */
    private String sequence;

    /**
     * Constructor for the leaf
     * 
     * @param inSeq
     *            the sequence that gets stored in it
     */
    public Leaf(String inSeq) {
        sequence = inSeq;
    }


    /**
     * Accessor method for the Node's sequence
     * 
     * @return
     *         the sequence itself
     */
    public String getSeq() {
        return sequence;
    }

    /**
     * Mutator method for the Node's sequence
     * 
     * @param inSeq
     *            the new sequence for the leaf
     */
// public void setSeq(String inSeq) {
// sequence = inSeq;
// }


    /**
     * 
     * Part of the recursive insert function. splits the leaf
     * and then calls insert again on the new internal node.
     * it will return that internal node back to what called this
     * 
     * @param inSeq
     *            the sequence to be inserted
     * @param depth
     *            the current depth of the node within the entire tree
     * @return
     *         returns the result of the recursive call, which will always
     *         be the resulting "new root" that is at the position this used
     *         to be in
     */
    public Node insert(String inSeq, int depth) {

        // If the sequence matches this leaf's sequence, it's a duplicate and we
        // need to flag it as an error
        if (inSeq.compareTo(getSeq()) == 0) {
            DNADB.isDuplicate = true;
            return this;
        }

        Internal temp = new Internal();
        temp.setPoint(getSeq().charAt(depth), this);
        return temp.insert(inSeq, depth);
    }


    /**
     * part of the recursive print function. this one of the base cases
     * here, it will print out the sequence and any additional info based
     * on the type that was passed
     * 
     * @param type
     *            the type of print function
     * 
     * @return the string for this leaf
     */
    public String print(char type) {
        String rString = "";
        switch (type) {
            case 'r':
                rString = sequence;
                break;
            case 'l':
                rString = sequence + " " + (sequence.length() - 1);
                break;
            case 's':
                double a = 0;
                double c = 0;
                double g = 0;
                double t = 0;
                for (int i = 0; i < sequence.length(); i++) {
                    switch (sequence.charAt(i)) {
                        case 'A':
                            a++;
                            break;
                        case 'C':
                            c++;
                            break;
                        case 'G':
                            g++;
                            break;
                        case 'T':
                            t++;
                            break;
                    }
                }
                double aPer = 100.00 * a / (double)(sequence.length() - 1);
                double cPer = 100.00 * c / (double)(sequence.length() - 1);
                double gPer = 100.00 * g / (double)(sequence.length() - 1);
                double tPer = 100.00 * t / (double)(sequence.length() - 1);

                rString = String.format("%s A:%.2f C:%.2f G:%.2f T:%.2f",
                    sequence, aPer, cPer, gPer, tPer);
                break;
        }
        return rString;

    }


    /**
     * The end of the simple version of search
     * 
     * @param sequence
     *            the sequence to compare this to
     * 
     * @param currDepth
     *            the current depth into the tree
     * 
     * @return returns itself since we found it
     */
    public Node search(String seq, int currDepth) {
        DNADB.visited = DNADB.visited + 1;
        return this;
    }


    /**
     * The end of the recursive version of search
     * 
     * @return returns a string to append to the final returned string
     */
    public String searchAll() {
        DNADB.visited++;
        return sequence.substring(0, sequence.length() - 1) + "\n";
    }


    /**
     * The end of the remove function
     * 
     * @param sequence
     *            the sequence to compare with to see if this node needs to be
     *            removed
     * 
     * @param currDepth
     *            the current depth into this tree
     * 
     * @return returns a flyweight to remove this node or itself to not remove
     *         it
     */
    public Node remove(String seq, int currDepth) {
        if (seq.equals(this.sequence)) {
            return DNADB.fw;
        }
        else {
            return this;
        }
    }
}
