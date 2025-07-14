using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace DevCommon.Extended
{
#if UNITY_EDITOR
    public static class CEnumBuilder
    {
        public static void CreateEnum(string a_SaveDirectory, string a_AssemblyName, EnumAttribute a_EnumAttribute)
        {
            AppDomain t_CurrentDomain = AppDomain.CurrentDomain;
            AssemblyName t_Name = new AssemblyName(a_AssemblyName);
            //AssemblyBuilder t_AssemblyBuilder = t_CurrentDomain.DefineDynamicAssembly(t_Name, AssemblyBuilderAccess.RunAndSave, a_SaveDirectory);
            //ModuleBuilder t_ModuleBuilder = t_AssemblyBuilder.DefineDynamicModule(t_Name.Name, t_Name.Name + ".dll");

           // EnumBuilder t_EnumBuilder = t_ModuleBuilder.DefineEnum(a_EnumAttribute.Name, TypeAttributes.Public, typeof(int));
            foreach (var item in a_EnumAttribute.EnumItems)
            {
               // t_EnumBuilder.DefineLiteral(item.Value, item.Key);
            }
          //  t_EnumBuilder.CreateType();
            //t_AssemblyBuilder.Save(t_Name.Name + ".dll");
        }

        public static void CreateEnum(string a_SaveDirectory, string a_AssemblyName, List<EnumAttribute> a_EnumAttributes)
        {
            AppDomain t_CurrentDomain = AppDomain.CurrentDomain;
            AssemblyName t_Name = new AssemblyName(a_AssemblyName);
            //AssemblyBuilder t_AssemblyBuilder = t_CurrentDomain.DefineDynamicAssembly(t_Name, AssemblyBuilderAccess.RunAndSave, a_SaveDirectory);
            //ModuleBuilder t_ModuleBuilder = t_AssemblyBuilder.DefineDynamicModule(t_Name.Name, t_Name.Name + ".dll");

            foreach (var attribute in a_EnumAttributes)
            {
              //  EnumBuilder t_EnumBuilder = t_ModuleBuilder.DefineEnum(attribute.Name, TypeAttributes.Public, typeof(int));
                foreach (var item in attribute.EnumItems)
                {
                    //t_EnumBuilder.DefineLiteral(item.Value, item.Key);
                }
               // t_EnumBuilder.CreateType();
            }
           // t_AssemblyBuilder.Save(t_Name.Name + ".dll");
        }
    }
#endif

    public class EnumAttribute
    {
        public string Name { get; set; } = "";
        public Dictionary<int, string> EnumItems { get; set; } = new Dictionary<int, string>();
    }
}