using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.MyClass
{
    public interface IAuthorizationManager
    {
        string Authenticate(string username, string password);

        IDictionary<string, Tuple<string, string, DateTime>> Tokens { get; }
    }
}
