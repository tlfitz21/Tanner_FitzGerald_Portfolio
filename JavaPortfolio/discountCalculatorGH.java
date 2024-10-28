/* Tanner FitzGerald
 * 9/29/2024
 * CSC 222-Programming Assignment 3
 * 
 * This program will prompt for an amount of software packages purchased and return the
 * bundling discount applied for that amount, as well as the resulting total price.
 * 
 * each package costs $99, and the bundle discounts are as follows: 10-19 = 10%,
 * 20-49 = 20%, 50-99 = 30%, and 100+ = 40%.
 * 
 */
import java.util.*;
public class discountCalculatorGH{
    public static void main(String[] args) {
        //Input
        double discount,discountAmt,totalPrice;
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter the number of packages: ");
        int packageAmt = keyboard.nextInt();
        //Proccess
        if (packageAmt < 10){
            discount = 0;
        }
        else if (packageAmt < 20){
            discount = .1;
        }
        else if (packageAmt < 50){
            discount = .2;
        }
        else if (packageAmt < 100){
            discount = .3;
        }
        else{
            discount = .4;
        }
        discountAmt = packageAmt * 99 * discount;
        totalPrice = (packageAmt * 99) - discountAmt;
        //Output
        System.out.printf("Amount of discount: %.2f\n",discountAmt);
        System.out.printf("Total amount after discount: %.2f",totalPrice);
        keyboard.close();
    }
}