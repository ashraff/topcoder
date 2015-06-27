using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

[DataContract]
public class Board
{
    #region Fields

    [DataMember]
    public Dictionary<int, int> PegCountByType = new Dictionary<int, int>();
    [DataMember]
    public Dictionary<int, int> PegType = new Dictionary<int, int>();

    #endregion Fields

    #region Properties

    [DataMember]
    public List<Move> AllMoves
    {
        get;
        set;
    }

    [DataMember]
    public List<Peg> AvailablePegs
    {
        get;
        set;
    }

    [DataMember]
    public List<Point> AvailableSpace
    {
        get;
        set;
    }

    [DataMember]
    public int BoardSize
    {
        get;
        set;
    }

    [DataMember]
    public int MaxPegValue
    {
        get;
        set;
    }

    [DataMember]
    public int MinPegValue
    {
        get;
        set;
    }

    [DataMember]
    public string[] RawBoard
    {
        get;
        set;
    }

    [DataMember]
    public int[] RawPegValue
    {
        get;
        set;
    }

    #endregion Properties
}

public class Move
{
    #region Properties

    public Point Location
    {
        get;
        set;
    }

    public string Movement
    {
        get;
        set;
    }

    #endregion Properties
}

public class Peg
{
    #region Properties

    public int Id
    {
        get;
        set;
    }

    public char IdAsChar
    {
        get;
        set;
    }

    public Point Location
    {
        get;
        set;
    }

    public bool RightCandidateForMove
    {
        get;
        set;
    }

    public int Value
    {
        get;
        set;
    }

    #endregion Properties
}

public class PegJumping
{
    #region Methods

    public static ProfitableMovev2 ProfitableMovev4(Board b, string[] board)
    {
        ProfitableMovev2 profitableMove = null;
        Peg p;
        List<Peg> pegsToMove = b.AvailablePegs.Where(a => a.RightCandidateForMove == true).OrderBy(a => a.Value).ToList<Peg>();
        for (int i = 0; i < pegsToMove.Count; i++)
        {
            p = pegsToMove[i];
            if (board[p.Location.Row][p.Location.Col] != '.')
            {
                bool movePossible = false;
                string movement = string.Empty;
                int newRow = p.Location.Row, newCol = p.Location.Col;
                int totalValue = 0;
                List<string> prevLoca = new List<string>();
                prevLoca.Add(newRow + ":" + newCol);
                do
                {

                    movePossible = false;
                    
                    int uCount = OrphanedPeg(b, board, newRow - 2, newCol);
                    int dCount = OrphanedPeg(b, board, newRow + 2, newCol);
                    int lCount = OrphanedPeg(b, board, newRow, newCol-2);
                    int rCount = OrphanedPeg(b, board, newRow, newCol+2);

                    int min = Math.Min(uCount, Math.Min(dCount, Math.Min(lCount, rCount)));

                    if (min == uCount && isUpGood(board, newRow, newCol) && !prevLoca.Contains((newRow - 2) + ":" + newCol))
                    {
                        totalValue += b.PegType[Int32.Parse("" + board[newRow - 1][newCol])];
                        movePossible = true;
                        movement += "U";
                        newRow = newRow - 2;
                        prevLoca.Add(newRow + ":" + newCol);

                    }
                    else if (min == dCount && isDownGood(board, newRow, newCol) && !prevLoca.Contains((newRow + 2) + ":" + newCol))
                    {
                        totalValue += b.PegType[Int32.Parse("" + board[newRow + 1][newCol])];
                        movePossible = true;
                        movement += "D";
                        newRow = newRow + 2;
                        prevLoca.Add(newRow + ":" + newCol);

                    }
                    else if (min == lCount && isLeftGood(board, newRow, newCol) && !prevLoca.Contains(newRow + ":" + (newCol - 2)))
                    {
                        totalValue += b.PegType[Int32.Parse("" + board[newRow][newCol - 1])];
                        movePossible = true;
                        movement += "L";
                        newCol = newCol - 2;
                        prevLoca.Add(newRow + ":" + newCol);
                    }
                    else if (min == rCount && isRightGood(board, newRow, newCol) && !prevLoca.Contains(newRow + ":" + (newCol + 2)))
                    {
                        totalValue += b.PegType[Int32.Parse("" + board[newRow][newCol + 1])];
                        movePossible = true;
                        movement += "R";
                        newCol = newCol + 2;
                        prevLoca.Add(newRow + ":" + newCol);
                    }

                } while (movePossible);

                if (!string.IsNullOrEmpty(movement))
                {
                    totalValue = movement.Length * totalValue;
                    if (profitableMove == null)
                    {
                        profitableMove = new ProfitableMovev2(movement, p);
                        profitableMove.Value = totalValue;

                    }
                    else if (profitableMove.Value < totalValue)
                    {
                        profitableMove = new ProfitableMovev2(movement, p);
                        profitableMove.Value = totalValue;
                    }
                }
            }

        }

        return profitableMove;
    }

