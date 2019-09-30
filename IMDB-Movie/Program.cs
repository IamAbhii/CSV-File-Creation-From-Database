using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using CsvHelper;

namespace IMDB_Movie
{
    class Program
    {
        static void Main(string[] args)
        {
            Entities2 db = new Entities2();
            //InsertUsers(db);
            //InsertMovies(db);
            //InsertRatings(db);
            CreateCSV(db);
            Console.ReadLine();
        }

        public static void InsertUsers(Entities2 db)
        {
            string path = @"C:\Users\brahm\Desktop\data\users.txt";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            string[] s = File.ReadAllLines(path);

            foreach (string item in s)
            {
                string[] items = item.Split('|');

                db.Database.ExecuteSqlCommand("EXEC InsertNewUser @Id, @Age, @Gender, @Occupation",
                    new[] { new SqlParameter("Id", items[0]), new SqlParameter("Age", items[1]), new SqlParameter("Gender", items[2]), new SqlParameter("Occupation", items[3]) });
            }

            Console.WriteLine("DONE");
        }

        public static void InsertMovies(Entities2 db)
        {
            string path = @"C:\Users\brahm\Desktop\data\movies.txt";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            string[] s = File.ReadAllLines(path);

            foreach (string item in s)
            {
                string[] items = item.Split('|');

                db.Database.ExecuteSqlCommand("EXEC InsertNewMovie @Id, @Name",
                    new[] { new SqlParameter("Id", items[0]), new SqlParameter("Name", items[1]) });
            }

            Console.WriteLine("DONE");
        }
        public static void InsertRatings(Entities2 db)
        {
            string path = @"C:\Users\brahm\Desktop\data\ratings.txt";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            string[] s = File.ReadAllLines(path);
            int counter = 0;
            foreach (string item in s)
            {
                string[] items = item.Split('\t');

                db.Database.ExecuteSqlCommand("EXEC InsertNewRating @UserId, @MovieId, @Rating",
                    new[] { new SqlParameter("UserId", items[0]), new SqlParameter("MovieId", items[1]), new SqlParameter("Rating", items[2]) });
                counter++;
                Console.WriteLine("Finished {0} Rows", counter);
            }

            Console.WriteLine("DONE");
        }

        static void CreateCSV(Entities2 db)
        {
            var data = db.Database.SqlQuery<CSVData>("GetTopRatedMovies");
            List<CSVData> csvData = new List<CSVData>();
            foreach(var d in data)
            {
                csvData.Add(new CSVData{ Id = d.Id, Name=d.Name,Rating=d.Rating,NumberOfRatings=d.NumberOfRatings });
            }

            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer))
            {
                csvWriter.Configuration.Delimiter = ",";
                csvWriter.Configuration.HasHeaderRecord = true;
                csvWriter.Configuration.AutoMap<CSVData>();

                //csvWriter.WriteHeader<CSVData>();
                csvWriter.WriteRecords(data);

                writer.Flush();
                var result = Encoding.UTF8.GetString(mem.ToArray());
                File.WriteAllText(@"C:\Users\brahm\source\repos\IMDB-Movie\data.csv", result);
                Console.WriteLine(result);
                
            }
        }

    }
    public class CSVData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public int NumberOfRatings { get; set; }
    }
}
