using System;

namespace ObjectWalker
{
    public interface ITypeAcceptor
    {
        bool IsResponsible(Type t);
    }
}