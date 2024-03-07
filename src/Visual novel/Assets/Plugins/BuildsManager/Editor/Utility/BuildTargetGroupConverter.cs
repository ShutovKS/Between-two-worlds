using UnityEditor;

namespace BuildsManager.Utility
{
    public static class BuildTargetGroupConverter
    {
        public static BuildTargetGroup ToBuildTargetGroup(this BuildTarget buildTarget)
        {
            return buildTarget switch
            {
                BuildTarget.StandaloneWindows64 => BuildTargetGroup.Standalone,
                BuildTarget.StandaloneWindows => BuildTargetGroup.Standalone,
                BuildTarget.StandaloneLinux64 => BuildTargetGroup.Standalone,
                BuildTarget.StandaloneOSX => BuildTargetGroup.Standalone,

                BuildTarget.Android => BuildTargetGroup.Android,
                BuildTarget.iOS => BuildTargetGroup.iOS,

                BuildTarget.WebGL => BuildTargetGroup.WebGL,

                BuildTarget.PS5 => BuildTargetGroup.PS5,
                BuildTarget.PS4 => BuildTargetGroup.PS4,

                BuildTarget.GameCoreXboxOne => BuildTargetGroup.GameCoreXboxOne,
                BuildTarget.XboxOne => BuildTargetGroup.XboxOne,

                BuildTarget.tvOS => BuildTargetGroup.tvOS,

                BuildTarget.Switch => BuildTargetGroup.Switch,

                BuildTarget.EmbeddedLinux => BuildTargetGroup.EmbeddedLinux,
                BuildTarget.Bratwurst => BuildTargetGroup.Bratwurst,
                BuildTarget.WSAPlayer => BuildTargetGroup.WSA,
                BuildTarget.QNX => BuildTargetGroup.QNX,

                // Deprecated
                BuildTarget.GameCoreXboxSeries => BuildTargetGroup.GameCoreXboxSeries,
                BuildTarget.StandaloneLinuxUniversal => BuildTargetGroup.Standalone,
                BuildTarget.StandaloneOSXIntel64 => BuildTargetGroup.Standalone,
                BuildTarget.LinuxHeadlessSimulation => BuildTargetGroup.LinuxHeadlessSimulation,
                BuildTarget.StandaloneOSXIntel => BuildTargetGroup.Standalone,
                BuildTarget.StandaloneLinux => BuildTargetGroup.Standalone,
                BuildTarget.BlackBerry => BuildTargetGroup.BlackBerry,
                BuildTarget.SamsungTV => BuildTargetGroup.SamsungTV,
                BuildTarget.XBOX360 => BuildTargetGroup.XBOX360,
                BuildTarget.WP8Player => BuildTargetGroup.WP8,
                BuildTarget.Stadia => BuildTargetGroup.Stadia,
                BuildTarget.Lumin => BuildTargetGroup.Lumin,
                BuildTarget.Tizen => BuildTargetGroup.Tizen,
                BuildTarget.PSP2 => BuildTargetGroup.PSP2,
                BuildTarget.N3DS => BuildTargetGroup.N3DS,
                BuildTarget.WiiU => BuildTargetGroup.WiiU,
                BuildTarget.PSM => BuildTargetGroup.PSM,
                BuildTarget.PS3 => BuildTargetGroup.PS3,

                // Deprecated
                // BuildTarget.StandaloneOSXUniversal => BuildTargetGroup.Standalone,
                // BuildTarget.WebPlayerStreamed => BuildTargetGroup.WebPlayer,
                // BuildTarget.WebPlayer => BuildTargetGroup.WebPlayer,
                // BuildTarget.iPhone => BuildTargetGroup.iOS,

                BuildTarget.NoTarget => BuildTargetGroup.Unknown,
                _ => BuildTargetGroup.Unknown
            };
        }
    }
}