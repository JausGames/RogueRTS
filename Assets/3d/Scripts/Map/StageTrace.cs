using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class StageTrace
{
    static string trace;
    static public void Trace(string trace)
    {
        StageTrace.trace += "\n" + trace;
    }

    static public void ExportTrace()
    {
        var dir = "D:\\Unity\\_trace";
        var fileCount = DirCount(new DirectoryInfo(dir));
        System.IO.File.WriteAllText(dir + "\\trace_" + fileCount + ".txt", trace);
    }
    static public long DirCount(DirectoryInfo d)
    {
        long i = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            if (fi.Name.Contains("trace") && fi.Extension.Contains("txt"))
                i++;
        }
        return i;
    }
}
