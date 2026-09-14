
/**
 * Memory Manager class.
 * This version uses an array in memory.
 * This version implements the buddy method.
 *
 * @author Tanner FitzGerald
 * @version <Put something here>
 */

public class MemManager {

    // establish data structures
 
    /**
     * The memory pool
     */
    public byte[] memPool;

    /**
     * The free list, an array of linkedLists
     */
    public LinkedList[] freeList;

    /**
     * the "freelist" where we put removed nodes to be reused later
     */
    public LinkedList recycleBin;

    /**
     * Create a new MemManager object.
     *
     * @param startSize
     *            Initial size of the memory pool
     */
    public MemManager(int startSize) {

        // create the memory pool
        memPool = new byte[startSize];

        // create the freelist by converting the startSize into the result of
        // log base2
        int freeSize = (int)(Math.log((double)startSize) / Math.log(2.0));
        // establish the freeList based on its size
        freeList = new LinkedList[freeSize + 1];
        for (int i = 0; i < freeList.length; i++) {
            freeList[i] = new LinkedList(new Node(0));
            freeList[i].clear();
        }
        // set the last only entry to the largest block size at index 0
        freeList[freeSize].append(0);

        // assign the recycling bin
        recycleBin = new LinkedList(new Node(0));
        recycleBin.clear();
    }


    /**
     * 
     * given a byte array and its size, find a free slot in the memory pool, add
     * it to the pool, then update freeList and recycling if necessary
     * 
     * @param bytes
     *            the array of bytes
     * @param size
     *            the size of the array
     * @return
     *         returns the index the bytes were inserted at in the memPool
     */
    public int findFree(byte[] bytes, int size) {

        // return value
        int index;
        // searching for the smallest possible block size
        int exp = 0;
        while (size > Math.pow(2, exp)) {
            exp++;
        }
        int blockSize = (int)Math.pow(2, exp);

        // if you see an entry in the slot you need, use it.
        while (exp >= freeList.length) {
            resizePool();
            resizeFree();
        }
        if (!freeList[exp].isEmpty()) {

            // record the first free block's mempool index
            index = freeList[exp].getHead().getData();
            // copy the byte array into the memPool at the starting index for
            // size
            System.arraycopy(bytes, 0, memPool, index, size);
            // append the used freeList node to the recycling bin and remove it
            // from the freeList entry
            freeList[exp].toStart();
            recycleBin.append(freeList[exp].remove());
            // return the index the bytes were entered at

            return index;
        }

        // if you don't see a freeblock of the size you need, we need to create
        // one by cutting bigger ones in half
        else {
            // start looking at the size above what you need
            int i = exp + 1;
            while (true) {

                // if you don't have a big enough size for the bytes, resize
                if (i == freeList.length) {
                    resizePool();
                    resizeFree();
                    i--;
                }
                // then,
                // if you find a full slot

                if (!freeList[exp].isEmpty()) {

                    // record the first free block's mempool index
                    index = freeList[exp].getHead().getData();
                    // copy the byte array into the memPool at the starting
                    // index for
                    // size
                    System.arraycopy(bytes, 0, memPool, index, size);
                    // append the used freeList node to the recycling bin and
                    // remove it
                    // from the freeList entry
                    recycleBin.append(freeList[exp].remove());
                    // return the index the bytes were entered at

                    return index;
                }

                if (!freeList[i].isEmpty()) {

                    // add a node in the slot before the one you found to be
                    // full containing the
                    // index the full node had, and another immediately after
                    // it.
                    freeList[i - 1].append(freeList[i].remove());
                    // find the memPool position the newly created node will
                    // need to be set at
                    int halfIndex = freeList[i - 1].getTail().getData()
                        + (int)Math.pow(2, i - 1);
                    // add a new node with memPool position at the halfindex
                    // (check recycling first)
                    freeList[i - 1].append(checkRecycle(halfIndex));
                    // keep repeating the cutting in half until we get to the
                    // appropriate size
                    while (blockSize < (int)Math.pow(2, i - 1)) {
                        i--;
                        freeList[i - 1].append(freeList[i].remove());
                        halfIndex = freeList[i - 1].getHead().getData()
                            + (int)Math.pow(2, i - 1);
                        freeList[i - 1].append(checkRecycle(halfIndex));
                    }
                    break;
                }

                // otherwise, just look at the next size
                else {
                    i++;
                    continue;
                }

            }
            // add the bytes into the memory pool
            index = freeList[exp].getHead().getData();
            System.arraycopy(bytes, 0, memPool, index, size);
            // append the used freeList node to the recycling bin and remove it
            // from the freeList entry
            recycleBin.append(freeList[exp].remove());
            // return the memory pool index
            return index;

        }
    }


