#Tanner FitzGerald
#3/31/24
#CSC 221 Assignment
'''

This program will prompt the user for a dollar amount and calculate how many
Chocolate bars they can buy. Each chocolate bar costs 1 dollar, and comes with
a coupon that you can redeem 6 of to get a new chocolate bar. The program will
output the amount of chocolate bars and leftover coupons the user will have
after spending all their money.

'''

bars = int(input("How much money can you spend on chocolate bars?: "))
coupons = bars

#code block loops a body that 'redeems' 6 coupons for a chocolate bar until\
#less than 6 coupons remain
while coupons > 5:
    new_bars = coupons // 6
    bars = new_bars + bars
    coupons = (coupons % 6) + new_bars

print("After redeeming coupons, you would have", coupons, "leftover coupon(s)\
 and can purchase a total of", bars, " chocolate bars.")