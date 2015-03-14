using System;
using System.Collections.Generic;
using System.IO;

namespace Kunukn.XmlVisual.Core.Utilities
{
    /// <summary>
    /// Kunuk Nykjaer
    /// </summary>
    public static class FileUtil
    {
        public static List<string> ReadFile(FileInfo fi)
        {
            return ReadFile(fi.FullName);
        }

        public static List<string> ReadFile(string path)
        {
            var list = new List<string>();            

            try
            {
//                using (var reader = File.OpenText(path))
                using (var reader = new StreamReader(path, System.Text.Encoding.UTF8, true)) //æ ø å
                {                    
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        list.Add(line);
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                Logger.LogError(ex.Message, ex.StackTrace);
            }
                                                    
            return list;
        }

       
        public static bool WriteFile(string data, string filepath)
        {
            bool success = false;            
            try
            {
                using (StreamWriter streamWriter = File.CreateText(filepath))
                {
                    streamWriter.Write(data);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                Logger.LogError(ex.Message, ex.StackTrace);
            }
            
            return success;                                    
        }


        /// <summary>
        /// KN
        /// Creates the folders if not exists for file path
        /// </summary>
        public static bool CreateFilePath(string filepath)
        {
            bool success = false;
            try
            {
                var fi = new FileInfo(filepath);                                
                if (fi.Directory!=null && !fi.Directory.Exists)
                    Directory.CreateDirectory(fi.Directory.ToString());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                Logger.LogError(ex.Message, ex.StackTrace);
            }

            return success;
        }

        /// <summary>
        /// KN
        /// delete file first if exists
        /// folder path is created if not exists
        /// </summary>
        public static bool WriteFile(List<string> data, string filepath)
        {
            bool success = false;
            try
            {                
                var fi = new FileInfo(filepath);
                if (fi.Exists)                
                    fi.Delete();
                
                if (fi.Directory!=null && !fi.Directory.Exists)
                    Directory.CreateDirectory(fi.Directory.ToString() );  
                                                    
                using (StreamWriter streamWriter = fi.CreateText() )
                {
                    int i = 0;
                    int len = data.Count;
                    foreach (var line in data)
                    {
                        streamWriter.Write(line);
                        i++;
                        if (i < len)
                            streamWriter.Write(Environment.NewLine);
                    }
                                        
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+Environment.NewLine+ex.StackTrace);
                Logger.LogError(ex.Message, ex.StackTrace);
            }

            return success;
        }


        public static bool AppendFile(string data, string path)
        {
            bool success = false;            
            try
            {
                using (StreamWriter streamWriter = File.AppendText(path))
                {
                    streamWriter.Write(data);
                    success = true;
                }                                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                Logger.LogError(ex.Message, ex.StackTrace);
            }
            
            return success;
        }

        public static string[] GetFiles(string path)
        {
            return GetFiles(path, null);
        }

        public static string[] GetFiles(string path, string prefix)
        {
            string p = prefix ?? string.Empty;
            string[] filePaths = null;

            if (path == null)
                return filePaths;

            if (Directory.Exists(path))
            {
                filePaths = Directory.GetFiles(path, p + "*.*"); // get all filetypes
            }
            return filePaths;
        }

        public static bool DeleteFile(string path) 
        {
            var fi = new FileInfo(path);
            try
            {                
                fi.Delete();                
                return true;
            }
            catch
            {
                return false;
            }   
        }
    }
}