    /**
     * 
     * remove an entry from the memory pool
     * 
     * @param pos
     *            the position the removal starts at
     * @param size
     *            how many bytes to remove
     */
    public void remove(int pos, int size) {
        // 0 out the memPool

        // find the smallest possible block size needed to zero out
        int exp = 0;
        while (size > Math.pow(2, exp)) {
            exp++;
        }
        int blockSize = (int)Math.pow(2, exp);

        // fill the mempool from pos to the end of the blocksize with zeros
        for (int i = pos; i < pos + blockSize; i++) {
            memPool[i] = 0;
        }

        // add to the freeList

        // acquire the target list and set its current pointer to the head
        LinkedList subjectList = freeList[exp];
        subjectList.toStart();

        // if the list is empty, just add a new node with prepend(check
        // recycling first)
        if (subjectList.getCurr() == null)
            subjectList.prepend(checkRecycle(pos));

        // otherwise,
        else {
            // iterate curr pointer through the list until it's pointing to a
            // node that our new one should be before
            while (subjectList.getCurr().getData() < pos) {
                if (!subjectList.forward())
                    break;
            }

            // if that's the head, prepend our new node
            if (subjectList.getCurr() == subjectList.getHead()) {
                if (subjectList.getCurr().getData() > pos) {
                    subjectList.prepend(checkRecycle(pos));
                    subjectList.back();
                }
                else {
                    subjectList.append(checkRecycle(pos));
                    subjectList.forward();
                }
            }

            // other wise, go back and then insert our new node after the
            // previous one
            else {
                subjectList.back();
                subjectList.insertAfter(checkRecycle(pos));
                subjectList.forward();
            }
        }

        // record the node the just got inserted

        Node node = subjectList.getCurr();
// System.out.println(subjectList.getCurr().getData());
// System.out.println(subjectList.getCurr().getNext());

// System.out.println("before the merge:");
//
// for (int o = 0; o < freeList.length; o++) {
// if (freeList[o].getHead() == null) System.out.println("list at " + o + " is
// null");
// else
// {
// System.out.println("list at " + o + " is ");
// freeList[o].toStart();
// System.out.println(freeList[o].getCurr().getData() + " -> ");
// freeList[o].forward();
// while (freeList[o].getCurr().getNext() != null)
// {
// System.out.println(freeList[o].getCurr().getData() + " -> ");
// freeList[o].forward();
// }
// }
// }

        // merge if necessary
        merge(node, blockSize);
    }


