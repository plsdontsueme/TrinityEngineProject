
using System.Diagnostics;

namespace TrinityEngineProject
{
    internal static class TgMessage
    {
        //static string errorLog;

        public static void PrintArray(Array array)
        {
            string message = $"[{array.Length} Elements]";
            int index = 0;
            foreach (var element in array)
            {
                message += $"{Environment.NewLine}[{index}] {element.ToString()}";
                index++;
            }
            Send(message);
        }
        public static void Send(string message) => Send(new StackTrace().GetFrame(1).GetMethod().ReflectedType.Name, message);
        public static void Send(object sender, string message) => Send(sender.ToString(), message);
        public static void Send(string sender, string message)
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
