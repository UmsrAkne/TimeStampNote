namespace TimeStampNote.Models
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class TextReader
    {
        public string OpenEditor(string fileName)
        {
            File.Create(fileName).Dispose();
            var proc = new Process();
            proc.StartInfo.FileName = fileName;
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
            proc.WaitForExit();

            string text;

            using (var reader = new StreamReader(fileName))
            {
                text = reader.ReadToEnd();
            }

            File.Delete(fileName);
            return text;
        }
    }
}
