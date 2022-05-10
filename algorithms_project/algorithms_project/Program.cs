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
            Console.WriteLine("\nfounded word2--> " + "\"" + word2 + "\" " + "with ids: {" + String.Join(",", wordset2) + "}\n");

            var word1_ancestors = new Dictionary<int, List<int>>();
            var word2_ancestors = new Dictionary<int, List<int>>();
            // loop through all ids in a wordset
            foreach (int id in wordset1)
            {
                Console.WriteLine("\nlooping through id: " + id);


                // create a dictionary to store all possible pathes from a node to root

                var word1_path_to_root = new Dictionary<int,List<int>>();
                
                // calling the dfs function to return a list of ancestors for a node1
                
                 word1_ancestors = dfs(id, hypernyms,word1_path_to_root);
                
                foreach(KeyValuePair<int,List<int>> kv in word1_ancestors)
                {
                    Console.WriteLine("\npossible pathes from node1 to root: " + string.Join(" -> ", kv.Value) + "\n");

                }

                // create the output reading file



            }

            // loop through all ids in a wordset2

            foreach (int id in wordset2)
            {
                Console.WriteLine("\nlooping through id: " + id);


                // create a dictionary to store all possible pathes from a node to root

                var word2_path_to_root = new Dictionary<int, List<int>>();

                // calling the dfs function to return a list of ancestors for a node1

                 word2_ancestors = dfs(id, hypernyms, word2_path_to_root);
                

                foreach (KeyValuePair<int, List<int>> kv in word2_ancestors)
                {
                    Console.WriteLine("\npossible pathes from node2 to root: " + string.Join(" -> ", kv.Value) + "\n");

                }

            }

            // by finding all pathes from node 1 and node 2 till the root we need to implement finding the lca function

            // initialize a list to hold the indexes of lowest common ancestors for 2 node
            /* var depth_dic=new Dictionary<int, int>();*/
            var depth = new List<int>();
            var lcas_indexes=new Dictionary<int,List< int >>();

            // loop through all keys in the 2 dictionaries
            
            foreach(KeyValuePair<int,List<int>> kv1 in word1_ancestors)
            {
                var word1_list=kv1.Value;
                foreach(KeyValuePair<int, List<int>> kv2 in word2_ancestors)
                {
                    var word2_list=kv2.Value;
                    var ls=new List<int>();

                    ls = lca(word1_list, word2_list);
                    int index = ls[0];
                    depth.Add(ls[1]);
                    if (!lcas_indexes.ContainsKey(index))
                    {
                        lcas_indexes.Add(index,word1_list);
                        

                    }
                }
            }
            int finalDepth;
            finalDepth=depth.Min(); 

            var lca_index=lcas_indexes.Keys.Min();
            var lca_id = lcas_indexes[lca_index][lca_index];
            Console.WriteLine("lowest common ancestor: " + lca_id);

            Console.WriteLine( wordnet_dic[lca_id][0]);
            Console.WriteLine("depth: "+ finalDepth);
            Console.WriteLine("==================================================================================================");


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

/*     creating a dfs function that takes a node id and the adjacency list (hypernyms dictionary)
     and returns a dictionary of all pathes from the node to the root */

    public static List<int> list=new List<int>();
    public static Dictionary<int, List<int>> dfs(int node, Dictionary<int, List<int>> hypernyms, Dictionary<int, List<int>> ancestors)
    {
        if (hypernyms[node].Count == 0)
        {
            list.Add(node);
            var ls = new List<int>(list);
            ancestors.Add(ancestors.Count, ls);

            list.Clear();
        }
        else
        {
           
            foreach (var adjacent in hypernyms[node])
            {
                list.Add(node);
                dfs(adjacent, hypernyms, ancestors);
                
            }

        }


        return ancestors;


    }

    // creating the lca function
    public static List<int> lca(List<int> word1_list, List<int> word2_list)
    {
        int word1_depth = word1_list.Count-1;
        int word2_depth = word2_list.Count-1;
        int word1_current_index = 0;
        int word2_current_index = 0;
        int depth=0;

        while(word1_depth != word2_depth)
        {
            if (word1_depth > word2_depth)
            {
                word1_current_index++;
                word1_depth--;
                depth++;
            }
            else
            {
                word2_current_index++;
                word2_depth--;
                depth++;
            }
        }

        while (word1_list[word1_current_index] != word2_list[word2_current_index])
        {
            word1_current_index++;
            word2_current_index++;
            depth = depth + 2;
        }
        var ls=new List<int>();
        ls.Add(word1_current_index);
        ls.Add(depth);
        return ls;
    }


}




