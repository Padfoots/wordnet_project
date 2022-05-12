internal class Synsets
{
    // function read_synsets implementation
   public static Dictionary<int, List<string>> read_synsets(string text_file)
    {
        // create a dictionary for the synsets where the key is the id (int) and the value is a list of strings

        var wordNetDic = new Dictionary<int, List<string>>();


        // read data from the synsets file and put it into the dictionary

        var reader = new StreamReader(text_file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            int id = int.Parse(values[0]);
            var array = values[1].Split(' ');
            var synsets = new List<string>(array);
            wordNetDic.Add(id, synsets);
        }
        reader.Close();

        // printing every key value pair in the dictionary

        /*
                    foreach (KeyValuePair<int, List<string>> keyValuePair in wordNetDic)
                    {
                        Console.WriteLine("id:" + keyValuePair.Key + " word: " + String.Join(",", keyValuePair.Value));
                    }
        */
        return wordNetDic;

    }
}