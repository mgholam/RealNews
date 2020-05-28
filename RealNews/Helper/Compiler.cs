using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;

namespace RealNews
{
    class Compiler
    {
        static MethodInfo _process = null;
        internal static string CompileAndRun(string codefilename, object[] methodparams)
        {
            if (_process == null)
            {
                // compile .cs file 
                CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");

                CompilerParameters compilerparams = new CompilerParameters();
                compilerparams.GenerateInMemory = true;
                compilerparams.GenerateExecutable = false;

                compilerparams.ReferencedAssemblies.Add(typeof(System.Text.RegularExpressions.Regex).Assembly.Location);

                string code = "using System; public class plugin{%CODE%}"
                    .Replace("%CODE%", File.ReadAllText(codefilename));

                CompilerResults results = compiler.CompileAssemblyFromSource(compilerparams, code);

                if (results.Errors.HasErrors == true)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var e in results.Errors)
                        sb.AppendLine(e.ToString());
                    var s = sb.ToString();
                    throw new Exception(s);
                }
                else
                {
                    _process = results.CompiledAssembly
                        .GetType("plugin")
                        .GetMethod("Process");
                }
            }
            var res = (string)_process.Invoke(null, BindingFlags.Static, null, methodparams, null);
            return res;

        }
    }
}
