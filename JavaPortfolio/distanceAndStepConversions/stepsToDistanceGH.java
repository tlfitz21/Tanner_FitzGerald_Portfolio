/* Tanner FitzGerald
 * 9/17/24
 * CSC 222- Programming Assignment 2: Part 2
 * 
 * This program will prompt for the amount of steps taken on a hike, and convert that to the distance that was traveled.
 * 
 * The program assumes one step is equal to one yard.
 * 
 */

import java.util.*;
public class stepsToDistanceGH {
    public static void main(String[] args) {
        //input
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter number of steps taken: ");
        int steps = keyboard.nextInt();
        //process
        int miles = steps/1760;
        int yards = steps%1760;
        //output
        System.out.print("You traveled " + miles + " miles, and " + yards + " yards!");
        keyboard.close();
    }
}
