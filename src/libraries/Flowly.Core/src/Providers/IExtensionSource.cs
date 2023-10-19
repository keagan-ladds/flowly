using System;
using System.Collections.Generic;
using System.Text;

namespace Flowly.Core.Providers
{
    public interface IExtensionSource
    {
        IExtensionProvider BuildProvider();
    }
}
