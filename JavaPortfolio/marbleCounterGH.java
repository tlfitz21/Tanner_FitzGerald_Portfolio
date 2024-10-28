/* Tanner FitzGerald
 * 10/3/24
 * CSC 222-Programming Assignment 4
 * 
 * This program will repeatedly ask the user to input a color of marble until they are done. Then it will output
 * the amount of each color entered, the total amount of marbles, and the "winning" color of marbles with the
 * highest count
 * 
 * If there is a tie in the highest count, the winner will be decided arbitrarily. 
 * 
 */
import java.util.*;
public class marbleCounterGH{
    public static void main(String[] args) {
        //input
        Scanner keyboard = new Scanner(System.in);
        int r = 0, b = 0, y = 0;
        while (true){
            System.out.print("Enter marble color: ");
            char input = keyboard.next().charAt(0);
            if (Character.toLowerCase(input) == 'r'){
                r++;
            }
            else if (Character.toLowerCase(input) == 'b'){
                b++;
            }
            else if (Character.toLowerCase(input) == 'y'){
                y++;
            }
            else if (Character.toLowerCase(input) == 'q'){
                break;
            }
            else{
                System.out.println("Invalid entry, try again.");
            }
        }
        //Proccess
        String winner = "start";
        double randNum = Math.random();
        if (r>b){
            if (r>y){
                winner = "red";
            }
            else if (y>r){
                winner = "yellow";
            }
            else{
                if (randNum < .5){
                    winner = "red";
                }
                else{
                    winner = "yellow";
                }
            }
        }
        else if (b>r){
            if (b>y){
                winner = "blue";
            }
            else if (b<y){
                winner = "yellow";
            }
            else{
                if (randNum < .5){
                    winner = "blue";
                }
                else{
                    winner = "yellow";
                }
            }
        }
        else{
            if (r > y){              
                if (randNum < .5){
                    winner = "red";
                }
                else{
                    winner = "blue";
                }
            }
            else if (y > r){
                winner = "yellow";
            }
            else{
                if (randNum < 0.34){
                    winner = "red";
                }
                else if (randNum < 0.67){
                    winner ="blue";
                }
                else{
                    winner = "yellow";
                }
            }
        }
        //output
        System.out.println("The number of red marbles is "+r);
        System.out.println("The number of blue marbles is "+b);
        System.out.println("The number of yellow marbles is "+y);
        System.out.println("The total number of marbles is "+(r+y+b));
        System.out.println("The winning color is "+winner+"!");
        keyboard.close();
    }
}