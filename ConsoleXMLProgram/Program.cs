using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Text;

namespace ConsoleXMLProgram
{
    class Program
    {
        static void Main(string[] args)
        {

            //File is located in "HaddadAlbertAssignment7\HaddadAlbertAssignment7\bin\Debug\netcoreapp3.1\App_Data"

            //Puts the entire csv into an array
            var courses = ProcessCourseCSV(System.AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Courses.csv");

            //puts instructors into an array
            var instructors = ProcessInstructorCSV(System.AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Instructors.csv");

            //Start of new XDoc
            XDocument coursesDoc =
                new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XComment("ASU Course Listings"),
                        new XElement("Courses"));

            //Input info into that doc
            foreach (Course c in courses)
            {
                XElement crs =
                new XElement("Course",
                    new XElement("Title", c.Title),
                    new XElement("Subject", c.Subject),
                    new XElement("Code", c.Code.ToString()),
                    new XElement("ID", c.ID),
                    new XElement("Instructor", c.Instructor),
                    new XElement("Location", c.Location));

                coursesDoc.Element("Courses").Add(crs);
            }

            //Saves doc to filesystem
            coursesDoc.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\CourseList.xml");
            
            Console.WriteLine("\n");
            //2.3a------------------------------------------------------------------------------------------------
            IEnumerable<XElement> query1 =
                from c in coursesDoc.Element("Courses").Elements("Course")
                where (string)c.Element("Subject") == "CPI"
                where (int)c.Element("Code") >= 200
                orderby (string)c.Element("Instructor")
                select c;

            foreach (XElement x in query1)
            {
                Console.WriteLine(x.Element("Instructor").Value + " --- " + x.Element("Title").Value);
            }

            Console.WriteLine("\n");
            //2.3b------------------------------------------------------------------------------------------------
            var query2 =
                from c in coursesDoc.Element("Courses").Elements("Course")
                group c by c.Element("Subject").Value into group1
                select new
                {
                    subject = group1.Key,
                    Count = group1.Count(),
                    code =
                    from c in group1
                    group c by c.Element("Code").Value into group2
                    select new
                    {
                        num = group2.Key,
                        count = group2.Count()
                    }
                };

            foreach (var item in query2)
            {
                foreach (var g in item.code)
                {
                    if (g.count > 1)
                        Console.WriteLine(item.subject + " - " + g.num);
                }
            }

            Console.WriteLine("\n");
            //2.4------------------------------------------------------------------------------------------------
            IEnumerable<XElement> query3 =
                from c in coursesDoc.Element("Courses").Elements("Course")
                join inst in instructors
                         on c.Element("Instructor").Value equals inst.Name
                orderby c.Element("Code").Value
                select new XElement("Q3",
                new XElement("Email", inst.Email),
                new XElement("Code", c.Element("Code").Value),
                new XElement("Subject", c.Element("Subject").Value));

            foreach (XElement i in query3)
            {
                if (Int32.Parse(i.Element("Code").Value) < 300 && Int32.Parse(i.Element("Code").Value) > 200)
                {
                    Console.WriteLine(i.Element("Subject").Value + " " + Int32.Parse(i.Element("Code").Value) + " - " + i.Element("Email").Value);
                }
            }


        }

        //Process course csv into course object
        private static List<Course> ProcessCourseCSV(string path)
        {
            return File.ReadAllLines(path)
                .Skip(1)
                .Where(row => row.Length > 0)
                .Select(Course.ParseRow).ToList();
        }

        //Process course csv into instructor object
        private static List<Instructor> ProcessInstructorCSV(string path)
        {
            return File.ReadAllLines(path)
                .Skip(1)
                .Where(row => row.Length > 0)
                .Select(Instructor.ParseRow).ToList();
        }
    }

        
        
        


    //Class Defining Each Element in Course
    public class Course
    {
        public String Subject { get; set; }
        public int Code { get; set; }
        public String Title { get; set; }
        public String ID { get; set; }
        public String Instructor { get; set; }
        public String Days { get; set; }
        public String Start { get; set; }
        public String End { get; set; }
        public String Location { get; set; }
        public String Dates { get; set; }
        public String Units { get; set; }
        public String Enrollment { get; set; }

        internal static Course ParseRow(string row)
        {
            var columns = row.Split(",");

            return new Course()
            {
                Subject = columns[0].Substring(0, 3),
                Code = int.Parse(columns[0].Substring(4)),
                Title = columns[1],
                ID = columns[2],
                Instructor = columns[3],
                Days = columns[4],
                Start = columns[5],
                End = columns[6],
                Location = columns[7],
                Dates = columns[8],
                Units = columns[9],
                Enrollment = columns[10]
            };
        }
    }

    //Class defining each element in instructor
    public class Instructor
    {
        public String Name { get; set; }
        public String Office { get; set; }
        public String Email { get; set; }

        internal static Instructor ParseRow(string row)
        {
            var columns = row.Split(",");

            return new Instructor()
            {
                Name = columns[0],
                Office = columns[1],
                Email = columns[2]
            };
        }
    }
}


