
public class HashEntry {
    private String key;
    private int nodeIndex;

    public HashEntry(String key, int nodeIndex) {
        this.key = key;
        this.nodeIndex = nodeIndex;
    }


    public String getKey() {
        return key;
    }


    public void setKey(String key) {
        this.key = key;
    }


    public int getNodeIndex() {
        return nodeIndex;
    }


    public void setNodeIndex(int nodeIndex) {
        this.nodeIndex = nodeIndex;
    }

}
