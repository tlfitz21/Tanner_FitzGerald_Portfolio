class GameRoom{
    
    //the PIN for the lobby
    private pin;

    //the current "game" that is being hosted
    private currGame;

    //an array of "Student" objects.
    private activeStudents;

    //create a random pin
    genPin()
    {
        this.pin = Math.floor(10000 + Math.random() * 90000).toString();
    }

    //fetch the game we want
    findGame(gameName: string)
    {
        //code for retrieving the game object from our database
    }

    findStudents()
    {
        /*code for collecting student objects and storing them into the activeStudents
        array. it will pass the pin that was generated
        */
    }
    
    /*given a collection of student objects and the game of choice, propt each question and
    store the results in the student objects. 
    */
    startGame(gameName: string)
    {
        
        this.genPin();
        this.findGame(gameName);
        this.findStudents();
        //for every student in this game,
        for(let i = 0; i < this.activeStudents.length(); i++){
            
            //set that student's current question set to this game's question set
            this.activeStudents[i].setQuestionSet(this.currGame.getQuestions());

        }

    }

    /*checks to make sure all students are either out of time or answered all of the questions
    it then returns the activeStudent array after it's been updated so the teacher account class
     can store the results long term based on the name or ID variable in student class
    */
    endGame(): Student[]
    {
        //for every student in this game,
        for(let i = 0; i < this.activeStudents.length(); i++){
            
            //cancel ending the game if any student isn't finished yet
            if (this.activeStudents[i].getIsDone() == false) return;

        }

        return this.activeStudents;
    }

}