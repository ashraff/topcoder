namespace PegJump
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Program
    {
        #region Methods

        public static void SerializeToXML(Board board)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Board));

            using (TextWriter textWriter = new StreamWriter(@"C:\temp\baord.xml"))
            {
                using (var writer = new XmlTextWriter(textWriter))
                {
                    writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                    serializer.WriteObject(writer, board);
                    writer.Flush();

                }
            }
        }

        static Board DeserializeFromXML()
        {
            Board board = null;
            DataContractSerializer serializer = new DataContractSerializer(typeof(Board));
            using (TextReader textReader = new StreamReader(@"C:\temp\baord.xml"))
            {
                using (var reader = new XmlTextReader(textReader))
                {
                    board = (Board)serializer.ReadObject(reader);
                }
            }
            return board;
        }

        static void Main(string[] args)
        {
            int pegTypeCount = Int32.Parse(Console.ReadLine());
            int[] pegType = new int[pegTypeCount];
            for (int i = 0; i < pegTypeCount; i++)
                pegType[i] = Int32.Parse(Console.ReadLine());

            int boardSize = Int32.Parse(Console.ReadLine());
            string[] board = new string[boardSize];

            for (int i = 0; i < boardSize; i++)
            {
                board[i] = Console.ReadLine();
                //Console.Error.WriteLine(board[i]);
            }
            PegJumping pj = new PegJumping();
            string[] moves = pj.getMoves(pegType, board);
            Console.WriteLine(moves != null ? moves.Length : 0);
            for (int i = 0; i < moves.Length; i++)
                Console.WriteLine(moves[i]);

            /* Board b = DeserializeFromXML();

             PegJumping pj = new PegJumping();
             string[] moves = pj.findBestMovesv5(b, b.RawBoard);
             if (moves != null)
             {
             foreach (string s in moves)
                 Console.WriteLine(s);
             }*/
        }

        #endregion Methods

        #region Other

        /*
         *
         *
        public static void Main(string[] args)
        {
        PathDefense program = new PathDefense();
        program.MAX_LENGTH_TO_PLACE_TOWER = Int32.Parse(args[0]);
        program.PERIMETER_TO_COVER_BASE = Int32.Parse(args[1]);
        program.writer = new LogWriter("Log Init..");
        int N = Int32.Parse(Console.ReadLine());
        int money = Int32.Parse(Console.ReadLine());
        string[] board = new string[N];
        for (int i = 0; i < N; i++)
            board[i] = Console.ReadLine();
        int creepHealth = Int32.Parse(Console.ReadLine());
        int creepMoney = Int32.Parse(Console.ReadLine());
        int NT = Int32.Parse(Console.ReadLine());
        int[] towerType = new int[NT];
        for (int i = 0; i < NT; i++)
            towerType[i] = Int32.Parse(Console.ReadLine());

        program.init(board, money, creepHealth, creepMoney, towerType);
        for (int t = 0; t < 2000; t++)
        {
            money = Int32.Parse(Console.ReadLine());
            NT = Int32.Parse(Console.ReadLine());
            int[] creep = new int[NT];
            for (int i = 0; i < NT; i++)
                creep[i] = Int32.Parse(Console.ReadLine());
            N = Int32.Parse(Console.ReadLine());
            int[] baseHealth = new int[N];
            for (int i = 0; i < N; i++)
                baseHealth[i] = Int32.Parse(Console.ReadLine());

            int[] ret = program.placeTowers(creep, money, baseHealth);
            Console.WriteLine(ret.Length);
            for (int i = 0; i < ret.Length; i++)
                Console.WriteLine(ret[i]);

        }
        }
         * */

        #endregion Other
    }
}