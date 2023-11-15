using System.Diagnostics;
using System.IO.Enumeration;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace MJU23v_DTP_T2
{
    internal class Program
    {
        static List<Link> links = new List<Link>();
        class Link
        {
            public string category, group, name, info, link;
            public Link(string category, string group, string name, string info, string link)
            {
                this.category = category;
                this.group = group;
                this.name = name;
                this.info = info;
                this.link = link;
            }

            public Link(string line)
            {
                string[] part = line.Split('|');
                category = part[0];
                group = part[1];
                name = part[2];
                info = part[3];
                link = part[4];
            }
            public void Print(int i)
            {
                Console.WriteLine($"|{i,-2}|{category,-10}|{group,-10}|{name,-20}|{info,-40}|");
            }
            public void OpenLink()
            {
                Process application = new Process();
                application.StartInfo.UseShellExecute = true;
                application.StartInfo.FileName = link;
                application.Start();
                // application.WaitForExit();
            }
            public string ToString()
            {
                return $"{category}|{group}|{name}|{info}|{link}";
            }
        }
        static void Main(string[] args)
        {
            string filename = @"..\..\..\links\links.lis";
            LoadFromFile(filename);

            Console.WriteLine("Välkommen till länklistan! Skriv 'hjälp' för hjälp!");
            do
            {
                Console.Write("> ");
                string cmd = Console.ReadLine().Trim();
                string[] parts = cmd.Split();
                string command = parts[0];
                filename = HandelCommand(filename, parts, command);

            } while (true);
        }

        private static string HandelCommand(string filename, string[] parts, string command)
        {
            if (command == "sluta")
            {
                Console.WriteLine("Hej då! Välkommen åter!");
            }
            else if (command == "hjälp")
            {
                Console.WriteLine("hjälp           - skriv ut den här hjälpen");
                Console.WriteLine("sluta           - avsluta programmet");
            }
            else if (command == "ladda")
            {
                filename = Load(filename, parts);
            }
            else if (command == "lista")
            {
                int i = 0;
                foreach (Link L in links)
                    L.Print(i++);
            }
            else if (command == "ny")
            {
                Console.WriteLine("Skapa en ny länk:");
                Console.Write("  ange kategori: ");
                string category = Console.ReadLine();
                Console.Write("  ange grupp: ");
                string group = Console.ReadLine();
                Console.Write("  ange namn: ");
                string name = Console.ReadLine();
                Console.Write("  ange beskrivning: ");
                string descr = Console.ReadLine();
                Console.Write("  ange länk: ");
                string link = Console.ReadLine();
                Link newLink = new Link(category, group, name, descr, link);
                links.Add(newLink);
            }
            else if (command == "spara")
            {
                if (parts.Length == 2)
                {
                    filename = $@"..\..\..\links\{parts[1]}";
                }
                using (StreamWriter streamWriter = new StreamWriter(filename))
                {
                    foreach (Link L in links)
                    {
                        streamWriter.WriteLine(L.ToString());
                    }
                }
            }
            else if (command == "ta")
            {
                if (parts[1] == "bort")
                {
                    links.RemoveAt(Int32.Parse(parts[2]));
                }
            }
            else if (command == "öppna")
            {
                if (parts[1] == "grupp")
                {
                    foreach (Link L in links)
                    {
                        if (L.group == parts[2])
                        {
                            L.OpenLink();
                        }
                    }
                }
                else if (parts[1] == "länk")
                {
                    int ix = Int32.Parse(parts[2]);
                    links[ix].OpenLink();
                }
            }
            else
            {
                Console.WriteLine("Okänt kommando: '{command}'");
            }

            return filename;
        }

        private static string Load(string filename, string[] parts)
        {
            if (parts.Length == 2)
            {
                filename = $@"..\..\..\links\{parts[1]}";
            }
            links = new List<Link>();
            using (StreamReader streamReader = new StreamReader(filename))
            {
                int i = 0;
                string line = streamReader.ReadLine();
                while (line != null)
                {
                    Console.WriteLine(line);
                    Link L = new Link(line);
                    links.Add(L);
                    line = streamReader.ReadLine();
                }
            }

            return filename;
        }

        private static void LoadFromFile(string filename)
        {
            using (StreamReader streamReader = new StreamReader(filename))
            {
                int i = 0;
                string line = streamReader.ReadLine();
                while (line != null)
                {
                    Console.WriteLine(line);
                    Link L = new Link(line);
                    L.Print(i++);
                    links.Add(L);
                    line = streamReader.ReadLine();
                }
            }
        }
    }
}