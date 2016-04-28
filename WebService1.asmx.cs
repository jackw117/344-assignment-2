using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WebApplication1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    [ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        string originalPath = @"C:\Users\Jack\Downloads\enwiki-20160407-all-titles-in-ns0";
        string outputTest = @"C:\Users\Jack\Downloads\outputTest.txt";
        string outputPath = @"C:\Users\Jack\Downloads\output.txt";
        string outputPathNoUnderscore = @"C:\Users\Jack\Downloads\outputNoUnderscore.txt";
        
        public static Trie allWords = new Trie();

        [WebMethod]
        public void onlyAZ()
        {
            using (StreamReader sr = new StreamReader(originalPath))
            {
                using (StreamWriter sw = new StreamWriter(outputPath))
                {
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();
                        if (Regex.IsMatch(line, @"^[a-zA-Z_]+$"))
                        {
                            sw.WriteLine(line);
                        }
                    }
                }
            }
        }

        [WebMethod]
        public void downloadWiki()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("assignment2blob");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference("outputNoUnderscore.txt");
            var path = @"C:\Users\Jack\Desktop\outputNoUnderscore.txt";
            blockBlob.DownloadToFile(path, FileMode.Create);
        }

        [WebMethod]
        public void replaceUnderscore()
        {
            using (StreamReader sr = new StreamReader(outputPath))
            {
                using (StreamWriter sw = new StreamWriter(outputPathNoUnderscore))
                {
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();
                        line = line.Replace('_', ' ');
                        sw.WriteLine(line);
                    }
                }
            }
        }

        [WebMethod]
        public string addToTrie()
        {
            string line = "";
            using (StreamReader sr = new StreamReader(outputPathNoUnderscore))
            {
                Process currentProcess = Process.GetCurrentProcess();
                long memorySize = currentProcess.PrivateMemorySize64;
                long workingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
                long mem = GC.GetTotalMemory(true);
                PerformanceCounter currentPerf = new PerformanceCounter("Memory", "Available MBytes");
                //50 = 53687091
                long stop = 102428800;
                long max = 1073741824;
                int check = 0;
                double available = currentPerf.NextValue();
                while (sr.EndOfStream == false)
                {
                    if (check == 1000)
                    {
                        currentProcess = Process.GetCurrentProcess();
                        mem = GC.GetTotalMemory(true);
                        if (mem > 953741824)
                        {
                            break;
                        }
                        check = 0;
                    }
                    line = sr.ReadLine();
                    allWords.addWord(line);
                    check++;
                }
            }
            return line;
        }

        //something like creating an empty string and adding results to it after typing each letter
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<string> searchTrie(string search)
        {
            List<string> results = allWords.checkWord(search);
            return results;
        }
    }
}
