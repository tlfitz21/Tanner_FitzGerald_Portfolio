/*Tanner FitzGerald
 *10/6/24
 *CSC 222: In-class Exercise 5(part 2)
 * 
 * This program will prompt the user for a non-negative integer, and
 * contains a method, isPrime, that returns true if user input is a prime number, and false if it isn't.
 * 
 */
import java.util.*;
public class primeIdentifierGH {
    public static void main(String[] args) {
        //input
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter a non-negative integer: ");
        int n = keyboard.nextInt();
        //process and output
        if (isPrime(n)){
            System.out.println(n+" is a prime number!");
        }
        else{
            System.out.println(n+" is not a prime number.");
        }
        keyboard.close();
    }
    public static boolean isPrime(int n){
        if (n == 0 || n == 1){
            return false;
        }
        for (int i = 2;i<n;i++){
            if (n%i == 0){
                return false;
            }
        }
        return true;
    }
}
