/* Tanner FitzGerald
 * 10/23/24
 * CSC 222- Programming Assignment 6
 * 
 * This program will prompt the user for an initial invest amount and the annual interest rate,
 * and output a table of values representing the expected future Investment amount for the next
 * 30 years.
 * 
 */
import java.util.*;
public class investmentCalculatorGH{
    public static void main(String[] args) {
        //Input
        Scanner keyboard = new Scanner(System.in);
        System.out.print("Enter original investment amount: ");
        double investAmt = keyboard.nextDouble();
        
        System.out.print("Enter annual interest rate: ");
        double monthIntRate = keyboard.nextDouble() / 12;

        //Process + output
        System.out.println("Years\tFuture Value");
        for (int y = 1; y <= 30; y++){
            System.out.printf(y+"\t%.2f\n", futureInvestmentValue(investAmt, monthIntRate, y));
        }
        keyboard.close();
    }
    public static double futureInvestmentValue( double investmentAmount, double monthlyInterestRate, int years){
        double futureInvestValue = investmentAmount * Math.pow(1 + monthlyInterestRate, years * 12);
        return futureInvestValue;
    }
}