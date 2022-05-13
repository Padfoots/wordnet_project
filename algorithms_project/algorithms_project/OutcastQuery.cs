
    internal class OutcastQuery
{
            // initialize function read outcast queries
    public static List<List<string>> read_outcast_queries(string file_name)
        {
            var reader = new StreamReader(file_name);
        // the first line contains the number of queries
       var n_outcastQueries=reader.ReadLine();
        
            List<List<string>> queries= new List<List<string>>();
            while (!reader.EndOfStream)
            {
            List<string> query = new List<string>();
                var line = reader.ReadLine();
                var words = line.Split(',');
                foreach(string word in words)
                {
                    query.Add(word);

                }
                //List<string> ls=new List<string>();
                queries.Add(query);
                

    
            }

         reader.Close();

        // printing the outcast queries
        foreach(var q in queries)
        {
            Console.WriteLine(string.Join(',',q));
        }

         return queries;
        } 
    public static Dictionary<string,List<int>> find_ids(Dictionary<int,List<string>> wordnet , List<string> query)
    {
        Dictionary<string,List<int>> query_ids=new Dictionary<string,List<int>>();
        foreach(KeyValuePair<int,List<string>> kv in wordnet)
        {
            foreach(string word in query)
            {
                if (kv.Value.Contains(word))
                {
                    if (!query_ids.ContainsKey(word))
                    {   
                        List<int> ids = new List<int>();
                        ids.Add(kv.Key);

                        query_ids.Add(word, ids);
                    }
                    else
                    {
                        query_ids[word].Add(kv.Key);
                    }

                }
            }
        }
        // printing founded ids
        foreach(KeyValuePair<string,List<int>> kv in query_ids)
        {
            Console.WriteLine("key: " + kv.Key + "\t\t\t values: " + String.Join(',', kv.Value));

        }
        return query_ids;
    }



            
    
}

