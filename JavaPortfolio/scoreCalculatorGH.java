/*Tanner FitzGerald
 * 11/3/2024
 * CSC 222 Programming Assigment 7
 * 
 * This program will prompt the user to enter an amount of scores to do calculations with, and prompt for
 * each individual score. It will then output the average score, the highest score, and a comparison of each
 * score to the average score.
 * 
 */
import java.util.*;
public class scoreCalculatorGH{
    public static void main(String[] args){
        //Input
        Scanner keyboard = new Scanner(System.in);
        System.out.print("How many scores do you have?: ");
        int amtScores = keyboard.nextInt();
        double[] scores = new double[amtScores];
        for (int i = 0; i < scores.length; i++){
            System.out.print("Enter a score: ");
            scores[i] = keyboard.nextDouble();
        }

        //Process
        double sum = 0;
        double max = Double.MIN_VALUE;
        for (int i = 0; i < scores.length; i++){
            if (scores[i] > max) max = scores[i];
            sum = sum + scores[i];
        }
        double average = sum / scores.length;

        //output
        System.out.printf("The average score is %.2f\n", average);
        System.out.printf("The highest score is %.2f\n", max);
        for (int i = 0; i < scores.length; i++){
            if (scores[i] < average) System.out.printf("%.2f is below average by %.2f\n", scores[i], average - scores[i]);

            else if (scores[i] > average) System.out.printf("%.2f is above average by %.2f\n", scores[i], scores[i] - average);

            else System.out.printf("%.2f is exactly at the average score!\n", scores[i]);
        }
        keyboard.close();
    }
}