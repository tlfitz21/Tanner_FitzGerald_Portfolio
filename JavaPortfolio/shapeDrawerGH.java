/* Tanner FitzGerald
 * 10/15/24
 * CSC 222- In-class exercise 6
 * 
 * This program will draw a shape for the user, by prompting them to give it
 * a character the shape is made of, the size of the shape, and which shape it is.
 * 
 */

 import java.util.*;
 public class shapeDrawerGH{
    public static void main(String[] args) {
        instructions();
        while (true){
            int choice = menu();
            if (choice == 3){
                System.out.println("Bye");
                break;
            }
            drawShape(choice);
        }
    }

    public static void instructions(){
        System.out.println("This program will draw you a shape based on your inputs. First enter the desired shape, "+
                           "then the size, and finally the character used to draw it.");
    }

    public static int menu(){
        Scanner keyboard = new Scanner(System.in);
        System.out.println("\n1) Draw triangle");
        System.out.println("2) Draw diamond");
        System.out.println("3) Quit");
        System.out.print("Enter a choice: ");
        int choice = keyboard.nextInt();
        while (!(choice == 1 || choice == 2 || choice == 3)){
            System.out.println("Invalid Entry");
            System.out.print("Enter a choice: ");
            choice = keyboard.nextInt(); }
        return choice;
    }

    public static void drawShape(int choice){
        int size = getSize();
        char c = getChar();
        if (choice == 1){
            System.out.println("Drawing triangle of size "+size+" made of "+c+" characters:\n");
            drawTriangle(size, c);
        }
        else if (choice == 2){
            System.out.println("Drawing diamond of size "+size+" made of "+c+" characters:\n");
            drawDiamond(size, c);
        }
    }

    public static int getSize(){
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter the size of the shape: ");
        int size = keyboard.nextInt();
        return size;
    }

    public static char getChar(){
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter the character the shape is to be made of: ");
        char c = keyboard.next().charAt(0);
        return c;
    }

    public static void drawTriangle(int size, char c){
        for (int i = 0; i < size; i++){
            for (int k = 0; k < size - i - 1; k++){
                System.out.print(" ");
            }
            for (int j = 0; j < i*2 + 1; j++){
                System.out.print(c);
            }
            System.out.println();
        }
    }

    public static void drawDiamond(int size, char c){
        drawTriangle(size, c);
        drawBottom(size, c);
    }
    
    public static void drawBottom(int size, char c){
        for (int i = 0; i < size - 1; i++){
            for (int k = 0; k < i + 1; k++){
                System.out.print(" ");
            }
            for (int j = 0; j < -2*i + (size*2 -3);j++){
                System.out.print(c);
            }
            System.out.println();
        }
    }
 }