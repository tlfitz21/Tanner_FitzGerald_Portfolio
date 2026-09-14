
public class PTree {
    /**
     * for the output of getRootAndSizeList() I need to return 2 arrays this is
     * the index for the first array
     */
    public final static int ROOT_LIST_INDEX = 0;
    /**
     * for the output of getRootAndSizeList() I need to return 2 arrays this is
     * the index for the second array
     */
    public final static int ROOT_SIZE_LIST_INDEX = 1;

    private int[] array; // Node array
    /*
     * Because there can be gaps in the graph indexing we need to keep track of
     * where each index of the array corresponds to which index of the main
     * graph
     */
    private int[] listOfUsedNodeIndexes;

    PTree(int[] listOfUsedNodeIndexes) {
        this.listOfUsedNodeIndexes = listOfUsedNodeIndexes;
        array = new int[listOfUsedNodeIndexes.length]; // Create node array
        for (int i = 0; i < array.length; i++) {
            array[i] = -1; // Each node is its own root to start
        }

    }


    // Merge two subtrees if they are different
    public void union(int a, int b) {
        // convert from graph indexes to ptree indexes
        for (int i = 0; i < listOfUsedNodeIndexes.length; i++) {
            if (a == listOfUsedNodeIndexes[i]) {
                a = i;
            }
            else if (b == listOfUsedNodeIndexes[i]) {
                b = i;
            }
        }
        int root1 = find(a); // Find root of node a
        int root2 = find(b); // Find root of node b
        if (root1 != root2) { // Merge two trees
            array[root1] = root2;
        }
    }


    // Return the root of curr's tree
    public int find(int curr) {
        while (array[curr] != -1) {
            curr = array[curr];
        }
        return curr; // Now at root
    }


    public int countConnectedComponents() {
        int out = 0;
        for (int i = 0; i < array.length; i++) {
            if (array[i] == -1) {
                out++;
            }
        }
        return out;
    }


    public int[][] getRootAndSizeList() {
        int[] rootList = new int[countConnectedComponents()];
        int[] sizesOfConnectedComponents = new int[rootList.length];

        int rootListSize = 0;

        for (int i = 0; i < array.length; i++) {
            int root = find(i);
            boolean rootFound = false;
            for (int j = 0; j < rootListSize && !rootFound; j++) {
                if (rootList[j] == listOfUsedNodeIndexes[root]) {
                    sizesOfConnectedComponents[j]++;
                    rootFound = true;
                }
            }
            if (!rootFound) {
                rootList[rootListSize] = listOfUsedNodeIndexes[root];
                sizesOfConnectedComponents[rootListSize++]++;
            }
        }
        return new int[][] { rootList, sizesOfConnectedComponents };
    }


    public int[] getAllNodeIndexsWithSameRoot(
        int root,
        int numberOfConnectionsToRoot) {
        int[] connectedComponentsNodesIndexes =
            new int[numberOfConnectionsToRoot];
        for (int i = 0, j =
            0; j < connectedComponentsNodesIndexes.length; i++) {
            int rootfound = find(i);
            if (listOfUsedNodeIndexes[rootfound] == root) {
                connectedComponentsNodesIndexes[j++] = listOfUsedNodeIndexes[i];
            }
        }

        return connectedComponentsNodesIndexes;
    }
}
