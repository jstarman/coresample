using System;

namespace CoreSample
{
    public class SomeLogic
    {
        public string GetSomething(bool shouldIGetSomething)
        {
            if (shouldIGetSomething)
                return "something";

            return string.Empty;
        }
    }
}
