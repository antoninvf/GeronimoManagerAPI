using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GeronimoUpdaterAPI.Controllers;

[EnableCors("flwn")]
[ApiController]
[Route("")]
public class DownloadsController : ControllerBase
{
    private readonly ILogger<DownloadsController> _logger;

    public DownloadsController(ILogger<DownloadsController> logger)
    {
        _logger = logger;
    }
    
    private string _path = "/opt/flwnfiles/geronimo";

    private List<CdnFile> GetFiles(string version)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" && RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) _path = "/Volumes/ssd/opt/flwnfiles/geronimo";
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) _path = "Z:/opt/flwnfiles/geronimo";

        // recursively get all files in the directory and subdirectories
        var files = Directory.GetFiles(_path, "*.zip", SearchOption.AllDirectories);
        var fileList = new List<CdnFile>();
        foreach (var e in files)
        {
            var file = new CdnFile();
            file.FileName = Path.GetFileName(e);
            file.Modpack = Path.GetFileName(Path.GetDirectoryName(e)) ?? "/";
            file.Version = file.FileName.Split("(")[1].Split(").zip")[0];
            file.DownloadLink = $"https://files.flwn.dev/geronimo/{Uri.EscapeDataString(file.Modpack)}/{Uri.EscapeDataString(file.FileName)}";
            fileList.Add(file);
        }
        
        // filter by version
        if (version != "") fileList = fileList.Where(x => x.Version.Contains(version)).ToList();
        
        return fileList;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CdnFile>> GetMc()
    {
        return GetFiles("");
    }
    
    [HttpGet("modpacks/{modpack}")]
    public ActionResult<IEnumerable<CdnFile>> GetVersions(string modpack)
    {
        return GetFiles("").Where(x => Uri.EscapeDataString(x.Modpack.ToLower()).Equals(Uri.EscapeDataString(modpack.ToLower()))).ToList();
    }
    
    [HttpGet("modpacks/{modpack}/{version}")]
    public ActionResult<CdnFile> GetVersionsForModpack(string modpack, string version)
    {
        return GetFiles("").Where(x => Uri.EscapeDataString(x.Modpack.ToLower()).Equals(Uri.EscapeDataString(modpack.ToLower())) && x.Version.Contains(version)).ToList()[0];
    }

    [HttpGet("modpacks")]
    public ActionResult<IEnumerable<string>> GetModpacks()
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" && RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) _path = "/Volumes/ssd/opt/flwnfiles/geronimo";
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) _path = "Z:/opt/flwnfiles/geronimo";
        
        // get all folders in the directory
        var folders = Directory.GetDirectories(_path).Select(Path.GetFileName).ToList();

        return folders!;
    }
}