
    internal class Hypernyms
    {

    public static Dictionary<int, List<int>> read_hypernyms(string text_file)
    {
        var hypernyms = new Dictionary<int, List<int>>();

        // reading hypernyms file
        var reader = new StreamReader(text_file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            int child = int.Parse(values[0]);
            var parents = new List<int>();

            // loop through parents in the array
            for (int i = 1; i < values.Length; i++)
            {
                parents.Add(int.Parse(values[i]));
            }

            hypernyms.Add(child, parents);
        }
        reader.Close();
        return hypernyms;
    }
}

