program MainTest(input, output);

uses
  Sysutils,
  Types, 
  StrUtils;

{$I hw3.pas}

var
	i: integer;
	out: TextFile;
	fin: TextFile;
	base, exponent, number, answer: LongInt;
	junk: Char;
	command: String;
	split: TStringDynArray;
	numbers: TIntegerDynArray;
	len: LongInt;
	b_o_s: biggest_or_smallest;

begin
	if ( paramCount() <> 2 ) then
	begin
		writeln( 'Usage: test-main <input> <output>' );

	end
	else
	begin
		Assign( out, paramStr(2) );
		Assign( fin, paramStr(1) );
		Reset(fin);
		Rewrite(out);
		While not Eof(fin) do
		begin
		read( fin, command );
		{ now let's see how we can split up this command }
		split := SplitString( command, ' ' );
		if comparestr(split[0], 'power') = 0 then
		begin
			base := StrToInt(split[1]);
			exponent := StrToInt(split[2]);
			answer := power(base, exponent);
			write( out, base );
			write( out, ' ^ ' );
			write( out, exponent );
			write( out, ' = ' );
			writeln( out, answer );
		end
		else if comparestr(split[0], 'find') = 0 then
		begin
			len := length(split);
			SetLength( numbers, len-2 );
			for i := 2 to len-1 do
			begin
				number := StrToInt(split[i]);
				numbers[i-2] := number;
			end;
			if comparestr(LowerCase(split[1]), 'biggest') = 0 then
			begin
				b_o_s := BIGGEST;
				number := find( numbers, b_o_s);
				write(out, 'find BIGGEST ' );
				for i := 0 to length(numbers)-2 do
					write(out, numbers[i], ' ' );
				write(out,  numbers[length(numbers)-1], ' is ');	
				writeln(out, number);
			end
			else
			begin
				b_o_s := SMALLEST;
				number := find( numbers, b_o_s);
				write(out, 'find SMALLEST ' );
				for i := 0 to length(numbers)-2 do
					write(out, numbers[i], ' ' );
				write(out,  numbers[length(numbers)-1], ' is ');	
				writeln(out, number);
			end;
		end
		else if comparestr(split[0], 'log') = 0 then
		begin
			number := StrToInt(split[1]);
			answer := log_base_2(number);
			writeln(out, 'log_base_2 of ', number, ' = ', answer );
		end
		else
			writeln( 'unknwon command' );
		read(fin, junk ); { maybe getting rid of newline? }
		end;
		close(fin);
		close(out);
	end	
end.
