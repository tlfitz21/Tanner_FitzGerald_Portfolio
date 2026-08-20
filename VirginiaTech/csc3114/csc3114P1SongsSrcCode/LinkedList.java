
public class LinkedList 
{

    // instance variables

    /**
     * head: the first node of the list
     */
    private Node head;
    /**
     * tail: the last node of the list 
     */
    private Node tail;
    /**
     * curr: the node the list is pointing to
     */
    private Node curr;

    /**
     * constructor
     * 
     * @param head
     *            the first node of the list
     * 
     *            sets the head, tail, and current pointers to the only node in
     *            the list
     */
    public LinkedList(Node head)
    {
        this.head = head;
        this.curr = head;
        this.tail = head;
    }

    // accessors


    /**
     * @return
     *         the first node
     */
    public Node getHead() 
    {
        return head;
    }


    /**
     * @return
     *         the last node
     */
    public Node getTail() 
    {
        return tail;
    }


    /**
     * @return
     *         the current node
     */
    public Node getCurr() 
    {
        return curr;
    }

    //Behavioral methods 
    
    /**
     * iterates curr to the next node
     * 
     * @return
     *         returns false if you're already at the end
     */
    public boolean forward() 
    {
        if (curr == tail)
            return false;
        curr = curr.getNext();
        return true;
    }


    /**
     * iterates curr to the previous node
     * 
     * @return
     *         returns false if you're already at the beginning
     */
    public boolean back() 
    {
        if (curr == head)
            return false;
        curr = curr.getPrev();
        return true;
    }

    /**
     * just sets current pointer to the beginning
     */
    public void toStart()
    {
        curr = head;
    }
    
    /**
     * checks if the list is empty
     * 
     * @return
     *         returns true if it's empty
     */
    public boolean isEmpty() 
    {
        if (head == null)
            return true;
        else
            return false;
    }


    /**
     * insert a new node after the curr pointer
     * 
     * @param data
     *            data to be held by the new node
     * 
     */
    public void insertAfter(int data) 
    {
        //just pass append if you're at the tail
        if (curr == tail)
            this.append(data);
        else 
        {
            Node newNode = new Node(data, curr.getNext(), curr);
            curr.getNext().setPrev(newNode);
            curr.setNext(newNode);
        }
    }

    /**
     * insert a new node after the curr pointer
     * 
     * @param node
     *          the node to be inserted
     * 
     */
    public void insertAfter(Node node) 
    {
        //just pass append if you're at the tail
        if (curr == tail)
            this.append(node);
        else 
        {
            //Node newNode = new Node(data, curr.getNext(), curr);
            node.setNext(curr.getNext());
            node.setPrev(curr);
            curr.getNext().setPrev(node);
            curr.setNext(node);
        }
    }

    /**
     * adds a new node to the end of the list
     * 
     * @param data
     *            the data to be held by the new node
     */
    public void append(int data) 
    {
        Node newNode = new Node(data, null, tail);
        if (this.isEmpty()) 
        {
            head = newNode;
            curr = newNode;
            tail = newNode;
        }
        else 
        {
            tail.setNext(newNode);
            newNode.setPrev(tail);
            tail = newNode;
        }
    }
    
    public void append(Node node) 
    {
        if (this.isEmpty()) 
        {
            head = node;
            curr = node;
            tail = node;
        }
        else {
            tail.setNext(node);
            node.setPrev(tail);
            tail = node;
        }
    }


    /**
     * adds a new node to the beginning of the list
     * 
     * @param data
     *            the data to be held by the new node
     */
    public void prepend(int data) 
    {
        Node newNode = new Node(data, head);
        if (this.isEmpty()) 
        {
            head = newNode;
            curr = newNode;
            tail = newNode;
        }
        else 
        {
            head.setPrev(newNode);
            newNode.setNext(head);
            head = newNode;
        }
    }
    
    /**
     * adds a new node to the beginning of the list
     * 
     * @param node
     *          the node to be added
     */
    public void prepend(Node node) 
    {
        if (this.isEmpty()) 
        {
            head = node;
            curr = node;
            tail = node;
        }
        else 
        {
            head.setPrev(node);
            node.setNext(head);
            head = node;
        }
    }


    /**
     * removes the node at the curr pointer from the linked list
     * 
     * @return
     *         returns the node that was removed
     */
    public Node remove() 
    {
        Node rNode = curr;
        
        //if there's only one element
        if (head == tail) {
            head = null;
            tail = null;
            curr = null;
            return rNode;
        }
        //don't have to worry about previous node if it's the head
        else if (curr == head) 
        {
            head = head.getNext();
            head.setPrev(null);
            this.forward();
            rNode.setNext(null);
            return rNode;
        }
      //don't have to worry about next node if it's the tail
        else if (curr == tail) 
        {
            tail = curr.getPrev();
            tail.setNext(null);
            this.back();
            rNode.setPrev(null);
            
            return rNode;
        }
        //make sure to reassign pointers to/from prev and next nodes
        else 
        {
            this.forward();
            rNode.getPrev().setNext(rNode.getNext());
            rNode.getNext().setPrev(rNode.getPrev());
            rNode.setNext(null);
            rNode.setPrev(null);
            
            return rNode;
        }
    }


    /**
     * completely empties the list
     * 
     * @return
     *         returns true if the list was successfully cleared
     */
    public boolean clear() 
    {
        head = null;
        tail = null;
        curr = null;
        if (this.isEmpty())
            return true;
        else
            return false;
    }


    /**
     * prints the list contents(the nodes' data) out sequentially from head
     * 
     * @return
     *         returns a string representing the list to be printed
     */
    public String print() 
    {
        //in case the list is empty
        if (this.isEmpty()) 
        {
            System.out.println("Empty List");
            return "Empty List";
        }
        String outString = "The list: ";
        //store what curr is set to so it can be returned to it after traversal
        Node temp = curr;
        curr = head;
        //print each nodes data one node at a time
        System.out.print("The list: " + Integer.toString(curr.getData())
            + " -> ");
        outString += Integer.toString(curr.getData()) + " -> ";
        while (this.forward()) 
        {
            System.out.print(Integer.toString(curr.getData()) + " -> ");
            outString += Integer.toString(curr.getData()) + " -> ";
        }
        //reassign curr to what it was before and return the string
        curr = temp;
        return outString;
    }
}