    public string[] findBestMovesv5(Board b, string[] board)
    {
        Stopwatch w = new Stopwatch();
        w.Start();
        ProfitableMovev2 goodMove = null;
        do
        {
            goodMove = ProfitableMovev4(b, board);
            if (goodMove != null)
            {
                Move m = new Move();
                m.Location = new Point(goodMove.Peg.Location.Row, goodMove.Peg.Location.Col);
                m.Movement = goodMove.Move;
                int newRow = goodMove.Peg.Location.Row, newCol = goodMove.Peg.Location.Col;
                for (int i = 0; i < goodMove.Move.Length; i++)
                {

                    if (goodMove.Move[i] == 'R')
                    {
                        board[newRow] = ReplaceAt(board[newRow], newCol, '.');
                        board[newRow] = ReplaceAt(board[newRow], newCol + 1, '.');
                        board[newRow] = ReplaceAt(board[newRow], newCol + 2, goodMove.Peg.IdAsChar);
                        newCol = newCol + 2;

                    }
                    /** LEFT **/
                    else if (goodMove.Move[i] == 'L')
                    {
                        board[newRow] = ReplaceAt(board[newRow], newCol, '.');
                        board[newRow] = ReplaceAt(board[newRow], newCol - 1, '.');
                        board[newRow] = ReplaceAt(board[newRow], newCol - 2, goodMove.Peg.IdAsChar);
                        newCol = newCol - 2;

                    }
                    /** UP **/
                    else if (goodMove.Move[i] == 'D')
                    {
                        board[newRow] = ReplaceAt(board[newRow], newCol, '.');
                        board[newRow + 1] = ReplaceAt(board[newRow + 1], newCol, '.');
                        board[newRow + 2] = ReplaceAt(board[newRow + 2], newCol, goodMove.Peg.IdAsChar);
                        newRow = newRow + 2;

                    }
                    /** DOWN **/
                    else if (goodMove.Move[i] == 'U')
                    {
                        board[newRow] = ReplaceAt(board[newRow], newCol, '.');
                        board[newRow - 1] = ReplaceAt(board[newRow - 1], newCol, '.');
                        board[newRow - 2] = ReplaceAt(board[newRow - 2], newCol, goodMove.Peg.IdAsChar);
                        newRow = newRow - 2;

                    }
                }

                b.AllMoves.Add(m);
            }
            UpdateAvailablePegs(b, board);
        }
        while (goodMove != null);
        Console.Error.WriteLine("time taken is " + w.ElapsedMilliseconds);
        //for (int i = 0; i < board.Length; i++)
        //  Console.Error.WriteLine(board[i]);
        return ConvertMovesToStringArray(b);
    }

    public string[] getMoves(int[] pegValue, string[] board)
    {
        Board b = new Board();
        b.BoardSize = board.Length;
        b.RawBoard = board;
        b.RawPegValue = pegValue;
        b.AvailablePegs = new List<Peg>();
        b.AvailableSpace = new List<Point>();
        b.AllMoves = new List<Move>();
        for (int i = 0; i < pegValue.Length; i++)
        {
            b.PegType[i] = pegValue[i];
        }

        b.MaxPegValue = b.PegType.Values.Max();
        b.MinPegValue = b.PegType.Values.Min();

        for (int i = 0; i < board.Length; i++)
        {
            for (int j = 0; j < board[i].Length; j++)
            {
                if (board[i][j] == '.')
                {
                    Point availableSpace = new Point();
                    availableSpace.Row = i;
                    availableSpace.Col = j;
                    b.AvailableSpace.Add(availableSpace);

                }
                else
                {
                    Point pegLoc = new Point();
                    pegLoc.Row = i;
                    pegLoc.Col = j;

                    Peg peg = new Peg();
                    peg.Id = Int32.Parse(board[i][j] + "");
                    peg.IdAsChar = board[i][j]; // To Avoid Conversion in Loops
                    peg.Value = pegValue[peg.Id];
                    peg.Location = pegLoc;
                    //b.PegCountByType[Move.Id] += 1;
                    if ((j + 1 < b.BoardSize && j + 2 < b.BoardSize && board[i][j + 1] != '.' && board[i][j + 2] == '.') || (j - 1 >= 0 && j - 2 >= 0 && board[i][j - 1] != '.' && board[i][j - 2] == '.') || (i + 1 < b.BoardSize && i + 2 < b.BoardSize && board[i + 1][j] != '.' && board[i + 2][j] == '.') || (i - 1 >= 0 && i - 2 >= 0 && board[i - 1][j] != '.' && board[i - 2][j] == '.'))
                    {
                        peg.RightCandidateForMove = true;
                    }
                    b.AvailablePegs.Add(peg);
                }
            }
        }

        return findBestMovesv5(b, board);
    }

