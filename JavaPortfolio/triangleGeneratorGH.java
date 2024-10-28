/*Tanner FitzGerald
* 10/6/24
 *CSC 222: In-class Exercise 5(part 1)
 *
 *This program will prompt the user for an input size of the triangle,
 *then output a right triangle made of "*" of that size.
 * 
 */
import java.util.*;
public class triangleGeneratorGH{
    public static void main(String[] args) {
        //Input
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter the size of the triangle: ");
        int n = keyboard.nextInt();
        //Proccess and Output
        for (int i = 0; i<n; i++){
            for(int j = 0; j<i*2+1;j++){
                System.out.print("*");
            }
            System.out.println();
        }
        keyboard.close();
    }
}