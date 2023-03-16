using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace ObsidianNoteLink;

public class Core
{
    static void Main(string[] args)
    {
        
        string sourcePath = StringConst.sourcePath;
        string destinationPath = StringConst.destinationPath;
        
        var csFiles = Directory.GetFiles(sourcePath, "*.cs", SearchOption.AllDirectories);
        
        var noteRegex = new Regex(@"\[\s*Note\s*\]\s*(?<note>[\s\S]*?)\*/", RegexOptions.Multiline);
        
        var comments = from file in csFiles
            let projectName = Path.GetFileName(Path.GetDirectoryName(file))
            //let projectPathName = Path.GetFullPath(file)
            let projectPathName = Path.Combine(destinationPath, Path.GetRelativePath(sourcePath, Path.GetDirectoryName(file)))
                let className = Path.GetFileNameWithoutExtension(file)
                let fileText = File.ReadAllText(file)
                let matches = noteRegex.Matches(fileText)
                
                where matches.Count > 0
                let noteText = string.Join(Environment.NewLine, matches.Cast<Match>().Select(m => m.Groups["note"].Value.Trim()))
                group noteText by new {ProjectName = projectPathName, ClassName = className} into g
                select new {g.Key.ProjectName, g.Key.ClassName, NoteText = string.Join(Environment.NewLine, g.ToArray())};

        foreach (var comment in comments)
        {
            //string projectPath = Path.Combine(destinationPath, Path.GetFileName(Path.GetDirectoryName(comment.ProjectName)) ?? string.Empty);
            Console.WriteLine("Finding comments in: " + comment.ClassName + "...");
            string projectPath = Path.Combine(destinationPath, comment.ProjectName);
            Console.WriteLine("Generating directory: " + projectPath + "...");
            
            if (!Directory.Exists(projectPath))
            {
                Console.WriteLine("Creating directory: " + projectPath + "...");
                Directory.CreateDirectory(projectPath);
            }

            string classPath = Path.Combine(projectPath, comment.ClassName + ".md");
            Console.WriteLine("Writing file: " + classPath + "...");
            File.WriteAllText(classPath, comment.NoteText);
        }
                
    }
}

