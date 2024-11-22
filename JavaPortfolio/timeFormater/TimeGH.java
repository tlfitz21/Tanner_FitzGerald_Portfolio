/*Tanner FitzGerald
 *11/10/2024 
 *CSC 222 Programming Assignment 8 
 * 
 * This program contains a user-defined class called "Time". It has 6 data fields for different
 * measurements, and 3 methods.
 * 
 * -input() will prompt the user for each time measurement and store the values in the object's data fields. It also
 * checks the validity of the hour, minute, and second measurements and re-prompts if invalid entry is used.
 * 
 * -printStandardTime() will convert the hour field to what it would be in standard time, and checks whether to use AM or
 * PM. It then outputs the standard time.
 * 
 * -printMilitaryFormat simply outputs the military time.
 * 
 */
package timeFormater;
import java.util.*;
public class TimeGH {
    private int year, month, day, hour, minute, second;

    public void input(){
        Scanner keyboard = new Scanner(System.in);

        System.out.print("Enter year: ");
        year = keyboard.nextInt();

        System.out.print("Enter month: ");
        month = keyboard.nextInt();

        System.out.print("Enter day: ");
        day = keyboard.nextInt();

        System.out.print("Enter hour: ");
        hour = keyboard.nextInt();
        while (hour < 0 || hour > 23){
            System.out.print("Invalid entry, please enter an integer between 0 and 23: ");
            hour = keyboard.nextInt();
        }
        System.out.print("Enter minute: ");
        minute = keyboard.nextInt();
        while (minute < 0 || minute > 59){
            System.out.print("Invalid entry, please enter an integer between 0 and 59: ");
            minute = keyboard.nextInt(); 
        }    
        System.out.print("Enter second: ");
        second = keyboard.nextInt();
        while (second < 0 || second > 59){
            System.out.print("Invalid entry, please enter an integer between 0 and 59: ");
            second = keyboard.nextInt();    
        }
        keyboard.close();
    }

    public void printStandardTime(){
        String dayHalf = " AM";
        int newHour = 0;
        if (hour > 12) dayHalf = " PM";

        if (hour == 0) newHour = 12;
        else if (hour > 0 && hour < 13) newHour = hour;
        else newHour = hour - 12;
        
        System.out.println("Based on your entry, Standard time is "+month+"-"+day+"-"+year+", "+newHour+":"+minute+":"+second+dayHalf+".");

    }

    public void printMilitaryTime(){
        System.out.println("Based on your entry, Military time is "+day+"-"+month+"-"+year+", "+hour+":"+minute+":"+second+".");
    }
}
