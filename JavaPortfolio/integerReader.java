/*Tanner FitzGerald
 * 11/22/24
 * CSC 222 Programming assignment 10
 * 
 * This program will read the "data" file in this folder. It prompts the user for how
 * many integers from the file they want to read, and will return that number of integers
 * to output, as well as the sum of all the returned integers.
 * 
 */
import java.util.*;
import java.io.*;
public class integerReader{
    public static void main(String[] args){

        //Creating Scanners for user input and reading the file
        Scanner keyboard = new Scanner(System.in);
        Scanner fileReader = null;

        try{
            fileReader = new Scanner(new FileInputStream("data.txt"));
        }

        catch(FileNotFoundException e){
            System.out.println("Could not find file "+e+".");
            System.exit(0);
        }

        //Input
        System.out.print("How many numbers do you want to read?: ");
        int numAmt = keyboard.nextInt();
        int sum = 0;
        
        //Process + output
        System.out.println("The numbers are: ");
        for (int i = 0; i < numAmt; i++){
            int currentInt = fileReader.nextInt();
            System.out.println(currentInt);
            sum = sum + currentInt;
        }
        System.out.println("The total is: "+sum);
        keyboard.close();
        fileReader.close();
    }
}