
using System.Diagnostics;

namespace TrinityEngineProject
{
    internal static class TgMessage
    {
        //static string errorLog;

        public static void SendMessage(string message) => SendMessage(new StackTrace().GetFrame(1).GetMethod().ReflectedType.Name, message);
        public static void SendMessage(object sender, string message) => SendMessage(sender.ToString(), message);
        public static void SendMessage(string sender, string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{sender}] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
        }

        public static void ThrowWarning(string message = "unspecified")
        {
            var methodInfo = new StackTrace().GetFrame(1).GetMethod();
            var className = methodInfo.ReflectedType.Name;
            string senderInfo = $"[{className}] {methodInfo}";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(senderInfo);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(message);

            //errorLog += Environment.NewLine + senderInfo + Environment.NewLine + message;
        }
    }
}
