using System.Diagnostics;
using System.Runtime.InteropServices;
using Gml.Web.Api.Core.Services;
using GmlCore.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Gml.Web.Api.Core.Hubs;

public class GitHubLauncherHub(IGitHubService gitHubService, IGmlManager gmlManager) : BaseHub
{
    private const string _launcherGitHub = "https://github.com/Gml-Launcher/Gml.Launcher";

    public async Task Download(string branchName, string host, string folderName)
    {
        try
        {
            var projectPath = Path.Combine(gmlManager.LauncherInfo.InstallationDirectory, "Launcher", branchName);

            if (Directory.Exists(projectPath))
            {
                SendCallerMessage("Лаунчер уже существует в папке, удалите его перед сборкой");
                return;
            }

            projectPath = Path.Combine(gmlManager.LauncherInfo.InstallationDirectory, "Launcher");

            ChangeProgress(nameof(GitHubLauncherHub), 5);
            var allowedVersions = await gitHubService.GetRepositoryTags("Gml-Launcher", "Gml.Launcher");

            if (allowedVersions.All(c => c != branchName))
            {
                SendCallerMessage($"Полученная версия лаунчера \"{branchName}\" не поддерживается");
                return;
            }

            ChangeProgress(nameof(GitHubLauncherHub), 10);
            var newFolder = await gitHubService.DownloadProject(projectPath, branchName, _launcherGitHub);
            ChangeProgress(nameof(GitHubLauncherHub), 20);

            await gitHubService.EditLauncherFiles(newFolder, host, folderName);
            ChangeProgress(nameof(GitHubLauncherHub), 30);

            ChangeProgress(nameof(GitHubLauncherHub), 100);
            SendCallerMessage($"Проект \"{branchName}\" успешно создан");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            await gmlManager.Notifications.SendMessage("Ошибка при загрузке клиента лаунчера", exception);
        }
        finally
        {
            await Clients.Caller.SendAsync("LauncherDownloadEnded");
        }
    }

    public async Task Compile(string version, string[] osTypes)
    {
        try
        {
            if (!gmlManager.Launcher.CanCompile(version, out string message))
            {
                SendCallerMessage(message);
                return;
            }

            Log("Start compilling...");

            if (await gmlManager.LauncherInfo.Settings.SystemProcedures.InstallDotnet())
            {
                var eventObservable = gmlManager.Launcher.BuildLogs.Subscribe(Log);

                await gmlManager.Launcher.Build(version, osTypes);

                eventObservable.Dispose();

                SendCallerMessage("Лаунчер успешно скомпилирован");
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            await gmlManager.Notifications.SendMessage("Ошибка при загрузке клиента лаунчера", exception);
        }
        finally
        {
            await Clients.Caller.SendAsync("LauncherBuildEnded");
        }
    }
}
