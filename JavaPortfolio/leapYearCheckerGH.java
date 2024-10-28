/* Tanner FitzGerald
 * 9/21/24
 * In class exercise 3 for CSC222
 * 
 * This program will prompt the user for a year, and output whether or not it is a leap year
 * The program contains 3 different process solutions, the first two are commented out. They all function,
 * and the third one does it while only using 1 if-else statement.
 * 
 * leap years occur every four years(on years divisable by 4), except for on most century years. Century years
 * that are divisable by 400 however, are leap years. EX: 2001 is not, 2004 is, 2100 is not, 2000 is.
 * 
 */
import java.util.*;
public class leapYearCheckerGH{
    public static void main(String[] args) {
        //Input
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter a year: ");
        int year = keyboard.nextInt();
        boolean flag = true;
        //Process
        /* Solution 1:
        if (year%4 != 0) flag = false;
        else if (year%100 != 0) flag = true;
        else if (year%400 !=0) flag = false;
        else flag = true;
        
        Solution 2:
        if (year%4 == 0){
            flag = true;
            if (year%100 == 0 && year%400 !=0){
                flag = false;
            }
        }
        else{
            flag = false;
        }
        */
        //Solution 3: (I used only 1 if-else statement)
        if (year%4 !=0 || year%100 == 0 && year%400 !=0){
            flag = false;
        }
        else{
            flag = true;
        }
        
        //Output
        if (flag == true) {
            System.out.println("It's a leap year.");
        }
        else{
        System.out.println("It's not a leap year.");
        }
        keyboard.close();
    }
}