    /**
     * 
     * a recursive function that will take a freeList node and its size, and
     * merge it with its buddy if its buddy is empty. then it will call itself
     * with the new, larger node recursively until the larger node's buddy is
     * full
     * 
     * @param node
     * @param size
     */
    public void merge(Node node, int size) {
        // find buddyIndex and the linked list the nodes are in using bitwise
        // XOR
        int buddyInd = size ^ node.getData();

        
        //look at the subject list
        int exp = 0;
        while (size > Math.pow(2, exp)) {
            exp++;
        }
        LinkedList subjectList = freeList[exp];
        
        //determine if the buddy is free and of the same size as our new node
        subjectList.toStart();
        boolean goodBuddy = false;
        
        if (subjectList.getCurr() == null) return;
        
        while (true)
        {
            if (subjectList.getCurr().getData() == buddyInd)
            {
                goodBuddy = true;
                break;
            }
            if (!subjectList.forward()) break;
        }
        
        // base case, if the node is full, stop merging
        if (!goodBuddy)
            return;

        // recursive case, start merging and then call merge recursively until
        // done
        // acts differently depending on if the buddyIndex comes before or after
        // the node
        else if (buddyInd > node.getData()) {
            // find the list in question
            
            // remove the buddy
            subjectList.toStart();
// System.out.println(subjectList.getCurr().getData());
            while (subjectList.getCurr().getData() != buddyInd) {
                subjectList.forward();
// System.out.println(subjectList.getCurr());
            }
            recycleBin.append(subjectList.remove());
            recycleBin.append(subjectList.remove());
            // go to the next size up, looking for where to insert the node
            subjectList = freeList[++exp];
            subjectList.toStart();
            while (subjectList.getCurr().getData() < node.getData()) {
                subjectList.forward();
            }
            if (subjectList.getHead() == subjectList.getCurr()) {
                subjectList.prepend(node);
// System.out.println("after prepending to 16 list: " +
// subjectList.getHead().getData());
// System.out.println(subjectList.getHead().getNext().getData());
// System.out.println(subjectList.getHead().getNext().getNext());
            }
            else {
                subjectList.back();
                subjectList.insertAfter(node);
            }
            // recursively call merge with the new, larger node
            merge(node, size * 2);
        }

        else {
            // find the node
            subjectList.toStart();
            while (subjectList.getCurr() != node) {
                subjectList.forward();
            }
            // remove the node
//            System.out.println(subjectList.getHead().getData());
//            System.out.println(subjectList.getHead().getNext().getData());
//            System.out.println(subjectList.getHead().getNext().getNext());
            recycleBin.append(subjectList.remove());

            // find the buddy node
            subjectList.toStart();
            while (subjectList.getCurr().getData() != buddyInd) {
                subjectList.forward();
            }
            Node buddyNode = subjectList.getCurr();
            recycleBin.append(subjectList.remove());
            // go to the next spot up
            subjectList = freeList[++exp];

            if (subjectList.isEmpty()) {
                subjectList.append(buddyNode);
            }
            else {
                subjectList.toStart();
                // insert the buddy node where it should be going
                while (subjectList.getCurr().getData() < buddyNode.getData()) {
                    subjectList.forward();
                }
                if (subjectList.getHead() == subjectList.getCurr()) {
                    subjectList.prepend(buddyNode);
                }
                else {
                    subjectList.back();
                    subjectList.insertAfter(buddyNode);
                }
            }
            // recursively call merge with the new, larger node
            merge(buddyNode, size * 2);
        }
    }


    /**
     * resize the memory pool by creating a new byte array twice the size, and
     * then copying the contents of the old one into the new one
     */
    public void resizePool() {
        byte[] newArr = new byte[memPool.length * 2];
        System.arraycopy(memPool, 0, newArr, 0, memPool.length);
        memPool = newArr;
    }


    /**
     * resize the freeList by creating a new array with just one more element,
     * which accounts for double the memory pool space, then copy the contents
     * of the old one into the new one
     */
    public void resizeFree() {
        int size = freeList.length;
        LinkedList[] newArr = new LinkedList[freeList.length + 1];
        System.arraycopy(freeList, 0, newArr, 0, freeList.length);
        freeList = newArr;
        freeList[freeList.length - 1] = new LinkedList(new Node(0));
        freeList[freeList.length - 1].clear();
        freeList[freeList.length - 2] = new LinkedList(new Node((int)Math.pow(2,
            size - 1)));

    }


    /**
     * 
     * to be called when a new node is inserted into freeList. it will check if
     * there's an unused node object we can reassign instead, and return that.
     * if not, it creates a new node itself and returns that
     * 
     * @param data
     *            the data for the new node
     * @return
     *         returns the new or reassigned node
     */
    public Node checkRecycle(int data) {
        if (recycleBin.isEmpty())
            return new Node(data);
        else {
            recycleBin.getHead().setData(data);
            return recycleBin.remove();
        }
    }

}