    private static string[] ConvertMovesToStringArray(Board b)
    {
        string[] output = new string[b.AllMoves.Count];
        for (int i = 0; i < b.AllMoves.Count; i++)
        {
            output[i] = b.AllMoves[i].Location.Row + " " + b.AllMoves[i].Location.Col + " " + b.AllMoves[i].Movement;
            //Console.Error.WriteLine(output[i]);

        }
        return output;
    }

    private static void UpdateAvailablePegs(Board b, string[] board)
    {
        b.AvailablePegs = new List<Peg>();
        for (int i = 0; i < board.Length; i++)
        {
            for (int j = 0; j < board[i].Length; j++)
            {
                if (board[i][j] != '.')
                {
                    Point pegLoc = new Point();
                    pegLoc.Row = i;
                    pegLoc.Col = j;

                    Peg peg = new Peg();
                    peg.Id = Int32.Parse(board[i][j] + "");
                    peg.IdAsChar = board[i][j]; // To Avoid Conversion in Loops
                    peg.Location = pegLoc;
                    peg.Value = b.PegType[peg.Id];
                    if ((j + 1 < b.BoardSize && j + 2 < b.BoardSize && board[i][j + 1] != '.' && board[i][j + 2] == '.') || (j - 1 >= 0 && j - 2 >= 0 && board[i][j - 1] != '.' && board[i][j - 2] == '.') || (i + 1 < b.BoardSize && i + 2 < b.BoardSize && board[i + 1][j] != '.' && board[i + 2][j] == '.') || (i - 1 >= 0 && i - 2 >= 0 && board[i - 1][j] != '.' && board[i - 2][j] == '.'))
                    {
                        peg.RightCandidateForMove = true;
                    }
                    b.AvailablePegs.Add(peg);
                }
            }
        }
    }

    private static bool isDownGood(Board b, string[] board, Peg p)
    {
        return (p.Location.Row + 1 < b.BoardSize && p.Location.Row + 2 < b.BoardSize && board[p.Location.Row + 1][p.Location.Col] != '.' && board[p.Location.Row + 2][p.Location.Col] == '.');
    }

    private static bool isDownGood(string[] board, int r, int c)
    {
        return (r + 1 < board.Length && r + 2 < board.Length && board[r + 1][c] != '.' && board[r + 2][c] == '.');
    }

    private static bool isLeftGood(string[] board, Peg p)
    {
        return (p.Location.Col - 1 >= 0 && p.Location.Col - 2 >= 0 && board[p.Location.Row][p.Location.Col - 1] != '.' && board[p.Location.Row][p.Location.Col - 2] == '.');
    }

    private static bool isLeftGood(string[] board, int r, int c)
    {
        return (c - 1 >= 0 && c - 2 >= 0 && board[r][c - 1] != '.' && board[r][c - 2] == '.');
    }

    private static bool isRightGood(Board b, string[] board, Peg p)
    {
        return (p.Location.Col + 1 < b.BoardSize && p.Location.Col + 2 < b.BoardSize && board[p.Location.Row][p.Location.Col + 1] != '.' && board[p.Location.Row][p.Location.Col + 2] == '.');
    }

    private static bool isRightGood(string[] board, int r, int c)
    {
        return (c + 1 < board.Length && c + 2 < board.Length && board[r][c + 1] != '.' && board[r][c + 2] == '.');
    }

    private static bool isUpGood(string[] board, Peg p)
    {
        return (p.Location.Row - 1 >= 0 && p.Location.Row - 2 >= 0 && board[p.Location.Row - 1][p.Location.Col] != '.' && board[p.Location.Row - 2][p.Location.Col] == '.');
    }

    private static bool isUpGood(string[] board, int r, int c)
    {
        return (r - 1 >= 0 && r - 2 >= 0 && board[r - 1][c] != '.' && board[r - 2][c] == '.');
    }

    private static int OrphanedPeg(Board b, string[] board, int newRow, int newCol)
    {
        int empty = 0;
        if (newRow + 1 < b.BoardSize && board[newRow + 1][newCol] == '.')
        {
            empty++;
        }

        if (newRow - 1 >= 0 && board[newRow - 1][newCol] == '.')
        {
            empty++;
        }

        if (newCol + 1 < b.BoardSize && board[newRow][newCol + 1] == '.')
        {
            empty++;
        }

        if (newCol - 1 >= 0 && board[newRow][newCol - 1] == '.')
        {
            empty++;
        }
        return empty;
    }
    
