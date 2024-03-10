using System.Diagnostics;

var sw = Stopwatch.StartNew();

if (args.Length == 0)
{
    Console.WriteLine("No file path provided!");
    return;
}
else if (args.Length > 1)
{
    Console.WriteLine("Too many arguments! Provide only one file path.");
    return;
}

var path = args[0];

Console.WriteLine("Checking if file exists...:");

var fileExists = File.Exists(path);

if (!fileExists)
{
    Console.WriteLine($"File does not exist: {path}");
    return;
}

Console.WriteLine($"File exists!: {File.Exists(path)}");

var lines = File.ReadLinesAsync(path);

Console.WriteLine("Calculating sum of precipitation...");
var lineNumber = 0;

var stations = new Dictionary<string, StationData>();



// Actual stuff
await foreach (var line in lines)
{
    lineNumber++;
    if (lineNumber % 1000000 == 0) {
        Console.WriteLine($"Processed {lineNumber} lines");
    }
    
    var lineDump = line.Split(';');
    var station = lineDump[0];
    var precipitation = float.Parse(lineDump[1]);
    
    if (!stations.ContainsKey(station))
    {
        stations[station] = new StationData(precipitation);
        continue;
    }

    var currentStation = stations[lineDump[0]];
    currentStation.Min = Math.Min(currentStation.Min, precipitation);
    currentStation.Max = Math.Max(currentStation.Max, precipitation);
    currentStation.Sum += precipitation;
    currentStation.numberOfReadings++;
    currentStation.Mean = currentStation.Sum / currentStation.numberOfReadings;
}



// Output
Console.WriteLine($"Number of stations: {stations.Count}");
foreach (var station in stations)
{
    Console.WriteLine($"Station: {station.Key}");
    Console.WriteLine($"Min: {station.Value.Min}");
    Console.WriteLine($"Max: {station.Value.Max}");
    Console.WriteLine($"Mean: {station.Value.Mean}");
    Console.WriteLine($"Sum: {station.Value.Sum}");
    Console.WriteLine($"Number of readings: {station.Value.numberOfReadings}");
    Console.WriteLine();
}

sw.Stop();
Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");

class StationData(float initialValue)
{
    public float numberOfReadings = 1;
    public float Min = initialValue;
    public float Max = initialValue;
    public float Sum = initialValue;
    public float Mean;
}

