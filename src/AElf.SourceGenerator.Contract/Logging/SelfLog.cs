using System;
using System.Diagnostics;
using System.IO;
using AElf.SourceGenerator.Contract;

namespace AElf.SourceGenerator.Logging;

public class SelfLog
{
   public static void Write(string message)
   {
      try
      {
         var fullPath = Path.Combine(Path.GetTempPath(), ContractSourceGeneratorConstants.LogFileName);
         File.AppendAllText(fullPath, $"[{DateTime.Now:O}] {message}{Environment.NewLine}");
      }
      catch (Exception ex)
      {
         Debug.WriteLine(ex);
      }
   }
}