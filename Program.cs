
using System.ComponentModel;

var strInput = string.Empty;
var invalidInput = true;
int[] intRange = [1, 2];
int playerCount = 2;
//first find the number of human players
//for now we are going to assume 2 human players
// coding a tic tac toe AI will take a lot of time and i dont hate may self
//sorry viewers
do
{
    Console.Write("How many humans for todays game: ");
    strInput = Console.ReadLine() ?? string.Empty;
    invalidInput = !CheckIntInput(strInput, intRange, true, out playerCount);
    Console.Clear();

    if (invalidInput)
    {
        Console.WriteLine("I am sorry, I am looking for either a 1 or a 2 as input.");
    }

}while (invalidInput);



//run game
GameLoop(playerCount);

//end of game closing program after this
Console.WriteLine("Press Enter to exit.");
Console.ReadLine();



////////////////////////////////////////
////////////game loop////////////////
/////////////////////////////////////
void GameLoop(int numOfPlayer)
{
    //set up shop
    char[] pieces = ['X', 'O'];
    int turnCounter = 0;
    var XorO = 0;
    string errorMsg;
   
    char[,] theBoard = new char[3, 3];
        for (int i = 0; i < 3; i++)
    {
        for (int o = 0; o < 3; o++)
        {
            theBoard[i, o] = ' ';
        }
    }

    //1.)determine if the odd number of moves is done by a human or machine
    //2.) display board and get input of player if number of turns is even
    //2a.) if number of turn is odd get machince guess or player 'O'
    //3.) game loop

    //1.)
    var useComputerMoves = numOfPlayer == 1;
    string lastMove = String.Empty; ;
    //main game loop

    do
    {
        Console.Clear();
        DisplayBoard(theBoard);

        //grab whoses turn it is
        XorO = turnCounter % 2;

        if(XorO == 0 || useComputerMoves == false)
        {
            do
            {
                errorMsg = string.Empty;
                Console.Write("[Turn " + turnCounter +"] Where do you want to put your '" + pieces[XorO] + "': ");
                strInput = (Console.ReadLine() ?? string.Empty).ToUpper();
                errorMsg = CheckMoveInput(theBoard, strInput);
                if(string.IsNullOrWhiteSpace(errorMsg))
                {
                    var row = getRowIndexFromInput(strInput[0]);
                    var col = getColIndexFromInput(strInput[1]);
                    theBoard[row,col] = pieces[XorO];
                    lastMove = row.ToString() + " " + col.ToString();
                }
                else
                {
                    Console.WriteLine(errorMsg);
                }
            }while(string.IsNullOrWhiteSpace(errorMsg) == false);
        }
        else
        {
            CalculateComputerMove(theBoard, pieces[XorO],lastMove);
        }


            turnCounter++;
            
    } while (GameStateCheck(theBoard,turnCounter));

    Console.Clear();
    DisplayBoard(theBoard);
    if (turnCounter > 8)
    {
        Console.WriteLine("Interesting Game Falkin, the winning move is to not play.\n Its A tie!\n");
    }
    else
    {
        Console.WriteLine("'" + pieces[XorO] + "' WINS!/n");
    }
}
//////////////////////////////////////////
//////////////////////////////////////////
///////////////////////////////////////////
void CalculateComputerMove(char[,] theBoard, char myPiece,string lastmove)
{
    char theirPiece = myPiece == 'X' ? 'O' : 'X';
    var lastmoveParts = lastmove.Split(' ');
    int.TryParse(lastmoveParts[0], out int lastMoveRow);
    int.TryParse(lastmoveParts[1], out int lastMoveCol);
    //take center first if human didnt take it
    if (theBoard[1,1] == ' ')
    {
        theBoard[1,1] = myPiece;
        return;
    }
    //////////////////////////////////////////////////////////////////////////////////
    //check if someone can win right now
    //////////////////////////////////////////////////////////////////////////////////

    //start by checking who has the center
    var whoHasCenter = theBoard[1, 1];
    
    //then we need to check the surrounding slots to see if someone can win, if so block
    for(int i = 0;i < 3; i++)
    {
        for(int o = 0;o < 3;o++)
        {
            if (i == 1 && o == 1)
                continue;

            var opI = opSideOfBoard(i);
            var opo = opSideOfBoard(o);
            if (theBoard[i,o] == whoHasCenter && theBoard[opI, opo] == ' ')
            {
                theBoard[opI,opo] = myPiece;
                return;
            }
        }
    }
    
    //check rows
    for (int i = 0; i < 2; i++)
    {
        var countOfMyPieceInrow = 0;
        var countOfTheirPieceInRow = 0;

        int[] winingSpace = [-1, -1];
        
        for (int o = 0; o < 2; o++)
        {
            if (theBoard[i, o] == myPiece)
                countOfMyPieceInrow++;
            else if (theBoard[i, o] == theirPiece)
                countOfTheirPieceInRow++;

            if (theBoard[i, o] == ' ')
            {
                winingSpace[0] = i;
                winingSpace[1] = o;
            }
        }
        if ((countOfMyPieceInrow == 2 || countOfTheirPieceInRow == 2) && !(winingSpace[0] == -1 || winingSpace[1] == -1))
        {
            theBoard[winingSpace[0], winingSpace[1]] = myPiece;
            return;
        }
    }
    //check cols
    for (int i = 0; i < 2; i++)
    {
        var countOfMyPieceInCol = 0;
        var CountOfTheirPieceInCol = 0;
        int[] winingSpace = [-1, -1];
        for (int o = 0; o < 2; o++)
        {
            if (theBoard[o, i] == myPiece)
                countOfMyPieceInCol++;
            else if(theBoard[o, i] == theirPiece)
                CountOfTheirPieceInCol++;
            if (theBoard[o, i] == ' ')
            {
                winingSpace[0] = o;
                winingSpace[1] = i;
            }
        }
        if ((countOfMyPieceInCol == 2 || CountOfTheirPieceInCol == 2) && !(winingSpace[0] == -1 || winingSpace[1] == -1))
        {
            theBoard[winingSpace[0], winingSpace[1]] = myPiece;
            return;
        }
    }
    ///////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////
    ///

    ////////////////////////////////
    ///put piece Next to last move//
    /////////////////////////////////

    //if center
    var newMoveRow = -1;
    var newMoveCol = -1;
    if (lastMoveRow == 1 && lastMoveCol == 1)
    {
        checkCornorsFor(theBoard, ' ', out newMoveRow, out newMoveCol);

        if (newMoveRow >= 0 && newMoveCol >= 0)
        {
            theBoard[newMoveRow, newMoveCol] = myPiece;
            return;
        }

        checkSidesFor(theBoard, ' ', out newMoveRow, out newMoveCol);
        if (newMoveRow >= 0 && newMoveCol >= 0)
        {
            theBoard[newMoveRow, newMoveCol] = myPiece;
            return;
        }
    }
    else
    {
        checkNextToLastMove(theBoard, lastMoveRow, lastMoveCol, out newMoveRow, out newMoveCol);
        if (newMoveRow != -1 && newMoveCol != -1)
        {
            theBoard[newMoveRow, newMoveCol] = myPiece;
            return;
        }
        else
        {
            // when all else fails, you can win
           // just take the first open space
           // there is no hope for you here choosen undead
            for (int i = 0; i < 3; i++)
            {
                for(int o = 0; o< 3; o++)
                {
                    if (theBoard[i, o] == ' ')
                    {
                        theBoard[i,o] = myPiece;
                        return;
                    }
                }
            }
        }
    }
}

