/* Tanner FitzGerald
 * 9/29/2024
 * CSC 222-In class Exercise 4
 * 
 * This program will output a table of Celsius values from 0-20 and their
 * equivalent values in Fahrenheit.
 * 
 * A solution is provided with a for loop,
 * but a while loop version is commented out as well.
 * 
 */
public class temperatureTableGH{
    public static void main(String[] args) {
        int c = 0;
        System.out.println("Celsius\tFahrenheit");
        System.out.println("------------------");
        /*
        while (c <=20){
            double f = 1.8*c + 32;
            System.out.printf("%d\t%.1f\n",c,f);
            c++;
        }
        */
        for (c=0;c<=20;c++){
            double f = 1.8*c + 32;
            System.out.printf("%d\t%.1f\n",c,f);
        }
        
    }
}