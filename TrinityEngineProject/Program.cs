using TrinityEngineProject;

Console.SetWindowSize(Math.Min(140, Console.LargestWindowWidth), Math.Min(34, Console.LargestWindowHeight));

string screen = Environment.NewLine + Environment.NewLine;

string[] title = File.ReadAllLines(",..//..//..//..//..//Data//Common//ascii-title.txt");
foreach (string line in title)
    screen += "                  " + line + Environment.NewLine;

screen += Environment.NewLine + Environment.NewLine;

string[] art = File.ReadAllLines(",..//..//..//..//..//Data//Common//ascii-art.txt");
foreach (string line in art)
    screen += line.Substring(10, 119) + Environment.NewLine;

for (int i = 0; i < 5; i++)
    screen += Environment.NewLine;

screen += File.ReadAllText(",..//..//..//..//..//Data//Common//startup-notice.txt");

int l = 0;
bool b = true;
int startdelay = 0;
foreach (string line in screen.Split(Environment.NewLine))
{
    if (l >= Console.WindowHeight)
    {
        if (b)
        {
            Thread.Sleep(800 * startdelay);
            b = false;
        }
        else
            Thread.Sleep(8 * startdelay);
    }
    else l++;


    Console.WriteLine(line);
}


using (TgMain window = new TgMain(1920, 1080, "trinity"))
{
    window.Run();
}