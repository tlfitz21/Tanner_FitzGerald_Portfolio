#Tanner FitzGerald
#5/1/24
#CSC 221 Assignment
'''

This program contains a class, "Authenticate", that is to be imported into another
module. Its methods will create a set password, (that is: 71421) and generate
a random sequence associating to each possible digit(between 1-3). It will prompt
the user to observe the corellation and input the correct combination.

'''

import numpy as np

class Authenticate:
    
    '''
    
    Class Description: Objects of this class can store a hidden password,\
    create a random array of numbers that correleate to each single digit,\
    store user's entered digits, prompt user for encrypted digits, and check\
    for validity
    
    -contains 3 instance variables, that are all arrays. One stores the actual\
    password, another holds random digits, another stores user's input
    -contains 3 methods, on creates 10 random numbers between 1 and 3, another\
    displays them to the user, and another prompts user for input, and checks\
    for validity
    
    '''
    
    def __init__(self):
        
        '''
        
        Function Description: intitializes instance variables for class
        Inputs: object name
        Outputs: none
        
        '''
        
        self.actual_password = np.array([7, 1, 4, 2, 1])
        self.random_numbers = np.array([0,0,0,0,0,0,0,0,0,0])
        self.entered_digits = np.array([0,0,0,0,0])
        
    def generateRandomNumber(self):
        
        '''
        
        Function Description: creates random numbers between 1-3 and stores\
        them in all 10 spaces of random_numbers array
        Inputs: object name
        Outputs: the array self.random_numbers
        
        '''
        #code block generates a random number between 1 and 3 once for every
        #slot in the random numbers array
        import random
        for i in range(10):
            self.random_numbers[i] = random.randint(1,3)
        return self.random_numbers
    
    def printRandomNumber(self, randnum):
        
        '''
        
        Function Description: displays the random numbers to user
        Inputs: object name and the array of random numbers
        Outputs: printed text
        
        '''        
        #prints the random numbers one at a time
        print('NUM: ', end='')
        for i in range(len(randnum)):
            print(randnum[i], end=' ')
        print()
            
    def inputUserEntry(self):
        
        '''
        
        Function description: prompts user for code associated to the random
        numbers. Then checks if the code matches the actual password
        Inputs: object name
        Outputs: True or False
        
        '''
        #code block prompts for entered digits, stores them, then checks for\
        #validity by seeing if each entered digit matches what the actual\
        #password digit is associated with
        code = input("Enter code:")
        for i in range(5):
            self.entered_digits[i] = int(code[i])
        for i in range(5):
            if self.entered_digits[i] !=\
               self.random_numbers[self.actual_password[i]]:
                return False
        return True

        
        
