
    internal class Hypernyms
    {

    // function read_hypernyms implementation

    public static Dictionary<int, List<int>> read_hypernyms(string text_file)
    {
        //create an adjacency list

        var hypernyms = new Dictionary<int, List<int>>();

        // reading hypernyms file

        var reader = new StreamReader(text_file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            int id = int.Parse(values[0]);
            var ancestors = new List<int>();

            // loop through parents in the array
            for (int i = 1; i < values.Length; i++)
            {
                ancestors.Add(int.Parse(values[i]));
            }

            hypernyms.Add(id, ancestors);
        }
        reader.Close();

        // printing every key value pair in hypernyms dictionary

        /*            foreach (KeyValuePair<int, List<int>> keyValuePair in hypernyms)
                    {
                        Console.WriteLine("id: " + keyValuePair.Key + " parents ids: " + String.Join(",", keyValuePair.Value));
                    }*/


        return hypernyms;
    }
}

