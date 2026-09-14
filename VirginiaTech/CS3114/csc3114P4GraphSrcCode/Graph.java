
public class Graph {

    // the adjacency list
    private LinkedList[] adjList;
    private int size;
    private int initalSize;

    public Graph(int initalSize) {
        this.initalSize = initalSize;
        adjList = new LinkedList[initalSize];
        size = 0;
    }


    public int insert() {
        if (isFull()) {
            resize();
        }
        LinkedList list = new LinkedList();
        int i;
        for (i = 0; i < adjList.length; i++) {
            if (adjList[i] == null) {
                adjList[i] = list;
                size++;
                break;
            }
        }
        return i;
    }


    public void addEdge(int u, int v) {
        LinkedList first = adjList[u];
        LinkedList second = adjList[v];
        first.append(new Node(v));
        // i think we are not suposed to connect the songs to their artists
        // we can change this later if it causes a problem

        second.append(new Node(u));
    }


    /**
     * 
     * remove this entry from the graph
     * 
     * @param index
     * 
     * @return
     *         returns true unless the index doesn't exist or the was no
     *         connection at one of its nodes for some reason
     */
    public boolean remove(int index) {

        if (index >= adjList.length)
            return false;

        // store the list to be removed
        LinkedList removeList = adjList[index];
        if (removeList == null)
            return false;
        if (removeList.getCurr() != null) {

            // a variable for the removeList's connections
            LinkedList connectList;

            // remove the connection to removeList's node for each of its
            // connections
            do {
                // store the connected node
                connectList = adjList[removeList.getCurr().getIndex()];
                // find and remove the connection to removeList's node.

                connectList.remove(index);
            }
            while (removeList.forward());
        }
        adjList[index] = null;
        size--;
        return true;

    }


    public boolean hasEdge(int u, int v) {
        return adjList[u].find(v) != null;
    }


    public int getSize() {
        return size;
    }


    public void clear() {
        adjList = new LinkedList[initalSize];
        size = 0;
    }


    public void printgraph(StringBuilder log) {
        PTree pt = creatPTree();
        int[][] rootList = pt.getRootAndSizeList();
        log.append("There are ").append(rootList[PTree.ROOT_LIST_INDEX].length)
            .append(" connected components\n");
        if (rootList[PTree.ROOT_LIST_INDEX].length == 0) {
            log.append("The largest connected component has ").append(0).append(
                " elements\n");
            log.append("The diameter of the largest component is ").append(0)
                .append("\n");
            return;
        }
        // find max
        int indexOfMax = 0;
        for (int i = 0; i < rootList[PTree.ROOT_SIZE_LIST_INDEX].length; i++) {
            if (rootList[PTree.ROOT_SIZE_LIST_INDEX][i] > rootList[PTree.ROOT_SIZE_LIST_INDEX][indexOfMax]) {
                indexOfMax = i;
            }
        }
        log.append("The largest connected component has ").append(
            rootList[PTree.ROOT_SIZE_LIST_INDEX][indexOfMax]).append(
                " elements\n");
        int[] labelsForFloyedMatrix = pt.getAllNodeIndexsWithSameRoot(
            rootList[PTree.ROOT_LIST_INDEX][indexOfMax],
            rootList[PTree.ROOT_SIZE_LIST_INDEX][indexOfMax]);
        int[][] floyedMatrix = floyd(labelsForFloyedMatrix);
        int diameter = 0;
        for (int[] row : floyedMatrix) {
            for (int i = 0; i < row.length; i++) {
                if (row[i] > diameter && row[i] != Integer.MAX_VALUE) {
                    diameter = row[i];
                }
            }
        }
        log.append("The diameter of the largest component is ").append(diameter)
            .append("\n");
    }


    public PTree creatPTree() {
        PTree pt = new PTree(generateUsedIndexList());
        for (int i = 0; i < adjList.length; i++) {
            LinkedList l = adjList[i];
            if (l != null && l.getCurr() != null) {
                do {
                    pt.union(i, l.getCurr().getIndex());
                }
                while (l.forward());
            }
            if (l != null) {
                l.currReset();
            }

        }
        return pt;
    }


    public int[] generateUsedIndexList() {

        int[] usedIndexList = new int[getSize()];
        for (int i = 0, j = 0; i < adjList.length
            && j < usedIndexList.length; i++) {
            if (adjList[i] != null)
                usedIndexList[j++] = i;
        }

        return usedIndexList;
    }


    public int getCapacity() {
        return adjList.length;
    }


    private boolean isFull() {
        return size >= adjList.length;
    }


    private void resize() {
        LinkedList[] newList = new LinkedList[adjList.length << 1];
        for (int i = 0; i < adjList.length; i++) {
            newList[i] = adjList[i];
        }
        adjList = newList;
    }


    public int[][] floyd(int[] connectedComponentsNodesIndexes) {
        int[][] d =
            new int[connectedComponentsNodesIndexes.length][connectedComponentsNodesIndexes.length];

        // Initialize D with weights
        for (int i = 0; i < connectedComponentsNodesIndexes.length; i++) {
            for (int j = 0; j < connectedComponentsNodesIndexes.length; j++) {
                if (i == j) {
                    d[i][j] = 0;
                }
                else if (hasEdge(connectedComponentsNodesIndexes[i],
                    connectedComponentsNodesIndexes[j])) {
                    d[i][j] = 1;
                }
                else {
                    d[i][j] = Integer.MAX_VALUE;
                }
            }
        }
        // Compute all k paths
        for (int k = 0; k < connectedComponentsNodesIndexes.length; k++) {
            for (int i = 0; i < connectedComponentsNodesIndexes.length; i++) {
                for (int j =
                    0; j < connectedComponentsNodesIndexes.length; j++) {
                    if ((d[i][k] != Integer.MAX_VALUE)
                        && (d[k][j] != Integer.MAX_VALUE) && (d[i][j] > (d[i][k]
                            + d[k][j]))) {
                        d[i][j] = d[i][k] + d[k][j];
                    }
                }
            }
        }
        return d;
    }
}
