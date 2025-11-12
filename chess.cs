using System.ComponentModel;
using Microsoft.VisualBasic;

class Program
{

    static void Main(string[] args)
    {
        Chess();

    }

    static void Chess()
    {
        Console.WriteLine("Добро пожаловать в игру Шахматы!\n\n");
        Piece[,] desk = GenerateDesk();
        bool isWhiteTurn = true;
        while (true)
        {
            Console.WriteLine("Перед вами игральная доска:\n");
            DrawDesk(desk);

            if (isCheckmate(desk, isWhiteTurn))
            {
                Console.WriteLine($"Мат! {(!isWhiteTurn ? "Белые" : "Черные")} победили!");
                break;
            }

            if (isStalemate(desk, isWhiteTurn))
            {
                Console.WriteLine("Пат! Игра закончилась вничью!");
                break;
            }

            if (isKingChecked(desk, isWhiteTurn))
            {
                Console.WriteLine($"{((isWhiteTurn) ? "Белым" : "Черным")} шах!");
            }

            int startX;
            int startY;
            int endX;
            int endY;

            while (true)
            {
                while (!TryParseTurn(out startX, out startY, out endX, out endY, isWhiteTurn))
                {
                    Console.WriteLine("Ошибка ввода, попробуйте еще раз!");
                }

                if (CanTurn(startX, startY, endX, endY, isWhiteTurn, desk))
                {
                    desk[endY, endX] = desk[startY, startX];
                    desk[startY, startX] = Piece.Empty;
                    isWhiteTurn = !isWhiteTurn;
                    break;
                }
                else
                {
                    Console.WriteLine("Нельзя так ходить. Попробуйте еще раз.");
                }
            }
        }
    }

