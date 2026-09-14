.data
A_IN:
    .space 40
A_OUT:
    .space 40
PROMPT:
    .asciz "Enter an integer: "

.text
main:
    la a2, A_IN			# put a reference to the beginning of A_In into a2
    la a3, A_OUT		# put a reference to the beginning of A_OUT into a3
    addi a4, a4, 10		#store "10" in a4
    add a5, a5, zero	        #store "0" in a5
LOOP1:
	
	beq a4, a5, EXIT1	#if a4 and a5 equal each other, go to EXIT1 (so if we've finished the loop cycles, exit)
	
	la a0, PROMPT 		#load prompt into a0
	li a7, 4		#put "4" into a7. that tells ecall what call to make, which for this case is print string
	ecall			#this should actually print our string
	
	li a7, 5		#load ecall for readInt
	ecall			#stores next input integer into a0
	
	sw a0, (a2)		#put the received input into a2(current position in A_IN)
	addi a2, a2, 4		#increase array pointer by 4
	addi a5, a5, 1		#increase index by 1
	
	j LOOP1
	
EXIT1:
	
    add a5, zero, zero		#reset our index counter to 0
    addi a2, a2, -4		#a2 currently points to 0x10010028, but that's the first index of A_OUT. we want to start by copying the last index of A_In into the first of A_OUT,
    				#so, 0x10010024 copied into 0x10010028. so we need to subtract 4 from a2.
    
LOOP2:
    
    beq a4, a5, EXIT2		#if a4 and a5 equal each other, go to EXIT2 (so if we've finished the loop cycles, exit)
    lw a1, (a2)			#load what you see in a2 into a1, so we can store its value, not its address into a3
    
    sw a1, (a3)			#store what you see at a1 into a3 (starts with last index of A_IN into first index of A_OUT
    
    addi a2, a2, -4		#set the reference for A_IN to one index back
    addi a3,a3, 4		#set the reference for A_OUT to one index forward
    addi a5, a5, 1		#increment index
    
    j LOOP2
    
EXIT2:

    
    
    
    
    
    
    
    
    
    