mod hw4;

use hw4::log;
use hw4::std_dev;
use hw4::sum_list;
use hw4::double_list;
use hw4::average_list;
use std::env;
use std::fs::File;
use std::io::{BufReader, Error, BufRead, Write};

fn main() -> Result<(), Error>
{

    let args: Vec<String> = env::args().collect();

    if args.len() != 3
    {
        println!("Usage: test-hw4 <input> <output>");
    }
    else
    {
        let input = &args[1];
        let output = &args[2];
        let path = File::open(&input)?;
        let reader = BufReader::new(path);
        let mut out = File::create(output).expect("unable to create file");
        for line in reader.lines()
        {
            let this_line = line.unwrap();
            let words: Vec<&str> = this_line.split(" ").collect();
            if words[0] == "log"
            {
                let n = words[1].parse::<i32>().unwrap();
                let base = words[2].parse::<i32>().unwrap();
                let answer = log(n, base);
                writeln!(&mut out, "log ({}, {}) = {}", n, base, answer)?;
            }
            else if words[0] == "sum_list"
            {
                let mut list : Vec<f32> = Vec::new();
                for i in 0..words.len() - 1
                {
                    let number = words[i+1].parse::<f32>().unwrap();
                    list.push(number);
                }
                writeln!(&mut out, "sum_list {:?} is {}", &list, sum_list(&list))?;
            }
            else if words[0] == "double_list"
            {
                let mut list : Vec<f32> = Vec::new();
                for i in 0..words.len() - 1
                {
                    let number = words[i+1].parse::<f32>().unwrap();
                    list.push(number);
                }
                write!(&mut out, "double_list {:?}", &list )?;
                double_list(&mut list);
                writeln!(&mut out, " is {:?}",  &list)?;
            }
            else if words[0] == "average_list"
            {
                let mut list : Vec<f32> = Vec::new();
                for i in 0..words.len() - 1
                {
                    let number = words[i+1].parse::<f32>().unwrap();
                    list.push(number);
                }
                writeln!(&mut out, "average_list {:?} is {}", &list, average_list(&list))?;
            }
            else if words[0] == "std_dev"
            {
                let mut list : Vec<f32> = Vec::new();
                for i in 0..words.len() - 1
                {
                    let number = words[i+1].parse::<f32>().unwrap();
                    list.push(number);
                }
                let copy = list.clone();
                writeln!(&mut out, "std_dev {:?} is {}", &copy, std_dev(list))?;
            }
            else
            {
                println!( "Unknown command: {:?}", words );
            }
        }
    }

    Ok(())

}
