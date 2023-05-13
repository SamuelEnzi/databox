namespace CLI
{
    public class Config
    {
        public string? Server { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Database { get; set; }
        public string? JsonFile { get; set; }
        public string? Dataset { get; set; }
        public bool AutoCreate { get; set; } = true;

        public bool IsValid
        {
            get =>
                    !string.IsNullOrEmpty(Username) &&
                    !string.IsNullOrEmpty(Password) &&
                    !string.IsNullOrEmpty(Database) &&
                    !string.IsNullOrEmpty(JsonFile) &&
                    !string.IsNullOrEmpty(Dataset) &&
                    !string.IsNullOrEmpty(Server);
        }

        public static Config Parse(string[] args)
        {
            var temp = new Config(); 
            foreach (var props in ParseProperties(args))
                Assignmen(props, ref temp);
            return temp;
        }

        private static void Assignmen((string name, string value) prop, ref Config target)
        {
            if(prop.name.ToLower().Trim() != "password")
                Console.WriteLine($"{prop.name.ToLower()} -> {prop.value.Trim()}");

            switch (prop.name.ToLower().Trim())
            {
                case "server":
                    target.Server = prop.value.Trim();
                    break;
                case "username":
                    target.Username = prop.value.Trim();
                    break;
                case "password":
                    target.Password = prop.value.Trim();
                    break;
                case "database":
                    target.Database = prop.value.Trim();
                    break;
                case "jsonfile":
                    target.JsonFile = prop.value.Trim();
                    break;
                case "dataset":
                    target.Dataset = prop.value.Trim();
                    break;
                case "autocreate":
                    target.AutoCreate = prop.value.ToLower().Trim().Equals("true");
                    break;
                default:
                    break;
            }
        }

        private static IEnumerable<(string name, string value)> ParseProperties(string[] args)
        {
            foreach(var prop in args)
            {
                var chuncks = prop.Split('=');
                if (chuncks.Length != 2) continue;
                yield return (chuncks[0], chuncks[1]);
            }
        }
    }
}
