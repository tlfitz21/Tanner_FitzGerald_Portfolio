/*Tanner FitzGerald
 * 11/17/2024
 * CSC 222 Programming Assignment 9
 * 
 * This program contains the class "Triangle", which contains three data fields representing each side,
 * as well as 5 methods. 
 * -input() prompts user and stores doubles for each side, and validates if the triangle is valid based on the
 * rule that two sides must be larger than the third
 * -perimeter() returns the perimeter of the triangle
 * -isEquilateral() returns a boolean true if it's an equilateral triangel, and false if it isn't
 * -area() returns the area of the triangle
 * -output() will print all the information back to the user
 * 
 */
package triangleAnalyzer;
import java.util.*;
public class Triangle {
    
    private double side1, side2, side3;

    public void input(){
        Scanner keyboard = new Scanner(System.in);

        while (true){
            System.out.print("Enter the length of side 1: ");
            side1 = keyboard.nextDouble();
            System.out.print("Enter the length of side 2: ");
            side2 = keyboard.nextDouble();
            System.out.print("Enter the length of side 3: ");
            side3 = keyboard.nextDouble();

            if (side1 + side2 > side3 && side1 + side3 > side2 && side2 + side3 > side1){
                break;
            }
            else System.out.println("Invalid triangle, please try again.");
        }
        keyboard.close();
    }

    public double perimeter(){
        double perimeter = side1 + side2 + side3;
        return perimeter;
    }

    public boolean isEquilateral(){
        boolean isEquilateral = false;
        if (side1 == side2 && side1 == side3) isEquilateral = true;
        return isEquilateral;
    }

    public double area(){
        double s = (side1 + side2 + side3) / 2.0;
        double area = Math.sqrt(s * (s-side1) * (s-side2) * (s-side3));
        return area;
    }

    public void output(){
        System.out.println("The three sides that you entered are: "+side1+", "+side2+", and "+side3+".");
        System.out.println("The perimeter of the triangle is: "+perimeter());
        if (isEquilateral()) System.out.println("The triangle is an equilateral");
        else System.out.println("The triangle is not an equilateral");
        System.out.println("The area of the triangle is: "+area());
    }
}
