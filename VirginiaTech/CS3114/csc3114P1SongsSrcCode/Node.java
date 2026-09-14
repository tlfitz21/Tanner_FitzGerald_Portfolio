
public class Node
{

    // instance variables

    /**
     * data: the node's data
     */
    private int data;

    /**
     * next: pointer to the next node 
     */
    private Node next = null;

    /**
     * prev: pointer to the previous node
     */
    private Node prev = null;

    // Constructors

    /**
     * @param poolIndex
     *            takes just the data
     */
    public Node(int poolIndex) 
    {
        data = poolIndex;
    }


    /**
     * @param poolIndex
     * @param next
     *            takes the data and next node
     */
    public Node(int poolIndex, Node next)
    {
        data = poolIndex;
        this.next = next;
    }


    /**
     * @param poolindex
     * @param next
     * @param prev
     *            takes data, next node, and
     *            previous node
     */
    public Node(int poolindex, Node next, Node prev) 
    {
        data = poolindex;
        this.next = next;
        this.prev = prev;

    }


    // accessor methods
    /**
     * @return
     *         gets the data
     */
    public int getData() 
    {
        return this.data;
    }


    /**
     * @return
     *         gets the next pointer
     */
    public Node getNext() 
    {
        return this.next;
    }


    /**
     * @return
     *         gets the previous pointer
     */
    public Node getPrev() 
    {
        return this.prev;
    }


    // mutator methods
    /**
     * @param data
     *            sets the data
     */
    public void setData(int data) 
    {
        this.data = data;
    }


    /**
     * @param next
     *            sets the next pointer
     */
    public void setNext(Node next) 
    {
        this.next = next;
    }


    /**
     * @param prev
     *            sets the previous pointer
     */
    public void setPrev(Node prev)
    {
        this.prev = prev;
    }


}