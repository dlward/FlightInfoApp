using System;
using System.IO;
/***************************************************************************************
 * When working with this program, I decided to use C# since it's a new language for me, and something that I will definitely need to continue learning. I did feel
 * that it was similar to Java, which helped me a lot. My first thought when trying to solve this problem was trying to read in all of the data from the file at once, 
 * and then pick and choose what columns I wanted to use.  Since there were headers, I was able to save the column index for each of the columns I needed, and create
 * an array with only those 4. Once that was done, I parsed the date and times to change the timezone from GMT to local for each. The final step was to have the array
 * print out to a file, which I called EnvoyFlightData.txt, and put it in the application's folder.
 * 
 * I think that this was a good problem to test how someone would solve an issue like this, where you have many columns, and you need to be able to access only a certain 
 * few. I enjoyed dealing with this problem because in college, reading csv files and trying to parse the data isn't really the main focus, so trying to figure out how to
 * do these kinds of things is important. I'm sure there was a better way to solve the problem, but that's one of the things that I enjoy about programming, because there
 * are many ways to deal with a problem. One of the things that surprised me, was the amount of data that airlines need to keep track of. I wasnt expecting the amount of
 * columns in the csv file, especially with a sample dataset, but it helped me realize how much data it takes to make everything successful for the business.
 * 
 * 
 * 
 */
namespace FlightInfoApp
{
    class Program
    {
        private string[,] LoadCsv(string filename)
        {
            // Get the file's text.
            string whole_file = File.ReadAllText(filename);

            // Split into lines.
            whole_file = whole_file.Replace('\n', '\r');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            // See how many rows and columns there are.
            int num_rows = lines.Length;
            int num_cols = lines[0].Split(',').Length;

            // Allocate the data array.
            string[,] values = new string[num_rows + 1, 4];

            //Create indexes to store needed columns.
            int fltNumIndex = -1;
            int depStaIndex = -1;
            int actualOutIndex = -1;
            int depGMTAdjIndex = -1;

            //gather indexes for each header that's needed, by looking at the first row of data.
            for (int r = 0; r < 1; r++)
            {
                string[] line_r = lines[r].Split(',');
                for (int c = 0; c < num_cols; c++)
                {
                    if (line_r[c].ToString().Equals("FltNum")) { fltNumIndex = c; }
                    if (line_r[c].ToString().Equals("DepSta")) { depStaIndex = c; }
                    if (line_r[c].ToString().Equals("ActualOut")) { actualOutIndex = c; }
                    if (line_r[c].ToString().Equals("DepartingGMTAdjustment")) { depGMTAdjIndex = c; }
                }
            }

            //Gather required fields now that header indexes are known.
            for (int r = 1; r < num_rows; r++)
            {
                string[] line_r = lines[r].Split(',');
                for (int c = 0; c < num_cols; c++)
                {
                    if (c == fltNumIndex)
                        values[r, 0] = line_r[c];
                    if (c == depStaIndex)
                        values[r, 1] = line_r[c];
                    if (c == actualOutIndex)
                        values[r, 2] = line_r[c];
                    if(c == depGMTAdjIndex)
                        values[r, 3] = line_r[c];
                }
            }

            // Return the values.
            return AdjustToGMT(values);
        }
        /*
         * Takes in a string array and adjusts the time to GMT
         * 
         * Returns string array
         * 
         *   
        */
        private string[,] AdjustToGMT(string[,] values)
        {
            string[,] adjustedToGMT = values;
            string[] dateTime;
            string[] time;
            string[] timeAdjustment;

            for (int r = 1; r < adjustedToGMT.Length / 4 - 1; r++) {

                //Splits into date and time
                dateTime = adjustedToGMT[r, 2].Split('T', 2);
                time = dateTime[1].Split(':');
                timeAdjustment = adjustedToGMT[r, 3].Split(new Char[] { 'P', 'T', 'H', 'M' });

                if (timeAdjustment[0] == "-")
                {
                    time[0] = (Int32.Parse(time[0]) - Int32.Parse(timeAdjustment[2])).ToString();
                    time[1] = (Int32.Parse(time[1]) - Int32.Parse(timeAdjustment[3])).ToString();
                }
                else
                {
                    time[0] = (Int32.Parse(time[0]) + Int32.Parse(timeAdjustment[2])).ToString();
                    time[1] = (Int32.Parse(time[1]) + Int32.Parse(timeAdjustment[3])).ToString();
                }
                adjustedToGMT[r, 2] = dateTime[0] + 'T' + time[0] + ':' + time[1] + ':' + time[2];
                /*
                 //Viewing parsed data
                Console.WriteLine(timeAdjustment[0] + " " + timeAdjustment[2] + " " + timeAdjustment[3]);
                Console.WriteLine(time[0] + ":" + time[1] + ":" + time[2]);
                Console.WriteLine(dateTime[1]);
                */
            }
            return adjustedToGMT;
        }

        /*
         * Creates a file to write to, if a file with that name is already located there, 
         * it will not delete and recreate it.
         * 
         * Filename created: EnvoyFlightData.txt
         * Location: FlightInfoApp folder
         */
        private void CreateFile(string[,] values) {
            string path = @"..\..\..\..\EnvoyFlightData.txt";
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("FltNum" + ", " + "DepSta" + ", " + "GMT Time");
                    for (int r = 1; r < values.Length / 4 - 1; r++)
                    {
                        sw.WriteLine(values[r, 0] + ", " + values[r, 1] + ", " + values[r, 2]);
                    }
                }
                Console.WriteLine("File created in FlightInfoApp folder");
            }
            else
                Console.WriteLine("This Filename already exists, please delete it first!");
        }

        static void Main(string[] args)
        {
            Program prg = new Program();
            string filePath = "Envoy_Flights_20200212.csv";
            
            // Get the data.
            string[,] values = prg.LoadCsv(filePath);
            prg.CreateFile(values);

            /*
             * Testing parsing before starting on file creation portion
             * 
            for (int r = 1; r < values.Length / 4 - 1; r++)
            {
                Console.WriteLine(values[r, 0] + " | " + values[r, 1] + " | " + values[r, 2] + " | " + values[r, 3]);
            }

            Console.WriteLine();
            for (int r = 1; r < adjustedValues.Length / 4 - 1; r++)
            {
                Console.WriteLine(adjustedValues[r, 0] + " | " + adjustedValues[r, 1] + " | " + adjustedValues[r, 2] + " | " + adjustedValues[r, 3]);
            }*/
        }
    }
}
