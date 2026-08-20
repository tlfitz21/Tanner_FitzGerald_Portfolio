/**
 * Linked List implementation.
 * 
 *
 * @author CTanner FitzGerald
 * @version S26
 */
public class LinkedList {

    private Node head;
    private Node tail;
    private Node curr;

    public LinkedList() {
        head = null;
        tail = null;
        curr = null;
    }


    public void clear() {
        head = null;
        tail = null;
        curr = null;
    }


    public Node getHead() {
        return head;
    }


    public Node getTail() {
        return tail;
    }


    public Node getCurr() {
        return curr;
    }


    public boolean forward() {
        if (curr == null)
            return false;
        if (curr.getNext() == null)
            return false;
        curr = curr.getNext();
        return true;
    }


    public void currReset() {
        curr = head;
    }


    /**
     * 
     * Adds a new node to the list
     * 
     * @param newNode
     *            the node to be added
     */
    public void append(Node newNode) {
        if (head == null) {
            head = newNode;
            tail = newNode;
            curr = newNode;
        }
        else {
            tail.setNext(newNode);
            newNode.setPrev(tail);
            tail = newNode;
        }
    }


    /**
     * 
     * Looks for the node with a given index
     * returns null if it's not found
     * 
     * @param theIndex
     *            the index we're looking for
     * @return
     *         the node we're looking for, or null if not found
     */
    public Node find(int theIndex) {
        Node current = head;
        if (head == null)
            return null;

        do {
            if (current.getIndex() == theIndex) {
// curr = head;
                return current;
            }

            current = current.getNext();
        }
        while (current != null);

        // not found, so return null
// curr = head;
        return null;

    }


    public boolean remove(int theIndex) {
        // get the node in question
        Node theNode = find(theIndex);

        if (theNode == null)
            return false;

        if (head == theNode) {
            if (tail == theNode) {
                head = null;
                tail = null;
                curr = null;
            }
            else {
                head.getNext().setPrev(null);
                head = head.getNext();
                curr = head;
            }
        }

        else if (tail == theNode) {

            tail.getPrev().setNext(null);
            tail = tail.getPrev();

        }

        else {
            theNode.getPrev().setNext(theNode.getNext());
            theNode.getNext().setPrev(theNode.getPrev());
        }

// if (head != theNode)
// theNode.getPrev().setNext(theNode.getNext());
// if (tail != theNode)
// theNode.getNext().setPrev(theNode.getPrev());

        return true;

    }
}
