namespace GeronimoUpdaterAPI;

public class CdnFile
{
    public string FileName { get; set; }
    public string Modpack { get; set; }
    public string Version { get; set; }
    public string DownloadLink { get; set; }
    
    public CdnFile()
    {
        FileName = "";
        Modpack = "";
        Version = "";
        DownloadLink = "";
    }
}