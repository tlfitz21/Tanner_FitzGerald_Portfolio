#Tanner FitzGerald
#06/27/24
#Personal Practice Program
'''

This Program will take two integers from input, and perform "Bitwise opertions"
on them. so it will compare the binary equivalent of the integers digit by
digit using AND, OR, and XOR rules, and produce a new integer that is represented
by the resulting binary code

'''

#getting global variables
str_1, str_2 = input('Enter two integers seperated by a space: ').split()
bin_1 = bin(int(str_1))
bin_2 = bin(int(str_2))

#alter the strings into just two binary representations with leading zeroes
#in front of the shorter one
def longAndShort(num_1, num_2):
    if len(num_1) > len(num_2):
        longest = num_1
        shortest = num_2
    else:
        longest = num_2
        shortest = num_1
    diff = len(longest) - len(shortest)
    longest = longest[2:]
    shortest = shortest[2:]
    if diff == 0:
        pass
    else:
        short_list = ['0' for x in range(diff)]
        for x in range(len(shortest)):
            short_list.append(shortest[x])
        shortest = ''.join(short_list)
    return [shortest, longest]        
binary_1, binary_2 = longAndShort(bin_1, bin_2)
print("Binary code for your integers are: ", binary_1, binary_2)

#AND function
def bitAND(num_1, num_2):
    
    my_list = []
    for x in range(len(num_1)):
        if num_1[-(x+1)] == num_2[-(x+1)]:
            my_list.append(num_1[-(x+1)])
        else:
            my_list.append('0')
    my_list.append('b')
    my_list.append('0')
    my_list.reverse()
    my_string = ''.join(my_list)
    return my_string
print("The result of a bitwiseAND function is: ", int(bitAND(binary_1,binary_2),2))

#OR function
def bitOR(num_1, num_2):
    
    my_list = []
    for x in range(len(num_1)):
        if num_1[x] == '1' or num_2[x] == '1':
            my_list.append('1')
        else:
            my_list.append('0')
    my_string = ''.join(my_list)
    return my_string
print("The result of a bitwiseOR function is: ", int(bitOR(binary_1,binary_2),2))

#XOR function            
def bitXOR(num_1, num_2):
    
    my_list = []
    for x in range(len(num_1)):
        if (num_1[x] == '1' or num_2[x] == '1') and (num_1[x] != num_2[x]):
            my_list.append('1')
        else:
            my_list.append('0') 
    my_string = ''.join(my_list)
    return my_string
print("The result of a bitwiseXOR function is: ", int(bitXOR(binary_1,binary_2),2)) 