using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace Studio.Assets.Project
{
    public static class Persistence
    {
        private static SaveFileDialog CreateSaveAssetDialog<TAsset>(TAsset asset)
            where TAsset : Asset
        {
            var suggestedFileName = asset.Title + "." + asset.Extension;

            var dialog = new SaveFileDialog();
            dialog.DefaultExt = asset.Extension;
            dialog.Title = "Save file";
            dialog.FileName = suggestedFileName;
            dialog.Filter = asset.FileDescription + "|*." + asset.Extension;
            return dialog;
        }

        private static OpenFileDialog CreateOpenAssetDialog<TAsset>()
            where TAsset : Asset, new()
        {
            var dummy = new TAsset();

            var dialog = new OpenFileDialog();
            dialog.Filter = dummy.FileDescription + "|*." + dummy.Extension;
            return dialog;
        }

        public static bool SaveAsset(Asset asset)
        {
            if (asset.HasSaveFile || asset.CanGenerateFileName)
            {
                asset.Save();
                return true;
            }

            return SaveAssetAs(asset);
        }

        public static bool SaveAssetAs(Asset asset)
        {
            var dialog = CreateSaveAssetDialog(asset);
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var title = RemoveExtension(dialog.SafeFileName);
                asset.Save(dialog.FileName, title);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string RemoveExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return fileName.Substring(0, fileName.Length - extension.Length);
        }

        public static TAsset OpenAsset<TAsset>()
            where TAsset : Asset, new()
        {
            var dialog = CreateOpenAssetDialog<TAsset>();
            var result = dialog.ShowDialog();
            if (!result.Value)
                return default(TAsset);

            using (var s = dialog.OpenFile())
            {
                return Asset.Deserialize<TAsset>(dialog.FileName);
            }
        }
    }
}
