%get the input array, and create an empty array with 0s represnting each
%dice face at the array index
rolls = input("Enter your set of rolls: ");
tally = zeros(1,6);

%for each value of the input array, increment tally at the number's index
%by 1
i = 1;
while i < length(rolls) + 1
    tally(rolls(i)) = tally(rolls(i)) + 1;
    i = i + 1;
end

%for each number in tally, calculate it's frequency percentage and display
%necessary information
i =1;
while i < 7
    percentage = (tally(i) / length(rolls)) * 100;
    disp('Number of ' + string(i) + 's: ' + string(tally(i)) + ' That is ' + string(percentage) + '% of the total rolls.')
    disp('')
    i = i + 1;
end