    #region Other_Tries
    private static ProfitableMove ProfitableMove(Board b, string[] board, Peg p)
    {
        int rightCount = 1, leftCount = 1, upCount = 1, downCount = 1;
        bool cRP = true, cLP = true, cUP = true, cDP = true;
        int rightVal = 0, leftVal = 0, upVal = 0, downVal = 0;
        int rightMinCount = Int32.MaxValue, leftMinCount = Int32.MaxValue, upMinCount = Int32.MaxValue, downMinCount = Int32.MaxValue;
        for (int i = 1; i < b.BoardSize; i += 2)
        {

            if (cRP && p.Location.Col + i < board.Length && board[p.Location.Row][p.Location.Col + i] != '.' && p.Location.Col + i + 1 < board.Length && board[p.Location.Row][p.Location.Col + i + 1] == '.')
            {
                rightVal += b.PegType[Int32.Parse("" + board[p.Location.Row][p.Location.Col + i])];
                rightMinCount = OrphanedPeg(b, board, p.Location.Row, p.Location.Col + i + 1);

                rightCount++;
            }
            else cRP = false;

            if (cLP && p.Location.Col - i >= 0 && board[p.Location.Row][p.Location.Col - i] != '.' && p.Location.Col - i - 1 >= 0 && board[p.Location.Row][p.Location.Col - i - 1] == '.')
            {
                leftVal += b.PegType[Int32.Parse("" + board[p.Location.Row][p.Location.Col - i])];
                leftMinCount = OrphanedPeg(b, board, p.Location.Row, p.Location.Col - i - 1);

                leftCount++;
            }
            else cLP = false;

            if (cUP && (p.Location.Row - i >= 0 && p.Location.Row - i - 1 >= 0 && board[p.Location.Row - i][p.Location.Col] != '.' && board[p.Location.Row - i - 1][p.Location.Col] == '.'))
            {
                upVal += b.PegType[Int32.Parse("" + board[p.Location.Row - i][p.Location.Col])];
                upMinCount = OrphanedPeg(b, board, p.Location.Row - i - 1, p.Location.Col);

                upCount++;
            }
            else cUP = false;

            if (cDP && (p.Location.Row + i < board.Length && p.Location.Row + i + 1 < board.Length && board[p.Location.Row + i][p.Location.Col] != '.' && board[p.Location.Row + i + 1][p.Location.Col] == '.'))
            {
                downVal += b.PegType[Int32.Parse("" + board[p.Location.Row + i][p.Location.Col])];
                downMinCount = OrphanedPeg(b, board, p.Location.Row + i + 1, p.Location.Col);

                downCount++;
            }
            else cDP = false;
        }

        //int max = Math.Max(rightVal, Math.Max(leftVal, Math.Max(upVal, downVal)));
        int min = Math.Min(rightMinCount, Math.Min(leftMinCount, Math.Min(upMinCount, downMinCount)));

        /*if (max == rightVal) return new ProfitableMove('R', rightCount, rightVal);
        if (max == leftVal) return new ProfitableMove('L', leftCount, leftVal);
        if (max == upVal) return new ProfitableMove('U', upCount, upVal);
        if (max == downVal) return new ProfitableMove('D', downCount, downVal);*/

        if (min == rightMinCount) return new ProfitableMove('R', rightCount, rightVal);
        if (min == leftMinCount) return new ProfitableMove('L', leftCount, leftVal);
        if (min == upMinCount) return new ProfitableMove('U', upCount, upVal);
        if (min == downMinCount) return new ProfitableMove('D', downCount, downVal);

        if (isRightGood(b, board, p)) return new ProfitableMove('R', 1, 1);
        if (isLeftGood(board, p)) return new ProfitableMove('L', 1, 1);
        if (isUpGood(board, p)) return new ProfitableMove('U', 1, 1);
        if (isDownGood(b, board, p)) return new ProfitableMove('D', 1, 1);
        return new ProfitableMove('0', -1, 1);
    }

    private static ProfitableMove ProfitableMovev2(Board b, string[] board, Peg p)
    {
        int rightMinCount = Int32.MaxValue, leftMinCount = Int32.MaxValue, upMinCount = Int32.MaxValue, downMinCount = Int32.MaxValue;
        if (isRightGood(b, board, p))
        {
            rightMinCount = OrphanedPeg(b, board, p.Location.Row, p.Location.Col + 2);
        }

        if (isLeftGood(board, p))
        {
            leftMinCount = OrphanedPeg(b, board, p.Location.Row, p.Location.Col - 2);
        }

        if (isUpGood(board, p))
        {
            upMinCount = OrphanedPeg(b, board, p.Location.Row - 2, p.Location.Col);
        }

        if (isDownGood(b, board, p))
        {
            downMinCount = OrphanedPeg(b, board, p.Location.Row + 2, p.Location.Col);
        }

        int min = Math.Min(rightMinCount, Math.Min(leftMinCount, Math.Min(upMinCount, downMinCount)));
        if (min == rightMinCount) return new ProfitableMove('R', min, 1);
        if (min == leftMinCount) return new ProfitableMove('L', min, 1);
        if (min == upMinCount) return new ProfitableMove('U', min, 1);
        if (min == downMinCount) return new ProfitableMove('D', min, 1);

        if (isRightGood(b, board, p)) return new ProfitableMove('R', 1, 1);
        if (isLeftGood(board, p)) return new ProfitableMove('L', 1, 1);
        if (isUpGood(board, p)) return new ProfitableMove('U', 1, 1);
        if (isDownGood(b, board, p)) return new ProfitableMove('D', 1, 1);
        return new ProfitableMove('X', -1, 1);
    }

