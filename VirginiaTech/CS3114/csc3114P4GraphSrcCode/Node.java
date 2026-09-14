/**
 * each node of the graph
 * 
 * @author tlfitz
 * 
 * @version SP 2026
 */
public class Node {



    // for the adjacency list
    private Node next;
    private Node prev;
    
    //index of connection
    private int index;
    
    
    public Node(int index) {
        this.index = index;
        next = null;
        prev = null;
    }


    public int getIndex() {
        return index;
    }


    public void setIndex(int index) {
        this.index = index;
    }

    public Node getNext() {
        return next;
    }


    public void setNext(Node next) {
        this.next = next;
    }


    public Node getPrev() {
        return prev;
    }


    public void setPrev(Node prev) {
        this.prev = prev;
    }

}
