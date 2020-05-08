using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace battleship
{
    public class SaveScores
    {
        public void MyScores ()
        {
            

           
            
            string[] lines = { "1: AAA 100", "2: BBB 90", "3: CCC 80", "4: DDD 70", "5: EEE 60", "6: FFF  50", "7: GGG 40", "8: III 30" };


            System.IO.File.WriteAllLines(@"C:\Users\benya\Scores.txt", lines);



            if (File.Exists(@"C:\Users\benya\Scores.txt"))

            {
                Console.WriteLine("Scores have been saved to Score.txt");
            
            }


         
            
        }
    
        }
    }

