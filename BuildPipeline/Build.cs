using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild {
	public static int Main() => Execute<Build>(x => x.RunBuild);

	Target RunBuild => _ => _
		.Executes(() => {
			var latestCommit = GetLatestCommit();
			var version = $"{GetProjectVersion()}.{latestCommit}";
			Logger.Info($"Build project for version '{version}'");
			var unityPath = GetUnityPath();
			var arguments =
				$"-executeMethod UnityCiPipeline.CustomBuildPipeline.RunBuildForVersion -version={version} -buildTarget=WebGL " +
				$"-projectPath {RootDirectory} -quit -batchmode -nographics -logFile -";
			var proc = ProcessTasks.StartProcess(unityPath, arguments);
			proc.WaitForExit();
		});

	Target Publish => _ => _
		.Executes(() => {
			var toolPath = GetButlerPath();
			var version = GetProjectVersion();
			var target = "konh/blobber:html";
			var targetDir = RootDirectory / "Build";
			var proc = ProcessTasks.StartProcess(toolPath, $"push --userversion={version} --verbose {targetDir} {target}");
			proc.WaitForExit();
		});

	const string ServerProject = "Server";

	Target CleanServer => _ => _
		.Executes(() => {
			DotNetClean(new DotNetCleanSettings()
				.SetProject(ServerProject)
				.SetConfiguration(Configuration.Release));
		});

	Target RestoreServer => _ => _
		.Executes(() => {
			DotNetRestore(new DotNetRestoreSettings()
				.SetProjectFile(ServerProject));
		});

	Target CompileServer => _ => _
		.DependsOn(CleanServer, RestoreServer)
		.Executes(() => {
			DotNetBuild(new DotNetBuildSettings()
				.SetProjectFile(ServerProject)
				.SetConfiguration(Configuration.Release));
		});

	Target PublishServer => _ => _
		.DependsOn(CompileServer)
		.Executes(() => {
				var settings = new DotNetPublishSettings()
					.SetProject(ServerProject)
					.SetConfiguration(Configuration.Release)
					.SetRuntime("linux-arm")
					.SetSelfContained(true)
					.SetArgumentConfigurator(a => a.Add("/p:PublishSingleFile=true"));
				DotNetPublish(settings);
			});

	[Parameter]
	public string LocalPiHome;

	Target DeployServer => _ => _
		.Description("Deploy server to target Pi directory root")
		.Requires(() => LocalPiHome)
		.DependsOn(PublishServer)
		.Executes(() => {
			var buildConfigurationDir = RootDirectory / ServerProject / "bin" / "Release";
			var buildDir              = GetBuildDir(buildConfigurationDir);
			var targetPath            = (AbsolutePath) LocalPiHome / "LD47Server";
			var sourceDirPath         = buildDir / "linux-arm" / "publish";
			CopyDirectoryRecursively(sourceDirPath, targetPath,
				DirectoryExistsPolicy.Merge, FileExistsPolicy.OverwriteIfNewer);
		});

	[Parameter]
	public string SshHost;

	[Parameter]
	public string SshUserName;

	[Parameter]
	public string SshPassword;

	Target StopService => _ => _
		.Description("Stop server service")
		.Requires(() => SshHost)
		.Requires(() => SshUserName)
		.Requires(() => SshPassword)
		.Executes(() => {
			var context = new ServiceTarget.ExecutionContext(SshHost, SshUserName, SshPassword);
			ServiceTarget.StopService(context, "ld47server");
		});

	Target StartService => _ => _
		.Description("Start server service")
		.Requires(() => SshHost)
		.Requires(() => SshUserName)
		.Requires(() => SshPassword)
		.Executes(() => {
			var context = new ServiceTarget.ExecutionContext(SshHost, SshUserName, SshPassword);
			ServiceTarget.StartService(context, "ld47server");
		});

	string GetUnityPath() =>
		EnvironmentInfo.IsWin
			? "C:/Program Files/Unity/Hub/Editor/2020.1.6f1/Editor/Unity.exe"
			: "/Applications/Unity/Hub/Editor/2020.1.6f1/Unity.app/Contents/MacOS/Unity";

	string GetButlerPath() =>
		EnvironmentInfo.IsWin
			? RootDirectory / "Butler" / "Win" / "butler.exe"
			: RootDirectory / "Butler" / "MacOS" / "butler";

	string GetProjectVersion() {
		var lines = File.ReadAllLines(RootDirectory / "ProjectSettings/ProjectSettings.asset");
		var prefix = "bundleVersion: ";
		return lines
			.Select(l => l.Trim())
			.Where(l => l.StartsWith(prefix))
			.Select(l => l.Substring(prefix.Length))
			.First();
	}

	string GetLatestCommit() {
		var proc = ProcessTasks.StartProcess("git", "rev-parse --short HEAD");
		proc.WaitForExit();
		return proc.Output.First().Text;
	}

	static AbsolutePath GetBuildDir(AbsolutePath buildConfigurationDir) {
		var frameworkDirs = Directory.GetDirectories(buildConfigurationDir);
		if ( (frameworkDirs.Length == 0) ) {
			throw new InvalidOperationException($"No framework directories found at '{buildConfigurationDir}'");
		}
		if ( (frameworkDirs.Length > 1) ) {
			throw new InvalidOperationException(
				$"More than one framework directories found at '{buildConfigurationDir}'");
		}
		return (AbsolutePath) frameworkDirs[0];
	}
}