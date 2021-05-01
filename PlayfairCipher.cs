using System;
using System.Collections.Generic;

namespace PlayfairCipher_ns
{
    class PlayfairCipher
    {
        internal  LocalesStorage locale;

        private int rowCnt;
        internal int RowCnt
        {
            get { return rowCnt; }
        }

        private int colCnt;
        internal int ColCnt
        {
            get { return colCnt; }
        }
        internal PlayfairCipher(LocalesStorage.availableLocales l)
        {
            locale = new LocalesStorage(l);
            rowCnt = locale.keyMatrix.GetLength(0);
            colCnt = locale.keyMatrix.GetLength(1);
        }
        //---------------------------------------------------------------------------base
        internal void FillKeyMatrix(string codeStr)
        {
            if (codeStr.Length >= rowCnt * colCnt) codeStr = codeStr.Substring(0, rowCnt * colCnt);
            else
            {
                foreach (char c in codeStr) locale.alphabet.Remove(c);
                int lengthDeficit = rowCnt * colCnt - codeStr.Length;
                for (int i = 0; i < lengthDeficit; i++) codeStr += locale.alphabet[i];
            }
            //---
            for (int row = 0; row < rowCnt; row++)
            {
                for (int col = 0; col < colCnt; col++)
                {
                    locale.keyMatrix[row, col] = codeStr[row * colCnt + col];
                }
            }
        }//fill
        private string Code(string text, bool isEncode)
        {
            List<Bigramm> sourceBigramms = isEncode ? Bigramm(text) : BigrammDecode(text);
            Point p1, p2;
            Bigramm newBigr;
            string resStr = "";
            int delta = isEncode ? 1 : -1;
            foreach (Bigramm b in sourceBigramms)
            {
                p1 = FindCharPoint(b.one);
                p2 = FindCharPoint(b.two);
                if (p1.row == -1 || p1.col == -1 || p2.row == -1 || p2.col == -1)
                {
                    Console.WriteLine("(! " + b.ToString() + ")");
                    continue;
                }

                if (p1.col.Equals(p2.col))
                {
                    newBigr.one = locale.keyMatrix[(p1.row + delta + rowCnt) % rowCnt, p1.col];//row,col
                    newBigr.two = locale.keyMatrix[(p2.row + delta + rowCnt) % rowCnt, p2.col];
                }
                else if (p1.row.Equals(p2.row))
                {
                    newBigr.one = locale.keyMatrix[p1.row, (p1.col + delta + colCnt) % colCnt];
                    newBigr.two = locale.keyMatrix[p2.row, (p2.col + delta + colCnt) % colCnt];
                }
                else
                {
                    Point res1, res2;
                    FindOtherPointsInRect(p1, p2, out res1, out res2);
                    newBigr.one = locale.keyMatrix[res1.row, res1.col];
                    newBigr.two = locale.keyMatrix[res2.row, res2.col];
                }
                resStr += newBigr.ToString();
                //resStr += newBigr.ToString() + " ";//deb
            }
            if (!isEncode) { resStr = resStr.Replace(locale.serviceChar.ToString(), ""); }
            return resStr;
        }
        internal string Encode(string text) => Code(text, true);
        internal string Decode(string text) => Code(text, false);
        internal bool CheckStr(string s)
        {
            if (s.Contains(locale.serviceChar)) return false;
            foreach(char c in s)
            {
                if (!locale.alphabet.Contains(c)) return false;
            }
            return true;
        }
        //---------------------------------------------------------------------------service
        internal void PrintMatrix()
        {
            for(int row = 0; row < rowCnt; row++)
            {
                for (int col = 0; col < colCnt; col++)
                {
                    Console.Write(locale.keyMatrix[row, col] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        internal List<Bigramm> Bigramm(string plainText)
        {
            char buffer = '\0';
            Bigramm b;
            List<Bigramm> tempBList = new List<Bigramm>();

            foreach (char c in plainText)
            {
                if (buffer.Equals('\0'))//buf empty
                {
                    buffer = c;
                    continue;
                }
                if (c.Equals(buffer))//double c
                {
                    b.one = buffer;
                    b.two = locale.serviceChar;
                    tempBList.Add(b);

                    buffer = c;
                }
                else//diff c
                {
                    b.one = buffer;
                    b.two = c;
                    tempBList.Add(b);
                    buffer = '\0';
                }
            }
            if (!buffer.Equals('\0'))//fix of tail c
            {
                b.one = buffer;
                b.two = locale.serviceChar;
                tempBList.Add(b);
            }
            return tempBList;
        }
        internal List<Bigramm> BigrammDecode(string plainText)
        {
            List<Bigramm> tempBList = new List<Bigramm>();
            for (int pos = 0; pos < plainText.Length - 1; pos += 2)
            {
                Bigramm b;
                b.one = plainText[pos];
                b.two = plainText[pos + 1];
                tempBList.Add(b);
                //Console.Write(b.ToString() + " ");//deb
            }
            return tempBList;
        }
        internal Point FindCharPoint(char c)
        {
            for (int row = 0; row < rowCnt; row++)
            {
                for (int col = 0; col < colCnt; col++)
                {
                    if(locale.keyMatrix[row, col].Equals(c))
                    {
                        return new Point(col, row);
                    }
                }
            }
            return new Point(-1, -1);
        }
        internal string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        internal void FindOtherPointsInRect(Point source1, Point source2, out Point out1, out Point out2)
        {
            out1.col = source2.col;
            out1.row = source1.row;

            out2.col = source1.col;
            out2.row = source2.row;
        }
    }//c
    struct Bigramm
    {
        internal char one;
        internal char two;
        public override string ToString()
        {
            return one + "" + two;
        }
    }
    struct Point
    {
        internal int col;
        internal int row;
        internal Point(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
    }
    struct LocalesStorage
    {
        internal enum availableLocales
        {
            Rus,
            Eng
        }

        internal List<char> alphabet;
        internal char[,] keyMatrix;
        internal char serviceChar;

        internal LocalesStorage(availableLocales l)
        {
            switch (l)
            {
                case availableLocales.Rus:
                    {
                        alphabet = new List<char> {
                            'а','б','в','г','д','е',
                            'ё','ж','з','и','й','к',
                            'л','м','н','о','п','р',
                            'с','т','у','ф','х','ц',
                            'ч','ш','щ','ъ','ы','ь',
                            'э','ю','я',' ',',','.',
                        };
                        keyMatrix = new char[,]
                        {
                            { '0', '0', '0', '0', '0', '0',},
                            { '0', '0', '0', '0', '0', '0',},
                            { '0', '0', '0', '0', '0', '0',},
                            { '0', '0', '0', '0', '0', '0',},
                            { '0', '0', '0', '0', '0', '0',},
                            { '0', '0', '0', '0', '0', '0',},
                        };
                        serviceChar = 'ъ';
                    }; break;//rus
                case availableLocales.Eng:
                default:
                    {
                        alphabet = new List<char> {
                            'a','b','c','d','e','f',
                            'g','h','i','j','k','l',
                            'm','n','o','p','q','r',
                            's','t','u','v','w','x',
                            'y','z',' ','_',',','.',
                        };
                        keyMatrix = new char[,]{
                            { '0', '0', '0', '0', '0', '0',},
                            { '0', '0', '0', '0', '0', '0',},
                            { '0', '0', '0', '0', '0', '0',},
                            { '0', '0', '0', '0', '0', '0',},
                            { '0', '0', '0', '0', '0', '0',},
                        };
                        serviceChar = 'x';
                    }; break;//eng
            }//sw
        }//constructor

    }//struct
}//n
