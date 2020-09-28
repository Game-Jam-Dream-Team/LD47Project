using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Tooling;

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
			var target = "konh/ld47project:html";
			var targetDir = RootDirectory / "Build";
			var proc = ProcessTasks.StartProcess(toolPath, $"push --userversion={version} --verbose {targetDir} {target}");
			proc.WaitForExit();
		});

	string GetUnityPath() =>
		"/Applications/Unity/Hub/Editor/2020.1.6f1/Unity.app/Contents/MacOS/Unity";

	string GetButlerPath() =>
		RootDirectory / "Butler" / "MacOS" / "butler";

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
}