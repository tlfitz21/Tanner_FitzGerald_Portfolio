#Tanner FitzGerald
#4/27/24
#CSC 221 Assignment
'''

This program will call the class "VoteRecorder" from the voteRecorderGH module
and its methods. It records inputed votes for two candidates for president and
vice president. It will verify if the votes were correct after each entry, and
continuosly record votes until user says they are done. It will then display the
vote totals.

'''
    
import voteRecorderGH

class VoteRecorderDemo:
    
    '''
    
    Class description: 
    uses the VoteRecorder class to run races for president\
    and vice president. It contains one method for the process
    -the one(Main()) method will establish a new instance of VoteRecorder, set\
    the candidate names, reset the total vote count, then repeatedly create\
    new VoteRecorder objects for each voter that are then used to prompt,\
    check, display, and record votes. it then displays the total vote count
    
    '''
    
    def main(self):
        
        '''
        
        Function description:  establishes a new instance of VoteRecorder,\
        sets the candidate names, resets the total vote count, then repeatedly\
        creates new VoteRecorder objects for each voter that are then used to\
        prompt, check, display, and record votes. it then displays the total\
        vote count
        inputs: object name
        outputs: several displayed statemtents and prompts to let user vote\
        and see results of voting
        
        '''
        #Code block establishes a new instance of the VoteRecorder class to 
        #set candidate names an keep track of total vote
        Recorder = voteRecorderGH.VoteRecorder(str, str, str, str,int,int)
        Recorder.setCandidatesPresident('Annie', 'Bob')
        Recorder.setCandidatesVicePresident('John', 'Susan')
        Recorder.resetVotes()
        
        
        #code block sets up a dictionary to pair the while loop iteration(as\
        #a key) with a new instance of the VoteRecorder class to represent\
        #each new voter and their votes. It repeatedly creates new objects\
        #until user says there are no more voters
        myDict = {}
        i = 0
        while True:
            myDict[i] = voteRecorderGH.VoteRecorder('Annie', 'Bob', 'John',\
                                                  'Susan', int, int)
            myDict[i].getAndConfirmVotes()
            Recorder.votes_candidate_president_1 =\
            Recorder.votes_candidate_president_1 +\
            myDict[i].votes_candidate_president_1
            Recorder.votes_candidate_president_2 =\
            Recorder.votes_candidate_president_2 +\
            myDict[i].votes_candidate_president_2
            Recorder.votes_candidate_vice_president_1 =\
            Recorder.votes_candidate_vice_president_1 +\
            myDict[i].votes_candidate_vice_president_1
            Recorder.votes_candidate_vice_president_2 =\
            Recorder.votes_candidate_vice_president_2 +\
            myDict[i].votes_candidate_vice_president_2
            
            repeat = input("Type yes if there is another voter ")
            if repeat == 'yes' or repeat == 'Yes':
                i += 1
            else:
                break
            
        print(Recorder.getCurrentVotePresident())
        print(Recorder.getCurrentVoteVicePresident())

Demo = VoteRecorderDemo()
Demo.main()

    
    
    