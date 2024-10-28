/* Tanner FitzGerald
 * 10/27/24
 * CSC 222 in class exercise 7
 * 
 * This program will prompt the user for a list of integers, then output the maximum value of the list,
 * as well as a returned list that is sorted smallest to largest.
 * 
 */
import java.util.*;
public class arrayMaxAndSortGH{
    public static void main(String[] args) {
        //Input
        Scanner keyboard = new Scanner(System.in);
        int[] a = new int[10];
        System.out.print("Enter 10 integers: ");
        for (int i = 0; i<a.length; i++)
            a[i] = keyboard.nextInt();

        //process + output
        System.out.println("The maximum value of your integers is "+max(a));
        int[] b = sort(a);
        System.out.print("The sorted integers: ");
        for (int i = 0; i < a.length; i++){
            System.out.print(b[i]+" ");
        }
        keyboard.close();
    }

    public static int max(int[] a){
        int largest = a[0];
        for (int i = 1; i<a.length; i++){
            if (a[i] > largest)
                largest = a[i];
        }
        return largest;
    }

    public static int[] sort(int[] a){
        int[] result = new int[a.length];
        for (int i = 0; i < result.length; i++){
            int smallest = Integer.MAX_VALUE;
            int smallestIndex = 0;
            for (int j = 0; j < a.length; j++){
                if (a[j] < smallest){
                    smallest = a[j];
                    smallestIndex = j;
                }
            }
            result[i] = smallest;
            a[smallestIndex] = Integer.MAX_VALUE;
        }
        return result;
    }
}