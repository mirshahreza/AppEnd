using System.Security.Cryptography;
using System.Text;

namespace AppEndDynaCode
{
    // Guard to avoid unnecessary full DynaCode.Refresh when no server code changes
    public static class DynaCodeGuard
    {
        private static readonly string HashFileName = ".dyna-refresh.hash";

        public static void TryRefresh()
        {
            var serverCodeRoot = Path.Combine(AppEndCommon.AppEndSettings.ProjectRoot.FullName, "AppEndHost", "workspace", "server");
            if (!Directory.Exists(serverCodeRoot))
            {
                // Fallback: if folder not found, perform normal refresh
                DynaCode.Refresh();
                return;
            }

            string currentHash = CalculateFolderHash(serverCodeRoot);
            string hashStorePath = Path.Combine(serverCodeRoot, HashFileName);

            string? previousHash = null;
            if (File.Exists(hashStorePath))
            {
                try { previousHash = File.ReadAllText(hashStorePath).Trim(); }
                catch { previousHash = null; }
            }

            if (string.Equals(currentHash, previousHash, StringComparison.Ordinal))
            {
                // No changes detected; skip heavy refresh
                return;
            }

            // Changes detected; do refresh then persist new hash
            DynaCode.Refresh();
            try { File.WriteAllText(hashStorePath, currentHash); } catch { /* ignore */ }
        }

        private static string CalculateFolderHash(string root)
        {
            using var sha = SHA256.Create();
            var files = Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
                                 .OrderBy(f => f, StringComparer.Ordinal);
            var sb = new StringBuilder(4096);
            foreach (var f in files)
            {
                var fi = new FileInfo(f);
                sb.Append(f);
                sb.Append('|');
                sb.Append(fi.Length);
                sb.Append('|');
                sb.Append(fi.LastWriteTimeUtc.Ticks);
                sb.AppendLine();
            }
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
