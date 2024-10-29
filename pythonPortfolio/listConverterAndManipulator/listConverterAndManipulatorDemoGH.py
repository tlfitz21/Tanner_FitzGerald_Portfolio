#Tanner FitzGerald 
#4/20/24
#CSC 221 Assignment 
'''

This program will import listConverterAndManipulatorGH to use its functions. It
will prompt the user for a list of integers and manipulate, convert, and display
the list in various ways

'''

import listConverterAndManipulatorGH

def listManager():
    
    '''
    Function Description: runs all of the functions defined in the 
    ListConverterAndManipulator module. 
    -readList is called first, and outputs a list of prompted integers
    -listconverter is called, inputting the original_list and outputs several
    displayed print statements
    -listSlicer is then called, inputting the original_list and outputing
    a few displayed lists
    -listComprehension is then called, inputting the original list and
    outputting a new list
    -listSorter is then called, inputting the comprehension list and outputting
    -a new list
    -cinvertListToDictionary is then called, inputting the sorted list and
    outputting a dictionary
    outputs: several displayed print statements
    '''
    
    original_list = listConverterAndManipulatorGH.readList()
    listConverterAndManipulatorGH.listConverter(original_list)
    nested_list = listConverterAndManipulatorGH.listSlicer(original_list)
    comprehension_list = listConverterAndManipulatorGH.listComprehension\
(original_list)
    sorted_list = listConverterAndManipulatorGH.listSorter(comprehension_list)
    convert_list_to_dictionary = listConverterAndManipulatorGH.\
convertListToDictionary(sorted_list)
    print("[1] index of a list:", nested_list[1])
    print()
    print("[1][0] index of a list:", nested_list[1][0])
    print()
    print("Sorted list content in descending order:", sorted_list)
    print()
    print("Converted Dictionary:", convert_list_to_dictionary)
    
listManager()
