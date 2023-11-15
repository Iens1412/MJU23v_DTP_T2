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
            try
            {


                if (command == "sluta")
                {
                    Console.WriteLine("Hej då! Välkommen åter!");
                    Environment.Exit(0);
                }
                else if (command == "hjälp")
                {
                    Console.WriteLine("hjälp                    - skriv ut den här hjälpen");
                    Console.WriteLine("sluta                    - avsluta programmet");
                    Console.WriteLine("ladda /filnamn/          - ladda ned listan från filen du väljer");
                    Console.WriteLine("lista                    - lista innehållet på skärman");
                    Console.WriteLine("ny                       - skapa nytt objekt");
                    Console.WriteLine("spara /filnamn/          - spara listan till filen du väljer");
                    Console.WriteLine("ta bort /nummer/         - tabort en objekt från listan, nummer du skriver är nummret för obektet du vill ta bort");
                    Console.WriteLine("öppna grupp /gruppnamn/  - öppna länker eller länken du vill");
                    Console.WriteLine("öppna länk /obektnummer/ - öppna länken du vill genom objektnummer vilket står för vilken länk du vill öppna");
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
                    Ny();
                }
                else if (command == "spara")
                {
                    filename = Save(filename, parts);
                }
                else if (command == "ta")
                {
                    if (parts.Length >= 3 && parts[1] == "bort")
                    {
                        if (int.TryParse(parts[2], out int index) && index >= 0 && index <= links.Count - 1)
                        {
                            links.RemoveAt(index);
                            Console.WriteLine($"tog bort {index}");
                        }
                        else
                        {
                            Console.WriteLine("fel index, försöka igen med rätt index");
                        }
                    }
                }
                else if (command == "öppna")
                {
                    open(parts);
                }
                else
                {
                    Console.WriteLine($"Okänt kommando: '{command}'");
                }
            }catch (System.FormatException e) {  Console.WriteLine(e.ToString()); }
            catch (System.IndexOutOfRangeException e ) { Console.WriteLine(e.ToString());}
            catch (Exception e) { Console.WriteLine(e.ToString()); }
           

            return filename;
        }

        private static void open(string[] parts)
        {
            try
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
                    if (int.TryParse(parts[2], out int index) && index >= 0 && index <= links.Count - 1)
                    {
                        links[index].OpenLink();
                    }
                    else
                    {
                        Console.WriteLine("fel index, försöka igen med rätt index");
                    }
                }
                else
                {
                    Console.WriteLine($"Okänt kommando: '{parts[1]}'");
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Format exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"okänd exception hänt: {ex.Message}");
            }
            
        }

        private static string Save(string filename, string[] parts)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"okänd exception hänt: {ex.Message}");
            }

            return filename;
        }

        private static void Ny()
        {
            Console.WriteLine("Skapa en ny länk:");
            Console.Write("  ange kategori: ");
            string category = Console.ReadLine();
            Console.Write("  ange grupp: ");
            string group = Console.ReadLine();
            Console.Write("  ange namn: ");
            string name = Console.ReadLine();
            Console.Write("  ange beskrivning: ");
            string info = Console.ReadLine();
            Console.Write("  ange länk: ");
            string link = Console.ReadLine();
            Link newLink = new Link(category, group, name, info, link);
            links.Add(newLink);
        }

        private static string Load(string filename, string[] parts)
        {
            try
            {


                if (parts.Length == 2)
                {
                    filename = $@"..\..\..\links\{parts[1]}";
                }
                if (!File.Exists(filename))
                {
                    Console.WriteLine("filen finns inte. snälla skriv in filnamn som finns i katalog links.");
                    return filename;
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
            } catch (Exception ex) { Console.WriteLine("error: " + ex.Message); }

            return filename;
        }

        private static void LoadFromFile(string filename)
        {
            try
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
            }catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}