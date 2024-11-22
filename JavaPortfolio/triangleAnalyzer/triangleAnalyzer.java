/*Tanner FitzGerald
 * 11/17/2024
 * CSC 222 Programming Assignment 9
 * 
 * This program creates an object of the "Triangle" class in this folder, and runs its
 * input() and output() methods to prompt user for sides of a triangle, then output information
 * about it including its perimeter, area, and whether or not it is an equilateral triangle
 * 
 */
package triangleAnalyzer;
public class triangleAnalyzer{
    public static void main(String[] args) {
        Triangle t1 = new Triangle();
        t1.input();
        t1.output();
    }
}
