/*Tanner FitzGerald
 *Personal practice project
 *9/12/24
 *
 * This program will take an input string (sentence) and return the word count of that string.
 * For now, we'll assume the user isn't typing repeating spaces in their sentences.
 * 
 */
import java.util.*;
public class getWordCountGH {
    public static void main(String[] args) {
        //Input
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter your sentence: ");
        String inputSentence = keyboard.nextLine();
        //Process
        String[] inputWords = inputSentence.split(" ");
        int wordCount = inputWords.length;
        //Output
        System.out.println("There are " + wordCount + " words in this sentence.");
        keyboard.close();

    }

}
