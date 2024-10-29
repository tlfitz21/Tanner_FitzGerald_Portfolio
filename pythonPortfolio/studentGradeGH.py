#Tanner FitzGerald
#4/10/24
#CSC 221 Assignment
'''

This Program prompts user for a student's grades on 4 assessments, and calculates
the resulting final grade. It outputs the grade as a percentage and letter grade.

It allows the user to make corrections to their input after entering data, and 
continuously calculates student data until the user says they are done.

for the calculations, the midterm is worth the same amount as both quizzes combined,
and the final test is worth twice that amount.

'''

student_name = str
quiz_1 = int
quiz_2 = int
mid_term_test = int
final_test = int
percent_grade = int
final_letter_grade = chr

def readStudentData():
    """
    Function description: prompt user for grade student data, and checks for
    validity of the data
    data includes(int)
    student_name(int)
    quiz_1(int)
    quiz_2(int)
    mid_term_test(int)
    final_test(int)
    valid scores are 0-10 for the quizzes and 0-100 for the tests
    
    """
    global student_name
    global quiz_1
    global quiz_2
    global mid_term_test
    global final_test
    
    student_name = input("Enter the student's name: ")

   #each of the next four blocks of code prompt user for the score of that
   #assignment. It then checks if the score is valid. if the input is invalid,
   #it prompts user to enter a valid number instead.
    q1_test = int(input("Enter Quiz 1 score (Must be between 0 and 10\
 points): "))
    valid = 0
    while valid == 0:
        if q1_test <= 10 and q1_test >= 0:
            valid += 1
            quiz_1 = q1_test
        else:
            print("Sorry, but - \nQuiz 1 score must be between 0 and 10 - ")
            q1_test = int(input("Please enter a valid Quiz 1 score: "))
   
    q2_test = int(input("Enter Quiz 2 score (Must be between 0 and 10\
 points): "))
    valid = 0
    while valid == 0:
        if q2_test <= 10 and q2_test >= 0:
            valid += 1
            quiz_2 = q2_test
        else:
            print("Sorry, but - \nQuiz 2 score must be between 0 and 10 - ")
            q2_test = int(input("Please enter a valid Quiz 2 score: "))

    mt_test = int(input("Enter Midterm test score (Must be between 0 and 100\
 points): "))
    valid = 0
    while valid == 0:
        if mt_test <= 100 and mt_test >= 0:
            valid += 1
            mid_term_test = mt_test
        else:
            print("Sorry, but - \nMidterm test score must be between 0 and 100\
 points - ")
            mt_test = int(input("Please enter a valid midterm score: "))

    ft_test = int(input("Enter Final test score (Must be between 0 and 100\
 points): "))
    valid = 0
    while valid == 0:
        if ft_test <= 100 and ft_test >= 0:
            valid += 1
            final_test = ft_test
        else:
            print("Sorry, but - \nFinal exam score must be 0 to 100 points - ")
            ft_test = int(input("Please enter a valid final exam score: "))

def getPercentGrade():
    
    """
    function description: calculates final grade percentage
    inputs: each individual score(quiz_1,quiz_2,mid_term_test, and final_test)
    returns: final_letter_grade(int)- the caculated result

    """
    
    #code block calculates final grade by multiplying the ratio of correct
    #points by the weight of each assignment. It then adds all the scores.
    local_percent_grade = int
    global percent_grade
    local_percent_grade = int(float(25) * (quiz_1 + quiz_2) / 20 + float(25) * \
                              mid_term_test / 100 + float(50) * final_test /\
                               100 + 0.5)
    percent_grade = local_percent_grade
    return local_percent_grade
    
def findFinalLetterGrade(percent_grade):
    
    '''
    function description: converts grade percentage into a final letter grade
    percent_grade: the value passed to the function
    final_letter_grade(chr): the character represting the letter grade
    
    '''
    
    global final_letter_grade
    if percent_grade <= 100 and percent_grade >= 90:
        final_letter_grade = 'A'
    elif percent_grade <= 89 and percent_grade >= 80:
        final_letter_grade = 'B'
    elif percent_grade <= 79 and percent_grade >= 70:
        final_letter_grade = 'C'
    elif percent_grade <= 69 and percent_grade >= 60:
        final_letter_grade = 'D'
    else:
        final_letter_grade = 'F'
    return final_letter_grade

