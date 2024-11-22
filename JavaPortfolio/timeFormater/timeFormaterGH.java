/*Tanner FitzGerald
 *11/10/2024 
 *CSC 222 Programming Assignment 8 
 * 
 * This program calls the user-defined "Time" class to record input for year, month, day,
 * hour, minute, and second. It will then output that time in both standard and military
 * format.
 * 
 */
package timeFormater;
public class timeFormaterGH{
    public static void main(String[] args){
        TimeGH t1 = new TimeGH();
        t1.input();
        t1.printStandardTime();
        t1.printMilitaryTime();
    }
}
