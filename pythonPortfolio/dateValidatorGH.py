#Tanner FitzGerald
#3/25/24
#CSC 221 Assignment
'''

This program prompts the user to input a date and checks if the entry is valid

'''


#code block prompts user for date and tests if input is a valid date
invalid_date = 0
input_date = input("Please enter a date to be checked\nusing the format mm/dd/\
yyyy (mm for month, dd for day, yyyy for year): ")
#tests if the input date has the expected amount of characters
if len(input_date) != 10:        
    invalid_date = 1
#tests if the input has the correct seperators
if input_date[2] != '/' or input_date[5] != '/':     
    invalid_date = 1
    
#slices input string into substrings for the date parts, and creates integers\
#to reference them
input_month = input_date[0:2]
input_day = input_date[3:5]
input_year = input_date[6:10]
int_month = int(input_month)
int_day = int(input_day)
int_year = int(input_year)


print('Your date is ',input_month+':'+input_day+':'+input_year)
print("Also written as ", input_date)

#code block tests if the input year is a leap year
if int_year % 4 != 0:
    leap_year = False
elif int_year % 100 != 0:
    leap_year = True
elif int_year % 400 != 0:
    leap_year = False
else:
    leap_year = True

#testing if input month is a valid month
if int_month >= 1 and int_month <= 12:
    pass
else:
    invalid_date = 2

#testing if input day is valid
if input_month == '04' or input_month == '06' or input_month == '09' or\
input_month == '11':
    if int_day >= 1 and int_day <= 30:
        pass
    else:
        invalid_date = 3
elif input_month == '02':
    if leap_year == False:
        if int_day >= 1 and int_day <= 28:
            pass
        else:
            invalid_date = 4
    else:
        if int_day >= 1 and int_day <= 29:
            pass
        else:
            invalid_date = 5
else:
    if int_day >= 1 and int_day <= 31:
        pass
    else:
        invalid_date = 6

#displaying validity of date and why it is invalid if it is
if invalid_date == 0:
    print("It is a valid date.")
else:
    print("It is not a valid date.")
    if invalid_date == 1:
        print("The reason it is invalid: The input format was incorrect")
    elif invalid_date == 2:
        print("The reason it is invalid: The month value is not from 1 to 12")
    elif invalid_date == 3:
        print("The reason it is invalid: The day value is greater than 30 in a\
 month with just 30 days")
    elif invalid_date == 4:
        print("The reason it is invalid: The day value is greater than 28 in\
 February in a non leap year")
    elif invalid_date == 5:
        print("The reason it is invalid: The day value is greater than 29 in\
 February in a leap year")
    else:
        print("The reason it is invalid: The day value is gretaer than 31 in\
 a month with just 31 days")



















    
    
    
    
    
    
    
    
    
    
    
    
    
    