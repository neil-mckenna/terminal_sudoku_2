using TerminalSudoku_2;

namespace TerminalSudoko_2
{
    public class Program
    {
        #region variables & statics

        // debug option to show board
        public static bool DEBUG = false;

        // variable for global board size
        public const int BSIZE = 9;

        // 2 dimensional int array to store data boards
        public static int[,] solvedBoard = new int[BSIZE, BSIZE];
        public static int[,] riddleBoard = new int[BSIZE, BSIZE];

        // this is the option for varying difficulty (81 with a BSIZE of 9, 15 is basic difficulty start)
        public static int piecesToErase = 15;

        // a link to a a class for generatoring random numbers and shorthand
        public static RandomNumber? rnd = null;

        // main game loop
        static bool gameRunning = false;

        // this hold keypress information
        static ConsoleKeyInfo cKey;

        // this a list of real board location in a tuple form
        public static List<Tuple<int, int>> zeroList = new List<Tuple<int, int>>();

        // this is a list of board location but with a plus 1
        // so it easier for human to count instead of starting at 0
        public static List<Tuple<int, int>> readTuples = new List<Tuple<int, int>>();

        // this is for the current slection location in the list tuple
        public static Tuple<int, int>? selectedTuple;

        // this is the default selection in the list, I use this to cycle through list
        public static int selectedIndex = 0;

        // was used to hide information
        private static bool shown = false;

        // this is to stop adding tuples to my list for tidyiness
        private static bool addedTuples = true;


        #endregion

        #region Main

        static void Main(string[] args)
        {
            // change console colors
            SetConsoleColor();

            // initialise the random class
            rnd = new RandomNumber();

            // this was old method on initally creating the most basic board state
            // use this to initialayy create a basic board
            //int[,] newGrid = InitGrid(ref starterGrid);
            //DebugGrid(ref newGrid);
            //DisplayBoard(ref newGrid);


            SetupBoard();

            //Console.WriteLine("\tDebug solvedBoard\n");
            //DebugGrid(ref solvedBoard);

            //DisplayBoard(ref solvedBoard);

            CreateRiddleBoard(ref solvedBoard, ref riddleBoard);

            //Console.WriteLine("\tDebug riddleBoard\n");
            //DebugGrid(ref riddleBoard);


            // slow down the framerate, but change logic so it may be unnecessary
            int desiredFPS = 30;
            int deltaTime = 1000 / desiredFPS;

            gameRunning = true;

            do
            {
                DrawGameBoard();

                HeadsUpDisplay();

                if (DEBUG)
                {
                    Console.WriteLine("\nriddleBoard\n");
                    DebugGrid(ref riddleBoard);
                    Console.WriteLine("\nanswersBoard\n");
                    DebugGrid(ref solvedBoard);
                    Console.WriteLine("\n");
                }

                CheckForPlayerInput();

                //Console.ReadLine();

                // clear screen
                Console.Clear();
                // clear previous console history
                Console.WriteLine("\x1b[3J");


                //Thread.Sleep(deltaTime);
            }
            while (gameRunning);



            // need to show screen
            Console.ReadLine();
        }

        #endregion

        #region console & player input
        //--------------------------------------GAME-INPUT-----------------------------------
        // a method to handle player input

        private static void HeadsUpDisplay()
        {

            Console.WriteLine("\nDEBUG ON :\t" + DEBUG);
            Console.WriteLine("press U \t=>\t update selected");
            Console.WriteLine("press A \t=>\t cycle left");
            Console.WriteLine("press D \t=>\t cycle right");
            Console.WriteLine("press L \t=>\t list locations");
            Console.WriteLine("press C \t=>\t check if correct");
            Console.WriteLine("press N \t=>\t restart the game");
            Console.WriteLine("press Escape \t=>\t exit\n");


            zeroList.Sort();

            //Console.WriteLine("Zerolist count: " + zeroList.Count + " readTuples count: " + readTuples.Count);
            //Console.WriteLine("bool " +  addedTuples);


            while(addedTuples)
            {
                foreach (Tuple<int, int> tp in zeroList)
                {
                    //Console.WriteLine($"{tp.Item1 + 1},{tp.Item2 + 1}");
                    readTuples.Add(new Tuple<int, int>(tp.Item1 + 1, tp.Item2 + 1));
                }

                addedTuples = false;
            }

            //Console.WriteLine("bool " + addedTuples);


            selectedTuple = readTuples[selectedIndex];
            Console.WriteLine("Currently selected " + readTuples[selectedIndex]);

        }

