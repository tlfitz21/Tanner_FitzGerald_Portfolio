#Tanner FitzGerald
#7/2/24
#Pesonal Practice Project
'''

This program will imitate the right shift operation, without using >>.

'''

dividend, divisor_power = input("Enter your dividend, then your divisor power seperated by a space: ").split()
quotient = int(dividend) // (2** int(divisor_power))
print(quotient) 