def displayStudentData():
    
    '''
    
    function description: dsplays student data back to the user
    Just outputs the given and validated data: quiz_1, quiz_2, mid_term_test,
    final_test, and student_name. 
    Also outputs the calculated data: percent_grade and final_letter_grade
    
    '''
    print("Student Data List:")
    print("Name =", student_name)
    print("Quiz 1 score (out of 10 pts) =", quiz_1)
    print("Quiz 2 score (out of 10 pts) =", quiz_2)
    print("Midterm score (out of 100 pts) =", mid_term_test)
    print("Final exam score (out of 100 pts) =", final_test)
    print("Percent Grade =", percent_grade)
    print("Letter Grade =", final_letter_grade)

def correctStudentData():
    
    '''
    
    function description: displays all given, validated, and calculated student
    data to user and allows them to correct(overwrite) the data. It then
    displays the new, updated data. Finally, it runs the three previously
    defined functions with the new data
    inputs: all student data previously established
    outputs: newly overwritten, validated, and calculated student data
    
    '''
    
    global student_name
    global quiz_1
    global quiz_2
    global mid_term_test
    global final_test
    global percent_grade
    global final_letter_grade
    
    print("Correction processing for Student data")
    
    #the next 5 code blocks each display the currect student data, prompt user
    #to make a correction, then displays the new value back to them.
    print("Name =", student_name)
    student_name = input("Student's name: ")
    print("Name =", student_name)

    print("Quiz 1 score (out of 10 pts) =", quiz_1)
    q1_test = int(input("Quiz 1 score (out of 10 points): "))
    valid = 0
    while valid == 0:
        if q1_test <= 10 and q1_test >= 0:
            valid += 1
            quiz_1 = q1_test
        else:
            print("Sorry, but - \nQuiz 1 score must be between 0 and 10 - ")
            q1_test = int(input("Please enter a valid Quiz 1 score: "))
    print("Quiz 1 score (out of 10 pts) =", quiz_1)
    
    print("Quiz 2 score (out of 10 pts) =", quiz_2)
    q2_test = int(input("Quiz 2 score (out of 10 points): "))
    valid = 0
    while valid == 0:
        if q2_test <= 10 and q2_test >= 0:
            valid += 1
            quiz_2 = q2_test
        else:
            print("Sorry, but - \nQuiz 2 score must be between 0 and 10 - ")
            q2_test = int(input("Please enter a valid Quiz 2 score: "))
    print("Quiz 2 score (out of 10 pts) =", quiz_2)
    
    print("Midterm score (out of 100 pts) =", mid_term_test)
    mt_test = int(input("Midterm score (out of 100 points): "))
    valid = 0
    while valid == 0:
        if mt_test <= 100 and mt_test >= 0:
            valid += 1
            mid_term_test = mt_test
        else:
            print("Sorry, but - \nMidterm test score must be between 0 and 100\
 points - ")
            mt_test = int(input("Please enter a valid midterm score: "))
    print("Midterm score (out of 100 pts) =", mid_term_test)

    print("Final exam score (out of 100 pts) =", final_test)
    ft_test = int(input("Final Exam score (out of 100 points): "))
    valid = 0
    while valid == 0:
        if ft_test <= 100 and ft_test >= 0:
            valid += 1
            final_test = ft_test
        else:
            print("Sorry, but - \nFinal exam score must be 0 to 100 points - ")
            ft_test = int(input("Please enter a valid final exam score: "))
    print("Final exam score (out of 100 pts) =", final_test)
    getPercentGrade()
    findFinalLetterGrade(percent_grade)
    displayStudentData()
    
def studentGradeDemo():
    
    '''
    
    function description: displays program purpose to user, then runs all five
    other defined functions in this program repeatedly until user is done 
    with all students they have data for.
    inputs: all student data previously established, as well as all of the
    other functions
    outputs: a displayed conversation between this program and the user to show
    entered, calculated, validated, and corrected values.    
    
    '''
    
    print("This program reads student's name and quiz1, quiz2, midterm test,\
 and final test scores.\nValidates the scores within provided ranges.\
 Then, calculates the final grade percentage and\nletter grade. And\
 then, displays the student name and quiz1, quiz2, midterm test, and\
 final\ntest scores and final grade percentage and letter grade\
 for the student.")
 
    #code block is a while loop that will repeatedly run all of the other 5
    #functions until the user types 'N' for (No) more students
    repeat = "Y"
    while repeat == "Y" or repeat == "y":
        readStudentData()
        getPercentGrade()
        findFinalLetterGrade(percent_grade)
        displayStudentData()
        needsCorrecting = input("Do you need to make corrections? Reply Y for Yes or N for No: ")
        if needsCorrecting == "Y" or needsCorrecting == "y":
            correctStudentData()
        repeat = input("Do again: Processing a new student? (Y for Yes, or \
N for No ): ")
        
studentGradeDemo()