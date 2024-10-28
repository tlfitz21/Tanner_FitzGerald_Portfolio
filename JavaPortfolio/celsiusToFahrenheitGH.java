/*Tanner FitzGerald
*9/10/24
*CSC 222 Programming Assignment 1
*
*This program prompts the user for a temperature in celsius, and returns its
*equivalent in fahrenheit.
*
*/
import java.util.*;

class celsiusToFahreheitGH{
    public static void main(String[] args) {
       //input
       Scanner keyboard = new Scanner(System.in);
       System.out.print("Enter Celsius temperature: ");
       double cTemp = keyboard.nextDouble();
       //process
       double fTemp = 9.0/5.0 * cTemp + 32;
       //output
       System.out.println(cTemp+ " degrees Celsius is "+fTemp+ " degrees Fahrenheit!");

       keyboard.close();
    }
}