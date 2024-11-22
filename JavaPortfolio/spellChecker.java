/*Tanner FitzGerald
 * 11/17/24
 * CSC 222 Exercise 10
 * 
 * This program will prompt the user to enter the name of a file, then create a scanner to get the
 * contents of the file, and store them into an array called dictionary. It then allows the user to
 * enter words and outputs if the word exists in the dictionary or not, as well as where it is. Entering
 * "stop" will end the program.
 * 
 */

import java.util.*;
import java.io.*;
public class spellChecker{
    public static void main(String[] args) {
        //getting dictinary from input
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter the file name: ");
        String fileName = keyboard.nextLine();

        //creating array to store dictionary
        String[] dictionary = new String[10];

        //creating a new scanner to read the dictionary file
        Scanner inputStream = null;
        try{
            inputStream = new Scanner(new File(fileName));
        }
        catch (FileNotFoundException e){
            System.out.println("Error opening file "+fileName);
            System.exit(0);
        }

        //load content from file into array
        int i = 0;
        while (inputStream.hasNext()){
            dictionary[i] = inputStream.nextLine();
            i++;
        }

        //provide user the interface to search for a word
        while (true){
            System.out.print("Search for word: ");
            String target = keyboard.nextLine();
            //continuation condition check
            if (target.equals("stop")){
                System.out.println("Bye!");
                break;
            }
            boolean flag = false;
            for (int k = 0; k < dictionary.length; k++){
                if (target.equals(dictionary[k])){
                    flag = true;
                    System.out.println(target+" is found at position "+k);
                    break;
                }
            }
            if (!flag){
                System.out.println(target+" is not found");
            }
        }
        keyboard.close();
    }
}
