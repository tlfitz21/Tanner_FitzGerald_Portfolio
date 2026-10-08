 
type
	biggest_or_smallest = ( BIGGEST, SMALLEST );

//you are not allowed to use the function power
Function power( base: LongInt; exponent: LongInt ) : LongInt;

var
	count: LongInt;

Begin
	power := 1;	
	count := 0;
	while count < exponent do
	begin
		power := power * base;
		count := count + 1;
	end;

End;

// you are not allowed to use the built-in minvalue or maxvalue functions from
// pascal
function find( var v : TIntegerDynArray; var b_o_s : biggest_or_smallest ) : LongInt;

var
	index: integer;

Begin
	find := 0;
	index := 0;
	if (b_o_s = BIGGEST) then
	begin
		while (index < Length(v)) do
			begin
				if (v[index] > find) then
					find := v[index];
				index := index + 1;
			end;
	end
	else
	begin
		find := 2147483647;
		while (index < Length(v)) do
		begin
			if (v[index] < find) then
				find := v[index];
			index := index + 1;
		end;
	end;
End;

// you are not allowed to use the log2 function
// or the log function
function log_base_2( n : LongInt ) : LongInt;

var
	total: integer;

begin
	log_base_2 := 0;
	total := 1;

	while (total * 2) < n do
	begin
		total := total * 2;
		log_base_2 := log_base_2 + 1;
	end
end;