void checkNextToLastMove(char[,] theBoard,int lastX, int lastY, out int newMoveX, out int newMoveY)
{
    newMoveX = -1;
    newMoveY = -1;

    if (lastX - 1 >= 0)
    {
        if (theBoard[lastX - 1, lastY] == ' ')
        {
            newMoveX = lastX - 1;
            newMoveY = lastY;
            return;
        }
    }
    else if (lastX + 1 <= 2)
    {
        if (theBoard[lastX + 1, lastY] == ' ')
        {
            newMoveX = lastX + 1;
            newMoveY = lastY;
            return;
        }
    }


    else if (lastY - 1 >= 0)
    {
        if (theBoard[lastX, lastY - 1] == ' ')
        {
            newMoveX = lastX;
            newMoveY = lastY - 1;
            return;
        }
    }
    else if (lastX + 1 <= 2)
    {
        if (theBoard[lastX, lastY + 1] == ' ')
        {
            newMoveX = lastX;
            newMoveY = lastY + 1;
            return;
        }
    }

}
void checkCornorsFor(char[,] theBoard,char piece,out int x, out int y)
{
    x = -1;
    y = -1;
    for(int i = 0; i < 2; i += 2)
    {
        for (int o =0; o < 2; o += 2)
        {
            if (theBoard[i, o] == piece)
            {
                x = i; 
                y = o;
                return;
            }
        }
    }
}