    private static ProfitableMove ProfitableMovev3(Board b, string[] board, Peg p)
    {
        int rightMinCount = Int32.MaxValue, leftMinCount = Int32.MaxValue, upMinCount = Int32.MaxValue, downMinCount = Int32.MaxValue;
        if (OrphanedPeg(b, board, p.Location.Row, p.Location.Col) == 4) //Its orphaned peg,cannot be removed
        {
            return new ProfitableMove('X', -1, 1);
        }
        //Move Left
        if (p.Location.Col + 1 < board.Length && board[p.Location.Row][p.Location.Col + 1] != '.' && p.Location.Col - 1 >= 0 && board[p.Location.Row][p.Location.Col - 1] == '.')
        {
            leftMinCount = OrphanedPeg(b, board, p.Location.Row, p.Location.Col + 1);
        }
        //Move Right
        if (p.Location.Col + 1 < board.Length && board[p.Location.Row][p.Location.Col + 1] == '.' && p.Location.Col - 1 >= 0 && board[p.Location.Row][p.Location.Col - 1] != '.')
        {
            rightMinCount = OrphanedPeg(b, board, p.Location.Row, p.Location.Col - 1);
        }
        //Move Up
        if (p.Location.Row + 1 < board.Length && board[p.Location.Row + 1][p.Location.Col] != '.' && p.Location.Row - 1 >= 0 && board[p.Location.Row - 1][p.Location.Col] == '.')
        {
            upMinCount = OrphanedPeg(b, board, p.Location.Row + 1, p.Location.Col);
        }

        //Move Down
        if (p.Location.Row + 1 < board.Length && board[p.Location.Row + 1][p.Location.Col] == '.' && p.Location.Row - 1 >= 0 && board[p.Location.Row - 1][p.Location.Col] != '.')
        {
            downMinCount = OrphanedPeg(b, board, p.Location.Row - 1, p.Location.Col);
        }

        int min = Math.Min(rightMinCount, Math.Min(leftMinCount, Math.Min(upMinCount, downMinCount)));
        if (min != Int32.MaxValue && min == rightMinCount) return new ProfitableMove('R', min, 1);
        if (min != Int32.MaxValue && min == leftMinCount) return new ProfitableMove('L', min, 1);
        if (min != Int32.MaxValue && min == upMinCount) return new ProfitableMove('U', min, 1);
        if (min != Int32.MaxValue && min == downMinCount) return new ProfitableMove('D', min, 1);

        return new ProfitableMove('X', -1, 1);
    }

    private string[] findBestMoves(Board b, string[] board)
    {
        List<Peg> pegsToMove = null;
        bool anyMoveMade = false;
        Stopwatch watch = new Stopwatch();
        watch.Start();
        do
        {
            pegsToMove = b.AvailablePegs.Where(a => a.RightCandidateForMove == true).ToList<Peg>();
            anyMoveMade = false;
            b.PegType = b.PegType.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach (int key in b.PegType.Keys)
            {
                for (int i = 0; i < pegsToMove.Count; i++)
                {

                    Peg p = pegsToMove[i];
                    Move m = new Move();
                    m.Location = p.Location;
                    if (board[p.Location.Row][p.Location.Col] == p.IdAsChar) // PEG STILL AVAILABLE
                    {

                        ProfitableMove move = ProfitableMove(b, board, p);
                        for (int L = 0; L < move.MoveVal - 1; L++)
                        {
                            /** RIGHT **/
                            if (move.Move == 'R')
                            {
                                board[p.Location.Row] = ReplaceAt(board[p.Location.Row], p.Location.Col + L * 2, '.');
                                board[p.Location.Row] = ReplaceAt(board[p.Location.Row], p.Location.Col + L * 2 + 1, '.');
                                board[p.Location.Row] = ReplaceAt(board[p.Location.Row], p.Location.Col + L * 2 + 2, p.IdAsChar);

                                m.Movement += "R";
                                anyMoveMade = true;
                            }
                            /** LEFT **/
                            else if (move.Move == 'L')
                            {
                                board[p.Location.Row] = ReplaceAt(board[p.Location.Row], p.Location.Col - L * 2, '.');
                                board[p.Location.Row] = ReplaceAt(board[p.Location.Row], p.Location.Col - 1 - L * 2, '.');
                                board[p.Location.Row] = ReplaceAt(board[p.Location.Row], p.Location.Col - 2 - L * 2, p.IdAsChar);
                                m.Movement += "L";
                                anyMoveMade = true;
                            }
                            /** UP **/
                            else if (move.Move == 'D')
                            {
                                board[p.Location.Row + L * 2] = ReplaceAt(board[p.Location.Row + L * 2], p.Location.Col, '.');
                                board[p.Location.Row + 1 + L * 2] = ReplaceAt(board[p.Location.Row + 1 + L * 2], p.Location.Col, '.');
                                board[p.Location.Row + 2 + L * 2] = ReplaceAt(board[p.Location.Row + 2 + L * 2], p.Location.Col, p.IdAsChar);
                                m.Movement += "D";
                                anyMoveMade = true;
                            }
                            /** DOWN **/
                            else if (move.Move == 'U')
                            {
                                board[p.Location.Row - L * 2] = ReplaceAt(board[p.Location.Row - L * 2], p.Location.Col, '.');
                                board[p.Location.Row - 1 - L * 2] = ReplaceAt(board[p.Location.Row - 1 - L * 2], p.Location.Col, '.');
                                board[p.Location.Row - 2 - L * 2] = ReplaceAt(board[p.Location.Row - 2 - L * 2], p.Location.Col, p.IdAsChar);
                                m.Movement += "U";
                                anyMoveMade = true;
                            }

                        }

                        if (!string.IsNullOrEmpty(m.Movement))
                        {
                            b.AllMoves.Add(m);
                        }
                    }

                }
            }
            UpdateAvailablePegs(b, board);

            if (watch.ElapsedMilliseconds >= 14000)
            {
                break;
            }

        }
        while (pegsToMove.Count > 0 && anyMoveMade);
        return ConvertMovesToStringArray(b);
    }

