#Tanner FitzGerald
#2/20/24
#CSC 221 Assignment
'''

This was my first program in python, and it prompts for an integer and a float,
then outputs their sum, product, and quotient.

''' 

integer_prompt = input("Please enter an integer: ")   # Prompt and read a value
float_prompt = input("Please enter a floating point number: ") # Prompt and read a value

integer_number = int(integer_prompt)  # convert/cast the above read value to integer
float_number = float(float_prompt)    # convert/cast the above read value to float

# Calculating the sum, product, and division of the two values
sum = integer_number + float_number
product = integer_number * float_number
quotient = integer_number / float_number

# Display the two values, their sum, product, and divsion
print("The numbers are: ", integer_number, " and ", float_number)
print('Their sum is: ', sum)
print('Their product is:', product)
print('Their quotient is:', quotient)
