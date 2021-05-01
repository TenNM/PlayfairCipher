using System;

namespace PlayfairCipher_ns
{
    class Program
    {
        static void Main(string[] args)
        {
            //string plainText = "текст для тестирования произвольной длинны";
            string plainText = "arbitrary length test te_t";

            Console.WriteLine(plainText);

            //CipherPlayfair cipherPlayfair1 = new CipherPlayfair(LocalesStorage.availableLocales.Rus);
            PlayfairCipher pc = new PlayfairCipher(LocalesStorage.availableLocales.Eng);

            Console.WriteLine("String correct: {0}", pc.CheckStr(plainText));
            Console.WriteLine("Size: {0} x {1}", pc.RowCnt, pc.ColCnt);

            //cipherPlayfair1.FillKeyMatrix("безповтр");
            pc.FillKeyMatrix("norep_at");

            pc.PrintMatrix();

            string cipherPlayfair = pc.Encode(plainText);
            Console.WriteLine(cipherPlayfair);

            string plainPlayfair = pc.Decode(cipherPlayfair);
            Console.WriteLine(plainPlayfair);
        }
    }
}
