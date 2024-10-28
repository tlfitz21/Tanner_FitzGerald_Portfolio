/* Tanner FitzGerald
 * 9/14/2024
 *  CSC 222 Exercise 2
 * 
 * This Program will take the number of male and female students in the class and calculate the percentage of each 
 * in the total class.
 * 
 */
import java.util.*;
public class genderPercentageGH{
    public static void main(String[] args) {
        //input
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter the number of Male students: ");
        int male = keyboard.nextInt();
        System.out.print("Enter the number of Female students: ");
        int female = keyboard.nextInt();

        //process
        double malePercent = (double)male / (male+female);
        double femalePercent = (double)female / (male+female);
        
        //output
        System.out.printf("Male student percentage: %.1f%%\n",malePercent * 100);
        System.out.printf("Female student percentage: %.1f%%\n",femalePercent * 100);

        keyboard.close();
    }
}