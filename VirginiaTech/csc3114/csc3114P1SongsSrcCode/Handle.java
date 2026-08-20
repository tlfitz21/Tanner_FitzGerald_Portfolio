
public class Handle 
{
    //instance variables
    
    /**
     *memIndex: the index of memory pool the handle will be storing 
     */
    private int memIndex; 
    
    /**
     * 
     */
    private int byteSize;
    
    private long key;
    
    private long otherKey;
    
    private boolean isTomb;
    
    public Handle(int memIndex, int byteSize, long key, long otherKey)
    {
        this.memIndex = memIndex;
        this.byteSize = byteSize;
        this.key = key;
        this.otherKey = otherKey;  
        isTomb = false;
    }
    
    public int getMemIndex() 
    {
        return memIndex;
    }
    
    public int getByteSize() 
    {
        return byteSize;
    }
    
    public long getKey() 
    {
        return key;
    }
    
    public long getOtherKey() 
    {
        return otherKey;
    }
    
    public void setMemIndex(int index) 
    {
        memIndex = index;
    }
    
    public void setByteSize(int size) 
    {
        byteSize = size;
    }
    
    public void setKey(long key) 
    {
        this.key = key;
    }
    
    public void setOtherKey(long otherKey) 
    {
        this.otherKey = otherKey;
    }
    
    public boolean getTomb() {
        return isTomb;
    }
    
    public void setTomb(boolean set) {
        isTomb = set;
    }
}
