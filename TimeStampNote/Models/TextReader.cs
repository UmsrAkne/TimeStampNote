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
            return Open(fileName);
        }

        public string OpenEditor(string fileName, string defaultContent)
        {
            using (var sw = File.CreateText(fileName))
            { 
                sw.Write(defaultContent);
            }

            return Open(fileName);
        }

        private string Open(string fileName)
        {
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

            if (text.Substring(text.Length - 1) == "\n")
            {
                text = text.Remove(text.Length - 1, 1);
            }

            File.Delete(fileName);
            return text;
        }
    }
}
