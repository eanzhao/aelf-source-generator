using System.Diagnostics;

namespace AElf.Contract.SourceGenerator.Logging;

public class SelfLog
{
   private const string FileName = "ProtobufCodeGenerator.log";

   public static void Write(string message)
   {
      try
      {
         var fullPath = Path.Combine(Path.GetTempPath(), FileName);
         File.AppendAllText(fullPath, $"[{DateTime.Now:O}] {message}{Environment.NewLine}");
      }
      catch (Exception ex)
      {
         Debug.WriteLine(ex);
      }
   }
}