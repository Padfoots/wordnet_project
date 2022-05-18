internal class Synsets
{
    // a dictionary that holds the id and synsets

    public static Dictionary<int, List<string>> wordnet_dic = new Dictionary<int, List<string>>();

    // a dictionary that holds a word and it's correponding ids as a list of values

    public static Dictionary<string, List<int>> word2id_dic = new Dictionary<string, List<int>>();
    
    // function read_synsets implementation

    public static void read_synsets(string text_file)
    {
        // read data from the synsets file and put it into the dictionary

        var reader = new StreamReader(text_file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            int id = int.Parse(values[0]);
            var array = values[1].Split(' ');

            // adding to the wordNetdic
            var synsets = new List<string>(array);
            wordnet_dic.Add(id, synsets);

            // adding to the word2id_dic 

            foreach (var item in array)
            {
                if (word2id_dic.ContainsKey(item))
                {
                    word2id_dic[item].Add(id);
                }

                else
                {
                    List<int> wordset = new List<int>();
                    wordset.Add(id);
                    word2id_dic.Add(item, wordset);
                }

            }


        }
         reader.Close();


    }
    
    // getter function takes a word returns the ids for the words O(1)
    
    public static List<int> get_ids(string word)
    {
        return word2id_dic[word];
    }

    public static List<string> get_synsets(int id)
    {
        return wordnet_dic[id];
    }
}