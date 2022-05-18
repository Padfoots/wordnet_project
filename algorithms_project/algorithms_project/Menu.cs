
    internal class Menu
    {
    // creating a menu of test cases
    static string tc;
    public static string create_menu()
    {


        Console.WriteLine("enter a testcase number:\n " +
                "1-Small\\Case1_100_100 \n" +
                "2-Small\\Case2_1000_500 \n" +
                "3-Medium\\Case1_10000_5000 \n" +
                "4-Medium\\Case2_10000_50000 \n" +
                "5-Large\\Case1_82K_100K_5000RQ \n" +
                "6-Large\\Case2_82K_300K_1500RQ\n" +
                "7-Large\\Case3_82K_300K_5000RQ\n" +
                "a-Sample\\Case1\n" +
                "b-Sample\\Case2\n" +
                "c-Sample\\Case3\n" +
                "d-Sample\\Case4\n" +
                "e-Sample\\Other special cases\\2 commons case (Bidirectional)\n" +
                "f-Sample\\Other special cases\\Many-Many (Noun in more than 1 synset)\n");

        char choice = (char)Console.ReadLine()[0];
        switch (choice)
        {
            case '1': tc = dir.c_s1; break;
            case '2': tc = dir.c_s2; break;
            case '3': tc = dir.c_m1; break;
            case '4': tc = dir.c_m2; break;
            case '5': tc = dir.c_l1; break;
            case '6': tc = dir.c_l2; break;
            case '7': tc = dir.c_l3; break;
            case 'a': tc = dir.s_1; break;
            case 'b': tc = dir.s_2; break;
            case 'c': tc = dir.s_3; break;
            case 'd': tc = dir.s_4; break;
            case 'e': tc = dir.s_5; break;
            case 'f': tc = dir.s_6; break;


        }
        return tc;
    }
    }

