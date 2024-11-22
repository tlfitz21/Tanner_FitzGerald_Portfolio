/*Tanner FitzGerald
 *11/10/24 
 *CSC 222 Exercise 9
 * 
 *This program will create objects of the "Dog" class defined in this folder. It will test it by running
 *a few of its methods. 
 *  
 */
package petRecorder;
public class petRecorder{
    public static void main(String[] args) {
        Dog d1 = new Dog();
        d1.writeOutput();
        d1.setBoosterShot(true);
        System.out.println("The booster shot is updated: "+d1.getBoosterShot());

        Dog d2 = new Dog("Nyx", 5, 15, "Cockapoo", true);
        d2.writeOutput();
    }
}