    private string[] findBestMovesv2(Board b, string[] board)
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();
        bool anyMoveMade = false;
        int totalPegs = b.AvailablePegs.Count;
        do
        {
            anyMoveMade = false;
            ProfitableMove goodMove = new ProfitableMove('X', -1, 1);
            Peg profitablePeg = null;
            List<Peg> pegsToMove = b.AvailablePegs.Where(a => a.RightCandidateForMove == true).ToList<Peg>();
            foreach (Peg peg in pegsToMove)
            {
                ProfitableMove currentMove = ProfitableMove(b, board, peg);
                if (goodMove.Move == 'X' || goodMove.MoveVal < currentMove.MoveVal)
                {
                    goodMove = currentMove;
                    profitablePeg = peg;
                }
            }
            if (profitablePeg != null)
            {
                Move m = new Move();
                m.Location = profitablePeg.Location;
                for (int L = 0; L < goodMove.MoveCount - 1; L++)
                {
                    /** RIGHT **/
                    if (goodMove.Move == 'R')
                    {
                        board[profitablePeg.Location.Row] = ReplaceAt(board[profitablePeg.Location.Row], profitablePeg.Location.Col + L * 2, '.');
                        board[profitablePeg.Location.Row] = ReplaceAt(board[profitablePeg.Location.Row], profitablePeg.Location.Col + L * 2 + 1, '.');
                        board[profitablePeg.Location.Row] = ReplaceAt(board[profitablePeg.Location.Row], profitablePeg.Location.Col + L * 2 + 2, profitablePeg.IdAsChar);

                        m.Movement += "R";
                        anyMoveMade = true;
                    }
                    /** LEFT **/
                    else if (goodMove.Move == 'L')
                    {
                        board[profitablePeg.Location.Row] = ReplaceAt(board[profitablePeg.Location.Row], profitablePeg.Location.Col - L * 2, '.');
                        board[profitablePeg.Location.Row] = ReplaceAt(board[profitablePeg.Location.Row], profitablePeg.Location.Col - 1 - L * 2, '.');
                        board[profitablePeg.Location.Row] = ReplaceAt(board[profitablePeg.Location.Row], profitablePeg.Location.Col - 2 - L * 2, profitablePeg.IdAsChar);
                        m.Movement += "L";
                        anyMoveMade = true;
                    }
                    /** UP **/
                    else if (goodMove.Move == 'D')
                    {
                        board[profitablePeg.Location.Row + L * 2] = ReplaceAt(board[profitablePeg.Location.Row + L * 2], profitablePeg.Location.Col, '.');
                        board[profitablePeg.Location.Row + 1 + L * 2] = ReplaceAt(board[profitablePeg.Location.Row + 1 + L * 2], profitablePeg.Location.Col, '.');
                        board[profitablePeg.Location.Row + 2 + L * 2] = ReplaceAt(board[profitablePeg.Location.Row + 2 + L * 2], profitablePeg.Location.Col, profitablePeg.IdAsChar);
                        m.Movement += "D";
                        anyMoveMade = true;
                    }
                    /** DOWN **/
                    else if (goodMove.Move == 'U')
                    {
                        board[profitablePeg.Location.Row - L * 2] = ReplaceAt(board[profitablePeg.Location.Row - L * 2], profitablePeg.Location.Col, '.');
                        board[profitablePeg.Location.Row - 1 - L * 2] = ReplaceAt(board[profitablePeg.Location.Row - 1 - L * 2], profitablePeg.Location.Col, '.');
                        board[profitablePeg.Location.Row - 2 - L * 2] = ReplaceAt(board[profitablePeg.Location.Row - 2 - L * 2], profitablePeg.Location.Col, profitablePeg.IdAsChar);
                        m.Movement += "U";
                        anyMoveMade = true;
                    }

                }

                if (!string.IsNullOrEmpty(m.Movement))
                {
                    b.AllMoves.Add(m);
                }
                UpdateAvailablePegs(b, board);
            }
            if (watch.ElapsedMilliseconds >= 14000)
            {
                Console.Error.WriteLine(string.Format("Board of Size {0} with Peg Count {1} caused the timeout issue", b.BoardSize, totalPegs));
                break;

            }

        } while (anyMoveMade);
        Console.Error.WriteLine(string.Format("{0} Pegs outlived of total {1} Pegs.", b.AvailablePegs.Count, totalPegs));
        return ConvertMovesToStringArray(b);
    }

    private string[] findBestMovesv3(Board b, string[] board)
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();
        bool anyMoveMade = false;
        int totalPegs = b.AvailablePegs.Count;

        do
        {
            anyMoveMade = false;
            ProfitableMove goodMove = new ProfitableMove('X', -1, 1);

            List<Peg> pegsToMove = b.AvailablePegs.Where(a => a.RightCandidateForMove == true).OrderBy(a => a.Value).ToList<Peg>();
            if (pegsToMove.Count > 0)
            {
                Peg peg = pegsToMove[0];
                goodMove = ProfitableMovev2(b, board, peg);
                Move m = new Move();
                m.Location = peg.Location;
                int L = 0;
                /** RIGHT **/
                if (goodMove.Move == 'R')
                {
                    board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col, '.');
                    board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col + 1, '.');
                    board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col + 2, peg.IdAsChar);

                    m.Movement += "R";
                    anyMoveMade = true;
                }
                /** LEFT **/
                else if (goodMove.Move == 'L')
                {
                    board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col, '.');
                    board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col - 1, '.');
                    board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col - 2, peg.IdAsChar);
                    m.Movement += "L";
                    anyMoveMade = true;
                }
                /** UP **/
                else if (goodMove.Move == 'D')
                {
                    board[peg.Location.Row + L * 2] = ReplaceAt(board[peg.Location.Row], peg.Location.Col, '.');
                    board[peg.Location.Row + 1 + L * 2] = ReplaceAt(board[peg.Location.Row + 1], peg.Location.Col, '.');
                    board[peg.Location.Row + 2 + L * 2] = ReplaceAt(board[peg.Location.Row + 2], peg.Location.Col, peg.IdAsChar);
                    m.Movement += "D";
                    anyMoveMade = true;
                }
                /** DOWN **/
                else if (goodMove.Move == 'U')
                {
                    board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col, '.');
                    board[peg.Location.Row - 1] = ReplaceAt(board[peg.Location.Row - 1], peg.Location.Col, '.');
                    board[peg.Location.Row - 2] = ReplaceAt(board[peg.Location.Row - 2], peg.Location.Col, peg.IdAsChar);
                    m.Movement += "U";
                    anyMoveMade = true;
                }

                if (!string.IsNullOrEmpty(m.Movement))
                {
                    b.AllMoves.Add(m);
                }

            }

            if (watch.ElapsedMilliseconds >= 140000000)
            {
                Console.Error.WriteLine(string.Format("Board of Size {0} with Peg Count {1} caused the timeout issue", b.BoardSize, totalPegs));
                break;

            }

            UpdateAvailablePegs(b, board);

        } while (anyMoveMade);
        Console.Error.WriteLine(string.Format("{0} Pegs outlived of total {1} Pegs.", b.AvailablePegs.Count, totalPegs));
        return ConvertMovesToStringArray(b);
    }

    private string[] findBestMovesv4(Board b, string[] board)
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();
        bool anyMoveMade = false;
        int totalPegs = b.AvailablePegs.Count;

        do
        {
            anyMoveMade = false;
            ProfitableMove goodMove = new ProfitableMove('X', -1, 1);

            List<Peg> pegsToMove = b.AvailablePegs.OrderByDescending(a => a.Value).ToList<Peg>();
            for (int i = 0; i < pegsToMove.Count; i++)
            {
                Peg peg = pegsToMove[i];
                if (board[peg.Location.Row][peg.Location.Col] != '.')
                {
                    goodMove = ProfitableMovev3(b, board, peg);
                    Move m = new Move();
                    m.Location = new Point();
                    /** RIGHT **/
                    if (goodMove.Move == 'R')
                    {
                        //Console.Error.WriteLine("Move for Right {0},{1},{2},{3}", peg.Location.Row, peg.Location.Col, peg.Location.Col + 1, peg.Location.Col - 1);
                        board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col, '.');
                        board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col + 1, board[peg.Location.Row][peg.Location.Col - 1]);
                        board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col - 1, '.');
                        m.Location.Row = peg.Location.Row;
                        m.Location.Col = peg.Location.Col - 1;
                        m.Movement += "R";
                        anyMoveMade = true;
                    }
                    /** LEFT **/
                    else if (goodMove.Move == 'L')
                    {
                        //Console.Error.WriteLine("Move for Left {0},{1},{2},{3}", peg.Location.Row, peg.Location.Col, peg.Location.Col + 1, peg.Location.Col - 1);
                        board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col, '.');
                        board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col - 1, board[peg.Location.Row][peg.Location.Col + 1]);
                        board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col + 1, '.');
                        m.Location.Row = peg.Location.Row;
                        m.Location.Col = peg.Location.Col + 1;
                        m.Movement += "L";
                        anyMoveMade = true;
                    }
                    /** UP **/
                    else if (goodMove.Move == 'D')
                    {
                        //Console.Error.WriteLine("Move for Down {0},{1},{2},{3}", peg.Location.Row, peg.Location.Col, peg.Location.Row + 1, peg.Location.Row - 1);
                        board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col, '.');
                        board[peg.Location.Row + 1] = ReplaceAt(board[peg.Location.Row + 1], peg.Location.Col, board[peg.Location.Row - 1][peg.Location.Col]);
                        board[peg.Location.Row - 1] = ReplaceAt(board[peg.Location.Row - 1], peg.Location.Col, '.');
                        m.Location.Row = peg.Location.Row - 1;
                        m.Location.Col = peg.Location.Col;
                        m.Movement += "D";
                        anyMoveMade = true;
                    }
                    /** DOWN **/
                    else if (goodMove.Move == 'U')
                    {
                        //Console.Error.WriteLine("Move for Up {0},{1},{2},{3}", peg.Location.Row, peg.Location.Col, peg.Location.Row + 1, peg.Location.Row - 1);
                        board[peg.Location.Row] = ReplaceAt(board[peg.Location.Row], peg.Location.Col, '.');
                        board[peg.Location.Row - 1] = ReplaceAt(board[peg.Location.Row - 1], peg.Location.Col, board[peg.Location.Row + 1][peg.Location.Col]);
                        board[peg.Location.Row + 1] = ReplaceAt(board[peg.Location.Row + 1], peg.Location.Col, '.');
                        m.Location.Row = peg.Location.Row + 1;
                        m.Location.Col = peg.Location.Col;
                        m.Movement += "U";
                        anyMoveMade = true;
                    }

                    if (!string.IsNullOrEmpty(m.Movement))
                    {
                        b.AllMoves.Add(m);
                    }
                }

            }
            UpdateAvailablePegs(b, board);

            if (watch.ElapsedMilliseconds >= 14000)
            {
                Console.Error.WriteLine(string.Format("Board of Size {0} with Peg Count {1} caused the timeout issue", b.BoardSize, totalPegs));
                break;

            }

        } while (anyMoveMade);
        Console.Error.WriteLine(string.Format("{0} Pegs outlived of total {1} Pegs.", b.AvailablePegs.Count, totalPegs));
        return ConvertMovesToStringArray(b);
    }
    #endregion
    
    private string ReplaceAt(string input, int index, char newChar)
    {
        char[] chars = input.ToCharArray();
        chars[index] = newChar;
        return new string(chars);
    }

    #endregion Methods
}

