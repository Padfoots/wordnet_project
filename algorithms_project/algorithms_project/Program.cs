class Program
{
    static string synsets_file = "E:\\6\\Algorithms\\project\\Testcases\\Complete\\1-Small\\Case1_100_100\\Input\\1synsets.txt";
    static string hypernyms_file = "E:\\6\\Algorithms\\project\\Testcases\\Complete\\1-Small\\Case1_100_100\\Input\\2hypernyms.txt";
    static string RelationsQueries = "E:\\6\\Algorithms\\project\\Testcases\\Complete\\1-Small\\Case1_100_100\\Input\\3RelationsQueries.txt";
    static string OutcastQueries = "E:\\6\\Algorithms\\project\\Testcases\\Complete\\1-Small\\Case1_100_100\\Input\\4OutcastQueries.txt";
    static string output = "E:\\6\\Algorithms\\project\\Testcases\\Complete\\1-Small\\Case1_100_100\\Output\\Output1.txt";


    static void Main(string[] args)
    {
        // calling function to read synsets_file and return a dictionary 

        var wordnet_dic = read_synsets(synsets_file);

        // calling function to read hypernyms and return a dictionary with the child as key and a list of parents

        var hypernyms = read_hypernyms(hypernyms_file);

        // reading the queries

        var reader3 = new StreamReader(RelationsQueries);
        string n_of_queries = reader3.ReadLine();

        // wordset contains all the founded ids of all synsets that the given word belongs to
        
        var wordset1 = new List<int>();
        var wordset2 = new List<int>();
        while (!reader3.EndOfStream)
        {
            var line = reader3.ReadLine();

            var values = line.Split(',');
            string word1 = values[0];
            string word2 = values[1];

            // call function find_ids to find all ids for the 2 queries words and return a list that contains the set of synsets 

            var founded_queries = find_ids(word1, word2, wordnet_dic);
            wordset1 = founded_queries[0];
            wordset2 = founded_queries[1];

            Console.WriteLine("\nfounded word1--> " +"\""+ word1 +"\" " + "with ids: {" + String.Join(",", wordset1) + "}\n");
            Console.WriteLine("\nfounded word2--> " + "\"" + word1 + "\" " + "with ids: {" + String.Join(",", wordset2) + "}\n");


            // loop through all ids in a wordset
            foreach (int id in wordset1)
            {
                Console.WriteLine("\nlooping through id: " + id);

                // calling the dfs function to return a list of ancestors for a node

                var root_path = new List<int>();
                var ancestors = new List<List<int>>();
                var word1_ancestors = dfs(id, hypernyms, root_path);
                Console.WriteLine("\nancestors of word 1: (" + string.Join(',', word1_ancestors) + ")\n");
                

            }



        }

        // function read_synsets implementation

        static Dictionary<int, List<string>> read_synsets(string text_file)
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

        // function read_hypernyms implementation

        static Dictionary<int, List<int>> read_hypernyms(string text_file)
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

            foreach (KeyValuePair<int, List<int>> keyValuePair in hypernyms)
            {
                Console.WriteLine("id: " + keyValuePair.Key + " parents ids: " + String.Join(",", keyValuePair.Value));
            }


            return hypernyms;
        }
    }

    // function to traverse the dictionary to find the id of the 2 queries words

    public static List<List<int>> find_ids(string word1, string word2, Dictionary<int, List<string>> wordNetDic)
    {
        var founded_synsets = new List<List<int>>();
        var word1_synset_list = new List<int>();
        var word2_synset_list = new List<int>();
        foreach (KeyValuePair<int, List<string>> x in wordNetDic)
        {
            if (x.Value.Contains(word1))
            {
                word1_synset_list.Add(x.Key);
            }
            if (x.Value.Contains(word2))
            {
                word2_synset_list.Add(x.Key);
            }
        }
        founded_synsets.Add(word1_synset_list);
        founded_synsets.Add(word2_synset_list);
        return founded_synsets;
    }

    // creating a dfs function that takes a node id and the adjacency list (hypernyms dictionary)
    // and returns a list of ids of ancestors till the root ancestor

    public static List<int> dfs(int node, Dictionary<int, List<int>> hypernyms, List<int> path_to_root)
    {

        foreach (var adjacent in hypernyms[node])
        {
            path_to_root.Add(adjacent);
            dfs(adjacent, hypernyms, path_to_root);
        }
        
        return path_to_root;


    }


}




