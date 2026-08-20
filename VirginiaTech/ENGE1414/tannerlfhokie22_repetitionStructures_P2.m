%collect input, and establish a sum variable
days = input('How many days?: ');
sum = 0;

%for every day, prompt for the temperature high and add that to your sum
i = 0;
while i < days
    temp = input('temperature high: ');
    sum = sum + temp;
    i = i + 1;
end

%calculate the average temperature high and output necessary information
average = sum / days;
disp('The average temperature high here over those ' + string(days) + ' days is ' + string(average) + ' degrees.')
disp('To get that result I added up all the temperatures you gave me, and then divided that sum by the amount of total days. That gets you your average, or "mean central tendency"!')
