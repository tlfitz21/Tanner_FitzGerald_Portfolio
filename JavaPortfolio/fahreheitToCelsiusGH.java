/*Tanner FitzGerald
*9/7/24
*CSC 222 Exercise 1
*
*This program will prompt the user for a temperature in Fahrenheit, and return its
*equivalent value in celsius.
*
*/
import java.util.*;
class fahrenheitToCelsiusGH{
    public static void main(String[] args) {
       //input
       Scanner keyboard = new Scanner(System.in);
       System.out.print("Enter Fahrenheit temperature: ");
       double fTemp = keyboard.nextDouble();
       //process
       double cTemp = 5.0/9.0*(fTemp-32);
       //output
       System.out.println(fTemp+ " degrees Fahrenheit is "+cTemp+ " degrees Celsius!");

       keyboard.close();
    }
}