using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Edimsha.WPF.Utils
{
    /// <summary>
    /// FileDragDropHelper
    /// </summary>
    public class FileDragDropHelper
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public static readonly DependencyProperty IsFileDragDropEnabledProperty =
            DependencyProperty.RegisterAttached("IsFileDragDropEnabled", typeof(bool), typeof(FileDragDropHelper),
                new PropertyMetadata(OnFileDragDropEnabled));

        public static readonly DependencyProperty FileDragDropTargetProperty =
            DependencyProperty.RegisterAttached("FileDragDropTarget", typeof(object), typeof(FileDragDropHelper), null);

        public static bool GetIsFileDragDropEnabled(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsFileDragDropEnabledProperty);
        }

        public static void SetIsFileDragDropEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFileDragDropEnabledProperty, value);
        }

        public static bool GetFileDragDropTarget(DependencyObject obj)
        {
            return (bool) obj.GetValue(FileDragDropTargetProperty);
        }

        public static void SetFileDragDropTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(FileDragDropTargetProperty, value);
        }

        private static void OnFileDragDropEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            if (d is Control control) control.Drop += OnDrop;
        }

        private static void OnDrop(object sender, DragEventArgs dragEventArgs)
        {
            Logger.Info("Dropped");
            if (!(sender is DependencyObject d)) return;
            var target = d.GetValue(FileDragDropTargetProperty);
            if (target is IFileDragDropTarget fileTarget)
            {
                if (dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    fileTarget.OnFileDrop((string[]) dragEventArgs.Data.GetData(DataFormats.FileDrop));
                }
            }
            else
            {
                throw new Exception("FileDragDropTarget object must be of type IFileDragDropTarget");
            }
        }
        
        public static IEnumerable<string> IsDirectoryDropped(IEnumerable<string> filepaths, bool iterateSubdirectories)
        {
            Logger.Info("Dropped directory");
            var temp = new List<string>();

            foreach (var path in filepaths)
            {
                if (Directory.Exists(path))
                    temp.AddRange(iterateSubdirectories
                        ? Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                        : Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly));
                else
                    temp.Add(path);
            }

            return temp;
        }
    }
}