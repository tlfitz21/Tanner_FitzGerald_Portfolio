/*Tanner FitzGerald
 *8/27/24
 *Personal practice project
 *
 * This Progam will simulate the videogame "snake" somewhat.
 * It contains a function that will take a given length of a game board
 * side and return the amount of times the snake can eat before it runs
 * out of area
 * 
 * The snake starts at 1, and doubles with every apple eaten
 * 
 * This was one of my first Java programs, and so it has many glaring "learing moments" as I
 * was figuring out how to transition from python.
 * 
 */

import java.util.Scanner;

 public class SnakeFillGH{

    public static void main(String[] args) {

        //prompt user for side length, convert to integer
        Scanner myScanner = new Scanner(System.in);
        System.out.print("Enter Length of side: ");
        String sideLength = myScanner.nextLine();
        Integer intSideLength = Integer.valueOf(sideLength);

        //establish grid area, snake length, apple counter
        Integer area = intSideLength * intSideLength;
        Integer snake = 1;
        Integer apples = 0;

        //eat the apples
        while (snake < area){
            snake *= 2;
            apples += 1;
        }
        System.out.println(apples - 1);
        myScanner.close();
    }
 }