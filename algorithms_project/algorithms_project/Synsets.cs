internal class Synsets
{
    // a dictionary that holds the id and synsets

    public static Dictionary<int, List<string>> id_words = new Dictionary<int, List<string>>();

    // a dictionary that holds a word and it's correponding ids as a list of values

    public static Dictionary<string, List<int>> word_ids = new Dictionary<string, List<int>>();
    

    // read data from the synsets file and put it into the dictionary
    public static void read_synsets(string text_file)
    {
        var reader = new StreamReader(text_file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            int id = int.Parse(values[0]);
            var array = values[1].Split(' ');

            // id_words has the id as a value and a list of string which is the synset for the id
            var synsets = new List<string>(array);
            id_words.Add(id, synsets);

            //word_ids contains every 
            foreach (var word in synsets)
            {
                // if 2 occurences of the same word happend add the new id to the list of the word
                if (word_ids.ContainsKey(word))
                {
                    word_ids[word].Add(id);
                }

                // create a new list add the word along the the list created having the id 
                else
                {
                    List<int> wordset = new List<int>();
                    wordset.Add(id);
                    word_ids.Add(word, wordset);
                }
            }
        }
         reader.Close();
    }
    
    // getter function takes a word returns the ids for the words O(1)
    public static List<int> get_ids(string word)
    {
        return word_ids[word];
    }
    // getter function takes the id and returns the synset list 
    public static List<string> get_synsets(int id)
    {
        return id_words[id];
    }
}