public class Point
{
    #region Constructors

    public Point(int p1, int p2)
    {
        this.Row = p1;
        this.Col = p2;
    }

    public Point()
    {
        // TODO: Complete member initialization
    }

    #endregion Constructors

    #region Properties

    public int Col
    {
        get;
        set;
    }

    public int Row
    {
        get;
        set;
    }

    #endregion Properties
}

public class ProfitableMove
{
    #region Constructors

    public ProfitableMove(char p, int downCount, int downVal)
    {
        // TODO: Complete member initialization
        this.Move = p;
        this.MoveCount = downCount;
        this.MoveVal = downVal;
    }

    public ProfitableMove(char p, int downCount, int downVal, Peg peg)
    {
        // TODO: Complete member initialization
        this.Move = p;
        this.MoveCount = downCount;
        this.MoveVal = downVal;
        this.Peg = peg;
    }

    #endregion Constructors

    #region Properties

    public char Move
    {
        get;
        set;
    }

    public int MoveCount
    {
        get;
        set;
    }

    public int MoveVal
    {
        get;
        set;
    }

    public Peg Peg
    {
        get;
        set;
    }

    #endregion Properties
}

public class ProfitableMovev2
{
    #region Constructors

    public ProfitableMovev2(string p1, Peg p2)
    {
        this.Move = p1;
        this.Peg = p2;
    }

    #endregion Constructors

    #region Properties

    public string Move
    {
        get;
        set;
    }

    public Peg Peg
    {
        get;
        set;
    }

    public int Value
    {
        get;
        set;
    }

    #endregion Properties
}