#Tanner FitzGerald
#5/1/24
#CSC 221 Assignment
'''

This program imports a class, "Authenticate", from the authenticateGH module
Its methods will create a set password, (that is: 71421) and generate
a random sequence associating to each possible digit(between 1-3). It will prompt
the user to observe the corellation and input the correct combination.

'''

import authenticateGH

class AuthenticateDemo:
    
    '''
    
    Class description: objects of this class contain just one method, main(),
    that will display the introductory prompts to user,create a new object of\
    the aunthenicate class, display random numbers to user, prompt user for\
    code and check validity, then display to user if code was correct or not
    
    '''
    
    def main(self):
        
        '''
        
        Function Description: displays the introductory prompts to user,\
        creates a new object of the aunthenicate class, displays random\
        numbers to user, prompts user for code and checks validity, then\
        displays to user if code was correct or not
        inputs: the object name
        outputs: a short text conversation with the user
        
        '''
        
        print("Welcome! To log in, enter the random digits from 1-3 that")
        print("correspond to your PIN number.")
        print("PIN: 0 1 2 3 4 5 6 7 8 9")
        Authenticator = authenticateGH.Authenticate()
        Authenticator.printRandomNumber(Authenticator.generateRandomNumber())
        correct = Authenticator.inputUserEntry()
        if correct == True:
            print("Correct! You may now proceed.")
        else:
            print("Error, invalid password entered")
        
Demo = AuthenticateDemo()
Demo.main()