        private static void CheckForPlayerInput()
        {
            ConsoleKeyInfo cki = Console.ReadKey(true);

            //Console.WriteLine("Key pressed: " + cki.Key);
            int num = 0;

            // add a new value to the riddleBoard
            if (cki.Key == ConsoleKey.U || cki.Key == ConsoleKey.Enter)
            {
                Console.WriteLine("Enter a new value (1,9) for the selected location");
                string? inputNum = Console.ReadLine();
                //Console.WriteLine("\nNumber entered " + inputNum);
                if (inputNum != null)
                {
                    bool res = int.TryParse(inputNum, out num);
                    if (res)
                    {
                        //Console.WriteLine("number given " + num);
                        if (selectedTuple != null)
                        {
                            riddleBoard[selectedTuple.Item1 - 1, selectedTuple.Item2 - 1] = num;
                            //Console.WriteLine("selected location updated to " + num);
                        }
                        else
                        {
                            Console.WriteLine("Selected is null!");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Not a valid number, nothing changed");
                    }
                }
            }

            // cycle left through available options
            if (cki.Key == ConsoleKey.D)
            {
                if (selectedTuple != null)
                {
                    if (selectedIndex >= readTuples.Count - 1)
                    {
                        selectedIndex = 0;
                        Console.WriteLine(readTuples[selectedIndex]);
                    }
                    else
                    {
                        selectedIndex++;
                        Console.WriteLine(readTuples[selectedIndex]);
                    }


                }
                else
                {
                    Console.WriteLine("Selected is null!");
                }
            }

            // cycle right through available options
            if (cki.Key == ConsoleKey.A)
            {
                if (selectedTuple != null)
                {
                    if (selectedIndex < 1)
                    {
                        selectedIndex = readTuples.Count - 1;
                        Console.WriteLine(readTuples[selectedIndex]);
                    }
                    else
                    {
                        selectedIndex--;
                        Console.WriteLine(readTuples[selectedIndex]);
                    }

                }
                else
                {
                    Console.WriteLine("Selected is null!");
                }
            }

            // show all available locations
            if (cki.Key == ConsoleKey.L)
            {
                Console.WriteLine("Available locations 1st number down , second along");
                foreach (Tuple<int, int> nt in readTuples)
                {
                    Console.Write(nt.Item1 + "," + nt.Item2 + "\t");

                }

                Console.ReadKey();
            }

            if (cki.Key == ConsoleKey.C)
            {
                CheckComplete();
                Console.ReadKey();
            }

            if (cki.Key == ConsoleKey.N)
            {
                Console.WriteLine("Rebuilding the board!");
                RestartBoard();
                Console.ReadKey();
            }

            if (cki.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("Exiting Game!");
                gameRunning = false;
                System.Environment.Exit(0);

                Console.ReadKey();

            }
        }

        public static void RestartBoard()
        {
            zeroList.Clear();
            readTuples.Clear();

            addedTuples = true;

            SetupBoard();
            CreateRiddleBoard(ref solvedBoard, ref riddleBoard);

        }


        private static void SetConsoleColor()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        private static void SetupBoard()
        {
            // create x and y edges of the board between 1 - 9
            FillBoardEdges(ref solvedBoard);
            SolvedInnerGrid(ref solvedBoard);
        }

        public static void DrawGameBoard()
        {
            DisplayBoard(ref riddleBoard);

        }


        #endregion

        #region board functions
        //--------------------------------------BOARD-FUNCTIONS----------------------------------
        private static int[,] InitGrid(ref int[,] board)
        {
            for (int i = 0; i < BSIZE; i++)
            {
                for (int j = 0; j < BSIZE; j++)
                {

                    board[i, j] = (i * 3 + i / 3 + j) % 9 + 1;
                    //board[i, j] = 9;

                }

            }

            return board;
        }

        public static void DebugGrid(ref int[,] board)
        {
            string s = "";
            int sep = 0;

            for (int i = 0; i < BSIZE; i++)
            {
                s += "|";
                for (int j = 0; j < BSIZE; j++)
                {
                    s += board[i, j].ToString();

                    sep = j % 3;
                    if (sep == 2)
                    {
                        s += "|";
                    }
                }

                s += "\n";


            }

            Console.WriteLine(s);

        }
        #endregion

        #region display board
        //-------------------------------------DISPLAY-BOARD-----------------------------------
        private static void DisplayBoard(ref int[,] board)
        {


            Console.WriteLine("##\t                         \t##");
            Console.WriteLine("##\t       Neil's Sudoko     \t##");
            Console.WriteLine("##\t                         \t##");
            Console.WriteLine("##\t|-----------------------|\t##");
            // row 0
            Console.WriteLine($"##\t| {board[0, 0]} {board[0, 1]} {board[0, 2]} | {board[0, 3]} {board[0, 4]} {board[0, 5]} | {board[0, 6]} {board[0, 7]} {board[0, 8]} | \t##");

            // row 1
            Console.WriteLine($"##\t| {board[1, 0]} {board[1, 1]} {board[1, 2]} | {board[1, 3]} {board[1, 4]} {board[1, 5]} | {board[1, 6]} {board[1, 7]} {board[1, 8]} | \t##");

            //row 2
            Console.WriteLine($"##\t| {board[2, 0]} {board[2, 1]} {board[2, 2]} | {board[2, 3]} {board[2, 4]} {board[2, 5]} | {board[2, 6]} {board[2, 7]} {board[2, 8]} | \t##");

            // break
            Console.WriteLine("##\t|-----------------------|\t##");

            //row 3
            Console.WriteLine($"##\t| {board[3, 0]} {board[3, 1]} {board[3, 2]} | {board[3, 3]} {board[3, 4]} {board[3, 5]} | {board[3, 6]} {board[3, 7]} {board[3, 8]} | \t##");

            //row 4
            Console.WriteLine($"##\t| {board[4, 0]} {board[4, 1]} {board[4, 2]} | {board[4, 3]} {board[4, 4]} {board[4, 5]} | {board[4, 6]} {board[4, 7]} {board[4, 8]} | \t##");

            //row 5
            Console.WriteLine($"##\t| {board[5, 0]} {board[5, 1]} {board[5, 2]} | {board[5, 3]} {board[5, 4]} {board[5, 5]} | {board[5, 6]} {board[5, 7]} {board[5, 8]} | \t##");

            // break
            Console.WriteLine("##\t|-----------------------|\t##");

            // row 6
            Console.WriteLine($"##\t| {board[6, 0]} {board[6, 1]} {board[6, 2]} | {board[6, 3]} {board[6, 4]} {board[6, 5]} | {board[6, 6]} {board[6, 7]} {board[6, 8]} | \t##");

            //row 7
            Console.WriteLine($"##\t| {board[7, 0]} {board[7, 1]} {board[7, 2]} | {board[7, 3]} {board[7, 4]} {board[7, 5]} | {board[7, 6]} {board[7, 7]} {board[7, 8]} | \t##");

            // row 8
            Console.WriteLine($"##\t| {board[8, 0]} {board[8, 1]} {board[8, 2]} | {board[8, 3]} {board[8, 4]} {board[8, 5]} | {board[8, 6]} {board[8, 7]} {board[8, 8]} | \t##");

            // break
            Console.WriteLine("##\t|-----------------------|\t##");
        }
        #endregion

        #region location checks
        //--------------------------------------LOCATION-CHECKS--------------------------------------

        //this column contain the number
        static bool ColumnContainsNumber(int y, int value, ref int[,] board)
        {
            for (int x = 0; x < BSIZE; x++)
            {
                if (board[x, y] == value)
                {
                    return true;
                }
            }
            return false;
        }

        // this row contains the number
        static bool RowContainsNumber(int x, int value, ref int[,] board)
        {
            for (int y = 0; y < BSIZE; y++)
            {
                if (board[x, y] == value)
                {
                    return true;
                }
            }
            return false;
        }

        // this local 3 x 3 block contains the number
        private static bool BlockContainsNumber(int x, int y, int value, ref int[,] board)
        {

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[x - (x % 3) + i, y - (y % 3) + j] == value)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // check all
        static bool CheckAll(int x, int y, int value, ref int[,] board)
        {
            if (ColumnContainsNumber(y, value, ref board)) { return false; }
            if (RowContainsNumber(x, value, ref board)) { return false; }
            if (BlockContainsNumber(x, y, value, ref board)) { return false; }

            return true;
        }

        // non 0s
        static bool IsValidGrid(ref int[,] board)
        {
            for (int i = 0; i < BSIZE; i++)
            {
                for (int j = 0; j < BSIZE; j++)
                {
                    if (board[i, j] == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region generate board
        //--------------------------------------GENERATE-BOARD-----------------------------------
        private static void FillBoardEdges(ref int[,] board)
        {
            if (rnd == null)
            {
                throw new Exception("Random Number class is is null");
            }

            List<int> rowValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            List<int> columnValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            int value = rowValues[rnd.Next(0, rowValues.Count)];
            board[0, 0] = value;

            // 0, 0 remove in both directions
            rowValues.Remove(value);
            columnValues.Remove(value);

            // ROW first
            for (int r = 1; r < BSIZE; r++)
            {
                value = rowValues[rnd.Next(0, rowValues.Count)];
                board[r, 0] = value;
                // rmeove from row
                rowValues.Remove(value);

            }

            // COLUMNS
            for (int c = 1; c < BSIZE; c++)
            {
                value = columnValues[rnd.Next(0, columnValues.Count)];
                if (c < 3)
                {
                    while (BlockContainsNumber(0, 0, value, ref board))
                    {
                        value = columnValues[rnd.Next(0, columnValues.Count)];

                    }
                }
                board[0, c] = value;
                columnValues.Remove(value);
            }


        }

        static bool SolvedInnerGrid(ref int[,] board)
        {
            //DebugGrid(ref board);

            if (IsValidGrid(ref board))
            {
                return true;
            }

            // FIND FIRST FREE CELL
            int x = 0;
            int y = 0;

            for (int i = 0; i < BSIZE; i++)
            {
                for (int j = 0; j < BSIZE; j++)
                {
                    if (board[i, j] == 0)
                    {
                        x = i;
                        y = j;
                        //print(x + " , " + y);
                        break;
                    }
                }
            }

            List<int> possibilities = new List<int>();
            possibilities = GetAllPossibilities(x, y, ref board);

            for (int ps = 0; ps < possibilities.Count; ps++)
            {
                // SET A POSSIBLE VALUE
                board[x, y] = possibilities[ps];
                // BACKTRACK
                if (SolvedInnerGrid(ref board))
                {
                    return true;
                }

                // reset to 0 as false
                board[x, y] = 0;


            }



            return false;
        }

        static List<int> GetAllPossibilities(int x, int y, ref int[,] board)
        {
            List<int> possibilities = new List<int>();
            for (int val = 1; val <= BSIZE; val++)
            {
                if (CheckAll(x, y, val, ref board))
                {
                    possibilities.Add(val);
                }

            }

            return possibilities;
        }

        #endregion

        #region generate riddleboard
        //--------------------------------------GENERATE-RIDDLE-BOARD-----------------------------------
        public static void CreateRiddleBoard(ref int[,] solvedBoard, ref int[,] riddleBoard)
        {
            if (rnd == null)
            {
                throw new Exception("Random Number class is is null");
            }


            // COPY SOLVED GRID
            for (int i = 0; i < BSIZE; i++)
            {
                for (int j = 0; j < BSIZE; j++)
                {
                    // copy solved to riddle
                    riddleBoard[i, j] = solvedBoard[i, j];

                }
            }

            // ERASE FROM RIDDLE GRID
            for (int i = 0; i < piecesToErase; i++)
            {
                int x1 = rnd.Next(0, 9);
                int y1 = rnd.Next(0, 9);

                // REROLL UNTIL WE FIND ONE WITHOUT A ZERO IN BETWWEN
                while (riddleBoard[x1, y1] == 0)
                {
                    x1 = rnd.Next(0, 9);
                    y1 = rnd.Next(0, 9);
                }

                // Once we found one with NO
                riddleBoard[x1, y1] = 0;

                // add to list for ui manipulation
                zeroList.Add(new Tuple<int, int>(x1, y1));

            }

            //DebugGrid(ref riddleBoard);
        }

        #endregion

        #region win conditions
        //--------------------------------------WIN CONDITIONS-----------------------------------
        // a method to handle player input
        public static void SetInputInRiddle(int x, int y, int value)
        {
            riddleBoard[x, y] = value;

        }

        // check if won bool
        public static bool CheckIfWon()
        {
            for (int i = 0; i < BSIZE; i++)
            {
                for (int j = 0; j < BSIZE; j++)
                {
                    if (riddleBoard[i, j] != solvedBoard[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;

        }

        // check if won show display
        public static void CheckComplete()
        {
            if (CheckIfWon())
            {
                Console.WriteLine("Well done, YOU WON!");


            }
            else
            {
                Console.WriteLine("You Failed, Try Again!");

            }
        }

        #endregion


    }// program end

}// namespace end
