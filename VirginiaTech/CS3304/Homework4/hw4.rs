pub fn log( n: i32, base :i32 ) -> i32
{
    let mut j = 1;
    let mut count = 0;

    if base == 1 || base == 0
    {
        return 0;
    }

    while j * base < n
    {
        j = j * base;
        count = count + 1;
    }
    return count;
}

pub fn double_list( list: &mut Vec<f32> )
{
    for j in 0..list.len()
    {
        list[j] = list[j] * list[j];
    }
}

pub fn sum_list( list : & Vec<f32> ) -> f32
{
    
    let mut total: f32 = 0.0;

    for j in 0..list.len()
    {
        total = total + list[j];
    }
    return total;
}
pub fn average_list( list : & Vec<f32> ) -> f32
{
    
    let fSize: f32 = list.len() as f32;
    return sum_list(list) / fSize;
}

pub fn std_dev( list: Vec<f32> ) -> f32
{
    let mut numerator: f32 = 0.0;
    let avg: f32 = average_list(&list);
    let n: f32 = list.len() as f32;

    for j in 0..list.len()
    {
        let x = list[j] - avg;
        numerator = numerator + (x * x)
    }



    return (numerator / n).sqrt();
}

