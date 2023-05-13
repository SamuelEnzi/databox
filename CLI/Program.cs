using Core;
using CLI;
using Newtonsoft.Json.Linq;

const string ErrorLog = "error_log.txt";
if (!File.Exists(ErrorLog))
    File.Create(ErrorLog).Close();
var errorStream = new StreamWriter(File.OpenWrite(ErrorLog));

var config = Config.Parse(args);

if (!config.IsValid)
{
    Log("config not valid. make sure to set all required parameters: server, username, password, database, jsonfile");
    throw new Exception("config not valid. make sure to set all required parameters: server, username, password, database, jsonfile, dataset");
}


if (!File.Exists(config.JsonFile))
{
    Log($"could not find '{config.JsonFile}'");
    throw new FileNotFoundException($"could not find '{config.JsonFile}'");
}

var connection = new Connector(config.Server, config.Username, config.Password, config.Database);
var reader = await connection.Fetch("show tables");
List<string> tables = new List<string>();

Console.ForegroundColor = ConsoleColor.DarkYellow;
Console.WriteLine("tables:");
Console.ResetColor();

while (reader.Read())
{
    var table = reader.GetString(0);
    Console.WriteLine($"\t{table}");
    tables.Add(table);
}
await reader.CloseAsync();

Console.WriteLine();
var tableExists = false;
foreach (var table in tables)
    if (table.ToLower().Trim() == config.Dataset!.ToLower().Trim())
        tableExists = true;

if (!tableExists && !config.AutoCreate)
{
    Log($"dataset '{config.Dataset}' does not exist. enable autocreate to create the table");
    throw new Exception($"dataset '{config.Dataset}' does not exist. enable autocreate to create the table");
}

Console.WriteLine("loading file contents");

var objects = File.ReadAllText(config.JsonFile).Deserialize<List<JObject>>();
if (objects.Count <= 0)
{
    Log("file does not contain datasets");
    throw new Exception("file does not contain datasets");
}

if (!tableExists)
{
    try
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write("[Creating]");
        Console.ResetColor();
        Console.Write($" dataset '{config.Dataset}'");

        var sql = TableFactory.Create(objects.First(), config.Dataset);
        await connection.ExecuteAsync(sql);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(" OK");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(" ERROR");
        Console.ResetColor();
        Log($"error while creating table: '{ex.Message}'");
        throw new Exception($"error while creating table: '{ex.Message}'");
    }
}

var count = objects.Count();
var index = 0;
foreach(var obj in objects)
{
    index++;
    var percentage = Math.Round(((float)index / (float)count) * 100f, 1);
    try
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write("[Insert]");
        Console.ResetColor();
        Console.Write($" {index}/{count} {percentage}%");
        var sql = StatementFactory.Create(obj, config.Dataset);
        await connection.ExecuteAsync(sql);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(" OK");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(" ERROR");
        Console.ResetColor();
        Log($"error writeing object: '{ex.Message}'");
    }
}

Console.WriteLine("done");

void Log(string msg)
{
    errorStream.WriteLine($"[{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}] {msg}");
    errorStream.Flush();
}