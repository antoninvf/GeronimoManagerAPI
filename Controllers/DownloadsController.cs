using System.Globalization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GeronimoUpdaterAPI.Controllers;

[EnableCors("GangnamStyle")]
[ApiController]
[Route("[controller]")]
public class DownloadsController : ControllerBase
{
    private readonly ILogger<DownloadsController> _logger;

    public DownloadsController(ILogger<DownloadsController> logger)
    {
        _logger = logger;
    }
    
    private string path = "C:/cdn/geronimo";

    private List<CdnFile> getFiles(string version)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") path = "P:/cdn/geronimo";

        // recursively get all files in the directory and subdirectories
        var files = Directory.GetFiles(path, "*.zip", SearchOption.AllDirectories);
        var fileList = new List<CdnFile>();
        foreach (var e in files)
        {
            var file = new CdnFile();
            file.FileName = Path.GetFileName(e);
            file.Modpack = Path.GetFileName(Path.GetDirectoryName(e)) ?? "/";
            file.Version = file.FileName.Split("(")[1].Split(").zip")[0];
            file.DownloadLink = $"https://cdn.gangnamstyle.dev/geronimo/{Uri.EscapeDataString(file.Modpack)}/{Uri.EscapeDataString(file.FileName)}";
            fileList.Add(file);
        }
        
        // filter by version
        if (version != "") fileList = fileList.Where(x => x.Version.Contains(version)).ToList();
        
        return fileList;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CdnFile>> GetMc()
    {
        return getFiles("");
    }
    
    [HttpGet("modpacks/{modpack}")]
    public ActionResult<IEnumerable<CdnFile>> GetVersions(string modpack)
    {
        return getFiles("").Where(x => Uri.EscapeDataString(x.Modpack.ToLower()).Equals(Uri.EscapeDataString(modpack.ToLower()))).ToList();
    }

    [HttpGet("modpacks")]
    public ActionResult<IEnumerable<string>> GetModpacks()
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") path = "P:/cdn/geronimo";

        // get all folders in the directory
        var folders = Directory.GetDirectories(path).Select(Path.GetFileName).ToList();

        return folders!;
    }
}