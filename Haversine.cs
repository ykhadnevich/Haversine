using System.Diagnostics;
using System.Globalization;
Console.OutputEncoding = System.Text.Encoding.UTF8;

if (args.Length < 4)
{
    Console.WriteLine("Usage: --db=(path to csv file) --lat=(latitude) --long=(longitude) --size=(radius)");
    return;
}

string dbPath = "";
double lat = 0, lon = 0, size = 0;
foreach (var arg in args)
{
    if (arg.StartsWith("--db="))
        dbPath = arg[5..];
    else if (arg.StartsWith("--lat="))
        lat = double.Parse(arg[6..], CultureInfo.InvariantCulture);
    else if (arg.StartsWith("--long="))
        lon = double.Parse(arg[7..], CultureInfo.InvariantCulture);
    else if (arg.StartsWith("--size="))
        size = double.Parse(arg[7..], CultureInfo.InvariantCulture);
}

if (string.IsNullOrEmpty(dbPath) || lat == 0 || lon == 0 || size == 0)
{
    Console.WriteLine("Usage: --db=(path to csv file) --lat=(latitude) --long=(longitude) --size=(radius)");
    return;
}
var sw = Stopwatch.StartNew();

var lines = File.ReadAllLines(dbPath);
foreach (var line in lines)
{
    var parts = line.Split(';');

    if (string.IsNullOrEmpty(parts[0])) 
        continue;

    double lat1 = double.Parse(parts[0]);
    double lon1 = double.Parse(parts[1]);
    double distance = Haversine(lat, lon, lat1, lon1);
    if (distance <= size)
    {
        string category = parts[2];
        string subcategory = parts[3];
        string address = parts[4];

        Console.WriteLine($"Distance(m): {distance}, Location: {lat1};{lon1};, Category: {category}, Subcategory: {subcategory}, Address: {address}");
    }
}

sw.Stop();
Console.WriteLine($"Elapsed time: {sw.Elapsed}");


static double Haversine(double lat1, double lon1, double lat2, double lon2)
{
    const double r = 63713;
    const double deg2Rad = Math.PI / 180;
    double dLat = deg2Rad * (lat2 - lat1);
    double dLon = deg2Rad * (lon2 - lon1);
    double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
               Math.Cos(deg2Rad * lat1) * Math.Cos(deg2Rad * lat2) *
               Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    return r * c;
}
