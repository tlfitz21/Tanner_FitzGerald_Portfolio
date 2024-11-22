/*Tanner FitzGerald
 * 11/3/2024
 * CSC 222 Exercise 8
 * 
 * This program defines a class called "Time" that contains data fields for each part of the time,
 * as well as methods to prompt the user to fill them, and to print it back to them put together.
 * 
 */
package timeRecorder;
import java.util.*;
public class TimeGH {
    //data fields
    private int hour;
    private int minute;
    private int second;

    //methods
    public void input(){
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter hour: ");
        hour = keyboard.nextInt();
        System.out.print("Enter minute: ");
        minute = keyboard.nextInt();
        System.out.print("Enter second: ");
        second = keyboard.nextInt();
        keyboard.close();
    }
    
    public void print(){
        System.out.println(hour+ ":" + minute + ":" + second);
    }
}