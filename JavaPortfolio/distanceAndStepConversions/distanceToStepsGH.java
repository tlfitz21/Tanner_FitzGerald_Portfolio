/* Tanner FitzGerald
 * 9/17/24
 * CSC 222- Programming Assignment 2: Part 1
 * 
 * This program will prompt for the distance traveled on a hike, and convert that to an amount of steps taken.
 * 
 * The program assumes one step is equal to one yard.
 * 
 */

 import java.util.*;
 public class distanceToStepsGH{
    public static void main(String[] args) {
        //input
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter miles: ");
        int miles = keyboard.nextInt();
        System.out.print("Enter extra yards: ");
        int yards = keyboard.nextInt();
        //process
        int steps = 1760 * miles + yards;
        //output
        System.out.println("You took " + steps + " steps!");
        keyboard.close();
    }
 }