#Tanner FitzGerald
#4/27/24
#CSC 221 Assignment
'''

This program contains a class, "VoteRecorder", that is to be called into another
module. The methods in the class will help the other module record votes for
a presidential and vice presidential race.

'''


class VoteRecorder:
    
    '''
    
    Class Description: objects of this class can store prompt for, check,\
                       store, record, and display votes for president and vice\
                       presidential candidates.
                      
    -contains four class variables that record total votes for the race
    -contains 6 instance variables that store the names of candidates, and\
    temporarily store the current voter's votes.
    -in addition to the __init__ function for instance variables, the class\
    has 10 other methods that can be called to act on the object
    -two of these methods set the names for the candidates
    -two of these methods display the current total votes of each race
    -one method will reset the total votes to 0
    -three methods are used to prompt, check, and record votes from a voter
    -one method will confirm with the voter if they are happy with the votes
    -one method will get and confirm votes                   
                      
    '''
    
    votes_candidate_president_1 = int()
    votes_candidate_president_2 = int()
    votes_candidate_vice_president_1 = int()
    votes_candidate_vice_president_2 = int()
    
    def __init__ (self,name_candidate_president_1,name_candidate_president_2,\
                 name_candidate_vice_president_1,\
                 name_candidate_vice_president_2,my_vote_for_president,\
                 my_vote_for_vice_president):
        
        '''
        Function Description: Initializes instance variables for class
        inputs: object name, four strings, and 2 integers
        outputs: nothing
        
        '''
        
        self.name_candidate_president_1 = name_candidate_president_1
        self.name_candidate_president_2 = name_candidate_president_2
        self.name_candidate_vice_president_1 = name_candidate_vice_president_1
        self.name_candidate_vice_president_2 = name_candidate_vice_president_2
        self.my_vote_for_president = my_vote_for_president
        self.my_vote_for_vice_president = my_vote_for_vice_president
    
    def setCandidatesPresident(self,string_name_1,string_name_2):
        
        '''
        
        Function description: stores presidential candidate's names in class
        input: object name and two strings(the names of candidates)
        outputs: nothing
        
        '''
        
        self.name_candidate_president_1 = string_name_1
        self.name_candidate_president_2 = string_name_2

    def setCandidatesVicePresident(self,string_name_1,string_name_2):
        
        '''
        
        Functions description: stores vice presidential candidate's names in\
                               class
        inputs: onject name and two strings(the names of candidates)
        outputs: nothing
        
        '''
        
        self.name_candidate_vice_president_1 = string_name_1
        self.name_candidate_vice_president_2 = string_name_2
        
    def getCurrentVotePresident(self):
        
        '''
        
        Function Description: displays total votes for president
        inputs: object name
        outputs: displayed strings for vote totals
        
        '''
        
        return str(self.name_candidate_president_1 + ' has ' +\
               str(self.votes_candidate_president_1) + ' votes; ' +\
               self.name_candidate_president_2 + ' has ' +\
               str(self.votes_candidate_president_2) + ' votes;')

    def getCurrentVoteVicePresident(self):
        
        '''
        
        Function Description: displays total votes for vice president
        inputs: object name
        outputs: displayed strings for vote totals
        
        '''
    
        return str(self.name_candidate_vice_president_1 + ' has ' +\
               str(self.votes_candidate_vice_president_1) + ' votes; ' +\
               self.name_candidate_vice_president_2 + ' has ' +\
               str(self.votes_candidate_vice_president_2) + ' votes;')

    def resetVotes(self):
        
        '''
        
        Function Description: resets the total vote counts to 0
        inputs: object name
        outputs: nothing
        
        '''
        
        self.votes_candidate_president_1 = 0
        self.votes_candidate_president_2 = 0
        self.votes_candidate_vice_president_1 = 0
        self.votes_candidate_vice_president_2 = 0
        
    def getAVote(self,string_name_1,string_name_2):
        
        '''
        
        Function description: prompts user to enter 0-2 representing a vote\
                              for one of the candidates of that race. Also\
                              checks if input is valid or not
        inputs: object name and two strings(names of the candidates in race)
        outputs: integer 0,1, or 2 representing a vote
        
        '''
        #code block will continuously prompt for a 'vote' as an integer until
        #a valid value is entered
        i = 0
        while i < 1:
            print("Please choose a candidate:")
            print("0 - No one")
            print("1 -", string_name_1)
            print("2 -", string_name_2)
            vote = int(input())
            if vote >= 0 and vote <= 2:
                i += 1
                return vote
            else:
                pass
            
    def getVotes(self):
        
        '''
        
        Function description: displays both races to user, runs getAVote for
                              each race, then runs recordVotes 
        input: object name
        output: nothing
        
        '''
        
        print("YOU ARE VOTING FOR PRESIDENT")
        self.my_vote_for_president = self.getAVote\
        (self.name_candidate_president_1, self.name_candidate_president_2)
        print("YOU ARE VOTING FOR VICE PRESIDENT")
        self.my_vote_for_vice_president = self.getAVote\
        (self.name_candidate_vice_president_1,\
        self.name_candidate_vice_president_2)
        self.recordVotes()
    
    def recordVotes(self):
        
        '''
        
        Function description: looks at current voter's vote and records it\
                              into total votes for each race
        inputs: object name
        outputs: nothing
        
        '''
        if self.my_vote_for_president == 0:
            pass
        elif self.my_vote_for_president == 1:
           self.votes_candidate_president_1 += 1
        else:
            self.votes_candidate_president_2 += 1
        if self.my_vote_for_vice_president == 0:
            pass
        elif self.my_vote_for_vice_president == 1:
            self.votes_candidate_vice_president_1 += 1
        else:
            self.votes_candidate_vice_president_2 += 1
            
    def confirmVotes(self):
        
        '''
        
        function description: prompts user abput wether or not they are happy\
                              with the current votes. if not happy, undoes the\
                              last total vote incrementation
        inputs: object name
        outputs: either True or False representing if they are happy with the\
                 votes or not
        
        '''
        #code block establishes for itself what the name of the candidate\
        #voted for is, so the correct name can be output
        p_name = str()
        vp_name = str()
        if self.my_vote_for_president == 0:
            p_name = "no one"
        elif self.my_vote_for_president == 1:
            p_name = self.name_candidate_president_1
        else:
            p_name = self.name_candidate_president_2
        if self.my_vote_for_vice_president == 0:
            vp_name = "no one"
        elif self.my_vote_for_vice_president == 1:
            vp_name = self.name_candidate_vice_president_1
        else:
            vp_name = self.name_candidate_vice_president_2
            
        print("Your vote for president is", p_name)
        print("Your vote for vice president is", vp_name)
        happy = input("Type yes if you are happy with your vote ")
        
        #code block will either return True for the function, or subtract one\
        #from the total votes of the candidate previously voted for, then\
        #return false, depending on if the user is happy with the votes or not
        
        if happy == 'yes' or happy == 'Yes':
            return True
        else:
            if self.my_vote_for_president == 1:
                self.votes_candidate_president_1 -= 1
            elif self.my_vote_for_president == 2:
                self.votes_candidate_president_2 -= 1
            if self.my_vote_for_vice_president == 1:
                self.votes_candidate_vice_president_1 -= 1
            elif self.my_vote_for_vice_president == 2:
                self.votes_candidate_vice_president_2 -= 1
            return False
        
    def getAndConfirmVotes(self):
        
        '''
        
        Function description: calls getVotes and confirmVotes repeatedly until
                              user is happy with votes
        inputs: object name
        outputs: nothing
        
        '''
        
        repeat = False
        while repeat == False:
            
            self.getVotes()
            repeat = self.confirmVotes()
            
           









