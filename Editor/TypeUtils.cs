using System;
using System.Linq;

namespace LotW.CherryUI.Editor
{
    public static class TypeUtils
    {
        public static string GetFormattedName(Type t)
        {
            if (t.IsArray)
                return $"{GetFormattedName(t.GetElementType())}{t.FullName.Substring(t.FullName.LastIndexOf('['))}";
            if (t.IsGenericType && !t.IsGenericTypeDefinition)
                return $"{GetFormattedName(t.GetGenericTypeDefinition())}<{string.Join(',', t.GetGenericArguments().Select(x => GetFormattedName(x)))}>";
            if (t.IsGenericTypeDefinition)
                return t.FullName.Remove(t.FullName.IndexOf('`'));

            return t.FullName;
        } 
            
        
    }
}