#Tanner FitzGerald
#3/10/24
#CSC 221 Assignment
'''

This program will prompt the user for information, then use the Harris-Benedict
equation to calculate the total number of candy bars(at 230 calories each) they
would have to eat a day to maintain their weight.

'''
    
print("This program will calculate the number of 230 calorie\n"
   "candy bars to eat to maintain your weight.")
print()

age = int(input("Enter your age in years: "))
print()

feet, inches = input("Please enter a height in feet and inches.\n"
  "Example: enter 5 6, for 5 feet and 6 inches: ").split() 

print()
feet = int(feet)
inches = int(inches)
total_height = (feet * 12) + inches

weight = int(input("Enter your weight in pounds: "))
print()

woman_bmr = 655 + (4.3 * weight) + (4.7 * total_height) - (4.7 * age)
man_bmr = 66 + (6.3 * weight) + (12.9 * total_height) - (6.8 * age)

sex = input("Enter \"Male\" or \"Female\": ")
if sex == "Female" or sex == "female":
    print("A female with those measurements should eat", (woman_bmr / 230), "candy bars per day\
 to maintain\nher weight.")
else:
    print("A male with those measurments should eat", (man_bmr / 230), "candy bars per day\
 to maintain\nhis weight.")
