import java.io.IOException;

/**
 * The database implementation for this project.
 * We have two hash tables and graph.
 *
 * @author CS3114/5040 Staff
 * @version Spring 2026
 */
public class GraphDB implements GPInterface {

    private HashTable artHash;
    private HashTable songHash;
    private Graph graph;

    // ----------------------------------------------------------
    /**
     * Create a new GraphDB object.
     * But don't set anything -- that gets done by "create"
     */
    public GraphDB() {
    }


    /**
     * Create a brave new World.
     *
     * @param inHash
     *            Initial size for hash tables
     * @return Error messages if appropriate
     */
    public String create(int inHash) {
        if (inHash < 1) {
            return "Initial hash table size must be positive";
        }

        if (artHash != null) {
            return "Database already exists";
        }
        artHash = new HashTable(inHash);
        songHash = new HashTable(inHash);
        // needs an entry for both the songs and artists
        // no please stop makeing stuff up
        // this just brakes the test cases
        graph = new Graph(inHash);
        return null;
    }


    /**
     * Re-initialize the database
     * 
     * @return true on successful clear of database
     */
    public boolean clear() {
        // handles trying to clear before it was initialized
        if (artHash == null) {
            return false;
        }

        // create a new DB using the initial size
        artHash.clear();
        songHash.clear();
        graph.clear();
        return true;
    }


    // ----------------------------------------------------------
    /**
     * Insert to the hash table
     *
     * @param artistString
     *            Artist string to insert
     * @param songString
     *            Song string to insert
     * @return Status message as appropriate
     * @throws IOException
     */
    public String insert(String artistString, String songString)
        throws IOException {
        if (artHash == null) {
            return "Database not initialized";
        }
        if ((artistString == null || artistString.isEmpty())
            || (songString == null || songString.isEmpty())) {
            return "Input strings cannot be null or empty";
        }
        StringBuilder log = new StringBuilder();
        int artIndex = artHash.find(artistString);
        int songIndex = songHash.find(songString);

        if (artIndex != -1 && songIndex != -1 && graph.hasEdge(artIndex,
            songIndex)) {
            log.append('|').append(artistString).append("<SEP>").append(
                songString).append(
                    "| duplicates a record already in the database\n");
            return log.toString();
        }

        if (artIndex == -1) {
            int gsize = graph.getCapacity();
            artIndex = graph.insert();

            int size = artHash.getCapacity();
            artHash.insert(artistString, artIndex);

            if (gsize < graph.getCapacity()) {
                log.append("Graph size doubled to ").append(graph.getCapacity())
                    .append('\n');
            }

            if (size < artHash.getCapacity()) {
                log.append("artist hash table size doubled\n");
            }

            log.append('|').append(artistString).append(
                "| is added to the Artist database\n");

        }

        if (songIndex == -1) {
            int gsize = graph.getCapacity();
            songIndex = graph.insert();

            int size = songHash.getCapacity();
            songHash.insert(songString, songIndex);

            if (gsize < graph.getCapacity()) {
                log.append("Graph size doubled to ").append(graph.getCapacity())
                    .append('\n');
            }

            if (size < songHash.getCapacity()) {
                log.append("song hash table size doubled\n");
            }

            log.append('|').append(songString).append(
                "| is added to the Song database\n");

        }

        graph.addEdge(artIndex, songIndex);
        return log.toString();
    }


    // ----------------------------------------------------------
    /**
     * Remove from the hash table
     *
     * @param type
     *            The table to be removed from
     * @param nameString
     *            The string to be removed from the table
     * @return Status message as appropriate
     * @throws IOException
     */
    public String remove(String type, String nameString) throws IOException {
        if (artHash == null) {
            return "Database not initialized";
        }
        if ((type == null || type.isEmpty()) || (nameString == null
            || nameString.isEmpty())) {
            return "Input strings cannot be null or empty";
        }
        if (type.equalsIgnoreCase("artist")) {

            // remove it from the hash
            int artIndex = artHash.remove(nameString);

            // if it didn't exist, return error message
            if (artIndex == -1)
                return "|" + nameString
                    + "| does not exist in the Artist database";

            // otherwise, remove it and all connections to it from the graph
            graph.remove(artIndex);
            return "|" + nameString + "| is removed from the Artist database\n";
        }
        if (type.equalsIgnoreCase("song")) {

            // remove it from the hash
            int songIndex = songHash.remove(nameString);

            // if it didn't exist, return error message
            if (songIndex == -1)
                return "|" + nameString
                    + "| does not exist in the Song database";

            // otherwise, remove it and all connections to it from the graph
            graph.remove(songIndex);
            return "|" + nameString + "| is removed from the Song database\n";
        }
        return "Bad type value |" + nameString + "| on remove";
    }


    // ----------------------------------------------------------
    /**
     * Print out the hash table contents
     *
     * @param type
     *            Controls what object is being printed
     * @return Status message as appropriate
     * @throws IOException
     */
    public String print(String type) throws IOException {
        if (artHash == null) {
            return "Database not initialized";
        }
        if (type == null || type.isEmpty()) {
            return "Input strings cannot be null or empty";
        }

        if (type.equalsIgnoreCase("artist")) {
            StringBuilder log = new StringBuilder();
            log.append(artHash.toString());
            log.append("total artists: ").append(artHash.getCount()).append(
                '\n');
            return log.toString();
        }
        if (type.equalsIgnoreCase("song")) {
            StringBuilder log = new StringBuilder();
            log.append(songHash.toString());
            log.append("total songs: ").append(songHash.getCount()).append(
                '\n');
            return log.toString();
        }
        return "Bad print parameter";
    }


    // ----------------------------------------------------------
    /**
     * Print out the graph information
     *
     * @return The string that was printed
     */
    public String printgraph() {
        StringBuilder log = new StringBuilder();
        graph.printgraph(log);
        return log.toString();
    }
}
