using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.UI;

namespace RadzenStart.Services;

public class ReportStorageService : ReportStorageWebExtension
{
    private readonly string _reportDirectory;
    private const string FileExtension = ".repx";

    public ReportStorageService(IWebHostEnvironment env)
    {
        _reportDirectory = Path.Combine(env.ContentRootPath, "Reports");
        if (!Directory.Exists(_reportDirectory))
            Directory.CreateDirectory(_reportDirectory);
    }

    public override bool IsValidUrl(string url) =>
        !string.IsNullOrEmpty(url) && Path.GetFileName(url) == url;

    public override bool CanSetData(string url) => true;

    public override byte[] GetData(string url)
    {
        var path = Path.Combine(_reportDirectory, url + FileExtension);
        if (File.Exists(path))
            return File.ReadAllBytes(path);

        // Return empty report if file doesn't exist
        using var report = new XtraReport();
        using var ms = new MemoryStream();
        report.SaveLayoutToXml(ms);
        return ms.ToArray();
    }

    public override Dictionary<string, string> GetUrls()
    {
        if (!Directory.Exists(_reportDirectory))
            return new Dictionary<string, string>();

        return Directory.GetFiles(_reportDirectory, "*" + FileExtension)
            .ToDictionary(
                x => Path.GetFileNameWithoutExtension(x)!,
                x => Path.GetFileNameWithoutExtension(x)!);
    }

    public override void SetData(XtraReport report, string url)
    {
        var path = Path.Combine(_reportDirectory, url + FileExtension);
        report.SaveLayoutToXml(path);
    }

    public override string SetNewData(XtraReport report, string defaultUrl)
    {
        SetData(report, defaultUrl);
        return defaultUrl;
    }
}
