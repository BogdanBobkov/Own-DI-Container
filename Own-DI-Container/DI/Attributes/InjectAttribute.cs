using System;

namespace DI.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        
    }
}