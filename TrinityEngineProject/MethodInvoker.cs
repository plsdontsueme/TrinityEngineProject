
namespace TrinityEngineProject
{
    internal class MethodInvoker
    {
        private List<Action> methods = new List<Action>();
        private List<object[]> arguments = new List<object[]>();

        public IEnumerable<(Action, object[])> GetMethods => methods.Zip(arguments);

        public void AddMethod(Action method, params object[] arguments)
        {
            methods.Add(method);
            this.arguments.Add(arguments);
        }
        public void RemoveMethod(Action method)
        {
            int index = methods.IndexOf(method);
            if (index != -1) return;
            methods.RemoveAt(index);
            arguments.RemoveAt(index);
        }
        public void ClearMethods()
        {  
            methods.Clear();
            arguments.Clear();
        }

        public void InvokeAll()
        {
            for (int i = 0; i < methods.Count; i++)
            {
                methods[i].DynamicInvoke(arguments[i]);
            }
        }
    }
}