    static string GetPieceChar(Piece piece)
    {
        switch (piece)
        {
            case Piece.WhitePawn: return "♙";
            case Piece.WhiteRook: return "♖";
            case Piece.WhiteKnight: return "♘";
            case Piece.WhiteBishop: return "♗";
            case Piece.WhiteQueen: return "♕";
            case Piece.WhiteKing: return "♔";
            case Piece.BlackPawn: return "♟";
            case Piece.BlackRook: return "♜";
            case Piece.BlackKnight: return "♞";
            case Piece.BlackBishop: return "♝";
            case Piece.BlackQueen: return "♛";
            case Piece.BlackKing: return "♚";
            default: return " ";
        }
    }
    static Piece[,] GenerateDesk()
    {
        Piece[,] desk = new Piece[8, 8];
        desk[0, 0] = Piece.BlackRook;
        desk[0, 7] = Piece.BlackRook;
        desk[0, 1] = Piece.BlackKnight;
        desk[0, 6] = Piece.BlackKnight;
        desk[0, 2] = Piece.BlackBishop;
        desk[0, 5] = Piece.BlackBishop;
        desk[0, 4] = Piece.BlackKing;
        desk[0, 3] = Piece.BlackQueen;
        for (int i = 0; i < 8; i++)
        {
            desk[1, i] = Piece.BlackPawn;
        }

        desk[7, 0] = Piece.WhiteRook;
        desk[7, 7] = Piece.WhiteRook;
        desk[7, 1] = Piece.WhiteKnight;
        desk[7, 6] = Piece.WhiteKnight;
        desk[7, 2] = Piece.WhiteBishop;
        desk[7, 5] = Piece.WhiteBishop;
        desk[7, 4] = Piece.WhiteKing;
        desk[7, 3] = Piece.WhiteQueen;

        for (int i = 0; i < 8; i++)
        {
            desk[6, i] = Piece.WhitePawn;
        }

        for (int i = 2; i < 6; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                desk[i, j] = Piece.Empty;
            }
        }
        return desk;
    }

    static void DrawDesk(Piece[,] desk)
    {
        string separator = "  +---+---+---+---+---+---+---+---+";
        Console.WriteLine("\n    A   B   C   D   E   F   G   H");
        Console.WriteLine(separator);

        for (int i = 0; i < 8; i++)
        {
            Console.Write(8 - i + " |");

            for (int j = 0; j < 8; j++)
            {
                Console.Write(" " + GetPieceChar(desk[i, j]) + " |");
            }

            // Номер ряда справа для наглядности
            Console.WriteLine(" " + (8 - i));

            // Линия-разделитель после каждого ряда фигур
            Console.WriteLine(separator);
        }

        // Нижний ряд букв
        Console.WriteLine("    A   B   C   D   E   F   G   H\n");
    }



    static bool TryParseTurn(out int startX, out int startY, out int endX, out int endY, bool isWhiteTurn)
    {
        startX = 0;
        startY = 0;
        endX = 0;
        endY = 0;

        if (isWhiteTurn)
        {
            Console.WriteLine("Ход белых (например, e2e4):");
        }
        else
        {
            Console.WriteLine("Ход черных (например, e7e5):");
        }

        string input = Console.ReadLine();

        if (input == null || input.Length != 4)
        {
            return false;
        }

        char c1 = input[0];
        if (c1 < 'a' || c1 > 'h')
        {
            return false;
        }
        startX = c1 - 'a';


        if (!int.TryParse(input[1].ToString(), out int i1) || i1 < 1 || i1 > 8)
        {
            return false;
        }
        startY = 8 - i1;

        char c2 = input[2];
        if (c2 < 'a' || c2 > 'h')
        {
            return false;
        }
        endX = c2 - 'a';

        if (!int.TryParse(input[3].ToString(), out int i2) || i2 < 1 || i2 > 8)
        {
            return false;
        }
        endY = 8 - i2;
        return true;
    }

    static bool CanTurn(int startX, int startY, int endX, int endY, bool isWhiteTurn, Piece[,] desk)
    {
        Piece piece = desk[startY, startX];
        if (piece == Piece.Empty || isWhite(piece) != isWhiteTurn)
        {
            return false;
        }
        if (!CanPieceMoveUnsafe(startX, startY, endX, endY, desk))
        {
            return false;
        }
        Piece[,] copyDesk = (Piece[,])desk.Clone();
        copyDesk[endY, endX] = copyDesk[startY, startX];
        copyDesk[startY, startX] = Piece.Empty;
        return !isKingChecked(copyDesk, isWhiteTurn);
    }

    static bool CanPieceMoveUnsafe(int startX, int startY, int endX, int endY, Piece[,] desk)
    {
        Piece piece = desk[startY, startX];
        bool isWhitePiece = isWhite(piece);

        if (endX < 0 || endX > 7 || endY < 0 || endY > 7) return false;
        if (startX == endX && startY == endY) return false;
        Piece destinationPiece = desk[endY, endX];
        if (destinationPiece != Piece.Empty && isWhite(destinationPiece) == isWhitePiece) return false;

        switch (piece)
        {
            case Piece.WhitePawn:
                return CanWhitePawnTurn(startX, startY, endX, endY, desk, destinationPiece);
            case Piece.BlackPawn:
                return CanBlackPawnTurn(startX, startY, endX, endY, desk, destinationPiece);
            case Piece.WhiteKnight:
            case Piece.BlackKnight:
                return CanKnightTurn(startX, startY, endX, endY);
            case Piece.WhiteRook:
            case Piece.BlackRook:
                return CanRookTurn(startX, startY, endX, endY, isWhitePiece, desk);
            case Piece.WhiteBishop:
            case Piece.BlackBishop:
                return CanBishopTurn(startX, startY, endX, endY, isWhitePiece, desk);
            case Piece.WhiteQueen:
            case Piece.BlackQueen:
                return CanQueenTurn(startX, startY, endX, endY, isWhitePiece, desk);
            case Piece.WhiteKing:
            case Piece.BlackKing:
                return CanKingTurn(startX, startY, endX, endY);
        }
        return false;
    }

    static bool CanWhitePawnTurn(int startX, int startY, int endX, int endY, Piece[,] desk, Piece destinationPiece)
    {
        if (startX == endX && destinationPiece == Piece.Empty)
        {
            if (startY - 1 == endY) return true;
            if (startY == 6 && startY - 2 == endY && desk[5, startX] == Piece.Empty) return true;
        }
        if (Math.Abs(startX - endX) == 1 && startY - 1 == endY && destinationPiece != Piece.Empty) return true;
        return false;
    }

    static bool CanBlackPawnTurn(int startX, int startY, int endX, int endY, Piece[,] desk, Piece destinationPiece)
    {
        if (startX == endX && destinationPiece == Piece.Empty)
        {
            if (startY + 1 == endY) return true;
            if (startY == 1 && startY + 2 == endY && desk[2, startX] == Piece.Empty) return true;
        }
        if (Math.Abs(startX - endX) == 1 && startY + 1 == endY && destinationPiece != Piece.Empty) return true;
        return false;
    }

    static bool CanKnightTurn(int startX, int startY, int endX, int endY)
    {
        return (Math.Abs(endX - startX) == 1 && Math.Abs(endY - startY) == 2) || (Math.Abs(endX - startX) == 2 && Math.Abs(endY - startY) == 1);
    }
    static bool CanQueenTurn(int startX, int startY, int endX, int endY, bool isWhitePiece, Piece[,] desk)
    {
        return CanBishopTurn(startX, startY, endX, endY, isWhitePiece, desk) || CanRookTurn(startX, startY, endX, endY, isWhitePiece, desk);
    }
    static bool CanBishopTurn(int startX, int startY, int endX, int endY, bool isWhitePiece, Piece[,] desk)
    {

        if (Math.Abs(endX - startX) == Math.Abs(endY - startY)) return isPathClear(startX, startY, endX, endY, isWhitePiece, desk);
        return false;
    }
    static bool CanKingTurn(int startX, int startY, int endX, int endY)
    {
        return Math.Abs(endX - startX) <= 1 && Math.Abs(endY - startY) <= 1;
    }
    //ладья
    static bool CanRookTurn(int startX, int startY, int endX, int endY, bool isWhitePiece, Piece[,] desk)
    {
        if (startX == endX || startY == endY) return isPathClear(startX, startY, endX, endY, isWhitePiece, desk);
        return false;
    }

    static bool isWhite(Piece piece)
    {
        switch (piece)
        {
            case Piece.WhitePawn:
            case Piece.WhiteRook:
            case Piece.WhiteBishop:
            case Piece.WhiteKing:
            case Piece.WhiteQueen:
            case Piece.WhiteKnight: return true;
            default: return false;
        }
    }
    static bool isPathClear(int startX, int startY, int endX, int endY, bool isWhiteTurn, Piece[,] desk)
    {
        int signX = Math.Sign(endX - startX);
        int signY = Math.Sign(endY - startY);

        int currentX = startX + signX;
        int currentY = startY + signY;

        while (currentY != endY || currentX != endX)
        {
            if (desk[currentY, currentX] != Piece.Empty)
            {
                return false;
            }

            currentY += signY;
            currentX += signX;
        }
        return true;
    }

    static bool isKingChecked(Piece[,] desk, bool isWhiteTurn)
    {
        int kingY = -1;
        int kingX = -1;
        Piece kingPiece = isWhiteTurn ? Piece.WhiteKing : Piece.BlackKing;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (desk[i, j] == kingPiece)
                {
                    kingY = i;
                    kingX = j;
                    break;
                }
            }
            if (kingX != -1) break;
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece piece = desk[i, j];
                if (piece != Piece.Empty && isWhite(piece) != isWhiteTurn)
                {
                    if (CanPieceMoveUnsafe(j, i, kingX, kingY, desk))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    static bool isCheckmate(Piece[,] desk, bool isWhiteTurn)
    {
        if (!isKingChecked(desk, isWhiteTurn))
        {
            return false;
        }
        for (int startY = 0; startY < 8; startY++)
        {
            for (int startX = 0; startX < 8; startX++)
            {
                Piece piece = desk[startY, startX];
                if (piece != Piece.Empty && isWhite(piece) == isWhiteTurn)
                {
                    for (int endY = 0; endY < 8; endY++)
                    {
                        for (int endX = 0; endX < 8; endX++)
                        {
                            if (CanTurn(startX, startY, endX, endY, isWhiteTurn, desk))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }

        return true;
    }

    static bool isStalemate(Piece[,] desk, bool isWhiteTurn)
    {
        if (isKingChecked(desk, isWhiteTurn))
        {
            return false;
        }

        for (int startY = 0; startY < 8; startY++)
        {
            for (int startX = 0; startX < 8; startX++)
            {
                Piece piece = desk[startY, startX];
                if (piece != Piece.Empty && isWhite(piece) == isWhiteTurn)
                {
                    for (int endY = 0; endY < 8; endY++)
                    {
                        for (int endX = 0; endX < 8; endX++)
                        {
                            if (CanTurn(startX, startY, endX, endY, isWhiteTurn, desk))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }

        return true;
    }
}

public enum Piece
{
    Empty,
    WhitePawn,
    WhiteRook,
    WhiteBishop,
    WhiteQueen,
    WhiteKing,
    WhiteKnight,
    BlackPawn,
    BlackRook,
    BlackBishop,
    BlackQueen,
    BlackKing,
    BlackKnight
}

