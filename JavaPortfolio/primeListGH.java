/*Tanner FitzGerald
 *10/9/24
 *CSC 222- Programming Asignment 5
 * 
 * This program will prompt the user for an amount of prime numbers (n),
 * and will return a list of the first n prime numbers.
 * 
 * it includes a defined method, isPrime, which will take an integer input
 * and return true if it's prime, false if it isn't.
 * 
 */
import java.util.*;
public class primeListGH{
    public static void main(String[] args) {
        //Input
        Scanner keyboard = new Scanner(System.in);
        System.out.print("How many prime numbers do you want? ");
        int n = keyboard.nextInt();

        //Process + Output
        int itCount = 2;                    //counts iterations to be used as arguments for isPrime
        int primeCount = 0;                 //counts the tally of prime numbers found so far
        String suffix = "something went wrong";

        while (primeCount < n){
            if (isPrime(itCount)){
                primeCount++;
                if (primeCount%10 == 1 && primeCount%100 !=11){
                    suffix = "st";
                }
                else if (primeCount%10 == 2 && primeCount%100 !=12){
                    suffix = "nd";
                }
                else if (primeCount%10 == 3 && primeCount%100 !=13){
                    suffix = "rd";
                }
                else{
                    suffix = "th";
                }
                System.out.println("The "+primeCount+suffix+" prime number is "+itCount);
                itCount++;
                
            }
            else{
                itCount++;
            }
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