void checkSidesFor(char[,] theBoard,char piece,out int x,out int y)
{
    x = -1; 
    y = -1;
    //01
    //10
    //12
    //21
    int o = -1;
    for( int i = 0;i < 2; i++)
    {
        switch(i)
        {
            case 0:
                o = 1;
                break;
            case 1:
                o = o == 1 ? 0 : 2;
                break;
            case 2:
                o = 1;
                break;
            default:
                break;
        }

        if(theBoard[i, o] == piece)
        {
            x= i; 
            y = o; 
            return;
        }
    }
}

int opSideOfBoard(int num)
{
    if(num == 0)
    {
        return 2;
    }
    if (num == 2)
    {
        return 0;
    }

    return num;//will be 1
}
bool GameStateCheck(char[,] theBoard,int turnCounter)
{

    //return true if the game is still going
    //return false if we found a winner

    if(turnCounter > 8)
        { return false; }
    //check the diagonals first
    // going \
    if (theBoard[0, 0] != ' ' && theBoard[0, 0] == theBoard[1, 1] && theBoard[0, 0] == theBoard[2, 2])
    {
        return false;
    }
    //going /
    else if (theBoard[2, 0] != ' ' && theBoard[2, 0] == theBoard[1, 1] && theBoard[2, 0] == theBoard[0, 2])
    {
        return false;
    }

    //check the ...
    for (int i = 0;i < 3;i++)
    {
        //rows
        if (theBoard[i, 0] != ' ' && theBoard[i, 0] == theBoard[i, 1] && theBoard[i, 0] == theBoard[i, 2])
        {
            return false;
        }
        //cols
        else if (theBoard[0, i] != ' ' && theBoard[0, i] == theBoard[1, i] && theBoard[0, i] == theBoard[2, i])
        {
            return false;
        }
    }

    return true;
}
void DisplayBoard(char[,] theBoard)
{
    Console.WriteLine("  A B C");
    for (int i = 0; i < 3; i++)
    {
        Console.WriteLine((i + 1) + " " + theBoard[i, 0] + '|' + theBoard[i, 1] + '|' + theBoard[i, 2]);

        if (i != 2)
            Console.WriteLine("  -----");

    }
}
int getRowIndexFromInput(char row)
{
    return (row - '1');
}
int getColIndexFromInput(char col)
{ 
    return (col - 'A'); 
}
bool CheckInputIfExists(string input)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        return false;
    }

    return true;
}

String CheckMoveInput(char[,] theBoard,string theMove)
{
    /*
     * Check if i have input
     * Check if it is vaild input
     * check if there is a piece in the input
     */
    var errormsg = string.Empty;
    if (CheckInputIfExists(theMove) == false)
    {
        return "No Input Detected.";
    }

    if(theMove.Length == 2)
    { 
        var row = theMove[0];
        var col = theMove[1];
        
        if (col < 65 || col > 67)// if row = A,B,C
        {
            errormsg += col + " is not a vaild Column. Pleaes enter Column A,B, or C.\n";
        }

        if (row < 49 || row > 51 )
        {
            errormsg += row + " is not a valid row. Please Enter 1,2, or 3.\n";
        }

        if (string.IsNullOrWhiteSpace(errormsg))
        {
            var rowIndex = getRowIndexFromInput(row);
            var colIndex = getColIndexFromInput(col);
            if (theBoard[rowIndex, colIndex] != ' ')
            {
                errormsg += "There is Already an '" + theBoard[rowIndex, colIndex] + "' at " + row + col + ".\n";
            }
        }
    }
    else
    {
        return "Please enter the row and column you wish to place your piece.";
    }

    //if all is good do not return a msg.
    return errormsg;
}
bool CheckIntInput(string input, int[] range,bool includeLimits,out int intInput)
{

    intInput = -1;
    /*
     * 1.) first check if i have any input
     * 2.) check if the string is a valid number
     * 3.)check if we have a range
     *      3a.)if we do have a range check if my input is in it
     *          check agaist range
     *          if in range return true
     *          if not return false
     *      if we dont return true
     */

    //1.)
    if (CheckInputIfExists(input) == false)
    {
        return false;
    }
    //2.)
    if (int.TryParse(input, out intInput))
    {
        //3.)
        if(range.Length != 0)
        {
            //3a.)
            if (includeLimits)
            {
                if (intInput < range[0])
                {
                    return false;
                }
                else if (intInput > range[1])
                {
                    return false;
                }
            }
            else
            {
                if (intInput <= range[0])
                {
                    return false;
                }
                else if (intInput >= range[0])
                {
                    return false;
                }
            }
        }
    }
    else
    {
        return false;
    }

        //if we didnt hit a return false yet we must be good, have fun with that assumetion
        return true;
}




