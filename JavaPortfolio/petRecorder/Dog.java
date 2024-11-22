/*Tanner FitzGerald
*11/10/24
*CSC 222 Exercise 9
*
*this program creates a subclass of the "Pet" class called "Dog". It will inherit all its members and
*create two new data fields, as well as 7 new methods.
* 2 of these methods are constructors(one default and one non-default)
* 2 are mutators for the new data fields
* 2 are accesors for the new data fields
* 1 is a utility method for producing output
*
*/
package petRecorder;
public class Dog extends Pet{
    private String breed;
    private boolean boosterShot;

    public Dog(){
        super();
        breed = "No breed yet";
        boosterShot = false;
    }

    public Dog(String name, int age, double weight, String breed, boolean boosterShot){
        super(name, age, weight);
        this.breed = breed;
        this.boosterShot = boosterShot;
    }

    public void setBreed(String breed){
        this.breed = breed;
    }

    public void setBoosterShot(boolean boosterShot){
        this.boosterShot = boosterShot;
    }

    public String getBreed(){
        return breed;
    }

    public boolean getBoosterShot(){
        return boosterShot;
    }
    @Override
    public void writeOutput(){
        super.writeOutput();
        System.out.println("Breed: "+breed);
        System.out.println("Booster Shot: "+boosterShot);
    }
    
    
}