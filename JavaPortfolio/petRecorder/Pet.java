/*
 * 
 * This Program was provided to me as part of my assignment on learning Inheritence.
 * I am to create a "Dog" class that is derived from this class.
 * 
 * I did not make this program.
 * 
 */
package petRecorder;
public class Pet
{
    private String name;
    private int age; //in years
    private double weight; //in pounds

    public Pet ()   // default constructor
    {
        name = "No name yet.";
        age = 0;
        weight = 0;
    }


    public Pet (String initialName, int initialAge,
            double initialWeight)                  
    {
        name = initialName;
        if ((initialAge < 0) || (initialWeight < 0))
        {
            System.out.println ("Error: Negative age or weight.");
            System.exit (0);
        }
        else
        {
            age = initialAge;
            weight = initialWeight;
        }
    }


    
    public void setName (String newName)
    {
        name = newName; //age and weight are unchanged.
    }


    public void setAge (int newAge)
    {
        if (newAge < 0)
        {
            System.out.println ("Error: Negative age.");
            System.exit (0);
        }
        else
            age = newAge;
        //name and weight are unchanged.
    }
    public void setWeight (double newWeight)
    {
        if (newWeight < 0)
        {
            System.out.println ("Error: Negative weight.");
            System.exit (0);
        }
        else
            weight = newWeight; //name and age are unchanged.
    }


    public String getName ()
    {
        return name;
    }


    public int getAge ()
    {
        return age;
    }


    public double getWeight ()
    {
        return weight;
    }


    public void writeOutput ()
    {
        System.out.println ("Name: " + name);
        System.out.println ("Age: " + age + " years");
        System.out.println ("Weight: " + weight + " pounds");
    }
}
