#Tanner FitzGerald
#10/29/24
#Personal Practice Project
'''

This program contains the class "Calculator" which will emulate a four-function
calculator by continuously prompting the user for a function to perform and two
numbers to perform it on.

'''
class Calculator:
    
    def sumCalculator(numA, numB):
        sum = numA + numB
        return sum
    
    def differenceCalculator(numA, numB):
        difference = numA - numB
        return difference

    def productCalculator(numA, numB):
        product = numA * numB
        return product

    def quotientCalculator(numA, numB):
        quotient = numA/numB
        return quotient

    def prompter(self):
        print("Select + to find the sum.\nSelect - to find the difference.\nSelect * to find the product.\nSelect / to find the quotient.\nSelect Q to quit.")
        while True:
            choice = input("Select a function: ")
            if choice == "+":
                numA = float(input("Enter first number: "))
                numB = float(input("Enter second number: "))
                print(Calculator.sumCalculator(numA,numB))
            elif choice == "-":
                numA = float(input("Enter first number: "))
                numB = float(input("Enter second number: "))
                print(Calculator.differenceCalculator(numA, numB))
            elif choice == "*":
                numA = float(input("Enter first number: "))
                numB = float(input("Enter second number: "))
                print(Calculator.productCalculator(numA, numB))
            elif choice == "/":
                numA = float(input("Enter first number: "))
                numB = float(input("Enter second number: "))
                print(Calculator.quotientCalculator(numA, numB))
            elif choice == "Q" or choice == "q":
                print("Goodbye")
                break
            else:
                print("Invalid entry")
                pass
myCalc = Calculator()
myCalc.prompter()
