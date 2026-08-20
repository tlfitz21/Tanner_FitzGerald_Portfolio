.data
.text

.globl LL_PUSH_FRONT
.globl LL_COUNT
.globl LL_SEARCH
.globl LL_INDEX
.globl LL_GREATER_OR_EQUAL
.globl LL_REVERSE

# -----------------------------------------------------------------------------
# Push a integer to the front of the given linked list.
# Allocates memory for the linked list node on the heap.
#
# Pre:
# 	- $a0 contains the address of the start of the linked list
# 	- $a1 contains the integer to add to the node
# Post:
# 	- $a0 contains the new address of the start of the linked list
# -----------------------------------------------------------------------------
LL_PUSH_FRONT:
	# -- START SOLUTION --
	add a2, zero, a0
	
	li a7, 9
	addi a0,zero, 8
	ecall
	add a3, zero, a0
	
	sw a1, 0(a3)
	sw a2, 4(a3)
	
	add a0, a3, zero
	 
	# -- END SOLUTION --
	jr ra, 0


# -----------------------------------------------------------------------------
# Return the length of the given linked list.
# The length of the linked list is the number of nodes it contains.
# A list with only the sentinel node has length 0.
#
# Pre:
# 	- $a0 contains the address of the head of the linked list
# Post:
# 	- $a0 contains the length of the linked list
# -----------------------------------------------------------------------------
LL_COUNT:
	# -- START SOLUTION --
	lw a1, 4(a0)
	addi a0, zero, 0
	
	countLoop:
		beq a1, zero, countEnd
		addi a0, a0, 1
		lw a1, 4(a1)
		j countLoop		
	countEnd:
		
	# -- END SOLUTION --
	jr ra, 0


# -----------------------------------------------------------------------------
# Return the lowest index of the given integer in the list.
# Return -1 if the integer is not found.
#
# Pre:
# 	- $a0 contains the address of the head of the linked list
# 	- $a1 contains the integer to search for in the list
# Post:
# 	- $a0 contains the lowest index of the integer if found, -1 otherwise
# -----------------------------------------------------------------------------
LL_SEARCH:
	# -- START SOLUTION --
	lw a2, 0(a0)
	lw a3, 4(a0)
	addi a0, zero, -1
	addi a4, zero, -1
	
	searchLoop:
		beq a3, zero, searchEnd
		add a5 zero, a2
		addi a4, a4, 1
		lw a2, 0(a3)
		lw a3, 4(a3)
		bne a1, a5, searchLoop
		add a0, zero, a4
	searchEnd:
	
	# -- END SOLUTION --
	jr ra, 0


# -----------------------------------------------------------------------------
# Return the integer at the given index in the list, and an integer indicating
# success/failure.
# 
# The first integer is at index 0, second integer at index 1, and so forth.
# The index is invalid if it is greater or equal to the length of the list.
#
# Pre:
# 	- $a0 contains the address of the start of the linked list
# 	- $a1 contains the index of the list to retrieve
# 	- The index given is greater or equal to 0
# Post:
# 	- $a0 contains the integer at the given index if it is valid
# 	- $a1 contains 0 on success, or -1 on failure (if the index is invalid)
# -----------------------------------------------------------------------------
LL_INDEX:
	# -- START SOLUTION --
	lw a2, 4(a0)				#set a2 to the head pointer
	add a6, a0, zero			#set a6 to the address of the head
	addi a3, zero, -1			#set a3 to -1 (our index)
	add a5, zero, a1			#set a5 to target index
	addi a1, zero, -1			#set a1 to -1 (return value)
	
	indexLoop:
		beq a2, zero, indexEnd		#(if we're not at the end:)
		addi a3,a3,1			#(increment the index)
		lw a4, 0(a6)			#(put the current value into a4)
		add a6, a2, zero		#(change current value adress to current pointer)
		lw a2, 4(a2)			#(change current pointer to next pointer)
		bne a5, a3, indexLoop		#(if we're not at the target index, continue)
		add a0, zero, a4		#set a0 to current value
		addi a1, a1, 1			#set success val to true
	indexEnd:
	# -- END SOLUTION --
	jr ra, 0


# -----------------------------------------------------------------------------
# Return a heap-allocated array of all integers in the given linked list that
# are greater or equal to the given lower bound.
#
# If none of the integers in the linked list are greater or equal to the lower
# bound, return 0 for both the result array and its length.
# 
# Example 1:
# 	- (argument) Linked list: 1->5->3->7->4->9
# 	- (argument) Lower bound: 4
# 	- Values in result array on heap: 5, 7, 4, 9
# 	- (return value) Array pointer: 0x100040
# 	- (return value) Array length: 4
#
# Example 2:
# 	- (argument) Linked list: 1->5->3->7->4->9
# 	- (argument) Lower bound: 100
# 	- (return value) Array pointer: 0
# 	- (return value) Array length: 0
#
# Pre:
# 	- $a0 contains the address of the head of the linked list
# 	- $a1 contains the lower bound of the integers to retrieve
# Post:
# 	- $a0 contains the address of the heap-allocated result array
# 	- $a1 contains the length of the result array
# -----------------------------------------------------------------------------
LL_GREATER_OR_EQUAL:
	# -- START SOLUTION --
	
	#lw a2, 4(a0)
	add a2, a0, zero
	add a3, a1, zero
	addi a1, zero, 0
	
	GOELoop:
		beq zero, a2, GOEEnd
		lw a4, 0(a2)
		blt a4, a3 skip
		li a7, 9
		addi a0, zero, 4
		ecall
		sw a4, 0(a0)
		addi a1, a1, 1	
		
		skip:
		lw a2, 4(a2)
		j GOELoop
		
	GOEEnd:
	slli a5, a1, 2
	sub a0, a0, a5
	# -- END SOLUTION --
	jr ra, 0


# -----------------------------------------------------------------------------
# Reverse the given linked list in-place, then return the new head of the
# linked list.
#
# Pre:
# 	- $a0 contains the address of the head of the linked list
#       - $a1 contains the address of sentinel value of the linked list
# Post:
# 	- $a0 contains the new address of the head of the linked list
# -----------------------------------------------------------------------------
LL_REVERSE:
	# -- START SOLUTION --
	lw a2, 4(a0)		#a2 = head pointer
	add a3, a0, zero	#a3 = head value address
	
	reverseLoop:
	
		beq a2, zero, end
			
	#I ran out of time
		
	
	end:
	# -- END SOLUTION --
	jr ra, 0
