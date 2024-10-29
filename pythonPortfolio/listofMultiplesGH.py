#Tanner FitzGerald
#7/2/24
#Personal Practie Project
'''
This program takes a base number and a length, and returns a list of that length
containing sequential multiples of the base number
'''

num, length = (input("Enter your base number and desired list length seperated by a space: ").split())
my_list = []
for x in range(int(length)):
    my_list.append(int(num) * (x + 1))
print(my_list)