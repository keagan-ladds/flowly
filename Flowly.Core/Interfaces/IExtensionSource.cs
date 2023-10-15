using System;
using System.Collections.Generic;
using System.Text;

namespace Flowly.Core.Interfaces
{
    public interface IExtensionSource
    {
        IExtensionProvider Build();
    }
}
