#Tanner FitzGerald 
#4/20/24
#CSC 221 Assignment 
'''

This program contains functions for another program to use. It was designed to
practice importing it into a different module that calls its functions.

Th functions prompt the user for a list of integers, and display various manipulations
of this list.

'''
    
def readList():
    
    '''
    Function description: prompts user for 7 integers, then stores them as
    integers into a list
    returns: list of 7 integers
    '''
    #code block makes a temporary list with the split strings from input, and 
    #converts them to integers before storing them in the output list
    
    original_list = []
    input_list = (input("Enter 7 integer values, seperated by\
 spaces:").split())
    for x in input_list:
        original_list.append(int(x))
    return original_list

def listConverter(list):
    
    '''
    
    Function description: receives a list, then does the following to it:
    displays the min and max value
    converts it to a tuple, doubles the values of the tuple, and displays both
    converts it to a set, prompts user for a word to update it, displays both
    converts it to a dictinary, prompts user for a new entry, and displays both
    input: list
    output: several displayed print statements
    '''
    
    print()
    print()
    print("Original List:", list)
    
    min_val = min(list)
    max_val = max(list)
    print("Min Value in list:", min_val)
    print("Position of the Min value:", list.index(min_val))
    print()
    print("Max Value in the list:", max_val)
    print("Position of the Max value:", list.index(max_val))
    print()
    
    #code block creates a temporary list, then doubles each value of tuple for
    #the length of the tuple, then converts the temporary list to tuple
    converted_tuple = tuple(list)
    print("Converted Tuple from the list:", converted_tuple)
    temp_list = []
    for x in converted_tuple:
        temp_list.append(2 * x)
    modified_converted_tuple = tuple(temp_list)                      
    print("updated tuple:", modified_converted_tuple)
     
    converted_set = set(modified_converted_tuple)
    print("Converted set from a tuple:", converted_set)
    converted_set.add(input("Enter a word:"))
    add_converted_set = converted_set
    print("Updated set:", add_converted_set)
    
    #code block prompts for a key and value, then updates the dictionary with
    #that entry, then finally lets that equal the new dictionary
    converted_dictionary = dict.fromkeys(add_converted_set,0)
    print("Converted Dictionary:", converted_dictionary)
    input_key = input("Enter a text word used as a key:")
    input_value = input("Enter an integer value used as a value for above\
 key:")
    converted_dictionary.update({input_key:input_value})
    add_converted_dictionary = converted_dictionary
    print("Updated Dictionary:", add_converted_dictionary)

def listSlicer(list):
    
    '''
    
    Function Description: receives a list and creates 3 new lists by slicing
    the original list into parts
    inputs: list
    outputs: list called nested list
    
    '''
    
    print()
    print("Sliced Lists:")
    print()
    first_slice = list[:3]+list[7:]
    print("First sliced list content:", first_slice)
    print()
    second_slice = list[3:]+list[7:]
    print("Second sliced list content:", second_slice)
    print()
    nested_list = [second_slice,first_slice]
    print("Nested List content:", nested_list)
    print()
    return nested_list

def listComprehension(list):
    
    '''
    Function Description: receives a list, and creates a new list using list
    comprehension with each value less than or equal to 20 being multiplied by
    5
    inputs: list
    outputs: list called comprehension_list
        
    '''
    
    comprehension_list = [x * 5 for x in list if x <= 20]
    return comprehension_list

def listSorter(list):
    
    '''
    Function Description: receives a lit and sorts it by descending order
    inputs: list
    outputs: list called sorted_list
    '''
    
    list.sort(reverse = True)
    sorted_list = list
    return sorted_list

def convertListToDictionary(list):
    
    '''
    Function Description: receives a list and converts it into a dictionary 
    with the list elements turned into keys, and the values starting at 0, then
    incrementing by 1 for each key
    inputs: list
    outputs: dictionary called converted_dictionary
    '''      
    #code block creates an empty dictionary, then establishes a temporary
    #variable that is incremented in a for loop. each iteration of the loop
    #updates the dictionary with a new entry for the appropriate keys and
    #values
    converted_dictionary = {}
    y = 0
    for x in list:
        converted_dictionary[list[y]] = y
        y += 1
    return converted_dictionary

 