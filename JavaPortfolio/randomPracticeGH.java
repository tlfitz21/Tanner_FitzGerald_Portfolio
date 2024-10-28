/*Tanner FitzGerald
 *Personal practice project
 *8/27/24
 * 
 * This program will simply output either "Zero", "One", or "Two" randomly.
 * I used this program to learn how to transition from python to Java.
 * 
 */
public class randomPracticeGH {

    public static void main(String[] args) {
        int myRandomNum = (int) (Math.random() * 3);

        if (myRandomNum == 0){
            System.out.println("Zero");
        } 
        else if (myRandomNum == 1){
            System.out.println("One");
        }
        else{
            System.out.println("Two");
        }
    }
}

